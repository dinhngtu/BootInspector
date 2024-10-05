using System.Runtime.InteropServices;

namespace TcgLog {
    public class WbclLog : ByteRecord {
        private readonly List<RecordBase> _children;

        public WbclLog(ReadOnlySpan<byte> bytes, ParseSettings? settings = null) : base(bytes) {
            _children = [];

            var remain = bytes;
            try {
                var logHeader = TcgPcrEventRecord.CreateFirst(remain, settings);
                _children.Add(logHeader);
                remain = remain[logHeader.Size..];

                if (logHeader.PCRIndex == 0 && logHeader.EventType == TCG_EVENTTYPE.EV_NO_ACTION && logHeader.Digest.All(x => x == 0)) {
                    // valid header
                } else {
                    _children.Add(new ErrorRecord(remain, "invalid event log header"));
                    return;
                }

                var specId = (TcgEfiSpecIdEventRecord)logHeader.EventData;
            } catch (Exception ex) {
                _children = [new ExceptionRecord(bytes, ex)];
                return;
            }

            try {
                while (!remain.IsEmpty) {
                    var evt2 = TcgPcrEvent2Record.Create(remain);
                    _children.Add(evt2);
                    remain = remain[evt2.Size..];
                }
            } catch (Exception ex) {
                _children.Add(new ExceptionRecord(remain, ex));
            }
        }

        public override string Name { get; } = "TcgLog";

        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
