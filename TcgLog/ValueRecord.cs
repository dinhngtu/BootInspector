using System.Runtime.InteropServices;

namespace TcgLog {
    internal static class ValueUtils {
        public static T ParseValue<T>(ReadOnlySpan<byte> bytes) where T : unmanaged {
            if (typeof(T) == typeof(bool) && bytes.Length == sizeof(bool)) {
                return (T)(object)BitConverter.ToBoolean(bytes);
            } else if (typeof(T) == typeof(char) && bytes.Length == sizeof(char)) {
                return (T)(object)BitConverter.ToChar(bytes);
            } else if (typeof(T) == typeof(double) && bytes.Length == sizeof(double)) {
                return (T)(object)BitConverter.ToDouble(bytes);
            } else if (typeof(T) == typeof(short) && bytes.Length == sizeof(short)) {
                return (T)(object)BitConverter.ToInt16(bytes);
            } else if (typeof(T) == typeof(int) && bytes.Length == sizeof(int)) {
                return (T)(object)BitConverter.ToInt32(bytes);
            } else if (typeof(T) == typeof(long) && bytes.Length == sizeof(long)) {
                return (T)(object)BitConverter.ToInt64(bytes);
            } else if (typeof(T) == typeof(float) && bytes.Length == sizeof(float)) {
                return (T)(object)BitConverter.ToSingle(bytes);
            } else if (typeof(T) == typeof(ushort) && bytes.Length == sizeof(ushort)) {
                return (T)(object)BitConverter.ToUInt16(bytes);
            } else if (typeof(T) == typeof(uint) && bytes.Length == sizeof(uint)) {
                return (T)(object)BitConverter.ToUInt32(bytes);
            } else if (typeof(T) == typeof(ulong) && bytes.Length == sizeof(ulong)) {
                return (T)(object)BitConverter.ToUInt64(bytes);
            } else if (typeof(T) == typeof(nint) && bytes.Length == nint.Size) {
                unchecked {
                    if (nint.Size == sizeof(int)) {
                        return (T)(object)(nint)BitConverter.ToInt32(bytes);
                    } else {
                        return (T)(object)(nint)BitConverter.ToInt64(bytes);
                    }
                }
            } else if (typeof(T) == typeof(nuint) && bytes.Length == nuint.Size) {
                unchecked {
                    if (nuint.Size == sizeof(uint)) {
                        return (T)(object)(nuint)BitConverter.ToUInt32(bytes);
                    } else {
                        return (T)(object)(nuint)BitConverter.ToUInt64(bytes);
                    }
                }
            } else {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }
        }

        public static ReadOnlySpan<byte> ParseNext<T>(ReadOnlySpan<byte> bytes, out T value) where T : unmanaged {
            var size = Marshal.SizeOf<T>();
            var valBytes = bytes[..size];
            value = ParseValue<T>(valBytes);
            return bytes[size..];
        }
    }

    public class ValueRecord<T>(T value, string? name = null) : RecordBase where T : unmanaged {
        public override string Name { get; } = name ?? typeof(T).Name;
        public override RecordSource? Source => null;
        public override IReadOnlyList<RecordBase> Children => [];
        public override string? ToString() {
            return value.ToString();
        }
    }

    public class ParsedValueRecord<T>(ReadOnlySpan<byte> bytes, string? name = null) : ByteRecord(bytes) where T : unmanaged {
        private readonly object _value = ValueUtils.ParseValue<T>(bytes);

        public override string Name { get; } = name ?? typeof(T).Name;

        public override string ToString() {
            return _value.ToString() ?? throw new ArgumentNullException();
        }
    }
}
