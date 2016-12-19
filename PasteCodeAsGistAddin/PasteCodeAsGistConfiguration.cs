using System.IO;
using MarkdownMonster;
using Westwind.Utilities.Configuration;

namespace PasteCodeAsGistAddin
{
    public class PasteCodeAsGistConfiguration : AppConfiguration
    {
        public static PasteCodeAsGistConfiguration Current;

        static PasteCodeAsGistConfiguration()
        {
            Current = new PasteCodeAsGistConfiguration();
            Current.Initialize();
        }

        public string GithubUserToken { get; set; }
        public string GithubUsername { get; set; }


        public PasteCodeAsGistConfiguration()
        {
                
        }

        #region AppConfiguration
        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            var provider = new JsonFileConfigurationProvider<PasteCodeAsGistConfiguration>()
            {
                JsonConfigurationFile = Path.Combine(mmApp.Configuration.CommonFolder, "PasteCodeAsGistAddin.json")
            };

            if (!File.Exists(provider.JsonConfigurationFile))
            {
                if (!Directory.Exists(Path.GetDirectoryName(provider.JsonConfigurationFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(provider.JsonConfigurationFile));
            }

            return provider;
        }
        #endregion
    }
}