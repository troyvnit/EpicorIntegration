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
    
    public partial class PriceLst
    {
        public string Company { get; set; }
        public string ListCode { get; set; }
        public string CurrencyCode { get; set; }
        public string ListDescription { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string WarehouseList { get; set; }
        public bool GlobalPriceLst { get; set; }
        public bool GlobalLock { get; set; }
        public string ListType { get; set; }
        public byte[] SysRevID { get; set; }
        public System.Guid SysRowID { get; set; }
        public bool UseZeroPrice { get; set; }
    }
}
