using System.IO;
using MarkdownMonster;
using Westwind.Utilities.Configuration;

namespace PasteCodeAsGistAddin
{
    public class PasteCodeAsGitConfiguration : AppConfiguration
    {
        public static PasteCodeAsGitConfiguration Current;

        static PasteCodeAsGitConfiguration()
        {
            Current = new PasteCodeAsGitConfiguration();
            Current.Initialize();
        }

        public string GithubUserToken { get; set; }
        public string GithubUsername { get; set; }


        public PasteCodeAsGitConfiguration()
        {
                
        }

        #region AppConfiguration
        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            var provider = new JsonFileConfigurationProvider<PasteCodeAsGitConfiguration>()
            {
                JsonConfigurationFile = Path.Combine(mmApp.Configuration.CommonFolder, "PasteCodeAsGitAddin.json")
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