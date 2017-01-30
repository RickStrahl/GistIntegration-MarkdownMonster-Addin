using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using FontAwesome.WPF;
using MarkdownMonster;
using MarkdownMonster.AddIns;
using Newtonsoft.Json.Linq;
using PasteCodeAsGitAddin;
using Westwind.Utilities;

namespace PasteCodeAsGistAddin
{
    public class PasteCodeAsGistAddin : MarkdownMonsterAddin
    {

        #region Addin Overrides
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            Id = "PasteCodeAsGist";

            // by passing in the add in you automatically
            // hook up OnExecute/OnExecuteConfiguration/OnCanExecute
            var menuItem = new AddInMenuItem(this)
            {
                Caption = "Paste Code as Gist",

                // if an icon is specified it shows on the toolbar
                // if not the add-in only shows in the add-ins menu
                FontawesomeIcon = FontAwesomeIcon.Github,
            };

            // if you don't want to display main or config menu items clear handler
            //menuItem.ExecuteConfiguration = null;

            // Must add the menu to the collection to display menu and toolbar items            
            this.MenuItems.Add(menuItem);
        }


        public override void OnExecute(object sender)
        {
            var editor = GetMarkdownEditor();
            if (editor == null)
                return;
            
            var gist = new GistItem()
            {
                code = editor.AceEditor.getselection(false),
                language = "cs"
            };

            var form = new PasteCodeAsGitWindow(this);            
            form.Owner = Model.Window;
            form.Gist = gist;
            form.ShowDialog();

            if (form.Cancelled)
                return;

            if (!string.IsNullOrEmpty(gist.embedUrl))
                editor.SetSelectionAndFocus($"<script src=\"{gist.embedUrl}\"></script>\r\n");

            Model.Window.ShowStatus("Gist embedded", 5000);
            Model.Window.SetStatusIcon(FontAwesomeIcon.GithubAlt, Colors.Green);
        }

        public override void OnExecuteConfiguration(object sender)
        {
            // show the config file
            Model.Window.OpenTab(Path.Combine(mmApp.Configuration.CommonFolder, "PasteCodeAsGistAddin.json"));                
        }
        
        public override bool OnCanExecute(object sender)
        {
            return Model.IsEditorActive;
        }

        public override void OnApplicationShutdown()
        {
            base.OnApplicationShutdown();

            // ensure config file is updated with current settings
            PasteCodeAsGistConfiguration.Current.Write();
        }

        #endregion


        #region Post Gist
        public JObject CreateGistPostJson(GistItem gist)
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

        public GistItem PostGist(GistItem gist)
        {
            var json = CreateGistPostJson(gist);
            if (json == null)
                return null;

            var settings = new HttpRequestSettings
            {
                Url = "https://api.github.com/gists",
                HttpVerb = "POST",
                Content = json.ToString(),
                ContentType = "application/json; charset=utf-8;"                
            };                       
            settings.Headers.Add("User-agent", "Markdown Monster Markdown Editor Gist Add-in");
            settings.Headers.Add("Accept", "application/json");
            
            if (!gist.isAnonymous && !string.IsNullOrEmpty(PasteCodeAsGistConfiguration.Current.GithubUserToken))
                settings.Headers.Add("Authorization", "token " +PasteCodeAsGistConfiguration.Current.GithubUserToken);

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

            gist.embedUrl = gist.htmlUrl + ".js";            
            return gist;
        }
        #endregion
    }
}
