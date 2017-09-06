﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ERPAPPTRAINEntities : DbContext
    {
        public ERPAPPTRAINEntities()
            : base("name=ERPAPPTRAINEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<PriceLst> PriceLsts { get; set; }
        public virtual DbSet<PriceLstPart> PriceLstParts { get; set; }
    
        public virtual ObjectResult<sptyx_DMSCustOverDue_Result> sptyx_DMSCustOverDue()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSCustOverDue_Result>("sptyx_DMSCustOverDue");
        }
    
        public virtual ObjectResult<sptyx_DMSARInvoice_Result> sptyx_DMSARInvoice()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSARInvoice_Result>("sptyx_DMSARInvoice");
        }
    
        public virtual ObjectResult<sptyx_DMSCustInfo_Result> sptyx_DMSCustInfo()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSCustInfo_Result>("sptyx_DMSCustInfo");
        }
    
        public virtual ObjectResult<sptyx_DMSPriceList_Result> sptyx_DMSPriceList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSPriceList_Result>("sptyx_DMSPriceList");
        }
    
        public virtual ObjectResult<sptyx_DMSProduct_Result> sptyx_DMSProduct(string sysRevID, string uD_SysRevID)
        {
            var sysRevIDParameter = sysRevID != null ?
                new ObjectParameter("SysRevID", sysRevID) :
                new ObjectParameter("SysRevID", typeof(string));
    
            var uD_SysRevIDParameter = uD_SysRevID != null ?
                new ObjectParameter("UD_SysRevID", uD_SysRevID) :
                new ObjectParameter("UD_SysRevID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSProduct_Result>("sptyx_DMSProduct", sysRevIDParameter, uD_SysRevIDParameter);
        }
    }
}
