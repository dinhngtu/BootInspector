using System.Xml;

namespace TcgLog {
    public abstract class RecordBase {
        public abstract string Name { get; }
        public abstract string? Source { get; }
        public abstract IReadOnlyList<RecordBase> Children { get; }
        public virtual XmlElement ToXml(XmlDocument document) {
            var el = document.CreateElement(Name);
            if (Source != null) {
                el.SetAttribute("source", Source);
            }
            if (Children.Count > 0) {
                foreach (var child in Children) {
                    el.AppendChild(child.ToXml(document));
                }
            } else {
                el.InnerText = ToString() ?? "";
            }
            return el;
        }
    }
}
