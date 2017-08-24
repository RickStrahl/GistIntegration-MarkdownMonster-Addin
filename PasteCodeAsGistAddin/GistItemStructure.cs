using System;
using System.Collections.Generic;

namespace PasteCodeAsGistAddin
{ 
        public class GistStructure
        {
            public string url { get; set; }
            public string forks_url { get; set; }
            public string commits_url { get; set; }
            public string id { get; set; }
            public string git_pull_url { get; set; }
            public string git_push_url { get; set; }
            public string html_url { get; set; }
            public Dictionary<string, GistFileInfo> files { get; set; }
            public bool _public { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public string description { get; set; }
            public int comments { get; set; }
            public object user { get; set; }
            public string comments_url { get; set; }
            public Owner owner { get; set; }
            public Fork[] forks { get; set; }
            public History[] history { get; set; }
            public bool truncated { get; set; }
        }

        public class GistFileInfo
        {
            public string filename { get; set; }
            public string type { get; set; }
            public string language { get; set; }
            public string raw_url { get; set; }
            public int size { get; set; }
            public bool truncated { get; set; }
            public string content { get; set; }
        }

        public class Owner
        {
            public string login { get; set; }
            public int id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class Fork
        {
            public string url { get; set; }
            public User user { get; set; }
            public string id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class User
        {
            public string login { get; set; }
            public int id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class History
        {
            public User1 user { get; set; }
            public string version { get; set; }
            public DateTime committed_at { get; set; }
            public Change_Status change_status { get; set; }
            public string url { get; set; }
        }

        public class User1
        {
            public string login { get; set; }
            public int id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class Change_Status
        {
            public int total { get; set; }
            public int additions { get; set; }
            public int deletions { get; set; }
        }
}