﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace API.Models
{
    public class JobSeeker
    {
        [Key]
        public int Id { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string Id_Identity { get; set; }
        [ForeignKey("Id_Identity")]
        public virtual AppUser Identity { get; set; }  // navigation property
        public string Location { get; set; }
    }
}
