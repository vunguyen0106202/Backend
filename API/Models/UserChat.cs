using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace API.Models
{
    public class UserChat
    { 
        [Key]
        public int Id { get; set; }
        public string ContentChat { get; set; }
        public int CustomerId { get; set; }
        public int IdOrder { get; set; }
        public string Image { get; set; }
        public int IdStatus { get; set; }
        public DateTime TimeChat { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual AppUser AppUser { get; set; }
        public int? ChatTopicId { get; set; }
        [ForeignKey("ChatTopicId")]
        public virtual ChatTopics ChatTopic { get; set; }
    }
}
