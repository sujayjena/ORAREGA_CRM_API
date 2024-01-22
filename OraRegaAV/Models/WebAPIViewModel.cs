using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace OraRegaAV.Models
{
    public class WebAPIViewModel
    {
    }

    public class OurProduct_Request
    {
        public int Id { get; set; }

        public string ContentName { get; set; }
        public int Position { get; set; }

        [DefaultValue("W")]
        public string AppType { get; set; }
        public string FilesName { get; set; }
        public string Files { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public bool IsActive { get; set; }
    }

    public class OurService_Request
    {
        public int Id { get; set; }

        public string ContentName { get; set; }
        public int Position { get; set; }

        [DefaultValue("W")]
        public string AppType { get; set; }
        public string FilesName { get; set; }
        public string Files { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public bool IsActive { get; set; }
    }
}