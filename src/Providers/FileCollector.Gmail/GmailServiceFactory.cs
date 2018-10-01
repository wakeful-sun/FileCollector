using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileCollector.Gmail
{
    class GmailServiceFactory
    {
        static string[] Scopes = { GmailService.Scope.GmailModify };

        public static GmailService Create(GmailSettings settings, string userId)
        {
            BaseClientService.Initializer clientInitializer = new BaseClientService.Initializer
            {
                ApplicationName = "Gmail file collector",
                HttpClientInitializer = CreateCredentials(settings, userId).GetAwaiter().GetResult()
            };

            return new GmailService(clientInitializer);
        }

        static Task<UserCredential> CreateCredentials(GmailSettings settings, string userId)
        {
            using (FileStream stream = new FileStream(settings.CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                ClientSecrets secrets = GoogleClientSecrets.Load(stream).Secrets;
                IDataStore dataStore = new FileDataStore(settings.TokenPath, true);

                return GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, Scopes, userId, CancellationToken.None, dataStore);
            }
        }
    }
}
