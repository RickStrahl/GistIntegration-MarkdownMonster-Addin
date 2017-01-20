using System.ComponentModel;
using System.Runtime.CompilerServices;
using PasteCodeAsGistAddin.Annotations;

namespace PasteCodeAsGistAddin
{
    public class GistItem : INotifyPropertyChanged
    {
        

        public string code
        {
            get { return _code; }
            set
            {
                if (value == _code) return;
                _code = value;
                OnPropertyChanged();
            }
        }
        private string _code;

        

        public string description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }
        private string _description = string.Empty;


        

        public string filename
        {
            get { return _filename; }
            set
            {
                if (value == _filename) return;
                _filename = value;
                OnPropertyChanged();                
            }
        }
        private string _filename;

        

        public bool isPublic
        {
            get { return _isPublic; }
            set
            {
                if (value == _isPublic) return;
                _isPublic = value;
                OnPropertyChanged();
            }
        }
        private bool _isPublic;

        

        public bool isAnonymous
        {
            get { return _isAnonymous; }
            set
            {
                if (value == _isAnonymous) return;
                _isAnonymous = value;
                OnPropertyChanged();
            }
        }
        private bool _isAnonymous;

        public string id { get; set; }
        public string htmlUrl { get; set; }
        public string rawUrl { get; set; }
        public string embedUrl { get; set; }

        public string username { get; set; }

        public string language { get; set; } = "cs";

        public bool hasError { get; set; }
        public string errorMessage { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}