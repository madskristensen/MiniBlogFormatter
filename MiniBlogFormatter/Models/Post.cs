using System;
using System.Collections.Generic;
using System.Web;

public class Post
{
    public Post()
    {
        ID = Guid.NewGuid().ToString();
        Title = "My new post";
        Content = "the content";
        PubDate = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
        Categories = new string[0];
        Comments = new List<Comment>();
        IsPublished = true;
    }

    public string ID { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public string Slug { get; set; }

    public string Content { get; set; }

    public DateTime PubDate { get; set; }

    public DateTime LastModified { get; set; }

    public bool IsPublished { get; set; }

    public string[] Categories { get; set; }
    public List<Comment> Comments { get; set; }
}