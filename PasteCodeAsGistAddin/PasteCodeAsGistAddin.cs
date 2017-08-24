using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                Caption = "Gist",

                // if an icon is specified it shows on the toolbar
                // if not the add-in only shows in the add-ins menu
                FontawesomeIcon = FontAwesomeIcon.Github,
            };

            // if you don't want to display main or config menu items clear handler
            //menuItem.ExecuteConfiguration = null;

            // Must add the menu to the collection to display menu and toolbar items            
            MenuItems.Add(menuItem);


            //var menuItem2 = new AddInMenuItem(this)
            //{
            //    Caption = "Save and Load from Gist",

            //    // if an icon is specified it shows on the toolbar
            //    // if not the add-in only shows in the add-ins menu
            //    FontawesomeIcon = FontAwesomeIcon.GithubSquare,
            //    Execute = OnExecuteSaveAndLoadGist
            //};            
            //MenuItems.Add(menuItem2);
            
        }

        public override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            

            // create and add custom menu item
            var mitemOpen = new MenuItem()
            {
                Header = "Open from Gist",
                Name = "ButtonOpenFromGist"
            };
            mitemOpen.Click += (s, a) => OnExecuteLoadGist();
            if (!AddMenuItem(mitemOpen, menuItemName: "ButtonOpenFromHtml", mode: 0))
                mmApp.Log("Unable to add custom menu item in Paste Code As Gist Addin: " + mitemOpen.Name);

            var mitemSave = new MenuItem()
            {
                Header = "Save to Gist",
                Name = "ButtonSaveToGist"
            };
            mitemSave.Click += (s, a) => OnExecuteSaveGist();
            if (!AddMenuItem(mitemSave, menuItemName: "ButtonGeneratePdf", mode: 0))
                mmApp.Log("Unable to add custom menu item in Paste Code As Gist Addin: " + mitemSave.Name);
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

        void OnExecuteLoadGist()
        {
            var form = new LoadGistWindow(this)
            {
                Owner = Model.Window
            };
            form.Show();
        }

        void OnExecuteSaveGist()
        {
            var form = new SaveGistWindow(this);
            form.Owner = Model.Window;            
            form.Show();
        }

        public override void OnExecuteConfiguration(object sender)
        {
            // show the config file
            Model.Window.OpenTab(Path.Combine(mmApp.Configuration.CommonFolder, "PasteCodeAsGistAddin.json"));                
        }
        
        public override void OnApplicationShutdown()
        {
            base.OnApplicationShutdown();

            // ensure config file is updated with current settings
            PasteCodeAsGistConfiguration.Current.Write();
        }

        #endregion


    }
}
