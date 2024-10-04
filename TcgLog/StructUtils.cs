using System.Runtime.InteropServices;

namespace TcgLog {
    internal static class StructUtils {
        public static T ParseValue<T>(byte[] bytes) where T : struct {
            T value;
            if (bytes.Length != Marshal.SizeOf<T>()) {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }
            unsafe {
                var buf = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
                try {
                    fixed (void* src = &bytes[0]) {
                        Buffer.MemoryCopy(src, buf.ToPointer(), Marshal.SizeOf<T>(), bytes.Length);
                    }
                    value = Marshal.PtrToStructure<T>(buf);
                } finally {
                    Marshal.FreeHGlobal(buf);
                }
            }
            return value;
        }
    }
}
