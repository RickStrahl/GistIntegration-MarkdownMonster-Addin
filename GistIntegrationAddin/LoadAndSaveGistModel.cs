using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using FontAwesome.WPF;
using GistIntegration.Annotations;
using MahApps.Metro.Controls.Dialogs;
using MarkdownMonster;
using MarkdownMonster.Windows;

namespace GistIntegration
{
    public class LoadAndSaveGistModel : INotifyPropertyChanged
    {
        public LoadAndSaveGistModel(PasteCodeAsGistAddin addin)
        {
            Addin = addin;

        }


        public PasteCodeAsGistAddin Addin { get; set; }


        public PasteCodeAsGistConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                if (Equals(value, _configuration)) return;
                _configuration = value;
                OnPropertyChanged();
            }
        }

        private PasteCodeAsGistConfiguration _configuration;


        public ObservableCollection<GistItem> GistList
        {
            get { return _gistList; }
            set
            {
                if (Equals(value, _gistList)) return;
                _gistList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<GistItem> _gistList = new ObservableCollection<GistItem>();


        public GistItem ActiveItem
        {
            get { return _activeItem; }
            set
            {
                if (Equals(value, _activeItem)) return;
                _activeItem = value;
                OnPropertyChanged();
            }
        }

        private GistItem _activeItem = null;


        public bool SaveAsNewGist
        {
            get { return _saveAsNewGist; }
            set
            {
                if (value == _saveAsNewGist) return;
                _saveAsNewGist = value;
                ActiveItem = new GistItem();
                var file = Addin.Model?.ActiveDocument?.Filename;
                if (!string.IsNullOrEmpty(file))
                    ActiveItem.filename = Path.GetFileName(file);
                OnPropertyChanged();
            }
        }

        private bool _saveAsNewGist;



        public string GistUsername
        {
            get { return _gistUsername; }
            set
            {
                if (value == _gistUsername) return;
                if (value == _gistUsername) return;
                _gistUsername = value;
                OnPropertyChanged();
            }
        }

        private string _gistUsername;


        /// <summary>
        /// Loads gists with progress info
        /// </summary>
        /// <param name="loadOrSaveWindow"></param>
        public void LoadGists(dynamic loadOrSaveWindow)
        {
            ObservableCollection<GistItem> gists;

            if (string.IsNullOrEmpty(GistUsername))
            {
                GistList = new ObservableCollection<GistItem>();
                return;
            }

            dynamic window = loadOrSaveWindow;

            window.ShowStatus("Retrieving Gists from Github...");

            gists = new ObservableCollection<GistItem>(GistClient.ListGistsForUser(GistUsername,
                Configuration.GithubUserToken));
            if (gists == null || gists.Count < 1 || gists[0].hasError)
            {
                window.SetStatusIcon(FontAwesomeIcon.Warning, Colors.Orange);
                window.ShowStatus("Failed to retrieve Gists from Github...", 7000);
                return;
            }
            window.ShowStatus("Retrieved Gists from Github.", 5000);

            foreach (var gist in gists)
            {
                if (string.IsNullOrEmpty(gist.description))
                    gist.description = System.IO.Path.GetFileNameWithoutExtension(gist.filename);
            }

            GistList = gists;
            ActiveItem = gists[0];
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}