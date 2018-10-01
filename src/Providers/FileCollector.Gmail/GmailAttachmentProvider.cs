using FileCollector.Common;
using FileCollector.Common.Config;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileCollector.Gmail
{
    public class GmailAttachmentProvider : IFileProvider, IDisposable
    {
        readonly GmailService service;

        public GmailAttachmentProvider(GmailSettings settings)
        {
            service = GmailServiceFactory.Create(settings, Constants.Me);
        }

        public async Task<Result> Process(ProviderConfiguration configuration)
        {
            MessagesQuery messagesQuery = MessagesQuery.Create(configuration, service.Users);
            IList<Message> messages = await messagesQuery.ExecuteAsync();

            if (messages != null && messages.Count > 0)
            {
                string message = string.Empty;
                int i;

                for (i = 0; i < messages.Count; i++)
                {
                    if (i == 0)
                    {
                        await SaveAttachement(messages[0].Id, configuration.FileConfig, messagesQuery.FileNameFilter);
                        message = "Attachement saved.";
                    }
                    await SetRead(messages[i].Id);
                }
                return Result.Ok($"{message} Count of emails marked as READ: [{i}].");
            }
            return Result.Warning($"New messages from [{configuration.Email}] are not found.");
        }

        async Task SaveAttachement(string messageId, FileConfiguration config, Func<MessagePart, bool> fileNameFilter)
        {
            Message message = await service.Users.Messages.Get(Constants.Me, messageId).ExecuteAsync();

            List<MessagePart> parts = message.Payload.Parts.Where(fileNameFilter).ToList();

            if (parts.Count >= 1)
            {
                string attId = parts[0].Body.AttachmentId;
                MessagePartBody attachPart = await service.Users.Messages.Attachments.Get(Constants.Me, messageId, attId).ExecuteAsync();

                // Converting from RFC 4648 base64 to base64url encoding
                // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                string attachData = attachPart.Data.Replace('-', '+');
                attachData = attachData.Replace('_', '/');

                byte[] data = Convert.FromBase64String(attachData);
                string destinationFilePath = FileOperation.PrepareDestination(config);

                File.WriteAllBytes(destinationFilePath, data);
            }
            else
            {
                throw new InvalidProgramException("Configuration or implementation is invalid. Query result contains some emails with attachements, but no attachements available in message body.");
            }
        }

        Task SetRead(string messageId)
        {
            ModifyMessageRequest mods = new ModifyMessageRequest
            {
                RemoveLabelIds = new List<string> { "UNREAD" }
            };
            //TODO: add imported tag and check

            return service.Users.Messages.Modify(mods, Constants.Me, messageId).ExecuteAsync();
        }

        public void Dispose()
        {
            service.Dispose();
        }
    }
}
