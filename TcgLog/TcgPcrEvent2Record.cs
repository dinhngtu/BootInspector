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
        private readonly TPMT_HA _value;
        private readonly List<RecordBase> _children;

        public TpmtHa(byte[] bytes) : base(bytes) {
            _value = StructUtils.ParseValue<TPMT_HA>(bytes);
            _children = [
                new ParsedEnumRecord<TPM_ALG_ID>(_value.hashAlg),
                //
            ];
        }

        public override string Name { get; } = nameof(TPMT_HA);
        public override RecordSource? Source => RecordSourceAttribute.Get<TPMT_HA>();

        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
