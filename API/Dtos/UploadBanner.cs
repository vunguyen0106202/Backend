using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace API.Dtos
{
    public class UploadBanner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BannerType { get; set; }
        public string Link { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public IFormFile files { get; set; }
        //public IFormFile files { get; set; }
    }
}
