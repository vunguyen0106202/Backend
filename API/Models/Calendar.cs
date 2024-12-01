﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace API.Models
{
    public class Calendar
    {
        [Key]
        public int Id { get; set; }
        public string ToDo{ get; set; }
        public string? IdUser { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        [ForeignKey("IdUser")]
        public virtual AppUser AppUser { get; set; }
        public DateTime Time { get; set; }
    }
}
