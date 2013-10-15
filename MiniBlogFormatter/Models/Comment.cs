using System;

public class Comment
{
    public Comment()
    {
        ID = Guid.NewGuid().ToString();
        PubDate = DateTime.UtcNow;
    }

    public string ID { get; set; }
    public string Author { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string Content { get; set; }
    public DateTime PubDate { get; set; }
    public string Ip { get; set; }
    public string UserAgent { get; set; }
    public bool IsAdmin { get; set; }
}