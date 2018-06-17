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

namespace GistIntegration
{
    public class PasteCodeAsGistAddin : MarkdownMonsterAddin
    {
        private PasteCodeAsGitWindow PasteCodeAsGistWindow;
        private AddInMenuItem AddinMenuItem;

        #region Addin Overrides
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            Id = "GistIntegration";

            // by passing in the add in you automatically
            // hook up OnExecute/OnExecuteConfiguration/OnCanExecute
            AddinMenuItem = new AddInMenuItem(this)
            {
                Caption = "Gist Integration",

                // if an icon is specified it shows on the toolbar
                // if not the add-in only shows in the add-ins menu
                FontawesomeIcon = FontAwesomeIcon.Github,
                FontawesomeIconColor = "#247CAC"

                //IconImageSource = new ImageSourceConverter()
                //    .ConvertFromString("pack://application:,,,/GistIntegrationAddin;component/icon_22.png") as ImageSource                
            };

          
            // Must add the menu to the collection to display menu and toolbar items            
            MenuItems.Add(AddinMenuItem);            
        }

        public override void OnWindowLoaded()
        {
            base.OnWindowLoaded();

            //var mButtonContent = AddinMenuItem.MenuItemButton?.Content;
            //if (mButtonContent != null)
            //{
            //    var image = (Image) mButtonContent;
            //    //image.Source = ImageAwesome.CreateImageSource(FontAwesomeIcon.Github, new BrushConverter().ConvertFrom("#247CAC") as Brush);
            //    image.Height = 22;
            //    image.Width = 22;
            //}

            // create and add custom menu item
            var mitemOpen = new MenuItem()
            {
                Header = "Open from Gist",
                Name = "ButtonOpenFromGist"
            };
            mitemOpen.Click += (s, a) => OnExecuteLoadGist();            
            if (!AddMenuItem(mitemOpen, menuItemNameForInsertionAfter: "ButtonOpenFromHtml", mode: 0))
                mmApp.Log("Unable to add custom menu item in Paste Code As Gist Addin: " + mitemOpen.Name);

            var mitemSave = new MenuItem()
            {
                Header = "Save to Gist",
                Name = "ButtonSaveToGist"
            };
            mitemSave.Click += (s, a) => OnExecuteSaveGist();
            if (!AddMenuItem(mitemSave, menuItemNameForInsertionAfter: "ButtonGeneratePdf", mode: 0))
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

            PasteCodeAsGistWindow = new PasteCodeAsGitWindow(this);
            PasteCodeAsGistWindow.Owner = Model.Window;
            PasteCodeAsGistWindow.Gist = gist;
            PasteCodeAsGistWindow.ShowDialog();

            if (PasteCodeAsGistWindow.Cancelled)
                return;

            if (!string.IsNullOrEmpty(gist.embedUrl))
                editor.SetSelectionAndFocus($"<script src=\"{gist.embedUrl}\"></script>\r\n");

            Model.Window.ShowStatus("Gist embedded", 5000);
            Model.Window.SetStatusIcon(FontAwesomeIcon.GithubAlt, Colors.Green);
        }

        public void OnExecuteLoadGist()
        {
            var form = new LoadGistWindow(this);
            form.Owner = Model.Window;            
            form.Show();
        }

        public void OnExecuteSaveGist()
        {
            var form = new SaveGistWindow(this);
            form.Owner = Model.Window;            
            form.Show();
        }

        public override void OnExecuteConfiguration(object sender)
        {            
            OpenGistConfigurationTab();
        }
        
        public override void OnApplicationShutdown()
        {
            base.OnApplicationShutdown();

            // ensure config file is updated with current settings
            PasteCodeAsGistConfiguration.Current.Write();
        }

        #endregion

        #region Addin Helpers
        /// <summary>
        /// Opens the configuration tab to configure Gists
        /// </summary>
        public void OpenGistConfigurationTab()
        {
            mmApp.Model.Window.OpenTab(System.IO.Path.Combine(mmApp.Configuration.CommonFolder, "PasteCodeAsGistAddin.json"));
            mmApp.Model.Window.Activate();
        }
        #endregion
    }
}
