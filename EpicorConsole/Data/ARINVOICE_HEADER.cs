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
    
    public partial class ARINVOICE_HEADER
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public string DMSFlag { get; set; }
        public int InvoiceNum { get; set; }
        public string CustomerCode { get; set; }
        public string ShiptoCode { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string Status { get; set; }
        public string SalesmanCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string Remarks { get; set; }
        public string SO_DocNum { get; set; }
        public string CreditMemoDraft_DocNum { get; set; }
    }
}
