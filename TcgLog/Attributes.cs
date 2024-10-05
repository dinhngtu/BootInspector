namespace TcgLog {
    internal enum EnumFormatType {
        Decimal,
        Hex,
    }

    [AttributeUsage(AttributeTargets.Enum)]
    internal class EnumFormatAttribute(EnumFormatType format) : Attribute {
        public EnumFormatType Format { get; } = format;
    }

    [Flags]
    public enum RecordSourcePlatform {
        Unknown = 1,
        Uefi = 2,
        Windows = 4,
        Grub = 8,
        Linux = 16,
    }

    public class RecordSource {
        public string? Source;
        public RecordSourcePlatform Platform;
        public override string? ToString() {
            if (Platform == RecordSourcePlatform.Unknown) {
                return Source;
            } else {
                return $"{Platform}:{Source}";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Field)]
    internal class RecordSourceAttribute(string source, RecordSourcePlatform platform = RecordSourcePlatform.Unknown) : Attribute {
        public RecordSource Source { get; } = new RecordSource() { Source = source, Platform = platform };

        public static RecordSource? Get(Type type, RecordSourcePlatform? acceptablePlatforms = null) {
            if (Attribute.GetCustomAttribute(type, typeof(RecordSourceAttribute)) is RecordSourceAttribute a) {
                if (acceptablePlatforms == null || acceptablePlatforms.Value.HasFlag(a.Source.Platform)) {
                    return a.Source;
                }
            }
            return null;
        }

        public static RecordSource? Get<T>(RecordSourcePlatform? acceptablePlatforms = null) {
            return Get(typeof(T), acceptablePlatforms);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class EventDataAttribute(TCG_EVENTTYPE eventType) : Attribute {
        public TCG_EVENTTYPE EventType => eventType;
    }
}
