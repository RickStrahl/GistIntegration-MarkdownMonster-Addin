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
using GistIntegration;
using System.ComponentModel;

namespace PasteCodeAsGitAddin
{
    /// <summary>
    /// Interaction logic for PasteHref.xaml
    /// </summary>
    public partial class PasteCodeAsGitWindow 
    {
        private GistIntegration.PasteCodeAsGistAddin Addin { get; set; }

        public GistItem Gist { get; set; }

        public bool Cancelled { get; set; }

        public PasteCodeAsGitWindow(GistIntegration.PasteCodeAsGistAddin addin)
        {
            Addin = addin;

            Cancelled = true;
           
            Gist = new GistItem();

            InitializeComponent();
            

            mmApp.SetThemeWindowOverride(this);

            Loaded += PasteCode_Loaded;

            WebBrowserCode.Visibility = Visibility.Hidden;            

        }

        private MarkdownEditorSimple editor;

        private void PasteCode_Loaded(object sender, RoutedEventArgs e)
        {            
            Gist.filename = "file." + Gist.language;

            if (string.IsNullOrEmpty(PasteCodeAsGistConfiguration.Current.GithubUsername))
            {
                CheckAnonymous.Visibility = Visibility.Collapsed;
                Gist.isAnonymous = true;
            }
         
            DataContext = this;
            
            editor = new MarkdownEditorSimple(WebBrowserCode, Gist.code,"csharp");
            
            editor.IsDirtyAction = () =>
            {
                Gist.code = editor.GetMarkdown();                
                return true;
            };

            if (!string.IsNullOrEmpty(Gist.code))
            {
                Dispatcher.InvokeAsync(() =>
               {
                   TextFilename.SelectAll();
                   TextFilename.Focus();                   
               },System.Windows.Threading.DispatcherPriority.ApplicationIdle);                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == ButtonCancel)
                DialogResult = false;
            else
            {
                Gist = GistClient.PostGist(Gist);

                Cancelled = false;
                DialogResult = true;
            }

            Close();
        }

        private void TextFilename_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filename = TextFilename.Text;

            if (string.IsNullOrEmpty(filename))
                return;

            var ext = System.IO.Path.GetExtension(filename.ToLower());
            
            string syntax = editor.EditorSyntax;

            if (string.IsNullOrEmpty(syntax))
                return;

            editor.SetEditorSyntax(syntax);            
        }
    }
}
