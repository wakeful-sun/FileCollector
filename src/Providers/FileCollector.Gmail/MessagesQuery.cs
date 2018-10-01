using FileCollector.Common;
using FileCollector.Common.Config;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileCollector.Gmail
{
    class MessagesQuery
    {
        readonly UsersResource.MessagesResource.ListRequest request;
        public Func<MessagePart, bool> FileNameFilter { get; }

        public MessagesQuery(UsersResource.MessagesResource.ListRequest request, Func<MessagePart, bool> fileNameFilter)
        {
            this.request = request;
            FileNameFilter = fileNameFilter;
        }

        public async Task<IList<Message>> ExecuteAsync()
        {
            ListMessagesResponse messagesResponse = await request.ExecuteAsync();
            return messagesResponse.Messages;
        }

        public static MessagesQuery Create(ProviderConfiguration configuration, UsersResource users)
        {
            FileConfiguration fileConfig = configuration.FileConfig;

            Func<MessagePart, bool> fileNameFilter;

            UsersResource.MessagesResource.ListRequest messagesRequest = users.Messages.List(Constants.Me);
            messagesRequest.Q = $"from:({configuration.Email}) label:inbox has:attachment is:unread";

            if (!string.IsNullOrWhiteSpace(fileConfig.SourceFileName))
            {
                messagesRequest.Q += $" filename:{fileConfig.SourceFileName}";
                fileNameFilter = x => !string.IsNullOrWhiteSpace(x.Filename) && x.Filename.MatchPattern(fileConfig.SourceFileName);
            }
            else
            {
                fileNameFilter = x => !string.IsNullOrWhiteSpace(x.Filename);
            }

            return new MessagesQuery(messagesRequest, fileNameFilter);
        }
    }
}
