using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FontAwesome.WPF;
using GistIntegration.Annotations;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
                    Model.LoadGists(this);
            };
            DataContext = Model;

            Loaded += LoadGistWindow_Loaded;
        }

        private async void LoadGistWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Model.LoadGists(this);
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
        


        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Model.ActiveItem.id))
                return;

            ShowStatus("Retrieving Gist from Github...");            
            var gist = GistClient.GetGistFromServer(Model.ActiveItem.id, Model.Configuration.GithubUserToken);
            if (gist == null || gist.hasError)
            {
                SetStatusIcon(FontAwesomeIcon.Warning, Colors.Orange);
                ShowStatus("Failed to retrieve Gists from Github...",7000);
                return;                
            }
            ShowStatus("Gists retrieved.",7000);


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
                ShowStatus("Failed to delete Gist.", 7000);
                SetStatusIcon(FontAwesomeIcon.Warning, Colors.Red);
            }
            else
            {
                Model.GistList.Remove(gist);
                ShowStatus("Gist Deleted.", 7000);
            }
        }





        #region StatusBar Display

        public void ShowStatus(string message = null, int milliSeconds = 0)
        {
            if (message == null)
            {
                message = "Ready";
                SetStatusIcon();
            }

            StatusText.Text = message;

            if (milliSeconds > 0)
            {
                Dispatcher.DelayWithPriority(milliSeconds, (win) =>
                {
                    ShowStatus(null, 0);
                    SetStatusIcon();
                }, this);
            }
            WindowUtilities.DoEvents();
        }

        /// <summary>
        /// Status the statusbar icon on the left bottom to some indicator
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="color"></param>
        /// <param name="spin"></param>
        public void SetStatusIcon(FontAwesomeIcon icon, Color color, bool spin = false)
        {
            StatusIcon.Icon = icon;
            StatusIcon.Foreground = new SolidColorBrush(color);
            if (spin)
                StatusIcon.SpinDuration = 30;

            StatusIcon.Spin = spin;
        }

        /// <summary>
        /// Resets the Status bar icon on the left to its default green circle
        /// </summary>
        public void SetStatusIcon()
        {
            StatusIcon.Icon = FontAwesomeIcon.Circle;
            StatusIcon.Foreground = new SolidColorBrush(Colors.Green);
            StatusIcon.Spin = false;
            StatusIcon.SpinDuration = 0;
            StatusIcon.StopSpin();
        }


        #endregion
    }
}

