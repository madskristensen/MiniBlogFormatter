using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;

namespace MiniBlogFormatter
{
    public class DasBlogFormatter
    {
        private Regex rxFiles = new Regex("(href|src)=\"(([^\"]+)(/content/binary/)([^\"]+))\"", RegexOptions.IgnoreCase);

        public void Format(string originalFolderPath, string targetFolderPath)
        {
            FormatPosts(originalFolderPath, targetFolderPath);
            FormatComments(originalFolderPath, targetFolderPath);
        }

        private void FormatPosts(string originalFolderPath, string targetFolderPath)
        {
            foreach (string file in Directory.GetFiles(originalFolderPath, "*.xml"))
            {
                if (!file.EndsWith("dayentry.xml", StringComparison.OrdinalIgnoreCase))
                    continue;

                XmlDocument docOrig = LoadDocument(file);
                XmlNamespaceManager nsm = LoadNamespaceManager(docOrig);

                foreach (XmlNode entry in docOrig.SelectNodes("//Entry", nsm))
                {
                    Post post = new Post();
                    post.Categories = FormatCategories(entry.SelectSingleNode("Categories")).ToArray();
                    post.Title = entry.SelectSingleNode("Title").InnerText;
                    post.Slug = FormatterHelpers.FormatSlug(post.Title);
                    post.PubDate = DateTime.Parse(entry.SelectSingleNode("//Created").InnerText);
                    post.LastModified = DateTime.Parse(entry.SelectSingleNode("//Modified").InnerText);
                    post.Content = FormatFileReferences(entry.SelectSingleNode("Content").InnerText);
                    post.Author = entry.SelectSingleNode("Author").InnerText;
                    post.IsPublished = bool.Parse(ReadValue(entry.SelectSingleNode("IsPublic"), "true"));

                    string newFile = Path.Combine(targetFolderPath, entry.SelectSingleNode("EntryId").InnerText + ".xml");
                    Storage.Save(post, newFile);
                }
            }
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

        private void FormatComments(string originalFolderPath, string targetFolderPath)
        {
            foreach (string file in Directory.GetFiles(originalFolderPath, "*.xml"))
            {
                if (!file.EndsWith("dayfeedback.xml", StringComparison.OrdinalIgnoreCase))
                    continue;

                XmlDocument docOrig = LoadDocument(file);
                XmlNamespaceManager nsm = LoadNamespaceManager(docOrig);

                foreach (XmlNode entry in docOrig.SelectNodes("//Comment", nsm))
                {
                    XmlNode idNode = entry.SelectSingleNode("TargetEntryId");
                    string postFile = Path.Combine(targetFolderPath, idNode.InnerText + ".xml");

                    if (!File.Exists(postFile))
                        continue;

                    bool isSpam = ReadValue(entry.SelectSingleNode("SpamState"), "n/a") == "Spam";
                    if (isSpam)
                        continue;

                    Post post = Storage.LoadPost(postFile);

                    Comment comment = new Comment();
                    comment.Author = ReadValue(entry.SelectSingleNode("Author"), "n/a");
                    comment.Email = ReadValue(entry.SelectSingleNode("AuthorEmail"), "");
                    comment.Ip = entry.SelectSingleNode("AuthorIPAddress").InnerText;
                    comment.Website = entry.SelectSingleNode("AuthorHomepage").InnerText;
                    comment.Content = entry.SelectSingleNode("Content").InnerText;
                    comment.PubDate = DateTime.Parse(ReadValue(entry.SelectSingleNode("Created"), ""));
                    comment.ID = entry.SelectSingleNode("EntryId").InnerText;
                    comment.UserAgent = ReadValue(entry.SelectSingleNode("AuthorUserAgent"), "");

                    bool isOpenId = bool.Parse(ReadValue(entry.SelectSingleNode("OpenId"), "false"));

                    if (isOpenId)
                    {
                        comment.Author = "n/a";
                    }

                    post.Comments.Add(comment);

                    Storage.Save(post, postFile);
                }
            }
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
