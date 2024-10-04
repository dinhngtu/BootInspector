namespace TcgLog {
    internal static class ValueUtils {
        public static object ParseValue<T>(byte[] bytes) {
            if (typeof(T) == typeof(bool) && bytes.Length == sizeof(bool)) {
                return BitConverter.ToBoolean(bytes, 0);
            } else if (typeof(T) == typeof(char) && bytes.Length == sizeof(char)) {
                return BitConverter.ToChar(bytes, 0);
            } else if (typeof(T) == typeof(double) && bytes.Length == sizeof(double)) {
                return BitConverter.ToDouble(bytes, 0);
            } else if (typeof(T) == typeof(short) && bytes.Length == sizeof(short)) {
                return BitConverter.ToInt16(bytes, 0);
            } else if (typeof(T) == typeof(int) && bytes.Length == sizeof(int)) {
                return BitConverter.ToInt32(bytes, 0);
            } else if (typeof(T) == typeof(long) && bytes.Length == sizeof(long)) {
                return BitConverter.ToInt64(bytes, 0);
            } else if (typeof(T) == typeof(float) && bytes.Length == sizeof(float)) {
                return BitConverter.ToSingle(bytes, 0);
            } else if (typeof(T) == typeof(ushort) && bytes.Length == sizeof(ushort)) {
                return BitConverter.ToUInt16(bytes, 0);
            } else if (typeof(T) == typeof(uint) && bytes.Length == sizeof(uint)) {
                return BitConverter.ToUInt32(bytes, 0);
            } else if (typeof(T) == typeof(ulong) && bytes.Length == sizeof(ulong)) {
                return BitConverter.ToUInt64(bytes, 0);
            } else if (typeof(T) == typeof(nint) && bytes.Length == nint.Size) {
                unchecked {
                    if (nint.Size == sizeof(int)) {
                        return (nint)BitConverter.ToInt32(bytes, 0);
                    } else {
                        return (nint)BitConverter.ToInt64(bytes, 0);
                    }
                }
            } else if (typeof(T) == typeof(nuint) && bytes.Length == nuint.Size) {
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
    }

    public class ValueRecord<T>(T value, string? name = null) : RecordBase where T : unmanaged {
        public override string Name { get; } = name ?? typeof(T).Name;
        public override string? Source => null;
        public override IReadOnlyList<RecordBase> Children => [];
        public override string? ToString() {
            return value.ToString();
        }
    }

    public class ParsedValueRecord<T>(byte[] bytes, string? name = null) : ByteRecord(bytes) where T : unmanaged {
        private readonly object _value = ValueUtils.ParseValue<T>(bytes);

        public override string Name { get; } = name ?? typeof(T).Name;

        public override string ToString() {
            return _value.ToString() ?? throw new ArgumentNullException();
        }
    }
}
