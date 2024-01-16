using OraRegaAV.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class ServiceAddressParameters
    {
        [Required(ErrorMessage = ValidationConstant.AddressRequied_Msg)]
        [MaxLength(ValidationConstant.Address_MaxLength, ErrorMessage = ValidationConstant.Address_MaxLength_Msg)]
        public string Address { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = ValidationConstant.StateNameRequied_Msg)]
        public int StateId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = ValidationConstant.CityNameRequied_Msg)]
        public int CityId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = ValidationConstant.AreaNameRequied_Msg)]
        public int AreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = ValidationConstant.PincodeRequied_Msg)]
        public int PincodeId { get; set; }

        public bool IsDefault { get; set; }
    }
}
