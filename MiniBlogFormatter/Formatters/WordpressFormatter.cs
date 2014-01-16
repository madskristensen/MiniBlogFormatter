using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;


namespace MiniBlogFormatter
{
    public class WordpressFormatter
    {
        private Regex rxFiles = new Regex("(href|src)=\"(([^\"]+)(/content/binary/)([^\"]+))\"", RegexOptions.IgnoreCase);

        public void Format(string originalFolderPath, string targetFolderPath)
        {
            FormatPosts(originalFolderPath, targetFolderPath);
        }

        private void FormatPosts(string originalFolderPath, string targetFolderPath)
        {
            foreach (string file in Directory.GetFiles(originalFolderPath, "*.xml"))
            {
                XmlDocument docOrig = LoadDocument(file);
                XmlNamespaceManager nsm = LoadNamespaceManager(docOrig);
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(docOrig.NameTable);
                namespaceManager.AddNamespace("content", docOrig.DocumentElement.GetNamespaceOfPrefix("content"));
                namespaceManager.AddNamespace("dc", docOrig.DocumentElement.GetNamespaceOfPrefix("dc"));
                namespaceManager.AddNamespace("wp", docOrig.DocumentElement.GetNamespaceOfPrefix("wp"));


                foreach (XmlNode entry in docOrig.SelectNodes("//item", nsm))
                {
                    Post post = new Post();
                    XmlNodeList categories = entry.SelectNodes("category[@domain='category']");
                    List<string> resultCategories = new List<string>();
                    foreach (XmlNode category in categories)
                    {
                        resultCategories.Add(category.InnerText);
                    }

                    post.Categories = resultCategories.ToArray();
                    post.Title = entry.SelectSingleNode("title").InnerText;
                    post.Slug = FormatterHelpers.FormatSlug(post.Title);
                    post.PubDate = DateTime.Parse(entry.SelectSingleNode("pubDate").InnerText);
                    post.LastModified = DateTime.Parse(entry.SelectSingleNode("pubDate").InnerText);


                    post.Content = FormatFileReferences(entry.SelectSingleNode("content:encoded", namespaceManager).InnerText);
                    post.Author = entry.SelectSingleNode("dc:creator", namespaceManager).InnerText;
                    post.IsPublished = ReadValue(entry.SelectSingleNode("wp:status", namespaceManager), "publish") == "publish";

                    // FormatComments()
                    foreach (XmlNode comment in entry.SelectNodes("wp:comment", namespaceManager))
                    {
                        FomartComment(ref post, comment, namespaceManager);
                    }


                    string newFile = Path.Combine(targetFolderPath, entry.SelectSingleNode("wp:post_id", namespaceManager).InnerText + ".xml");
                    Storage.Save(post, newFile);
                }
            }
        }

        private void FomartComment(ref Post post, XmlNode entry, XmlNamespaceManager namespaceManager)
        {
            Comment comment = new Comment();
            comment.Author = ReadValue(entry.SelectSingleNode("wp:comment_author", namespaceManager), "n/a");
            comment.Email = ReadValue(entry.SelectSingleNode("wp:comment_author_email", namespaceManager), "");
            comment.Ip = entry.SelectSingleNode("wp:comment_author_IP", namespaceManager).InnerText;
            comment.Website = entry.SelectSingleNode("wp:comment_author_url", namespaceManager).InnerText;
            comment.Content = entry.SelectSingleNode("wp:comment_content", namespaceManager).InnerText;
            comment.PubDate = DateTime.Parse(ReadValue(entry.SelectSingleNode("wp:comment_date", namespaceManager), ""));
            comment.ID = entry.SelectSingleNode("wp:comment_id", namespaceManager).InnerText;
            comment.UserAgent = "n/a";
            post.Comments.Add(comment);
        }

        private string FormatFileReferences(string content)
        {
            foreach (Match match in rxFiles.Matches(content))
            {
                content = content.Replace(match.Groups[2].Value, "/posts/files/" + match.Groups[5].Value);
            }

            return content;
        }

        private static XmlNamespaceManager LoadNamespaceManager(XmlDocument docOrig)
        {
            XmlNamespaceManager nsm = new XmlNamespaceManager(docOrig.NameTable);
            nsm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            nsm.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
            return nsm;
        }

        private static XmlDocument LoadDocument(string file)
        {
            string doc = File.ReadAllText(file).Replace(" xmlns=\"urn:newtelligence-com:dasblog:runtime:data\"", string.Empty);

            XmlDocument docOrig = new XmlDocument();
            docOrig.LoadXml(doc);
            return docOrig;
        }

        private static string ReadValue(XmlNode node, string defaultValue = "")
        {
            if (node != null)
                return node.InnerText;

            return defaultValue;
        }

        private string FormatSlug(XmlNode node)
        {
            return FormatterHelpers.FormatSlug(node.InnerText);
        }

        private IEnumerable<string> FormatCategories(XmlNode catNode)
        {
            if (catNode == null || string.IsNullOrEmpty(catNode.InnerText))
                yield break;

            string[] categories = catNode.InnerText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string category in categories)
            {
                yield return category;
            }
        }

    }
}
