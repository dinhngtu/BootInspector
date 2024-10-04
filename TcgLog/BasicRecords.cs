namespace TcgLog {
    public abstract class ByteRecord(byte[] bytes) : RecordBase {
        public override string Name { get; } = "RawBytes";
        public override string? Source { get; } = null;
        public virtual byte[] GetBytes() { return bytes; }
        public override string? ToString() { return BitConverter.ToString(bytes); }
        public override IReadOnlyList<RecordBase> Children => [];
    }

    public class StringRecord(string value, string name = "Value") : RecordBase {
        public override string Name { get; } = name;
        public override string? Source { get; } = null;
        public override string? ToString() { return value; }
        public override IReadOnlyList<RecordBase> Children => [];
    }
}
