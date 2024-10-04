namespace TcgLog {
    internal enum EnumFormatType {
        Decimal,
        Hex,
    }

    [AttributeUsage(AttributeTargets.Enum)]
    internal class EnumFormatAttribute(EnumFormatType format) : Attribute {
        public EnumFormatType Format { get; } = format;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    internal class SourceAttribute(string source) : Attribute {
        public string Source { get; } = source;
        public static string? Get(Type type) {
            if (Attribute.GetCustomAttribute(type, typeof(SourceAttribute)) is SourceAttribute a) {
                return a.Source;
            } else {
                return null;
            }
        }
        public static string? Get<T>() {
            return Get(typeof(T));
        }
    }
}
