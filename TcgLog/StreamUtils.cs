namespace TcgLog {
    internal static class StreamUtils {
        public static byte[] ReadToEnd(this BinaryReader binaryReader) {
            var remaining = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
            return binaryReader.ReadBytes((int)remaining);
        }
    }
}
