using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.DbModels
{
    public class Question
    {
        public Question()
        {
            Options = new HashSet<Option>();
            Answers = new HashSet<Answer>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Text { get; set; }

        public int Order { get; set; }


        public bool IsActive { get; set; }

        [Timestamp]
        public Byte[] Timestamp { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<Option> Options { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}