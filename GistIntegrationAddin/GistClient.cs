using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Westwind.Utilities;

namespace GistIntegration
{

    public class GistClient
    {
        public const string GistUrl = "https://api.github.com";

        #region Post Gist

        public static GistItem PostGist(GistItem gist, string githubUserToken = null)
        {
            if (string.IsNullOrEmpty(githubUserToken))
                githubUserToken = PasteCodeAsGistConfiguration.Current.GithubUserToken;

            var json = CreateGistPostJson(gist);
            if (json == null)
                return null;

            var settings = new HttpRequestSettings
            {
                Url = GistUrl + "/gists",
                HttpVerb = "POST",
                Content = json.ToString(),
                ContentType = "application/json; charset=utf-8;"
            };
            settings.Headers.Add("User-agent", "Markdown Monster Markdown Editor Gist Add-in");
            settings.Headers.Add("Accept", "application/json");

            if (!gist.isAnonymous && !string.IsNullOrEmpty(githubUserToken))
                settings.Headers.Add("Authorization", "token " + githubUserToken);

            string result = null;
            try
            {
                result = HttpUtils.HttpRequestString(settings);
            }
            catch (Exception ex)
            {
                gist.hasError = true;
                gist.errorMessage = "Gist upload failed: " + ex.Message;
                return gist;
            }

            dynamic jsn = JValue.Parse(result);
            gist.htmlUrl = jsn.html_url;
            gist.id = jsn.id;


            JObject files = jsn.files;
            JProperty fileProp = files.First as JProperty;
            dynamic fileObj = fileProp.Value as JObject;

            gist.rawUrl = fileObj.raw_url;
            gist.filename = fileObj.filename;

            dynamic user = jsn.owner;
            if (user != null)
                gist.username = user.login;
            
            return gist;
        }


        /// <summary>
        /// Updates an existing Gist
        /// </summary>
        /// <param name="gist"></param>
        /// <param name="githubUserToken"></param>
        /// <returns></returns>
        public static GistItem UpdateGist(GistItem gist, string githubUserToken = null)
        {
            if (string.IsNullOrEmpty(githubUserToken))
                githubUserToken = PasteCodeAsGistConfiguration.Current.GithubUserToken;

            var json = CreateGistPostJson(gist);
            if (json == null)
                return null;

            var settings = new HttpRequestSettings
            {
                Url = GistUrl + $"/gists/{gist.id}" ,
                HttpVerb = "PATCH",
                Content = json.ToString(),
                ContentType = "application/json; charset=utf-8;"
            };
            settings.Headers.Add("User-agent", "Markdown Monster Markdown Editor Gist Add-in");
            settings.Headers.Add("Accept", "application/json");

            if (!gist.isAnonymous && !string.IsNullOrEmpty(githubUserToken))
                settings.Headers.Add("Authorization", "token " + githubUserToken);

            string result = null;
            try
            {
                result = HttpUtils.HttpRequestString(settings);
            }
            catch (Exception ex)
            {
                gist.hasError = true;
                gist.errorMessage = "Gist upload failed: " + ex.Message;
                return gist;
            }

            dynamic jsn = JValue.Parse(result);
            gist.htmlUrl = jsn.html_url;
            gist.id = jsn.id;


            JObject files = jsn.files;
            JProperty fileProp = files.First as JProperty;
            dynamic fileObj = fileProp.Value as JObject;

            gist.rawUrl = fileObj.raw_url;
            gist.filename = fileObj.filename;

            dynamic user = jsn.owner;
            if (user != null)
                gist.username = user.login;

            return gist;
        }

        /// <summary>
        /// Creates a JSON object from a Gist that contains only 
        /// partial Gist data required to post to a server
        /// </summary>
        /// <param name="gist"></param>
        /// <returns></returns>
        public static JObject CreateGistPostJson(GistItem gist)
        {
            dynamic obj = new JObject();

            obj.Add("description", new JValue(gist.description));
            obj.Add("public", new JValue(true));
            obj.Add("files", new JObject());

            obj.files.Add(gist.filename, new JObject());

            var fileObj = obj.files[gist.filename];
            fileObj.content = gist.code;

            return obj;
        }


        /// <summary>
        /// Retrieves a Gist from the server
        /// </summary>
        /// <param name="gistId"></param>
        /// <param name="githubToken"></param>
        /// <returns></returns>
        public static GistItem GetGistFromServer(string gistId, string githubToken = null)
        {
            var settings = new HttpRequestSettings
            {
                Url = GistUrl + "/gists/" + gistId,
                HttpVerb = "GET"
            };
            settings.Headers.Add("User-agent", "Markdown Monster Markdown Editor Gist Add-in");
            settings.Headers.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(githubToken))
                settings.Headers.Add("Authorization", "token " + githubToken);

            GistItem gist;
            try
            {
                var giststruct = HttpUtils.JsonRequest<GistStructure>(settings);

                var file = giststruct.files.FirstOrDefault().Value;

                gist = new GistItem()
                {
                    description = giststruct.description,
                    code = file.content,
                    filename = file.filename,
                    rawUrl = giststruct.url,
                    htmlUrl = giststruct.html_url,
                    username = giststruct.owner?.login,
                    isPublic = giststruct._public,
                    updated = giststruct.updated_at,
                    id = giststruct.id
                };
                if (string.IsNullOrEmpty(gist.username))
                    gist.isAnonymous = true;
            }            
            catch (Exception ex)
            {
                gist = new GistItem()
                {
                    hasError = true,
                    errorMessage = "Gist retrieval failed: " + ex.Message
                };
            }

            return gist;
        }


        /// <summary>
        /// checks to see if a given gist exists by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="githubUserToken"></param>
        /// <returns></returns>
        public bool DoesGistExist(string id, string githubUserToken = null)
        {
            var gist = GetGistFromServer(id, githubUserToken);
            if (gist.hasError)            
                return false;

            return true;
        }

        /// <summary>
        /// Retrieves a list of recent gists from a given user
        /// </summary>
        /// <param name="userId"></param>
        public static List<GistItem> ListGistsForUser(string userId, string githubToken = null)
        {            
            var settings = new HttpRequestSettings
            {
                Url = GistUrl + $"/users/{userId}/gists",
                HttpVerb = "GET"
            };
            settings.Headers.Add("User-agent", "Markdown Monster Markdown Editor Gist Add-in");
            settings.Headers.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(githubToken))
                settings.Headers.Add("Authorization", "token " + githubToken);

            List<GistItem> gists = new List<GistItem>();
            try
            {
                var giststructs = HttpUtils.JsonRequest<List<GistStructure>>(settings);

                foreach (var giststruct in giststructs)
                {
                    var file = giststruct.files.FirstOrDefault().Value;

                    var gist = new GistItem()
                    {
                        description = giststruct.description,                        
                        filename = file.filename,
                        rawUrl = giststruct.url,
                        htmlUrl = giststruct.html_url,
                        username = giststruct.owner?.login,
                        isPublic = giststruct._public,
                        updated = giststruct.updated_at,
                        id = giststruct.id
                    };
                    if (string.IsNullOrEmpty(gist.username))
                        gist.isAnonymous = true;                    

                    gists.Add(gist);
                }
            }
            catch (Exception ex)
            {
                var gist = new GistItem()
                {
                    hasError = true,
                    errorMessage = "Gist retrieval failed: " + ex.Message
                };
                gists.Add(gist);
            }

            return gists;
        }
        #endregion
    }


}

