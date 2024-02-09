using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models
{
    public class WebsiteSearchParameter
    {
        [DefaultValue("")]
        public string AppType { get; set; }
        public bool? IsActive { get; set; }

        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
    public class WebsiteSerachParameter
    {
        public bool? IsActive { get; set; }
        public string SearchValue { get; set; }

        [DefaultValue(0)]
        public int PageSize { get; set; }

        [DefaultValue(0)]
        public int PageNo { get; set; }
    }
    public class RefundAndCancellationPolicyRequest
    {
        public int Id { get; set; }
        public string RefundAndCancellationPolicy { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

    public class PaymentPolicyRequest
    {
        public int Id { get; set; }
        public string PaymentPolicy { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

    public class PrivacyAndPolicyRequest
    {
        public int Id { get; set; }
        public string PrivacyAndPolicy { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

    public class TermsAndConditionRequest
    {
        public int Id { get; set; }
        public string TermsAndCondition { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

