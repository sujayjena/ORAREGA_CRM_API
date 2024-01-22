using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class CareerPostRequest
    {
        public int CareerPostId { get; set; }

        [Required]
        public string JobTitle { get; set; }

        [Required]
        public string RequiredExp { get; set; }

        [Required]
        public string Vacancies { get; set; }

        [Required]
        public string JobLocation { get; set; }

        [Required]
        public string JobDetails { get; set; }

        public bool IsActive { get; set; }
    }
}