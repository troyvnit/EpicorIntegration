//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EpicorConsole.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class RPO_DETAIL
    {
        public string CompanyCode { get; set; }
        public string DocNum { get; set; }
        public string LineNum { get; set; }
        public string DocType { get; set; }
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string UoM { get; set; }
        public string BranchCode { get; set; }
        public string WhsCode { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public string SystemLog { get; set; }
        public string DMSFlag { get; set; }
    }
}
