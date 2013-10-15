using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

public static class Storage
{
    public static void Save(Post post, string file)
    {
        post.LastModified = DateTime.UtcNow;

        XDocument doc = new XDocument(
                        new XElement("post",
                            new XElement("title", post.Title),
                            new XElement("slug", post.Slug),
                            new XElement("author", post.Author),
                            new XElement("pubDate", post.PubDate.ToString("yyyy-MM-dd HH:mm:ss")),
                            new XElement("lastModified", post.LastModified.ToString("yyyy-MM-dd HH:mm:ss")),
                            new XElement("content", post.Content),
                            new XElement("ispublished", post.IsPublished),
                            new XElement("categories", string.Empty),
                            new XElement("comments", string.Empty)
                        ));

        XElement categories = doc.XPathSelectElement("post/categories");
        foreach (string category in post.Categories)
        {
            categories.Add(new XElement("category", category));
        }

        XElement comments = doc.XPathSelectElement("post/comments");
        foreach (Comment comment in post.Comments)
        {
            comments.Add(
                new XElement("comment",
                    new XElement("author", comment.Author),
                    new XElement("email", comment.Email),
                    new XElement("website", comment.Website),
                    new XElement("ip", comment.Ip),
                    new XElement("userAgent", comment.UserAgent),
                    new XElement("date", comment.PubDate.ToString("yyyy-MM-dd HH:m:ss")),
                    new XElement("content", comment.Content),
                    new XAttribute("isAdmin", comment.IsAdmin),
                    new XAttribute("id", comment.ID)
                ));
        }

        doc.Save(file);
    }

    public static Post LoadPost(string file)
    {
        XElement doc = XElement.Load(file);

        Post post = new Post()
        {
            ID = Path.GetFileNameWithoutExtension(file),
            Title = ReadValue(doc, "title"),
            Author = ReadValue(doc, "author"),
            Content = ReadValue(doc, "content"),
            Slug = ReadValue(doc, "slug").ToLowerInvariant(),
            PubDate = DateTime.Parse(ReadValue(doc, "pubDate")),
            LastModified = DateTime.Parse(ReadValue(doc, "lastModified", DateTime.Now.ToString())),
            IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
        };

        return post;
    }

    //private static void LoadCategories(Post post, XElement doc)
    //{
    //    XElement categories = doc.Element("categories");
    //    if (categories == null)
    //        return;

    //    List<string> list = new List<string>();

    //    foreach (var node in categories.Elements("category"))
    //    {
    //        list.Add(node.Value);
    //    }

    //    post.Categories = list.ToArray();
    //}
    //private static void LoadComments(Post post, XElement doc)
    //{
    //    var comments = doc.Element("comments");

    //    if (comments == null)
    //        return;

    //    foreach (var node in comments.Elements("comment"))
    //    {
    //        Comment comment = new Comment()
    //        {
    //            ID = ReadAttribute(node, "id"),
    //            Author = ReadValue(node, "author"),
    //            Email = ReadValue(node, "email"),
    //            Website = ReadValue(node, "website"),
    //            Ip = ReadValue(node, "ip"),
    //            UserAgent = ReadValue(node, "userAgent"),
    //            IsAdmin = bool.Parse(ReadAttribute(node, "isAdmin", "false")),
    //            Content = ReadValue(node, "content").Replace("\n", "<br />"),
    //            PubDate = DateTime.Parse(ReadValue(node, "date", "2000-01-01")),
    //        };

    //        post.Comments.Add(comment);
    //    }
    //}

    private static string ReadValue(XElement doc, XName name, string defaultValue = "")
    {
        if (doc.Element(name) != null)
            return doc.Element(name).Value;

        return defaultValue;
    }

    private static string ReadAttribute(XElement element, XName name, string defaultValue = "")
    {
        if (element.Attribute(name) != null)
            return element.Attribute(name).Value;

        return defaultValue;
    }
}