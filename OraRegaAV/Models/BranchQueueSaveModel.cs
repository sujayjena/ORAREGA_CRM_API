using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class BranchQueueSaveModel
    {
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Required")]
        public int[] QueueId { get; set; }
    }
}