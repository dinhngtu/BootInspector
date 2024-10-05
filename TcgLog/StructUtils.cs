using System.Runtime.InteropServices;

namespace TcgLog {
    internal static class StructUtils {
        public static T ParseValue<T>(ReadOnlySpan<byte> bytes) where T : struct {
            T value;
            if (bytes.Length != Marshal.SizeOf<T>()) {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }
            unsafe {
                fixed (void* src = bytes) {
                    value = Marshal.PtrToStructure<T>((nint)src);
                }
            }
            return value;
        }

        public static ReadOnlySpan<byte> ParseNext<T>(ReadOnlySpan<byte> bytes, out T value) where T : struct {
            var size = Marshal.SizeOf<T>();
            var valBytes = bytes[..size];
            value = ParseValue<T>(valBytes);
            return bytes[size..];
        }
    }
}
