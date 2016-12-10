using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FontAwesome.WPF;
using MarkdownMonster.AddIns;

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
    }


    public class PasteCodeAsGitConfiguration
    {
        public string GitUserToken { get; set; }
    }
}
