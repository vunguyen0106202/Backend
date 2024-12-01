using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class ChatTopics
    {
        [Key]
        public int Id { get; set; }
        public string Note { get; set; }
        public int CustomerId { get; set; }
        public int IdOrder { get; set; }
        public DateTime TimeChat { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public virtual ICollection<UserChat> UserChats { get; set; }
    }
}
