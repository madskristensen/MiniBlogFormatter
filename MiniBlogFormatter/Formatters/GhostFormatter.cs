using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MiniBlogFormatter.Models;
using Newtonsoft.Json;

namespace MiniBlogFormatter.Formatters
{
    public class GhostFormatter
    {
        private readonly Regex imageRegex = new Regex(@"/content/images/", RegexOptions.Compiled);

        public void Format(string jsonFilePath, string targetFolderPath)
        {
            FormatPosts(jsonFilePath, targetFolderPath);
        }

        private void FormatPosts(string jsonFilePath, string targetFolderPath)
        {
            var rawJsonData = File.ReadAllText(jsonFilePath);
            var ghostData = JsonConvert.DeserializeObject<GhostData>(rawJsonData);

            foreach (var ghostPost in ghostData.data.posts)
            {
                var post = new Post
                {
                    Categories = GetPostCategories(ghostData, ghostPost).ToArray(),
                    Title = ghostPost.title,
                    Slug = ghostPost.slug,
                    PubDate = ghostPost.published_at.HasValue ? ConvertFromUnixEpoch(ghostPost.published_at.Value) : DateTime.Now,
                    LastModified = ghostPost.updated_at.HasValue ? ConvertFromUnixEpoch(ghostPost.updated_at.Value) : DateTime.Now,
                    Content = FormatFileReferences(ghostPost.html),
                    Author = GetPostAuthor(ghostData, ghostPost),
                    IsPublished = ghostPost.published_at.HasValue
                };

                var newFile = Path.Combine(targetFolderPath, ghostPost.uuid + ".xml");
                Storage.Save(post, newFile);
            }
        }

        private static DateTime ConvertFromUnixEpoch(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // We need to remove the last 3 digits of the unixtimeStamp otherwise it throws an overflow
            var timeStampString = unixTime.ToString(CultureInfo.CurrentCulture);
            timeStampString = timeStampString.Substring(0, timeStampString.Length - 3);
            var longTimestampFromString = long.Parse(timeStampString);

            return epoch.AddSeconds(longTimestampFromString);
        }

        private string FormatFileReferences(string content)
        {
            foreach (Match match in imageRegex.Matches(content))
            {
                content = content.Replace(match.Groups[0].Value, "/posts/files/" );
            }

            return content;
        }

        private static IEnumerable<string> GetPostCategories(GhostData ghostData, GhostPost post)
        {
            var postCategoryIds = ghostData.data.posts_tags.Where(t => t.post_id == post.id).Select(c => c.tag_id).ToList();
            return postCategoryIds.Select(id => ghostData.data.tags.FirstOrDefault(t => t.id == id).name).ToArray();
        }

        private static string GetPostAuthor(GhostData ghostData, GhostPost post)
        {
            return ghostData.data.users.FirstOrDefault(u => u.id == post.created_by).name;
        }
    }
}
