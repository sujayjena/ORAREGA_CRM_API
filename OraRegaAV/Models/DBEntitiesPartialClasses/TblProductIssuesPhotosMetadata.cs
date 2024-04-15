using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace OraRegaAV.DBEntity
{
    public class TblProductIssuesPhotosMetadata
    {
        //[RegularExpression(ValidationConstant.ImageFileRegExp, ErrorMessage = ValidationConstant.ImageFileRegExp_Msg)]
        public string PhotoPath { get; set; }
    }

    [MetadataType(typeof(TblProductIssuesPhotosMetadata))]
    public partial class tblProductIssuesPhoto
    {
        public HttpPostedFile IssueSnap { get; set; }
    }
}