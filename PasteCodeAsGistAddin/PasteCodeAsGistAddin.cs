using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FontAwesome.WPF;
using MarkdownMonster.AddIns;
using Newtonsoft.Json.Linq;
using PasteCodeAsGitAddin;
using Westwind.Utilities;

namespace PasteCodeAsGistAddin
{
    public class PasteCodeAsGistAddin : MarkdownMonsterAddin
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            Id = "PasteCodeAsGistAddin";

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
                code = editor.AceEditor.getselection(false)                
            };

            var form = new PasteCodeAsGitWindow();            
            form.Owner = Model.Window;
            form.Gist = gist;
            form.ShowDialog();

            if (form.Cancelled)
                return;

            


            MessageBox.Show("Hello from your sample Addin", "Markdown Addin Sample",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public override void OnExecuteConfiguration(object sender)
        {
            MessageBox.Show("Configuration for our sample Addin", "Markdown Addin Sample",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public override bool OnCanExecute(object sender)
        {
            return true;
        }

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
                ContentType = "application/json; charset=utf-8;",
            };
            settings.Headers.Add("User-agent", "Markdown Monster Markdown Editor Gist Add-in");
            settings.Headers.Add("Accept", "application/json");

            var result = HttpUtils.HttpRequestString(settings);

            Console.WriteLine(result);

            dynamic jsn = JValue.Parse(result);
            gist.htmlUrl = jsn.html_url;
            gist.id = jsn.id;

            
            JObject files = jsn.files;
            JProperty fileProp = files.First as JProperty;
            dynamic fileObj = fileProp.Value as JObject;

            gist.rawUrl = fileObj.raw_url;
            gist.filename = fileObj.filename;

            dynamic user = jsn.user;
            if (user != null)
                gist.username = user.login;

            if(!string.IsNullOrEmpty(gist.username))
                gist.embedUrl = $"https://gist.github.com/{gist.username}/{gist.id}.js";
            else
                gist.embedUrl = $"https://gist.github.com/anonymous/{gist.id}.js";

            return gist;
        }
    }


    public class GistItem
    {
        
        public string code { get; set; }
        public string description  { get; set; }
        public string filename { get; set; }
        public bool isPublic  { get; set; }
        
        
        public string id { get; set; }
        public string htmlUrl { get; set; }
        public string rawUrl { get; set; }
        public string embedUrl { get; set; }

        public string username { get; set; }
    }
}
