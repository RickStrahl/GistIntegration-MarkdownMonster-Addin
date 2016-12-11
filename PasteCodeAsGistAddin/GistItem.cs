namespace PasteCodeAsGistAddin
{
    public class GistItem
    {
        public string code { get; set; }
        public string description { get; set; } = string.Empty;
        public string filename { get; set; }
        public bool isPublic  { get; set; }   
        public bool isAnonymous { get; set; }
        
        public string id { get; set; }
        public string htmlUrl { get; set; }
        public string rawUrl { get; set; }
        public string embedUrl { get; set; }

        public string username { get; set; }
        public string language { get; set; } = "cs";
        public bool hasError { get; set; }
        public string errorMessage { get; set; }
    }
}