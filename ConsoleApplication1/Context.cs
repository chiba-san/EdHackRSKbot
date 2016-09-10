using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DataBaseContext.DbModels;

namespace DataBaseContext
{
    public class Context:DbContext
    {
        public DbSet<Quiz> Quizzes { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Option> Options { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<UserForQuiz> UsersForQuizzes { get; set; }
        
        public DbSet<User> Users { get; set; }
    }
}
