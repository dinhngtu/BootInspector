using System.Runtime.InteropServices;

namespace TcgLog {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal unsafe struct TPMU_HA {
        [FieldOffset(0)]
        public fixed byte sha1[20];
        [FieldOffset(0)]
        public fixed byte sha256[32];
        [FieldOffset(0)]
        public fixed byte sm3_256[32];
        [FieldOffset(0)]
        public fixed byte sha384[48];
        [FieldOffset(0)]
        public fixed byte sha512[64];
    }

    [StructLayout(LayoutKind.Sequential)]
    [RecordSource("edk2/Tpm20.h", RecordSourcePlatform.Uefi)]
    internal struct TPMT_HA {
        public TPM_ALG_ID hashAlg;
        public TPMU_HA digest;
    }

    public class TpmtHa : ByteRecord {
        private readonly TPM_ALG_ID _hashAlg;

        public static TpmtHa Create(ReadOnlySpan<byte> bytes) {
            var remain = bytes;
            remain = EnumUtils<TPM_ALG_ID>.ParseNext(remain, out var hashAlg);
            var name = Enum.GetName<TPM_ALG_ID>(hashAlg) ?? EnumUtils<TPM_ALG_ID>.Format(hashAlg);
            switch (hashAlg) {
                case TPM_ALG_ID.TPM_ALG_SHA1:
                    return new TpmtHa(hashAlg, remain[..20], name);
                case TPM_ALG_ID.TPM_ALG_SHA256:
                case TPM_ALG_ID.TPM_ALG_SM3_256:
                    return new TpmtHa(hashAlg, remain[..32], name);
                case TPM_ALG_ID.TPM_ALG_SHA384:
                    return new TpmtHa(hashAlg, remain[..48], name);
                case TPM_ALG_ID.TPM_ALG_SHA512:
                    return new TpmtHa(hashAlg, remain[..64], name);
                default:
                    throw new NotImplementedException($"unsupported TPM_ALG_ID {hashAlg}");
            }
        }

        private TpmtHa(TPM_ALG_ID hashAlg, ReadOnlySpan<byte> digest, string name) : base(digest, name) {
            _hashAlg = hashAlg;
        }

        internal int RecordSize {
            get {
                return sizeof(TPM_ALG_ID) + base.Size;
            }
        }

        public override RecordSource? Source => RecordSourceAttribute.Get<TPMT_HA>();
    }

    public class TcgPcrEvent2Record : ByteRecord {
        private readonly uint _pcrIndex;
        private readonly TCG_EVENTTYPE _eventType;
        private readonly List<RecordBase> _children;

        public static TcgPcrEvent2Record Create(ReadOnlySpan<byte> bytes) {
            var remain = bytes;
            remain = ValueUtils.ParseNext<uint>(remain, out var pcrIndex);
            remain = EnumUtils<TCG_EVENTTYPE>.ParseNext(remain, out var eventType);

            remain = ValueUtils.ParseNext<uint>(remain, out var digestsCount);
            if (digestsCount == 0 || digestsCount > 5) throw new ArgumentOutOfRangeException($"invalid digestsCount {digestsCount}");

            var recordSize = bytes.Length - remain.Length;
            List<TpmtHa> digests = [];
            for (uint i = 0; i < digestsCount; i++) {
                var digest = TpmtHa.Create(remain);
                digests.Add(digest);
                remain = remain[digest.RecordSize..];
                recordSize += digest.RecordSize;
            }

            remain = ValueUtils.ParseNext<uint>(remain, out var eventSize);
            recordSize += 4;
            var eventData = remain[..(int)eventSize];
            recordSize += eventData.Length;

            return new TcgPcrEvent2Record(bytes[..recordSize], pcrIndex, eventType, digests, eventData);
        }

        private TcgPcrEvent2Record(ReadOnlySpan<byte> bytes, uint pcrIndex, TCG_EVENTTYPE eventType, List<TpmtHa> digests, ReadOnlySpan<byte> eventData) : base(bytes) {
            _pcrIndex = pcrIndex;
            _eventType = eventType;
            _children = [
                new ValueRecord<uint>(_pcrIndex, "PCRIndex"),
                new ParsedEnumRecord<TCG_EVENTTYPE>(_eventType, nameof(TCG_EVENTTYPE)),
                new AggregatedRecord<RecordBase>(digests, "Digest"),
                new ValueRecord<uint>((uint)eventData.Length, "EventSize"),
                new ByteRecord(eventData, "EventData"),
            ];
        }

        public override string Name { get; } = "TCG_PCR_EVENT2";
        public override RecordSource? Source { get; } = new RecordSource { Source = "edk2/UefiTcgPlatform.h", Platform = RecordSourcePlatform.Uefi };
        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
