using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using GistIntegration;
using Westwind.Utilities;

namespace PasteCodeAsGistAddinTests
{
    [TestClass]
    public class PasteCodeTests
    {

        [TestMethod]
        public void PostJsonTest()
        {
            var addin = new GistIntegration.PasteCodeAsGistAddin();

            var gist = new GistItem
            {
                code = "int x = 11102;",
                description = "Test addin code",
                filename = "test.cs",
                isPublic = true
            };
            var result = GistClient.PostGist(gist,PasteCodeAsGistConfiguration.Current.GithubUserToken);

            Assert.IsNotNull(result,"Gist is null");

            Console.WriteLine(result.htmlUrl);
            Console.WriteLine(result.id);
            Console.Write(result.embedUrl);

            ShellUtils.GoUrl(result.htmlUrl);
        }

        [TestMethod]
        public void GetGistTest()
        {
            var gistId = "ef851ce1597b97ee0c2dba06a858db07";

            var gist = GistClient.GetGistFromServer(gistId);

            Assert.IsFalse(gist.hasError);

            Console.WriteLine(JsonSerializationUtils.Serialize(gist, formatJsonOutput: true));
        }

        [TestMethod]
        public void GetMissingGistTest()
        {
            var gistId = "bogus";

            var gist = GistClient.GetGistFromServer(gistId);

            Assert.IsTrue(gist.hasError);

            Console.WriteLine(JsonSerializationUtils.Serialize(gist, formatJsonOutput: true));
        }

        [TestMethod]
        public void GetGistListForUserTest()
        {
            
            var gistList = GistClient.ListGistsForUser("rickstrahl");

            Assert.IsNotNull(gistList);
            Assert.IsTrue(gistList.Count > 0);

            Console.WriteLine(JsonSerializationUtils.Serialize(gistList, formatJsonOutput: true));
        }

        [TestMethod]
        public void CreateJson()
        {
            string code = "int x = 10002;";
            string description = "test snippet description";
            string fname = "test.cs";
            bool isPublic = true;


            dynamic obj = new JObject();

            obj.Add("description", new JValue(description));
            obj.Add("public", new JValue(true));
            obj.Add("files", new JObject());

            obj.files.Add(fname, new JObject());

            var fileObj = obj.files[fname];
            fileObj.content = code;

            Console.WriteLine(obj);

        }

        [TestMethod]
        public void CreateGistTest()
        {
            var json = @"{
  ""description"": ""the description for this gist"",
  ""public"": true,
  ""files"": {
		""file1.txt"": {
			""content"": ""String file contents""
	    }
	}
}";
            var settings = new HttpRequestSettings
            {
                Url = "https://api.github.com/gists",
                HttpVerb = "POST",
                Content = json,
                ContentType = "application/json; charset=utf-8;",
            };
            settings.Headers.Add("User-agent", "Markdown Monster Markdown Editor Gist Add-in");
            settings.Headers.Add("Accept", "application/json");

            var result = HttpUtils.HttpRequestString(settings);

            

            dynamic jsn = JValue.Parse(result);

            string htmlUrl = jsn.html_url;
            Console.WriteLine(htmlUrl);

            string id = jsn.id;
            Console.WriteLine(id);

            JObject files = jsn.files;

            JProperty fileProp = files.First as JProperty;
            dynamic fileObj = fileProp.Value as JObject;

            string rawUrl = fileObj.raw_url;
            string filename = fileObj.filename;

            
            Console.WriteLine(rawUrl);
            Console.WriteLine(filename);

            ShellUtils.GoUrl(htmlUrl);
        }
    }
}
