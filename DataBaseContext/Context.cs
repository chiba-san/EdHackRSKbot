using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DataBaseContext.DbModels;
using System.Text.RegularExpressions;

namespace DataBaseContext
{
    public class CustomChannel
    {
        public enum CustomType { Facebook, Telegram, None }

        public CustomType chType;

        public CustomChannel(string channelId)
        {
            if (channelId.ToLower().Equals("facebook"))
            {
                chType = CustomType.Facebook;
            }
            else if (channelId.ToLower().Equals("telegram"))
            {
                chType = CustomType.Telegram;
            }
            else
            {
                chType = CustomType.None;
            }
        }
    }

    public class Context:DbContext
    {
        public Context()
             : base("Name=DefaultConnection")
        {

        }
        public DbSet<Quiz> Quizzes { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Option> Options { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<UserForQuiz> UsersForQuizzes { get; set; }
        
        public DbSet<User> Users { get; set; }

        public User CheckUser(string name, string channelId)
        {
            var user = Users.FirstOrDefault(t => t.Telegram.ToLower().Equals(name.ToLower())
                                                    || t.Facebook.ToLower().Equals(name.ToLower())
                                                    || t.PhoneNumber.ToLower().Equals(name.ToLower()));
            if (user != null)
            {
                return user;
            }
            else
            {
                var channel = new CustomChannel(channelId);
                var result = Users.Add(new User()
                {
                    Facebook = (channel.chType == CustomChannel.CustomType.Facebook) ? name : null,
                    Telegram = (channel.chType == CustomChannel.CustomType.Telegram) ? name : null,
                    PhoneNumber = (channel.chType == CustomChannel.CustomType.None) ? name : null,
                });
                int res = SaveChanges();
                return result;
            }
        }

        public bool IsAdmin(User user)
        {
            return UsersForQuizzes.Any(t => t.UserId.Equals(user.Id));
        }

        public Quiz GetOnlineQuiz(User user)
        {
            IQueryable<Quiz> availableQuizzes = UsersForQuizzes.Where(t => t.UserId.Equals(user.Id)).Select(g => g.Quiz);
            return availableQuizzes.FirstOrDefault(t => t.Status == 2);
        }

        public IQueryable<Quiz> GetAvailableQuizzes(User admin)
        {
            return UsersForQuizzes.Where(t => t.UserId.Equals(admin.Id) && t.Quiz.Status == 1).Select(g => g.Quiz);
        }

        public Question GetActiveQuestion(Quiz quiz)
        {
            IQueryable<Question> availableQuestions = Questions.Where(t => t.QuizId.Equals(t.QuizId));
            return availableQuestions.FirstOrDefault(t => t.IsActive == true);
        }

        public Question SetActiveNextQuestion(Question currentQuestion)
        {
            currentQuestion.IsActive = false;
            int nextOneOrder = currentQuestion.Order + 1;
            Question nextOne = Questions.FirstOrDefault(t => t.QuizId.Equals(currentQuestion.QuizId) && t.Order == nextOneOrder);
            if (nextOne != null)
            {
                nextOne.IsActive = true;
                SaveChanges();
                return nextOne;
            }
            else
            {
                SaveChanges();
                return null;
            }
        }

        public Answer SetAnswer(string userResponse, Question question, User user)
        {
            MatchCollection matches = Regex.Matches(userResponse, @"\\d+", RegexOptions.None);
            var options = new List<int>();
            foreach (Match match in matches)
                options.Add(int.Parse(match.Groups[1].Value));
            var answers = question.Options.Where(t => t.IsAnswer).Select(g => g.Order);
            var isCorrect = options.All(answers.Contains);
            Answer answer = Answers.Add(new Answer() { IsCorrect = isCorrect, Question = question, QuestionId = question.Id, User = user, UserId = user.Id, UserResponse = userResponse });
            SaveChanges();
            return answer;
        }        
    }
}
