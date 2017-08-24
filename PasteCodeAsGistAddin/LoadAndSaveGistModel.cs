using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PasteCodeAsGistAddin.Annotations;

namespace PasteCodeAsGistAddin
{
    public class LoadAndSaveGistModel : INotifyPropertyChanged
    {
                

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


        public List<GistItem> GistList
        {
            get { return _gistList; }
            set
            {
                if (Equals(value, _gistList)) return;
                _gistList = value;
                OnPropertyChanged();
            }
        }
        private List<GistItem> _gistList = new List<GistItem>();


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


        public string GistId
        {
            get { return _gistId; }
            set
            {
                if (value == _gistId) return;
                _gistId = value;
                OnPropertyChanged();
            }
        }
        private string _gistId = "";

        

        public bool SaveAsNewGist
        {
            get { return _saveAsNewGist; }
            set
            {
                if (value == _saveAsNewGist) return;
                _saveAsNewGist = value;
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
                _gistUsername = value;
                OnPropertyChanged();
            }
        }
        private string _gistUsername;


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}