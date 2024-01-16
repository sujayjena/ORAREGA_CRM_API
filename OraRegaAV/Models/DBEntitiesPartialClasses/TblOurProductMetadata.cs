using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblOurProductMetadata
    {
        [Required(ErrorMessage = ValidationConstant.ContentRequied_Msg)]
        public string ContentName { get; set; }

        [Required(ErrorMessage = ValidationConstant.PositionRequied_Msg)]
        public int Position { get; set; }

        [Required(ErrorMessage = ValidationConstant.AppTypeRequied_Msg)]
        [DefaultValue("W")]
        public string AppType { get; set; }

        public string ImageFile { get; set; }
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(TblOurProductMetadata))]
    public partial class tblOurProduct
    {
        //public string ImageFileUrl { get; set; }
    }
}