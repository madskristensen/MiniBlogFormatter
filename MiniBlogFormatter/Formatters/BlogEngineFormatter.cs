using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace MiniBlogFormatter
{
    public class BlogEngineFormatter
    {
        private Regex rxFiles = new Regex("(href|src)=\"(([^\"]+)?/(file|image)\\.axd\\?(file|picture)=([^\"]+))\"", RegexOptions.IgnoreCase);
        private Regex rxAggBug = new Regex("<img (.*) src=(.*(aggbug.ashx).*) />", RegexOptions.IgnoreCase);

        public void Format(string fileName, string targetFolderPath, string categoriesFileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            XmlNode isDeletedNode = doc.SelectSingleNode("post/isdeleted");

            bool isDeleted = isDeletedNode != null ? isDeletedNode.InnerText == "True" : false;

            if (!isDeleted)
            {
                FormatSlug(doc);
                FormatFileReferences(doc);
                RemoveAggBug(doc);
                RemoveSpamComments(doc);

                XmlDocument categories = new XmlDocument();
                categories.Load(categoriesFileName);
                FormatCategories(doc, categories);

                string newFileName = Path.Combine(targetFolderPath, Path.GetFileName(fileName));
                doc.Save(newFileName);
            }
        }

        private void FormatFileReferences(XmlDocument doc)
        {
            XmlNode content = doc.SelectSingleNode("post/content");

            if (content != null)
            {
                foreach (Match match in rxFiles.Matches(content.InnerText))
                {
                    content.InnerText = content.InnerText.Replace(match.Groups[2].Value, "/posts/files/" + match.Groups[6].Value);
                }
            }
        }

        private void FormatSlug(XmlDocument doc)
        {
            XmlNode slug = doc.SelectSingleNode("//slug");

            if (slug != null)
            {
                slug.InnerText = FormatterHelpers.FormatSlug(slug.InnerText);
            }
        }

        private void RemoveAggBug(XmlDocument doc)
        {
            XmlNode content = doc.SelectSingleNode("post/content");

            if (content != null)
            {
                content.InnerText = rxAggBug.Replace(content.InnerText, string.Empty);
            }
        }

        private void RemoveSpamComments(XmlDocument doc)
        {
            XmlNodeList comments = doc.SelectNodes("//comment");

            for (int i = comments.Count - 1; i > -1; i--)
            {
                XmlNode comment = comments[i];
                bool approved = comment.Attributes["approved"] != null ? comment.Attributes["approved"].InnerText == "True" : true;
                bool deleted = comment.Attributes["deleted"] != null ? comment.Attributes["deleted"].InnerText == "True" : true;

                if (!approved || deleted)
                {
                    comment.ParentNode.RemoveChild(comment);
                }
            }
        }

        private void FormatCategories(XmlDocument doc, XmlDocument categoriesDoc)
        {
            XmlNodeList categories = doc.SelectNodes("//category");

            foreach (XmlNode category in categories)
            {
                string id = category.InnerText;
                XmlNode name = categoriesDoc.SelectSingleNode("//category[@id='" + id + "']");

                if (name != null)
                {
                    category.InnerText = name.InnerText;
                }
            }
        }
    }
}
