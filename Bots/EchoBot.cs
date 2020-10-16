// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.10.3

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace SearchBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = new StringBuilder();
            var inputText = turnContext.Activity.Text.Trim();
            SearchIndexClient client = GetSearchClient();
            var typingActivity = new Activity[] { new Activity { Type = ActivityTypes.Typing }, new Activity { Type = "delay", Value = 1000 } };
            await turnContext.SendActivitiesAsync(typingActivity);
            Microsoft.Azure.Search.Models.DocumentSearchResult<Microsoft.Azure.Search.Models.Document> results = await client.Documents.SearchAsync(inputText);

            if (results?.Results != null && results.Results.Any())
            {
                foreach (SearchResult<Document> item in results.Results.Take(5))
                {
                    replyText.AppendLine($"{item.Document["Path"].ToString()}");
                }
            }
            else
            {
                replyText.AppendLine("No results found!");
            }

            await turnContext.SendActivityAsync(MessageFactory.Text(replyText.ToString(), replyText.ToString()), cancellationToken);
        }

        private static SearchIndexClient GetSearchClient()
        {
            SearchIndexClient searchIndexClient = new SearchIndexClient(Startup.SearchServiceName, Startup.SearchIndexName, new SearchCredentials(Startup.QueryKey));
            return searchIndexClient;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome to the C# Corner Virtual .NET Conference Demo!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
