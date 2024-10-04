using System.IO;
using System.Xml;
using TcgLog;

namespace BootLogDecoder {
    internal class Program {
        static void Main(string[] args) {
            var logbytes = File.ReadAllBytes(args[0]);
            var log = new WbclLog(logbytes);
            var logxml = new XmlDocument();
            logxml.AppendChild(log.ToXml(logxml));
            var writerSettings = new XmlWriterSettings {
                Indent = true,
                IndentChars = new string(' ', 2),
            };
            using var writer = XmlWriter.Create(args[1], writerSettings);
            logxml.Save(writer);
        }
    }
}
