using OraRegaAV.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace OraRegaAV.DBEntity
{
    public class TblWorkOrderMetaData
    {
    }


    [MetadataType(typeof(TblWorkOrderMetaData))]
    public partial class tblWorkOrder : tblPermanentAddress
    {
        public string CompanyName { get; set; }

    }
}