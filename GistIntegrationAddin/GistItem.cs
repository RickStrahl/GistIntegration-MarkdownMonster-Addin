using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GistIntegration.Annotations;

namespace GistIntegration
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
        private bool _isPublic = true;

        

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
        private string _language = "cs";

        public string id { get; set; }
        public string htmlUrl { get; set; }
        public string rawUrl { get; set; }

        public string embedUrl
        {
            get
            {
                if (string.IsNullOrEmpty(htmlUrl))
                    return null;

                return htmlUrl + ".js";
            }
        }

        public string mmFilename
        {
            get
            {
                if (string.IsNullOrEmpty(htmlUrl))
                    return null;

                return htmlUrl + ".md";
            }
        }

        public DateTime updated { get; set; }

        public string username { get; set; }

        public string language
        {
            get => _language;
            set
            {
                _language = value;
                if (string.IsNullOrEmpty(_language)) return;

                if (_language == "csharp")
                    _language = "cs";
                else if (_language == "javascript")
                    _language = "js";
                else if (_language == "powershell")
                    _language = "ps";
                else if (_language == "foxpro")
                    _language = "prg";
            }
        }

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