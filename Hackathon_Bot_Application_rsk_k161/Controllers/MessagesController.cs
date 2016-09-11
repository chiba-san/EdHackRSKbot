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
using DataBaseContext.DbModels;

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

                Activity reply = activity.CreateReply($"");

                try
                {
                    using (DataBaseContext.Context db = new DataBaseContext.Context())
                    {
                        var userName = activity.From.Name;
                        var channel = activity.ChannelId;
                        var user = db.CheckUser(userName, channel);

                        if (activity.Text.Contains("cmd_"))
                        {
                            string[] data = activity.Text.Split('_');
                            string command = data[1];
                            string quizObj = data[2];
                            int objId = int.Parse(data[3]);

                            if (quizObj.Equals("quiz"))
                            {
                                var onlineQuize = db.Quizzes.FirstOrDefault(t => t.Id == objId);
                                if (onlineQuize != null)
                                {
                                    onlineQuize.Status = (command.Equals("start")) ? 2 : 1;
                                    db.SaveChanges();
                                    reply = activity.CreateReply($"Опрос \"{onlineQuize.Name}\" {(onlineQuize.Status == 2 ? "запущен" : "остановлен")}  в {DateTime.Now.ToShortTimeString()}");
                                    await connector.Conversations.SendToConversationAsync(reply);
                                }
                            }
                            else if (quizObj.Equals("question"))
                            {
                                var activeQuestion = db.Questions.FirstOrDefault(t => t.Id == objId);
                                if (activeQuestion != null)
                                {
                                    if (command.Equals("stop"))
                                    {
                                        StateClient stateClient = activity.GetStateClient();
                                        BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);
                                        userData.SetProperty<string>("questionId", objId.ToString());
                                        stateClient.BotState.SetUserData(activity.ChannelId, activity.From.Id, userData);
                                        reply = activity.CreateReply($"Вопрос \"{activeQuestion.Text}\" остановлен  в {DateTime.Now.ToShortTimeString()}");
                                        await connector.Conversations.ReplyToActivityAsync(reply);

                                        //Promt admin to "Next Question" or "Finish
                                        var onlineQuiz = db.GetOnlineQuiz(user);
                                        if (onlineQuiz != null)
                                        {
                                            reply = PromtUserForNextQuestionOrFinishQuiz(activity, onlineQuiz);
                                            await connector.Conversations.ReplyToActivityAsync(reply);
                                        }
                                    }
                                    else if (command.Equals("start"))
                                    {
                                        var question = db.SetActiveNextQuestion(activeQuestion);
                                        reply = activity.CreateReply($"Вопрос \"{question.Text}\" запущен в {DateTime.Now.ToShortTimeString()}");

                                        //Promt admin to "Next Question" or "Stop"                                 
                                        reply = PromtUserForNextQuestionOrStopQuiz(activity, activeQuestion);
                                        await connector.Conversations.ReplyToActivityAsync(reply);
                                    }
                                }
                            }
                            else
                            {

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
                                            reply = PromtUserForNextQuestionOrStopQuiz(activity, activeQuiestoin);
                                        }
                                        else
                                        {
                                            //Promt admin to "Next Question" or "Finish"
                                            reply = PromtUserForNextQuestionOrFinishQuiz(activity, onlineQuiz);
                                        }
                                        await connector.Conversations.SendToConversationAsync(reply);

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
                                            Buttons = cardButtons
                                        };
                                        Attachment plAttachment = plCard.ToAttachment();
                                        reply.Attachments.Add(plAttachment);
                                        await connector.Conversations.SendToConversationAsync(reply);
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
                    }
                }
                catch (Exception ex)
                {
                    reply = activity.CreateReply($"{ex.Message}");
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static Activity PromtUserForNextQuestionOrStopQuiz(Activity activity, Question activeQuiestoin)
        {
            Activity reply = activity.CreateReply($"");
            reply.Recipient = activity.From;
            reply.Type = "message";
            reply.Attachments = new List<Attachment>();

            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButtonNextQuest = new CardAction()
            {
                Value = "cmd_start_question_" + activeQuiestoin.Id.ToString(),
                Type = "postBack",
                Title = "Cледующий"
            };
            cardButtons.Add(plButtonNextQuest);

            CardAction plButtonStop = new CardAction()
            {
                Value = "cmd_stop_question_" + activeQuiestoin.Id.ToString(),
                Type = "postBack",
                Title = "Стоп"
            };
            cardButtons.Add(plButtonStop);

            HeroCard plCard = new HeroCard()
            {
                Title = "Вы можете:",
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            reply.Attachments.Add(plAttachment);
            return reply;
        }

        private static Activity PromtUserForNextQuestionOrFinishQuiz(Activity activity, Quiz current)
        {
            Activity reply = activity.CreateReply($"");
            reply.Recipient = activity.From;
            reply.Type = "message";
            reply.Attachments = new List<Attachment>();

            List<CardAction> cardButtons = new List<CardAction>();
            StateClient stateClient = activity.GetStateClient();
            BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);
            var lastActiveQuestionId = userData.GetProperty<string>("questionId");
            //FIND activequestion from previous state
            if (lastActiveQuestionId != null)
            {
                CardAction plButtonNextQuest = new CardAction()
                {

                    Value = "cmd_start_question_" + lastActiveQuestionId,
                    Type = "postBack",
                    Title = "Следующий"
                };
                cardButtons.Add(plButtonNextQuest);
            }

            CardAction plButtonStop = new CardAction()
            {
                Value = "cmd_stop_quiz_" + current.Id.ToString(),
                Type = "postBack",
                Title = "Закончить"
            };
            cardButtons.Add(plButtonStop);

            HeroCard plCard = new HeroCard()
            {
                Title = "Вы можете:",
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            reply.Attachments.Add(plAttachment);
            return reply;
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