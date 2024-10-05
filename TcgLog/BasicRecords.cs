using System.Xml;

namespace TcgLog {
    public class ByteRecord(ReadOnlySpan<byte> bytes, string name = "Bytes") : RecordBase {
        private readonly byte[] _bytes = bytes.ToArray();
        public override string Name { get; } = name;
        public override RecordSource? Source { get; } = null;
        public byte[] GetBytes() { return _bytes; }
        public int Size => _bytes.Length;
        public override string? ToString() { return BitConverter.ToString(_bytes); }
        public override IReadOnlyList<RecordBase> Children => [];
    }

    public class StringRecord(string value, string name = "Value") : RecordBase {
        public override string Name { get; } = name;
        public override RecordSource? Source { get; } = null;
        public override string? ToString() { return value; }
        public override IReadOnlyList<RecordBase> Children => [];
    }

    public class ErrorRecord(ReadOnlySpan<byte> bytes, string error) : ByteRecord(bytes) {
        public string Message => error;
        public override XmlElement ToXml(XmlDocument document, LogFormatterSettings? settings = null) {
            var el = base.ToXml(document, settings);
            el.SetAttribute("Error", Message);
            return el;
        }
    }

    public class ExceptionRecord(ReadOnlySpan<byte> bytes, Exception exception) : ByteRecord(bytes) {
        public string Message => exception.Message;
        public override XmlElement ToXml(XmlDocument document, LogFormatterSettings? settings = null) {
            var el = base.ToXml(document, settings);
            el.SetAttribute("Error", Message);
            return el;
        }
    }

    public class AggregatedRecord<T>(IEnumerable<RecordBase> records, string name = "Aggregate") : RecordBase {
        private readonly List<RecordBase> _children = records.ToList();
        public override string Name { get; } = name;
        public override RecordSource? Source => RecordSourceAttribute.Get<T>();
        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
