using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MarkdownMonster;
using PasteCodeAsGistAddin;

namespace PasteCodeAsGitAddin
{
    /// <summary>
    /// Interaction logic for PasteHref.xaml
    /// </summary>
    public partial class PasteCodeAsGitWindow 
    {

        public string Code { get; set; }        
        public string CodeLanguage { get; set; }
        public Dictionary<string,string> LanguageNames { get; set; }

        public GistItem Gist { get; set;  }

        public bool Cancelled { get; set; }
        
        public PasteCodeAsGitWindow()
        {
            Cancelled = true;

            LanguageNames = new Dictionary<string, string>()
            {
                {"cs", "Csharp"},
                {"vb", "Vb.Net"},
                {"cpp", "C++"},
                {"foxpro", "FoxPro"},
                {"fsharp", "Fsharp"},
              
                {"html", "Html"},
                {"xml", "Xml"},
                {"css", "Css"},
                {"js", "JavaScript"},
                {"ts", "TypeScript"},
                {"json", "Json"},

                { "md","Markdown" },
                {"sql","SQL" },                

                {"ruby", "Ruby"},
                {"py", "Python"},
                {"php", "PHP"},
                {"java", "Java"},
                {"swift", "Swift"},
                
                {"ps", "PowerShell"}                                
            };
            CodeLanguage = "cs";

            InitializeComponent();

            
            mmApp.SetThemeWindowOverride(this);

            Loaded += PasteCode_Loaded;
        }

        private void PasteCode_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;


            this.TextCodeLanguage.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == ButtonCancel)
                DialogResult = false;                
            else
            {
                Cancelled = false;
                DialogResult = true;                
            }

            Close();
        }
    }
}
