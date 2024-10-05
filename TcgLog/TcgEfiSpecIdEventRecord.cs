using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace TcgLog {
    enum UINTNSIZE : byte {
        UINTN_IS_UINT32 = 1,
        UINTN_IS_UINT64 = 2,
    }

    [StructLayout(LayoutKind.Sequential)]
    [RecordSource("edk2/UefiTcgPlatform.h", RecordSourcePlatform.Uefi)]
    internal unsafe struct TCG_EfiSpecIDEventStruct_Invariant {
        public fixed byte signature[16];
        public uint platformClass;
        public byte specVersionMinor;
        public byte specVersionMajor;
        public byte specErrata;
        public UINTNSIZE uintnSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    [RecordSource("edk2/UefiTcgPlatform.h", RecordSourcePlatform.Uefi)]
    internal struct TCG_EfiSpecIdEventAlgorithmSize {
        public TPM_ALG_ID algorithmId;
        public ushort digestSize;
    }

    [EventData(TCG_EVENTTYPE.EV_NO_ACTION)]
    public class TcgEfiSpecIdEventRecord : EventDataRecord {
        private readonly TCG_EfiSpecIDEventStruct_Invariant _invariant;
        private readonly List<RecordBase> _children;
        private readonly List<TCG_EfiSpecIdEventAlgorithmSize> _algorithms = [];

        public TcgEfiSpecIdEventRecord(ReadOnlySpan<byte> bytes) : base(bytes) {
            var remain = StructUtils.ParseNext<TCG_EfiSpecIDEventStruct_Invariant>(bytes, out _invariant);

            string signature;
            unsafe {
                fixed (void* pSignature = &_invariant.signature[0]) {
                    signature = Marshal.PtrToStringAnsi((nint)pSignature, 16);
                }
            }

            signature = signature.TrimEnd('\0');
            if (!"Spec ID Event03".Equals(signature, StringComparison.InvariantCulture)) {
                throw new NotImplementedException($"unsupported signature type {signature}");
            }
            if (_invariant.specVersionMajor != 2 || _invariant.specVersionMinor != 0) {
                throw new NotImplementedException($"unsupported spec version {_invariant.specVersionMajor}.{_invariant.specVersionMinor}");
            }

            remain = ValueUtils.ParseNext<uint>(remain, out var numberOfAlgorithms);
            if (numberOfAlgorithms == 0 || numberOfAlgorithms > 5) {
                throw new NotImplementedException($"unsupported numberOfAlgorithms {numberOfAlgorithms}");
            }
            for (int i = 0; i < numberOfAlgorithms; i++) {
                remain = StructUtils.ParseNext<TCG_EfiSpecIdEventAlgorithmSize>(remain, out var alg);
                _algorithms.Add(alg);
            }

            var vendorInfoSize = remain[0];
            var vendorInfo = remain.Slice(1, vendorInfoSize);

            _children = [
                new StringRecord(signature, nameof(_invariant.signature)),
                new ValueRecord<uint>(_invariant.platformClass, nameof(_invariant.platformClass)),
                new ValueRecord<byte>(_invariant.specVersionMinor, nameof(_invariant.specVersionMinor)),
                new ValueRecord<byte>(_invariant.specVersionMajor, nameof(_invariant.specVersionMajor)),
                new ValueRecord<byte>(_invariant.specErrata, nameof(_invariant.specErrata)),
                new ParsedEnumRecord<UINTNSIZE>(_invariant.uintnSize, nameof(_invariant.uintnSize)),
            ];
            var algRecords = new List<AggregatedRecord<TCG_EfiSpecIdEventAlgorithmSize>>();
            foreach (var alg in _algorithms) {
                algRecords.Add(new AggregatedRecord<TCG_EfiSpecIdEventAlgorithmSize>([
                    new ParsedEnumRecord<TPM_ALG_ID>(alg.algorithmId, nameof(alg.algorithmId)),
                    new ValueRecord<ushort>(alg.digestSize, nameof(alg.digestSize)),
                ], nameof(TCG_EfiSpecIdEventAlgorithmSize)));
            }
            _children.Add(new AggregatedRecord<int>(algRecords, "digestSize"));
            _children.Add(new ByteRecord(vendorInfo, nameof(vendorInfo)));
        }

        private static int HeaderSize { get; } = Marshal.SizeOf<TCG_EfiSpecIDEventStruct_Invariant>();
        private static int AlgorithmSize { get; } = Marshal.SizeOf<TCG_EfiSpecIdEventAlgorithmSize>();

        internal UINTNSIZE UintNSize => _invariant.uintnSize;
        internal IReadOnlyList<TCG_EfiSpecIdEventAlgorithmSize> Algorithms => _algorithms;

        public override string Name { get; } = "TCG_EfiSpecIDEventStruct";
        public override RecordSource? Source => RecordSourceAttribute.Get<TCG_EfiSpecIDEventStruct_Invariant>();
        public override IReadOnlyList<RecordBase> Children => _children;
    }
}
