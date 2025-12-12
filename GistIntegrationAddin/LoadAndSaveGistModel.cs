using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GistIntegration.Annotations;
using Westwind.Utilities;

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



        /// <summary>
        /// The search term on the form that filters the list
        /// </summary>
        public string SearchTerm
        {
            get { return _SearchTerm; }
            set
            {
                if (value == _SearchTerm) return;
                _SearchTerm = value;

                OnPropertyChanged(nameof(SearchTerm));
                OnPropertyChanged(nameof(GistList));
                OnPropertyChanged(nameof(FilteredGistList));
            }
        }
        private string _SearchTerm;



        public string ListCount
        {
            get
            {
                if (_gistList == null || _gistList.Count < 1)
                    return "No Gists";

                if (_gistList.Count == 1)
                    return "1 Gist";

                return $"{_gistList.Count} Gists";
            }
        }


        public ObservableCollection<GistItem> FilteredGistList
        {
            get
            {
                if (string.IsNullOrEmpty(SearchTerm))
                {
                    OnPropertyChanged(nameof(ListCount));
                    return _gistList;
                }

                var lst = _gistList.Where(g =>
                        StringUtils.Contains(g.filename, SearchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                        StringUtils.Contains(g.description, SearchTerm,
                            StringComparison.InvariantCultureIgnoreCase));

                
                var col = new ObservableCollection<GistItem>(lst);

                OnPropertyChanged(nameof(ListCount));
                return col;
            }
        }

        public ObservableCollection<GistItem> GistList
        {
            get { return _gistList; }
            set
            {
                if (Equals(value, _gistList)) return;
                _gistList = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredGistList));
                OnPropertyChanged(nameof(ListCount));
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
        public async Task LoadGists(dynamic loadOrSaveWindow)
        {
            ObservableCollection<GistItem> gists;

            if (string.IsNullOrEmpty(GistUsername))
            {
                GistList = new ObservableCollection<GistItem>();
                return;
            }

            dynamic window = loadOrSaveWindow;

            window.Status.ShowStatusProgress("Retrieving Gists from Github...");
            
            gists = new ObservableCollection<GistItem>(await GistClient.ListGistsForUserAsync(GistUsername,
                Configuration.GithubUserToken));

            if (gists == null || (gists.Count > 0 && gists[0].hasError))
            {
                
                window.Status.ShowStatusError("Failed to retrieve Gists from Github...");
                return;
            }
            if (gists.Count == 0)
            {
                window.Status.ShowStatusError($"No Gists found on Github for {GistUsername}.");
                GistList = [];
                return;
            }
            window.Status.ShowStatusSuccess($"Retrieved {gists.Count} Gists from Github for {GistUsername}.");

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