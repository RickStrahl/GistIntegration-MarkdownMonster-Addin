using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using FontAwesome.WPF;
using MarkdownMonster;
using MarkdownMonster.AddIns;
using MarkdownMonster.Utilities;
using Newtonsoft.Json.Linq;
using PasteCodeAsGitAddin;
using Westwind.Utilities;

namespace GistIntegration
{
    public class PasteCodeAsGistAddin : MarkdownMonsterAddin
    {
        public PasteCodeAsGitWindow PasteCodeAsGistWindow;
        public LoadGistWindow LoadGistWindow;
        private AddInMenuItem AddinMenuItem;

        #region Addin Overrides

        public PasteCodeAsGistAddin()
        {
        }
        
        public override Task OnApplicationInitialized(AppModel model)
        {
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

            // add an additional menu item
            var mi = new MenuItem()
            {
                Header = "Embed or Open existing Gist"
            };
            mi.Click += (s, e) =>
            {
                LoadGistWindow = new LoadGistWindow(this)
                {
                    Owner = Model.Window
                };
                LoadGistWindow.Show();
            };
            AddinMenuItem.AdditionalDropdownMenuItems.Add(mi);


            // Must add the menu to the collection to display menu and toolbar items            
            MenuItems.Add(AddinMenuItem);

            return Task.CompletedTask;
        }

        public override Task OnWindowLoaded()
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
                Header = "Open or Embed from Gi_st",
                Name = "ButtonOpenFromGist"
            };
            mitemOpen.Click += (s, a) => OnExecuteLoadGist();
            if (!AddMenuItem(mitemOpen, menuItemNameToFind: "ButtonOpenFromHtml", addMode: AddMenuItemModes.AddAfter))
                mmApp.Log("Unable to add custom menu item in Paste Code As Gist Addin: " + mitemOpen.Name);

            var mitemSave = new MenuItem()
            {
                Header = "Save to Gi_st",
                Name = "ButtonSaveToGist"
            };
            mitemSave.Click += (s, a) => OnExecuteSaveGist();
            if (!AddMenuItem(mitemSave, menuItemNameToFind: "ButtonGeneratePdf", addMode: AddMenuItemModes.AddAfter))
                mmApp.Log("Unable to add custom menu item in Paste Code As Gist Addin: " + mitemSave.Name);

            return Task.CompletedTask;
        }


        public override Task OnExecute(object sender)
        {

            Model.Window.Dispatcher.InvokeAsync(async () =>
            {
                var editor = GetMarkdownEditor();
                if (editor == null)
                    return;

                var code = await editor.GetSelection();
                string syntax = "cs";
                if (string.IsNullOrEmpty(code))
                    code = ClipboardHelper.GetText();

                if (!string.IsNullOrEmpty(code))
                    syntax = LanguageAutoDetection.Detect(code);
                if (syntax == null)
                    syntax = "cs";

                if (!string.IsNullOrEmpty(code))
                {
                    // strip leading white space
                    code = StringUtils.NormalizeIndentation(code);
                    if (code.TrimStart().StartsWith("```") && code.TrimEnd().EndsWith("```"))
                    {
                        var lines = StringUtils.GetLines(code);
                        var synt = lines[0].Replace("```", "").Trim();
                        if (!string.IsNullOrEmpty(synt))
                            syntax = synt;
                        lines = lines.Skip(1).Take(lines.Length - 2).ToArray();
                        code = string.Join(mmApp.NewLine, lines);
                    }
                }


                var gist = new GistItem()
                {
                    code = code,
                    language = syntax
                };

                PasteCodeAsGistWindow = new PasteCodeAsGitWindow(this);
                PasteCodeAsGistWindow.Owner = Model.Window;
                PasteCodeAsGistWindow.Gist = gist;
                PasteCodeAsGistWindow.ShowDialog();

                if (PasteCodeAsGistWindow.Cancelled)
                    return;

                if (!string.IsNullOrEmpty(gist.embedUrl))
                    await editor.SetSelectionAndFocus($"<script src=\"{gist.embedUrl}\"></script>{mmApp.NewLine}");

                Model.Window.ShowStatus("Gist embedded", 5000);
                Model.Window.SetStatusIcon(FontAwesomeIcon.GithubAlt, Colors.Green);


            }).Task.FireAndForget();

            return Task.CompletedTask;
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

        public override Task OnExecuteConfiguration(object sender)
        {
            OpenGistConfigurationTab();
            return Task.CompletedTask;
        }

        public override Task OnApplicationShutdown()
        {
            base.OnApplicationShutdown();

            // ensure config file is updated with current settings
            PasteCodeAsGistConfiguration.Current.Write();

            return Task.CompletedTask;
        }

        #endregion

        #region Addin Helpers

        /// <summary>
        /// Opens the configuration tab to configure Gists
        /// </summary>
        public void OpenGistConfigurationTab()
        {
            mmApp.Model.Window.OpenTab(System.IO.Path.Combine(mmApp.Configuration.CommonFolder,
                "PasteCodeAsGistAddin.json"));
            mmApp.Model.Window.Activate();
        }

        #endregion
    }
}
