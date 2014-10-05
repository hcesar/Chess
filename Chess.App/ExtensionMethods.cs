using Chess.App;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;

namespace System
{
    internal static class ExtensionMethods
    {
        public static LockedGraphics GetGraphics(this Image image)
        {
            return new LockedGraphics(image);
        }

        public static TEntity ToEntity<TEntity>(this Xml.XmlNode node) where TEntity : new()
        {
            var entity = new TEntity();
            node.ToEntity(entity);
            return entity;
        }

        public static void ToEntity<TEntity>(this Xml.XmlNode node, TEntity entity)
        {
            var nodes = node.ChildNodes.Cast<Xml.XmlNode>().ToList();
            foreach (var prop in entity.GetType().GetProperties())
            {
                var propNode = nodes.FirstOrDefault(i => i.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase));
                if (propNode == null)
                    continue;

                string value = propNode.InnerText;

                if (value.Length == 0)
                    continue;

                var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                try
                {
                    prop.SetValue(entity, converter.ConvertFromString(value));
                }
                catch
                {
                    throw new InvalidOperationException(string.Format("Não foi possível converter '{0}' em '{1}'.", value, prop.Name));
                }
            }
        }


        public static IList<TEntity> ToEntityList<TEntity>(this FileInfo xmlFile, string entitiesSelector) where TEntity : new()
        {
            if (!xmlFile.Exists)
                return new List<TEntity>();

            var xml = new XmlDocument();
            using (var stream = xmlFile.OpenRead())
                xml.Load(stream);

            return xml.SelectNodes(entitiesSelector).Cast<XmlNode>().Select(node => node.ToEntity<TEntity>()).ToList();
        }
    }
}