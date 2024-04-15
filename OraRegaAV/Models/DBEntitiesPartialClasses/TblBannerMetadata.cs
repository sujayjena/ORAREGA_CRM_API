using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblBannerMetadata
    {
        [Required(ErrorMessage = ValidationConstant.LinkRequied_Msg)]
        public string LinkName { get; set; }

        [Required(ErrorMessage = ValidationConstant.PositionRequied_Msg)]
        public int Position { get; set; }

        [Required(ErrorMessage = ValidationConstant.AppTypeRequied_Msg)]
        [DefaultValue("W")]
        public string AppType { get; set; }

        public string ImageFile { get; set; }
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(TblBannerMetadata))]
    public partial class tblBanner
    {
        //public string ImageFileUrl { get; set; }
    }
}