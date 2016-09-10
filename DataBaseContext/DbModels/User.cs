using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.DbModels
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Facebook { get; set; }

        public string Telegram { get; set; }

        public string PhoneNumber { get; set; }

        [Timestamp]
        public Byte[] Timestamp { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        [Display(Name = "Дата создания")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; set; }
    }
}
