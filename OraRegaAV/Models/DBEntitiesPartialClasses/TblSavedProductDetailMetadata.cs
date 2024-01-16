using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblSavedProductDetailMetadata
    {
        //[Range(1, int.MaxValue, ErrorMessage = ValidationConstant.ProductModelRequired_Msg)]
        //public int ProdModelId { get; set; }

        //[RegularExpression(ValidationConstant.ProdSerialNoRegExp, ErrorMessage = ValidationConstant.ProdSerialNoRegExp_Msg)]
        //[MaxLength(ValidationConstant.ProdSerialNo_MaxLength, ErrorMessage = ValidationConstant.ProdSerialNo_MaxLength_Msg)]
        //public string ProdSerialNo { get; set; }

        //[RegularExpression(ValidationConstant.ProdNumberRegExp, ErrorMessage = ValidationConstant.ProdNumberRegExp_Msg)]
        //[MaxLength(ValidationConstant.ProdNumber_MaxLength, ErrorMessage = ValidationConstant.ProdNumber_MaxLength_Msg)]
        //public string ProdNumber { get; set; }

        //public int? ProdDescId { get; set; }

        //public int? ProdConditionId { get; set; }

        //[MaxLength(ValidationConstant.OtherCommentOrDesc_MaxLength_500, ErrorMessage = ValidationConstant.ProdDescIfOther_MaxLength_Msg)]
        //public string ProdDescIfOther { get; set; }

        //[MaxLength(ValidationConstant.OtherCommentOrDesc_MaxLength_500, ErrorMessage = ValidationConstant.ProdModelIfOther_MaxLength_Msg)]
        //public string ProdModelIfOther { get; set; }
    }

    [MetadataType(typeof(TblSavedProductDetailMetadata))]
    public partial class tblSavedProductDetail
    {
    }
}
