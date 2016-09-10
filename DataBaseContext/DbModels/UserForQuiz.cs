using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.DbModels
{
    public class UserForQuiz
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public virtual User User { get; set; }


        /// <summary>
        /// TimeStamp for DB
        /// </summary>
        [Timestamp]
        public Byte[] Timestamp { get; set; }
    }
}