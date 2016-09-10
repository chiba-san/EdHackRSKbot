using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.DbModels
{
    public class Answer
    {
        public Answer()
        {
 
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserResponse { get; set; }

        public bool IsCorrect { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; set; }

        [Timestamp]
        public Byte[] Timestamp { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}