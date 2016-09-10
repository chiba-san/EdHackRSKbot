using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Hackathon_Bot_Application_rsk_k161
{
    //[BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                //tivity.From.Name
                using (DataBaseContext.Context db = new DataBaseContext.Context())
                {
                    var userName = activity.From.Name;
                    Activity reply = null;
                    try
                    {
                        var result = db.Users.Any(t => t.Telegram.Equals(userName) || t.Facebook.Equals(userName));
                        if (result)
                        {
                            reply = activity.CreateReply($"I've found you  {userName}");
                        }
                        else
                        {
                            reply = activity.CreateReply($"I don't know yu, pal!");
                        }
                    }
                    catch (Exception ex)
                    {
                        reply = activity.CreateReply($"{ex.Message}");
                    }
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
               
            }

            return null;
        }
    }
}