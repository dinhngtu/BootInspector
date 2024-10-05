using System.Runtime.InteropServices;

namespace TcgLog {
    public class WbclLog : ByteRecord {
        private readonly List<RecordBase> _children;

        public WbclLog(ReadOnlySpan<byte> bytes, ParseSettings? settings = null) : base(bytes) {
            _children = [];

            try {
                var logHeader = TcgPcrEventRecord.CreateFirst(bytes, settings);
                _children.Add(logHeader);

                if (logHeader.PCRIndex == 0 && logHeader.EventType == TCG_EVENTTYPE.EV_NO_ACTION && logHeader.Digest.Sum(x => (int)x) == 0) {
                    // valid header
                } else {
                    _children.Add(new ErrorRecord(bytes[logHeader.Size..], "invalid event log header"));
                    return;
                }
            } catch (Exception ex) {
                _children = [new ExceptionRecord(bytes, ex)];
            }
        }

        public override string Name { get; } = "TcgLog";

        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
