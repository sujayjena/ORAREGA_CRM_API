using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblPurchaseProofPhotosMetadata
    {
        //[RegularExpression(ValidationConstant.ImageFileRegExp, ErrorMessage = ValidationConstant.ImageFileRegExp_Msg)]
        public string PhotoPath { get; set; }
    }

    [MetadataType(typeof(TblPurchaseProofPhotosMetadata))]
    public partial class tblPurchaseProofPhoto
    {
        public HttpPostedFile ProofPhoto { get; set; }
    }
}