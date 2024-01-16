using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblPaymentTermMetadata
    {
        [Required(ErrorMessage = ValidationConstant.PaymentTermRequied_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.PaymentTerm_MaxLength_Msg)]
        public string PaymentTerms { get; set; }

    }
    [MetadataType(typeof(TblPaymentTermMetadata))]

    public partial class tblPaymentTerm
    { }
}