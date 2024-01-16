using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace OraRegaAV.DBEntity
{
    public class TblProductMetadata
    {
    }

    [MetadataType(typeof(TblProductMetadata))]
    public partial class tblProduct
    {
        public string ProductType { get; set; }
    }
}