using System.Runtime.InteropServices;

namespace TcgLog {
    public class WbclLog : ByteRecord {
        private readonly List<RecordBase> _children;

        public WbclLog(byte[] bytes) : base(bytes) {
            _children = [
                new TcgPcrEventRecord(bytes[0..Marshal.SizeOf<TCG_PCR_EVENT_HDR>()]),
            ];
        }

        public override string Name { get; } = "TcgLog";

        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
