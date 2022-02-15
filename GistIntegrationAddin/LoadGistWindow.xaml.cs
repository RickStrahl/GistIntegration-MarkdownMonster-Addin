using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MarkdownMonster;
using MarkdownMonster.Windows;
using Westwind.Utilities;

namespace GistIntegration
{
    /// <summary>
    /// Interaction logic for LoadAndSaveGist.xaml
    /// </summary>
    public partial class LoadGistWindow
    {
        public LoadAndSaveGistModel Model { get; set; }
        private PasteCodeAsGistAddin Addin { get; }

        public StatusBarHelper Status { get; set; }

        public LoadGistWindow(PasteCodeAsGistAddin addin)
        {
            Addin = addin;

            InitializeComponent();

            mmApp.SetThemeWindowOverride(this);

            Model = new LoadAndSaveGistModel(addin)
            {
                Configuration = PasteCodeAsGistConfiguration.Current,
                GistUsername = PasteCodeAsGistConfiguration.Current.GithubUsername,
            };
            Model.PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == "GistUsername")
                {
                    Task t = Model.LoadGists(this);
                }
            };
            DataContext = Model;

            Loaded += LoadGistWindow_Loaded;

            Status = new StatusBarHelper(StatusText, StatusIcon);
        }

        private async void LoadGistWindow_Loaded(object sender, RoutedEventArgs e)
        {   
            await Dispatcher.InvokeAsync(() =>
            {
                Model.LoadGists(this).FireAndForget();
            }, DispatcherPriority.Background);
        }
        


        public  void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Model.ActiveItem.id))
                return;

            Status.ShowStatusProgress("Retrieving Gist from Github...");            
            var gist = GistClient.GetGistFromServer(Model.ActiveItem.id, Model.Configuration.GithubUserToken);
            if (gist == null || gist.hasError)
            {
                
                Status.ShowStatusError("Failed to retrieve Gists from Github...");
                return;                
            }
            Status.ShowStatusSuccess("Gists retrieved.");


            var filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Gist_" + gist.id + "_" + gist.filename);
            File.WriteAllText(filename, gist.code,Encoding.UTF8);
            Addin.OpenTab(filename);            
            Close();
        }

        private async void ListGists_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ListGists.SelectedItem as GistItem;
            if (item == null) return;
            var li = ListGists.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            if (li == null) return;

            var gist = li.DataContext as GistItem;
            if (gist == null) return;

            await Dispatcher.InvokeAsync(async () => await PasteGistToEditor(gist));
        }


        
        public void ButtonDeleteGist_Click(object sender, RoutedEventArgs e)
        {
            var gist = Model.ActiveItem;
            if (gist == null)
                return;

            var msg =
                $@"Filename: {gist.filename}
Description: {gist.description}

Are you sure you want to delete this Gist?";

            var res = MessageBox.Show(msg, "Delete Gist", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.No)
                return;

            if (!GistClient.DeleteGist(gist.id))
            {
                Status.ShowStatusError("Failed to delete Gist.");
            }
            else
            {
                Model.GistList.Remove(gist);
                Status.ShowStatusSuccess("Gist Deleted.");
            }
        }

        public void ButtonBrowseToGist_Click(object sender, RoutedEventArgs e)
        {
            var gist = Model.ActiveItem;
            if (gist == null)
                return;

            ShellUtils.GoUrl(gist.htmlUrl);
        }

        
        private async void ButtonEmbed_Click(object sender, RoutedEventArgs e)
        {
            var gist = Model.ActiveItem;

            var editor = Model.Addin.Model?.ActiveEditor;
            if (editor == null) return;
            
            if (!string.IsNullOrEmpty(gist?.embedUrl))
            {
                await editor.SetSelectionAndFocus($"<script src=\"{gist.embedUrl}\"></script>" + mmApp.NewLine);
                Close();
            }
        }


        public async Task PasteGistToEditor(GistItem gist)
        {
            var editor = Model.Addin.Model?.ActiveEditor;
            if (editor == null) return;

            if (!string.IsNullOrEmpty(gist?.embedUrl))
            {
                await editor.SetSelectionAndFocus($"<script src=\"{gist.embedUrl}\"></script>" + mmApp.NewLine);
                Close();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Dispatcher.Invoke(() => mmApp.Model.Window.Activate(), DispatcherPriority.ApplicationIdle);
        }

        public void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            Close();

            Model.Addin.OnExecute(sender).FireAndForget();
        }

        private void ListGistsItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var lv = sender as ListBoxItem;
            if (lv == null)
            {
                var gist = ListGists.SelectedItem;
                lv = ListGists.ItemContainerGenerator.ContainerFromItem(gist) as ListBoxItem;
            }

            if (lv == null) return;
            var contextMenu = new GistListContextMenu(this, lv);
            contextMenu.ShowContextMenu();
        }

        public void EmbedCodeSnippetFromGist(string gistId)
        {
            var editor = Model.Addin.Model.ActiveEditor;
            var doc = editor?.MarkdownDocument;
            if (doc == null) 
                return;

            var gist = GistClient.GetGistFromServer(gistId);

            if (gist.hasError)
            {
                Status.ShowStatusError("Unable to retrieve Gist: " + gist.errorMessage);
            }

            var syntax = mmFileUtils.GetEditorSyntaxFromFileType(gist.filename);
            if (string.IsNullOrEmpty(syntax))
                syntax = "";


            var code = "```" + syntax + mmApp.NewLine +gist.code + mmApp.NewLine + "```" + mmApp.NewLine;
            Model.Addin.Model.ActiveEditor.SetSelectionAndFocus(code).FireAndForget();
        }
    }
}

