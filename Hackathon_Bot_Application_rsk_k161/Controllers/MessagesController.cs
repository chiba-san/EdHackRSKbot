using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;

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

                Activity typingAct = activity.CreateReply();
                typingAct.Type = ActivityTypes.Typing;
                await connector.Conversations.SendToConversationAsync(typingAct);

                var handledComand = HandleBusinessLogicMessage(activity);
                if (handledComand != null)
                {
                    await connector.Conversations.ReplyToActivityAsync(handledComand);
                }
                else
                {
                    Activity reply = activity.CreateReply($"");

                    try
                    {
                        using (DataBaseContext.Context db = new DataBaseContext.Context())
                        {
                            var userName = activity.From.Name;
                            var channel = activity.ChannelId;

                            var user = db.CheckUser(userName, channel);
                            bool isAdmin = db.IsAdmin(user);


                            if (isAdmin)
                            {
                                var onlineQuiz = db.GetOnlineQuiz(user);
                                if (onlineQuiz != null)
                                {
                                    var activeQuiestoin = db.GetActiveQuestion(onlineQuiz);
                                    if (activeQuiestoin != null)
                                    {
                                        //Promt admin to "Next Question" or "Stop"
                                    }
                                    else
                                    {
                                        //Promt admin to "Next Question" or "Finish"
                                    }
                                }
                                else
                                {

                                    //Display list and promt user to select
                                    reply.Recipient = activity.From;
                                    reply.Type = "message";
                                    reply.Attachments = new List<Attachment>();

                                    List<CardAction> cardButtons = new List<CardAction>();
                                    var availableQuizzes = db.GetAvailableQuizzes(user);
                                    foreach (var quiz in availableQuizzes)
                                    {
                                        CardAction plButton = new CardAction()
                                        {
                                            Value = "cmd_start_quiz_" + quiz.Id.ToString(),
                                            Type = "postBack",
                                            Title = quiz.Name
                                        };
                                        cardButtons.Add(plButton);
                                    }
                                    HeroCard plCard = new HeroCard()
                                    {
                                        Title = "Доступные опросы",
                                        Subtitle = "Выберите один",
                                        Buttons = cardButtons
                                    };
                                    Attachment plAttachment = plCard.ToAttachment();
                                    reply.Attachments.Add(plAttachment);
                                }
                            }
                            else
                            {
                                var onlineQuiz = db.GetOnlineQuiz(user);
                                if (onlineQuiz != null)
                                {
                                    var activeQuestion = db.GetActiveQuestion(onlineQuiz);
                                    if (activeQuestion != null)
                                    {
                                        //Check user state for answer 
                                        bool alreadyAnswered = false;
                                        if (alreadyAnswered)
                                        {
                                            activity.CreateReply($"Вы уже ответили на данный вопрос!");
                                        }
                                        else
                                        {
                                            //Promt Question with Options and wait for user responses                                        
                                        }
                                    }
                                    else
                                    {
                                        activity.CreateReply($"Ждем следующий вопрос!");
                                    }
                                }
                                else
                                {
                                    activity.CreateReply($"!");
                                }
                            }
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

        private Activity HandleBusinessLogicMessage(Activity message)
        {
            if (message.Text.Contains("cmd_"))
            {
                try
                {
                    using (DataBaseContext.Context db = new DataBaseContext.Context())
                    {
                        message = message.CreateReply($"Выполнено!");
                        string[] data = message.Text.Split('_');
                        string command = data[1];
                        string quizObj = data[2];
                        string objId = data[3];

                        if (quizObj.Equals("quiz"))
                        {
                            var onlineQuize = db.Quizzes.FirstOrDefault(t => t.Id == int.Parse(objId));
                            if (onlineQuize != null)
                            {
                                onlineQuize.Status = (command.Equals("start"))?2:1;
                                db.SaveChanges();
                                message = message.CreateReply($"Опрос \"{onlineQuize.Name}\" {(onlineQuize.Status==2? "запущен":"остановлен")}  в {DateTime.Now.ToShortTimeString()}");
                            }
                        }
                        return message;
                                                
                    }
                }
                catch (Exception ex)
                {
                    message = message.CreateReply($"{ex.Message}");
                    return message;
                }
            }
            else
                return null;
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