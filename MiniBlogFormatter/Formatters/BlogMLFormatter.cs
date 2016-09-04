using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MiniBlogFormatter
{
    public class BlogMLFormatter
    {
        XNamespace ns = "http://www.blogml.com/2006/09/BlogML";

        public void Format(string fileName, string targetFolderPath)
        {
            XDocument doc = XDocument.Load(fileName);

            Dictionary<string, string> authors = doc.Root.Element(ns + "authors").Elements(ns + "author").ToDictionary(x => x.Attribute("id").Value, x => x.Element(ns + "title").Value);
            Dictionary<string, string> categories = doc.Root.Element(ns + "categories").Elements(ns + "category").ToDictionary(x => x.Attribute("id").Value, x => x.Element(ns + "title").Value);

            foreach (XElement postData in doc.Root.Element(ns + "posts").Elements(ns + "post"))
            {
                Post post = ParsePost(postData, authors, categories);

                Storage.Save(post, Path.Combine(targetFolderPath, post.ID + ".xml"));
            }
        }

        public void ConvertFromBase64(string fileName, string targetFileName)
        {
            XDocument doc = XDocument.Load(fileName);
            var query = doc.Descendants(ns + "content")
                           .Where(node => (string)node.Attribute("type") == "base64");
            foreach (var item in query)
            {
                var content = Encoding.UTF8.GetString(Convert.FromBase64String(item.Value));

                item.SetValue(content);
            }

            doc.Save(targetFileName);
        }

        Post ParsePost(XElement postData, Dictionary<string, string> authors, Dictionary<string, string> categories)
        {
            Post post = new Post()
            {
                Title = postData.Element(ns + "title").Value,
                Content = postData.Element(ns + "content").Value,
                PubDate = (DateTime)postData.Attribute("date-created"),
                LastModified = (DateTime)postData.Attribute("date-modified"),
                IsPublished = (bool)postData.Attribute("approved"),
                Categories = postData.Descendants(ns + "category").Select(x => categories[x.Attribute("ref").Value]).ToArray()
            };

            XElement name = postData.Element(ns + "post-name");

            if (name != null)
                post.Slug = name.Value;
            else
                post.Slug = FormatterHelpers.FormatSlug(post.Title);

            XElement author = postData.Descendants(ns + "author").FirstOrDefault();

            if (author != null)
            {
                var thisArthor = author.Attribute("ref").Value;
                foreach(var thisKey in authors.Keys)
                {
                    // Fixes issue when author name has different cases from the Key value of Authors
                    if(thisArthor.Equals(thisKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        post.Author = authors[thisKey];
                    }
                }
            }

            foreach (XElement commentData in postData.Descendants(ns + "comment"))
            {
                if (commentData.Attribute("approved").Value == "true")
                {
                    post.Comments.Add(new Comment()
                    {
                        ID = commentData.Attribute("id").Value,
                        PubDate = (DateTime)commentData.Attribute("date-created"),
                        Content = commentData.Element(ns + "content").Value,
                        Author = (string)commentData.Attribute("user-name"),
                        Website = (string)commentData.Attribute("user-url"),
                        Email = (string)commentData.Attribute("user-email"),
                    });
                }
            }

            return post;
        }
    }
}
