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
        public virtual DbSet<xvtyx_DMSProduct> xvtyx_DMSProduct { get; set; }
    
        public virtual ObjectResult<sptyx_DMSCustBalance_Result> sptyx_DMSCustBalance()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSCustBalance_Result>("sptyx_DMSCustBalance");
        }
    
        public virtual ObjectResult<sptyx_DMSCustOverDue_Result> sptyx_DMSCustOverDue()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSCustOverDue_Result>("sptyx_DMSCustOverDue");
        }
    
        public virtual ObjectResult<sptyx_DMSPriceList_Result> sptyx_DMSPriceList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSPriceList_Result>("sptyx_DMSPriceList");
        }
    
        public virtual ObjectResult<sptyx_DMSCustInfo_Result> sptyx_DMSCustInfo()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sptyx_DMSCustInfo_Result>("sptyx_DMSCustInfo");
        }
    }
}
