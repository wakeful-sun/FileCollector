using FileCollector.Common;
using FileCollector.Common.Config;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileCollector.Gmail
{
    public class GmailAttachmentProvider : IFileProvider, IDisposable
    {
        static string[] Scopes = { GmailService.Scope.GmailModify };
        const string userId = "me";

        readonly GmailSettings settings;
        readonly GmailService service;


        public GmailAttachmentProvider(GmailSettings settings)
        {
            this.settings = settings;
            UserCredential credential;

            using (var stream =
                new FileStream(settings.CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    userId,
                    CancellationToken.None,
                    new FileDataStore(settings.TokenPath, true)).Result;
            }

            service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gmail file collector",
            });
        }

        public Task<Result> Process(ProviderConfiguration configuration)
        {
            Func<MessagePart, bool> fileNameFilter = x => !string.IsNullOrWhiteSpace(x.Filename);

            UsersResource.MessagesResource.ListRequest messagesRequest = service.Users.Messages.List(userId);
            messagesRequest.Q = $"from:({configuration.Email}) label:inbox has:attachment is:unread";
            if (!string.IsNullOrWhiteSpace(configuration.FileConfig.SourceFileName))
            {
                messagesRequest.Q += $" filename:{configuration.FileConfig.SourceFileName}";
                //TODO: apply pattern search
                fileNameFilter = x => !string.IsNullOrWhiteSpace(x.Filename) && x.Filename == configuration.FileConfig.SourceFileName;
            }

            IList<Message> messages = messagesRequest.Execute().Messages;
            if (messages != null)
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    if (i == 0)
                    {
                        SaveAttachement(messages[0].Id, configuration.FileConfig, fileNameFilter);
                        Console.WriteLine("Attachement saved");
                    }
                    Console.WriteLine("Message marked as READ.");
                    SetRead(messages[i].Id);
                }
            }
            else
            {
                Console.Write("No new message");
            }

            return Task.FromResult(Result.Ok());
        }

        void SaveAttachement(string messageId, FileConfiguration config, Func<MessagePart, bool> fileNameFilter)
        {
            Message message = service.Users.Messages.Get(userId, messageId).Execute();

            List<MessagePart> parts = message.Payload.Parts.Where(fileNameFilter).ToList();

            for (int i = 0; i < parts.Count; i++)
            {
                MessagePart part = parts[i];
                if (i == 0)
                {
                    Console.Write($"-->{part.Filename}<--");
                    string attId = part.Body.AttachmentId;
                    MessagePartBody attachPart = service.Users.Messages.Attachments.Get(userId, messageId, attId).Execute();

                    // Converting from RFC 4648 base64 to base64url encoding
                    // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                    string attachData = attachPart.Data.Replace('-', '+');
                    attachData = attachData.Replace('_', '/');

                    byte[] data = Convert.FromBase64String(attachData);
                    File.WriteAllBytes(Path.Combine(config.TargetDirectory, config.TargetFileName/*part.Filename*/), data);
                    Console.WriteLine("File saved");
                }
                else
                {
                    Console.WriteLine("File ignored");
                }
            }
        }

        public void SetRead(string messageId)
        {
            ModifyMessageRequest mods = new ModifyMessageRequest
            {
                RemoveLabelIds = new List<string> { "UNREAD" }
            };
            //TODO: add imported tag and check

            service.Users.Messages.Modify(mods, userId, messageId).Execute();
        }


        public void Dispose()
        {
            service.Dispose();
        }
    }
}
