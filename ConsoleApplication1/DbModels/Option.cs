using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.DbModels
{
    public class Option
    {
        public Option()
        {
 
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Text { get; set; }

        public int Order { get; set; }

        public bool IsAnswer { get; set; }

        [Timestamp]
        public Byte[] Timestamp { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}