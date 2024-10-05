using System.Xml;

namespace TcgLog {
    public abstract class RecordBase {
        public abstract string Name { get; }
        public abstract RecordSource? Source { get; }
        public abstract IReadOnlyList<RecordBase> Children { get; }

        public virtual XmlElement ToXml(XmlDocument document, LogFormatterSettings? settings = null) {
            var el = document.CreateElement(Name);
            if (settings != null && settings.Value.WriteSource && Source != null) {
                el.SetAttribute("Source", Source.ToString());
            }
            if (Children.Count > 0) {
                foreach (var child in Children) {
                    el.AppendChild(child.ToXml(document, settings));
                }
            } else {
                el.InnerText = ToString() ?? "";
            }
            return el;
        }
    }
}
