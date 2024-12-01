using API.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace API.Dtos.ModelOveride
{
    public class CategoryOveride
    { 
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
