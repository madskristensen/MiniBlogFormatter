namespace MiniBlogFormatter.Models
{
    public class GhostData
    {
        public Meta meta { get; set; }
        public Data data { get; set; }
    }

    public class Meta
    {
        public long exported_on { get; set; }
        public string version { get; set; }
    }

    public class Data
    {
        public GhostPost[] posts { get; set; }
        public User[] users { get; set; }
        public Role[] roles { get; set; }
        public Roles_Users[] roles_users { get; set; }
        public Permission[] permissions { get; set; }
        public object[] permissions_users { get; set; }
        public Permissions_Roles[] permissions_roles { get; set; }
        public Setting[] settings { get; set; }
        public Tag[] tags { get; set; }
        public Posts_Tags[] posts_tags { get; set; }
    }

    public class GhostPost
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string markdown { get; set; }
        public string html { get; set; }
        public object image { get; set; }
        public int featured { get; set; }
        public int page { get; set; }
        public string status { get; set; }
        public string language { get; set; }
        public object meta_title { get; set; }
        public object meta_description { get; set; }
        public int? author_id { get; set; }
        public long? created_at { get; set; }
        public int? created_by { get; set; }
        public long? updated_at { get; set; }
        public int? updated_by { get; set; }
        public long? published_at { get; set; }
        public int? published_by { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public object image { get; set; }
        public object cover { get; set; }
        public string bio { get; set; }
        public string website { get; set; }
        public string location { get; set; }
        public object accessibility { get; set; }
        public string status { get; set; }
        public string language { get; set; }
        public object meta_title { get; set; }
        public object meta_description { get; set; }
        public object last_login { get; set; }
        public long? created_at { get; set; }
        public int? created_by { get; set; }
        public long? updated_at { get; set; }
        public int? updated_by { get; set; }
    }

    public class Role
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public long? created_at { get; set; }
        public int? created_by { get; set; }
        public long? updated_at { get; set; }
        public int? updated_by { get; set; }
    }

    public class Roles_Users
    {
        public int id { get; set; }
        public int? role_id { get; set; }
        public int? user_id { get; set; }
    }

    public class Permission
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string name { get; set; }
        public string object_type { get; set; }
        public string action_type { get; set; }
        public object object_id { get; set; }
        public long? created_at { get; set; }
        public int? created_by { get; set; }
        public long? updated_at { get; set; }
        public int? updated_by { get; set; }
    }

    public class Permissions_Roles
    {
        public int id { get; set; }
        public int? role_id { get; set; }
        public int? permission_id { get; set; }
    }

    public class Setting
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public long? created_at { get; set; }
        public int? created_by { get; set; }
        public long? updated_at { get; set; }
        public int? updated_by { get; set; }
    }

    public class Tag
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public object description { get; set; }
        public object parent_id { get; set; }
        public object meta_title { get; set; }
        public object meta_description { get; set; }
        public long? created_at { get; set; }
        public int? created_by { get; set; }
        public long? updated_at { get; set; }
        public int? updated_by { get; set; }
    }

    public class Posts_Tags
    {
        public int id { get; set; }
        public int? post_id { get; set; }
        public int? tag_id { get; set; }
    }
}
