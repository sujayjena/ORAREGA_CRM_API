using Newtonsoft.Json;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OraRegaAV.DBEntity
{
    public class TblRoleMetadata
    {
        [Required(ErrorMessage = ValidationConstant.RoleNameRequied_Msg)]
        [RegularExpression(ValidationConstant.NameRegExp, ErrorMessage = ValidationConstant.RoleNameRegExp_Msg)]
        [MaxLength(ValidationConstant.Name_MaxLength, ErrorMessage = ValidationConstant.RoleName_MaxLength_Msg)]
        public string RoleName { get; set; }

        [JsonIgnore]
        public ICollection<tblMenuRoleMapping> tblMenuRoleMappings { get; set; }

        [JsonIgnore]
        public Nullable<int> ModifiedBy { get; set; }

        [JsonIgnore]
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        [JsonIgnore] 
        public Nullable<int> CreatedBy { get; set; }

        [JsonIgnore]
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }

    [MetadataType(typeof(TblRoleMetadata))]
    public partial class tblRole
    { 
    
    }

    public class RoleSearchParameters
    {
        public bool? IsActive { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
        //public string RoleName { get; set; }
        //public bool? IsActive { get; set; }
    }

    public class RoleResponseParameters
    {
        public string RoleName { get; set; }
        public bool? IsActive { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
