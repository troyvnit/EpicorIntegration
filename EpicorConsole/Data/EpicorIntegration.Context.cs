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
    
    public partial class EpicorIntergrationEntities : DbContext
    {
        public EpicorIntergrationEntities()
            : base("name=EpicorIntergrationEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CUST_BALANCE> CUST_BALANCE { get; set; }
        public virtual DbSet<CUST_OVER_DUE> CUST_OVER_DUE { get; set; }
        public virtual DbSet<CUSTOMER> CUSTOMERs { get; set; }
        public virtual DbSet<CUSTOMER_INFO> CUSTOMER_INFO { get; set; }
        public virtual DbSet<CUSTOMER_SHIPTO> CUSTOMER_SHIPTO { get; set; }
        public virtual DbSet<INVT_TRANS> INVT_TRANS { get; set; }
        public virtual DbSet<INVT_TRANS_DETAIL> INVT_TRANS_DETAIL { get; set; }
        public virtual DbSet<INVT_TRANS_HEADER> INVT_TRANS_HEADER { get; set; }
        public virtual DbSet<PDA_PAYMENT> PDA_PAYMENT { get; set; }
        public virtual DbSet<PO_DETAIL> PO_DETAIL { get; set; }
        public virtual DbSet<PO_HEADER> PO_HEADER { get; set; }
        public virtual DbSet<RPO_DETAIL> RPO_DETAIL { get; set; }
        public virtual DbSet<RPO_HEADER> RPO_HEADER { get; set; }
        public virtual DbSet<SO_HEADER_CONFIRM> SO_HEADER_CONFIRM { get; set; }
        public virtual DbSet<PRICE_LIST> PRICE_LIST { get; set; }
        public virtual DbSet<PRODUCT> PRODUCTs { get; set; }
        public virtual DbSet<SO_DETAIL> SO_DETAIL { get; set; }
        public virtual DbSet<SO_HEADER> SO_HEADER { get; set; }
        public virtual DbSet<ARINVOICE_DETAIL> ARINVOICE_DETAIL { get; set; }
        public virtual DbSet<ARINVOICE_HEADER> ARINVOICE_HEADER { get; set; }
    }
}
