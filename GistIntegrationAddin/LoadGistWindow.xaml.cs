using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                GistUsername = PasteCodeAsGistConfiguration.Current.GithubUsername
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
                Task t = Model.LoadGists(this);
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
        


        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
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

        private void ListGists_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ButtonOpen_Click(sender, null);
        }


        private void ButtonConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Addin.OpenGistConfigurationTab();
        }

        
        private void ButtonDeleteGist_Click(object sender, RoutedEventArgs e)
        {
            var gist = ((Button)sender).DataContext as GistItem;
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

        private void ButtonBrowseToGist_Click(object sender, RoutedEventArgs e)
        {
            var gist = ((Button)sender).DataContext as GistItem;
            if (gist == null)
                return;

            ShellUtils.GoUrl(gist.htmlUrl);
        }
    }
}

