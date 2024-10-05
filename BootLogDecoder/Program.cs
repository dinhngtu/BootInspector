using System.IO;
using System.Xml;
using System.Xml.Linq;
using TcgLog;

namespace BootLogDecoder {
    internal class Program {
        static void Main(string[] args) {
            var logbytes = File.ReadAllBytes(args[0]);

            var parseSettings = new ParseSettings() {
                AcceptablePlatforms = RecordSourcePlatform.Uefi | RecordSourcePlatform.Windows,
            };
            var log = new WbclLog(logbytes, parseSettings);

            var formatterSettings = new LogFormatterSettings {
                WriteSource = false,
            };
            var logxml = new XmlDocument();
            logxml.AppendChild(log.ToXml(logxml, formatterSettings));

            var writerSettings = new XmlWriterSettings {
                Indent = true,
                IndentChars = new string(' ', 2),
            };
            using var writer = XmlWriter.Create(args[1], writerSettings);
            logxml.Save(writer);
        }
    }
}
