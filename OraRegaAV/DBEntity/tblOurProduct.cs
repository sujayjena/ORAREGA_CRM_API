//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OraRegaAV.DBEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblOurProduct
    {
        public int Id { get; set; }
        public string ContentName { get; set; }
        public int Position { get; set; }
        public string AppType { get; set; }
        public string ImageFile { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ImageFilePath { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
