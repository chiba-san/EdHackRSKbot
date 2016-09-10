using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.DbModels
{
    public class Quiz
    {
        public Quiz()
        {
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public string Cover { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        [Display(Name = "Дата создания")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; set; }

        /// <summary>
        /// TimeStamp for DB
        /// </summary>
        [Timestamp]
        public Byte[] Timestamp { get; set; }

        public int OwnerId { get; set; }

        public virtual User Owner{get; set;}

        
        public virtual ICollection<Question> Questions { get; set; }
    }
}