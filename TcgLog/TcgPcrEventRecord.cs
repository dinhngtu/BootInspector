using System.Runtime.InteropServices;

namespace TcgLog {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct TCG_DIGEST {
        public fixed byte digest[20];
    }

    public class TcgDigestRecord(byte[] bytes) : ByteRecord(bytes) {
        public override string Name { get; } = nameof(TCG_DIGEST);
    }

    [StructLayout(LayoutKind.Sequential)]
    [RecordSource("edk2/Tpm20.h", RecordSourcePlatform.Uefi)]
    internal struct TCG_PCR_EVENT_HDR {
        public uint PCRIndex;
        public TCG_EVENTTYPE EventType;
        public TCG_DIGEST Digest;
        public uint EventSize;
    }

    public class TcgPcrEventRecord : ByteRecord {
        private readonly TCG_PCR_EVENT_HDR _value;
        private readonly List<RecordBase> _children;
        private readonly byte[] _digest = new byte[20];
        private readonly RecordBase _eventData;

        public static TcgPcrEventRecord CreateFirst(ReadOnlySpan<byte> bytes, ParseSettings? settings = null) {
            var headerBytes = bytes[..HeaderSize];
            var value = StructUtils.ParseValue<TCG_PCR_EVENT_HDR>(headerBytes);

            var digest = new byte[20];
            unsafe {
                Marshal.Copy((nint)(&value.Digest.digest[0]), digest, 0, 20);
            }

            if (value.EventSize < 0) throw new ArgumentOutOfRangeException(nameof(value.EventSize));
            var eventBytes = bytes.Slice(HeaderSize, (int)value.EventSize);
            ByteRecord eventData;
            try {
                eventData = new TcgEfiSpecIdEventRecord(eventBytes);
            } catch (Exception ex) {
                eventData = new ExceptionRecord(eventBytes, ex);
            }

            return new TcgPcrEventRecord(bytes[..(HeaderSize + eventData.Size)], value, digest, eventData, settings);
        }

        private TcgPcrEventRecord(ReadOnlySpan<byte> bytes, TCG_PCR_EVENT_HDR value, byte[] digest, RecordBase eventData, ParseSettings? settings) : base(bytes) {
            _value = value;
            _digest = digest;
            _eventData = eventData;
            _children = [
                new ValueRecord<uint>(_value.PCRIndex, nameof(_value.PCRIndex)),
                new ParsedEnumRecord<TCG_EVENTTYPE>(_value.EventType, nameof(_value.EventType), settings),
                new TcgDigestRecord(_digest),
                new ValueRecord<uint>(_value.EventSize, nameof(_value.EventSize)),
                new AggregatedRecord<TCG_PCR_EVENT_HDR>([_eventData], "EventData"),
            ];
        }

        public override string Name { get; } = "TCG_PCR_EVENT";
        public override RecordSource? Source => RecordSourceAttribute.Get<TCG_PCR_EVENT_HDR>();

        private static int HeaderSize { get; } = Marshal.SizeOf<TCG_PCR_EVENT_HDR>();

        internal uint PCRIndex => _value.PCRIndex;
        internal TCG_EVENTTYPE EventType => _value.EventType;
        internal byte[] Digest => _digest;
        internal uint EventSize => _value.EventSize;
        internal RecordBase EventData => _eventData;

        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
