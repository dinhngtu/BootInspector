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
    [Source("edk2/Tpm20.h")]
    internal struct TCG_PCR_EVENT_HDR {
        public uint PCRIndex;
        public TCG_EVENTTYPE EventType;
        public TCG_DIGEST Digest;
        public uint EventSize;
    }

    public class TcgPcrEventRecord : ByteRecord {
        private readonly TCG_PCR_EVENT_HDR _value;
        private readonly List<RecordBase> _children;

        public TcgPcrEventRecord(byte[] bytes) : base(bytes) {
            _value = StructUtils.ParseValue<TCG_PCR_EVENT_HDR>(bytes);
            var digest = new byte[20];
            unsafe {
                fixed (byte* src = &_value.Digest.digest[0]) {
                    Marshal.Copy((nint)src, digest, 0, 20);
                }
            }
            _children = [
                new ValueRecord<uint>(_value.PCRIndex, nameof(_value.PCRIndex)),
                new ParsedEnumRecord<TCG_EVENTTYPE>(_value.EventType, nameof(_value.EventType)),
                new TcgDigestRecord(digest),
                new ValueRecord<uint>(_value.EventSize, nameof(_value.EventSize)),
            ];
        }

        public override string Name { get; } = nameof(TCG_PCR_EVENT_HDR);
        public override string? Source => SourceAttribute.Get<TCG_PCR_EVENT_HDR>();

        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
