using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public partial class SaveGistWindow
    {

        public LoadAndSaveGistModel Model { get; set; }
        private PasteCodeAsGistAddin Addin { get; }

        public SaveGistWindow(PasteCodeAsGistAddin addin)
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
                    Model.LoadGists(this);
                }                    
            };
            DataContext = Model;

            Loaded += SaveGistWindow_Loaded;
        }

        private async void SaveGistWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Model.LoadGists(this);
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
        

        
        private void ButtonSaveGist_Click(object sender, RoutedEventArgs e)
        {
            Model.ActiveItem.code = Addin.GetMarkdown();

            GistItem gist;

            ShowStatus("Saving Gist...");
            if (!Model.SaveAsNewGist)
            {                
                gist = GistClient.UpdateGist(Model.ActiveItem, Model.Configuration.GithubUserToken);
            }
            else
                gist = GistClient.PostGist(Model.ActiveItem, Model.Configuration.GithubUserToken);

            if (gist != null && !gist.hasError)
            {
                ShowStatus("Gist has been saved...", 5000);                
                mmFileUtils.ShowExternalBrowser(gist.htmlUrl);
                Close();
            }
            else
            {
                SetStatusIcon(FontAwesomeIcon.Warning, Colors.Orange);
                ShowStatus("Gist was not saved - failed to save as Gist.", 7000);
            }

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

        /// <summary>
        /// Helper routine to show a Metro Dialog. Note this dialog popup is fully async!
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="style"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<MessageDialogResult> ShowMessageOverlayAsync(string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative,
            MetroDialogSettings settings = null)
        {
            return await this.ShowMessageAsync(title, message, style, settings);
        }

        #endregion

    }
}

