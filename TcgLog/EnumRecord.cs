using System.Linq;
using System.Reflection;

namespace TcgLog {
    internal struct EnumInfo {
        public string Name;
        public RecordSource? Source;
    }

    internal static class EnumUtils<T> where T : Enum {
        public static readonly Lazy<List<KeyValuePair<T, EnumInfo>>> Entries = new(() => GetEntries().ToList());
        static readonly Lazy<RecordSource?> TypeSource = new(() => RecordSourceAttribute.Get<T>());

        public static object ParseValue(ReadOnlySpan<byte> bytes) {
            if (Enum.GetUnderlyingType(typeof(T)) == typeof(char) && bytes.Length == sizeof(char)) {
                return BitConverter.ToChar(bytes);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(short) && bytes.Length == sizeof(short)) {
                return BitConverter.ToInt16(bytes);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(int) && bytes.Length == sizeof(int)) {
                return BitConverter.ToInt32(bytes);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(long) && bytes.Length == sizeof(long)) {
                return BitConverter.ToInt64(bytes);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(ushort) && bytes.Length == sizeof(ushort)) {
                return BitConverter.ToUInt16(bytes);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(uint) && bytes.Length == sizeof(uint)) {
                return BitConverter.ToUInt32(bytes);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(ulong) && bytes.Length == sizeof(ulong)) {
                return BitConverter.ToUInt64(bytes);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(nint) && bytes.Length == nint.Size) {
                unchecked {
                    if (nint.Size == sizeof(int)) {
                        return (nint)BitConverter.ToInt32(bytes);
                    } else {
                        return (nint)BitConverter.ToInt64(bytes);
                    }
                }
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(nuint) && bytes.Length == nuint.Size) {
                unchecked {
                    if (nuint.Size == sizeof(uint)) {
                        return (nuint)BitConverter.ToUInt32(bytes);
                    } else {
                        return (nuint)BitConverter.ToUInt64(bytes);
                    }
                }
            } else {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }
        }

        static IEnumerable<KeyValuePair<T, EnumInfo>> GetEntries() {
            foreach (var val in Enum.GetValues(typeof(T))) {
                var valName = Enum.GetName(typeof(T), val);
                if (valName == null) continue;
                var src = typeof(Enum).GetField(valName)?.GetCustomAttribute<RecordSourceAttribute>()?.Source ?? TypeSource.Value;
                yield return new KeyValuePair<T, EnumInfo>((T)val, new EnumInfo() { Name = valName, Source = src });
            }
        }

        public static string Format(T value) {
            if (Attribute.GetCustomAttribute(typeof(T), typeof(EnumFormatType)) is EnumFormatAttribute a && a.Format == EnumFormatType.Hex) {
                return value.ToString("X");
            } else {
                return value.ToString();
            }
        }

        public static RecordSource? GetSource(T value, IEnumerable<KeyValuePair<T, EnumInfo>> sources) {
            foreach (var kv in sources) {
                if (Enum.Equals(kv.Key, value)) {
                    return kv.Value.Source ?? TypeSource.Value;
                }
            }
            return TypeSource.Value;
        }

        public static IEnumerable<KeyValuePair<T, EnumInfo>> Filter(RecordSourcePlatform? acceptablePlatforms) {
            if (!acceptablePlatforms.HasValue) return Entries.Value;
            return Entries.Value.Where(x => x.Value.Source != null).Where(x => acceptablePlatforms.Value.HasFlag(x.Value.Source!.Platform));
        }
    }

    public class ParsedEnumRecord<T>(T value, string name = "Value", ParseSettings? settings = null) : RecordBase where T : Enum {
        public override string Name { get; } = name;
        public override RecordSource? Source => EnumUtils<T>.GetSource(value, EnumUtils<T>.Filter(settings?.AcceptablePlatforms));
        public override IReadOnlyList<RecordBase> Children => [];
        public override string? ToString() {
            return Enum.GetName(typeof(T), value) ?? EnumUtils<T>.Format(value);
        }
    }

    public class EnumRecord<T>(ReadOnlySpan<byte> bytes, ParseSettings? settings = null) : ByteRecord(bytes) where T : Enum {
        private readonly object _value = EnumUtils<T>.ParseValue(bytes);

        public override string Name { get; } = "Value";
        public override RecordSource? Source => EnumUtils<T>.GetSource((T)_value, EnumUtils<T>.Filter(settings?.AcceptablePlatforms));

        public override string? ToString() {
            return Enum.GetName(typeof(T), _value) ?? EnumUtils<T>.Format((T)_value);
        }
    }

    public class EnumFlagsRecord<T> : ByteRecord where T : Enum {
        private readonly object _value;
        private readonly List<RecordBase> _children;

        public EnumFlagsRecord(ReadOnlySpan<byte> bytes, ParseSettings? settings = null) : base(bytes) {
            _value = EnumUtils<T>.ParseValue(bytes);
            _children = [];
            var value = (ulong)_value;
            foreach (var kv in EnumUtils<T>.Filter(settings?.AcceptablePlatforms)) {
                if (((T)_value).HasFlag(kv.Key)) {
                    _children.Add(new StringRecord(kv.Value.Name));
                    value &= ~Convert.ToUInt64(kv.Key);
                }
            }
            if (value != 0) {
                _children.Add(new StringRecord(EnumUtils<T>.Format((T)(object)value)));
            }
        }

        public override string Name { get; } = typeof(T).Name;
        public override RecordSource? Source => RecordSourceAttribute.Get<T>();

        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
