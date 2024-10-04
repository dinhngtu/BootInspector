using System.Linq;

namespace TcgLog {
    internal static class EnumUtils {
        public static object ParseValue<T>(byte[] bytes) where T : Enum {
            if (Enum.GetUnderlyingType(typeof(T)) == typeof(char) && bytes.Length == sizeof(char)) {
                return BitConverter.ToChar(bytes, 0);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(short) && bytes.Length == sizeof(short)) {
                return BitConverter.ToInt16(bytes, 0);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(int) && bytes.Length == sizeof(int)) {
                return BitConverter.ToInt32(bytes, 0);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(long) && bytes.Length == sizeof(long)) {
                return BitConverter.ToInt64(bytes, 0);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(ushort) && bytes.Length == sizeof(ushort)) {
                return BitConverter.ToUInt16(bytes, 0);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(uint) && bytes.Length == sizeof(uint)) {
                return BitConverter.ToUInt32(bytes, 0);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(ulong) && bytes.Length == sizeof(ulong)) {
                return BitConverter.ToUInt64(bytes, 0);
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(nint) && bytes.Length == nint.Size) {
                unchecked {
                    if (nint.Size == sizeof(int)) {
                        return (nint)BitConverter.ToInt32(bytes, 0);
                    } else {
                        return (nint)BitConverter.ToInt64(bytes, 0);
                    }
                }
            } else if (Enum.GetUnderlyingType(typeof(T)) == typeof(nuint) && bytes.Length == nuint.Size) {
                unchecked {
                    if (nuint.Size == sizeof(uint)) {
                        return (nuint)BitConverter.ToUInt32(bytes, 0);
                    } else {
                        return (nuint)BitConverter.ToUInt64(bytes, 0);
                    }
                }
            } else {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }
        }

        public static IEnumerable<KeyValuePair<T, string>> GetEntries<T>() where T : Enum {
            foreach (var val in Enum.GetValues(typeof(T))) {
                if (val == null) throw new ArgumentNullException();
                yield return new KeyValuePair<T, string>((T)val, Enum.GetName(typeof(T), val) ?? throw new ArgumentNullException());
            }
        }

        public static string Format<T>(T value) where T : Enum {
            if (Attribute.GetCustomAttribute(typeof(T), typeof(EnumFormatType)) is EnumFormatAttribute a && a.Format == EnumFormatType.Hex) {
                return value.ToString("X");
            } else {
                return value.ToString();
            }
        }
    }

    public class ParsedEnumRecord<T>(T value, string name = "Value") : RecordBase where T : Enum {
        public override string Name { get; } = name;
        public override string? Source => SourceAttribute.Get<T>();
        public override IReadOnlyList<RecordBase> Children => [];
        public override string? ToString() {
            return Enum.GetName(typeof(T), value) ?? EnumUtils.Format(value);
        }
    }

    public class EnumRecord<T>(byte[] bytes) : ByteRecord(bytes) where T : Enum {
        private readonly object _value = EnumUtils.ParseValue<T>(bytes);

        public override string Name { get; } = "Value";
        public override string? Source => SourceAttribute.Get<T>();

        public override string? ToString() {
            return Enum.GetName(typeof(T), _value) ?? EnumUtils.Format((T)_value);
        }
    }

    public class EnumFlagsRecord<T> : ByteRecord where T : Enum {
        private static readonly Lazy<List<KeyValuePair<T, string>>> _entries = new(() => EnumUtils.GetEntries<T>().ToList());

        private readonly object _value;
        private readonly List<RecordBase> _children;

        public EnumFlagsRecord(byte[] bytes) : base(bytes) {
            _value = EnumUtils.ParseValue<T>(bytes);
            _children = [];
            foreach (var kv in _entries.Value) {
                if (((T)_value).HasFlag(kv.Key)) {
                    _children.Add(new StringRecord(kv.Value));
                }
            }
            if (!Equals(_value, default(T))) {
                _children.Add(new StringRecord(EnumUtils.Format((T)_value)));
            }
        }

        public override string Name { get; } = typeof(T).Name;
        public override string? Source => SourceAttribute.Get<T>();

        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
