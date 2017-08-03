USE [EpicorIntegration]
GO
/****** Object:  Table [dbo].[ARINVOICE_DETAIL]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ARINVOICE_DETAIL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[InvoiceNum] [int] NOT NULL,
	[InvoiceLine] [int] NOT NULL,
	[WhsCode] [nvarchar](40) NULL,
	[CompanyCode] [nvarchar](40) NULL,
	[BranchCode] [nvarchar](40) NULL,
	[ProductCode] [nvarchar](40) NULL,
	[UoM] [nvarchar](200) NOT NULL,
	[Quantity] [numeric](17, 5) NOT NULL,
	[Price] [numeric](17, 5) NOT NULL,
	[Discount_Amount] [numeric](17, 5) NOT NULL,
	[Discount_Percent] [numeric](17, 5) NOT NULL,
	[Amount] [numeric](17, 5) NULL,
	[SO_DocNum] [nvarchar](40) NOT NULL,
	[CreditMemoDraft_DocNum] [nvarchar](40) NULL,
 CONSTRAINT [PK_ARINVOICE_DETAIL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ARINVOICE_HEADER]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ARINVOICE_HEADER](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[InvoiceNum] [int] NOT NULL,
	[CustomerCode] [nvarchar](40) NULL,
	[ShiptoCode] [nvarchar](40) NULL,
	[CompanyCode] [nvarchar](40) NULL,
	[BranchCode] [nvarchar](40) NULL,
	[Status] [nvarchar](40) NULL,
	[SalesmanCode] [nvarchar](40) NULL,
	[TotalAmount] [numeric](17, 5) NOT NULL,
	[Type] [nvarchar](3) NOT NULL,
	[InvoiceDate] [datetime2](7) NULL,
	[Remarks] [nvarchar](510) NULL,
	[SO_DocNum] [nvarchar](40) NOT NULL,
	[CreditMemoDraft_DocNum] [nvarchar](40) NULL,
 CONSTRAINT [PK_ARINVOICE_HEADER] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CUST_BALANCE]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CUST_BALANCE](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerCode] [nvarchar](50) NOT NULL,
	[CompanyCode] [nvarchar](20) NOT NULL,
	[CreditLimit] [numeric](17, 5) NOT NULL,
	[Balance] [numeric](17, 5) NOT NULL,
 CONSTRAINT [PK_CUST_BALANCE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CUST_OVER_DUE]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CUST_OVER_DUE](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerCode] [nvarchar](50) NOT NULL,
	[CompanyCode] [nvarchar](20) NOT NULL,
	[PaymentTerm] [nvarchar](40) NOT NULL,
	[MonthID] [nvarchar](6) NULL,
	[CreditLimit] [numeric](17, 5) NOT NULL,
	[BalanceDue] [numeric](17, 5) NOT NULL,
	[OverDue] [numeric](17, 5) NOT NULL,
 CONSTRAINT [PK_CUST_OVER_DUE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CUSTOMER]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CUSTOMER](
	[CompanyCode] [nvarchar](30) NOT NULL,
	[CustomerCode] [nvarchar](30) NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[ForeignName] [nvarchar](200) NULL,
	[Address] [nvarchar](200) NULL,
	[Phone1] [nvarchar](40) NULL,
	[Phone2] [nvarchar](40) NULL,
	[Fax] [nvarchar](40) NULL,
	[ContactPerson] [nvarchar](180) NULL,
	[CreditLimit] [numeric](18, 0) NULL,
	[PriceListNum] [smallint] NOT NULL,
	[Currency] [nvarchar](6) NOT NULL,
	[Country] [nvarchar](6) NOT NULL,
	[PaymentTerm] [nvarchar](100) NOT NULL,
	[GroupTax] [nvarchar](16) NOT NULL,
	[TaxCode] [nvarchar](100) NULL,
	[BusinessRegistrationCertificate] [nvarchar](100) NULL,
	[CustomerType] [nvarchar](100) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[Status] [char](1) NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_CUSTOMER] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CUSTOMER_SHIPTO]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CUSTOMER_SHIPTO](
	[CompanyCode] [nvarchar](30) NOT NULL,
	[CustomerCode] [nvarchar](30) NOT NULL,
	[ShiptoCode] [nvarchar](30) NOT NULL,
	[AddressName] [nvarchar](100) NOT NULL,
	[Address1] [nvarchar](100) NULL,
	[Address2] [nvarchar](100) NULL,
	[Address3] [nvarchar](100) NULL,
	[Country] [nvarchar](6) NOT NULL,
	[DistrictCode] [nvarchar](100) NULL,
	[DistrictName] [nvarchar](200) NULL,
	[ProvinceCode] [nvarchar](100) NULL,
	[ProvinceName] [nvarchar](200) NULL,
	[RegionCode] [nvarchar](100) NULL,
	[RegionName] [nvarchar](200) NULL,
	[StreetCode] [nvarchar](100) NULL,
	[StreetName] [nvarchar](200) NULL,
	[WardCode] [nvarchar](100) NULL,
	[WardName] [nvarchar](200) NULL,
	[AttrCode0] [nvarchar](100) NULL,
	[AttrName0] [nvarchar](200) NULL,
	[AttrCode1] [nvarchar](100) NULL,
	[AttrName1] [nvarchar](200) NULL,
	[AttrCode2] [nvarchar](100) NULL,
	[AttrName2] [nvarchar](200) NULL,
	[AttrCode3] [nvarchar](100) NULL,
	[AttrName3] [nvarchar](200) NULL,
	[AttrCode4] [nvarchar](100) NULL,
	[AttrName4] [nvarchar](200) NULL,
	[AttrCode5] [nvarchar](100) NULL,
	[AttrName5] [nvarchar](200) NULL,
	[AttrCode6] [nvarchar](100) NULL,
	[AttrName6] [nvarchar](200) NULL,
	[AttrCode7] [nvarchar](100) NULL,
	[AttrName7] [nvarchar](200) NULL,
	[AttrCode8] [nvarchar](100) NULL,
	[AttrName8] [nvarchar](200) NULL,
	[AttrCode9] [nvarchar](100) NULL,
	[AttrName9] [nvarchar](200) NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NOT NULL,
	[LastUpdatedDateTime] [datetime] NOT NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_CUSTOMER_SHIPTO] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[ShiptoCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[INVT_TRANS]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[INVT_TRANS](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[Company] [nvarchar](8) NOT NULL,
	[Site] [nvarchar](20) NOT NULL,
	[PartDescription] [nvarchar](max) NOT NULL,
	[PartNum] [nvarchar](50) NOT NULL,
	[WareHouseCode] [nvarchar](8) NOT NULL,
	[TranDate] [date] NULL,
	[SysDate] [date] NULL,
	[OrderNum] [int] NOT NULL,
	[TranType] [nvarchar](7) NOT NULL,
	[TranNum] [int] NOT NULL,
	[TranClass] [nvarchar](2) NOT NULL,
	[TranQty] [decimal](22, 8) NULL,
	[LotNum] [nvarchar](30) NOT NULL,
	[InvtyUOM] [nvarchar](6) NOT NULL,
 CONSTRAINT [PK_INVT_TRANS] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[INVT_TRANS_DETAIL]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[INVT_TRANS_DETAIL](
	[CompanyCode] [nvarchar](40) NOT NULL,
	[DocNum] [nvarchar](40) NOT NULL,
	[LineNum] [nvarchar](40) NOT NULL,
	[DocType] [nvarchar](10) NOT NULL,
	[ProductCode] [nvarchar](100) NOT NULL,
	[Quantity] [numeric](19, 6) NOT NULL,
	[UoM] [nvarchar](40) NOT NULL,
	[BranchCode] [nvarchar](30) NOT NULL,
	[WhsCode] [nvarchar](16) NOT NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_INVT_TRANS_DETAIL] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DocNum] ASC,
	[LineNum] ASC,
	[DocType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[INVT_TRANS_HEADER]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[INVT_TRANS_HEADER](
	[CompanyCode] [nvarchar](40) NOT NULL,
	[DocNum] [nvarchar](40) NOT NULL,
	[RefNum] [nvarchar](40) NULL,
	[DocType] [nvarchar](10) NOT NULL,
	[PostingDate] [datetime] NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[BranchCode] [nvarchar](30) NOT NULL,
	[Module] [nvarchar](10) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[Status] [nvarchar](40) NOT NULL,
	[DMSFlag] [char](1) NULL,
 CONSTRAINT [PK_INVT_TRANS_HEADER] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DocNum] ASC,
	[DocType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PDA_PAYMENT]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PDA_PAYMENT](
	[PaymentID] [nvarchar](40) NOT NULL,
	[CustomerCode] [nvarchar](40) NOT NULL,
	[PayDate] [date] NOT NULL,
	[PayAmount] [numeric](19, 6) NOT NULL,
	[Remarks] [nvarchar](240) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_PDA_PAYMENT] PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PO_DETAIL]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PO_DETAIL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[CompanyCode] [nvarchar](30) NULL,
	[PONum] [int] NOT NULL,
	[POLine] [int] NOT NULL,
	[CalcTranType] [nvarchar](7) NOT NULL,
	[PartNum] [nvarchar](50) NOT NULL,
	[QtyOption] [nvarchar](10) NOT NULL,
	[CalcVendQty] [numeric](9, 0) NOT NULL,
	[PUM] [nvarchar](6) NOT NULL,
	[DocUnitCost] [numeric](9, 0) NOT NULL,
	[DocScrUnitCost] [numeric](9, 0) NOT NULL,
	[CalcDueDate] [datetime2](7) NOT NULL,
	[Taxable] [bit] NOT NULL,
	[RcvInspectionReq] [bit] NOT NULL,
	[BranchCode] [nvarchar](30) NULL,
	[WhsCode] [nvarchar](16) NULL,
 CONSTRAINT [PK_PO_DETAIL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PO_HEADER]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PO_HEADER](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[Status] [char](1) NOT NULL,
	[CompanyCode] [nvarchar](30) NULL,
	[PONum] [int] NOT NULL,
	[RefNum] [nvarchar](40) NOT NULL,
	[POType] [nvarchar](3) NOT NULL,
	[PaymentTerm] [nvarchar](4) NOT NULL,
	[Currency] [nvarchar](6) NOT NULL,
	[ShipViaCode] [nvarchar](4) NOT NULL,
	[PostingDate] [datetime2](7) NOT NULL,
	[ApprovalStatus] [nvarchar](2) NOT NULL,
	[ShipState] [nvarchar](50) NOT NULL,
	[DeliveryDate] [datetime2](7) NOT NULL,
	[PODate] [datetime2](7) NOT NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[ShippingDate] [datetime2](7) NOT NULL,
	[BranchCode] [nvarchar](30) NULL,
	[VendorNumber] [int] NOT NULL,
	[POTotal] [numeric](9, 0) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[EntryPerson] [nvarchar](75) NULL,
	[BuyerID] [nvarchar](8) NOT NULL,
	[ExchangeRate] [numeric](9, 0) NULL,
 CONSTRAINT [PK_PO_HEADER] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PRICE_LIST]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PRICE_LIST](
	[PriceListNum] [smallint] IDENTITY(1,1) NOT NULL,
	[PriceListName] [nvarchar](100) NOT NULL,
	[ItemCode] [nvarchar](100) NOT NULL,
	[Price] [numeric](18, 0) NOT NULL,
	[Currency] [nvarchar](100) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[ListCode] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_PRICE_LIST] PRIMARY KEY CLUSTERED 
(
	[PriceListNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PRODUCT]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PRODUCT](
	[ItemCode] [nvarchar](100) NOT NULL,
	[ItemName] [nvarchar](200) NOT NULL,
	[ForeignName] [nvarchar](200) NOT NULL,
	[BarCode] [nvarchar](200) NULL,
	[Status] [char](1) NOT NULL,
	[SalesUnit] [nvarchar](200) NOT NULL,
	[SalesItemsPerUnit] [numeric](18, 0) NOT NULL,
	[SalesUnitLength] [numeric](18, 0) NULL,
	[SalesUnitWidth] [numeric](18, 0) NULL,
	[SalesUnitHeight] [numeric](18, 0) NULL,
	[SalesUnitVolume] [numeric](18, 0) NULL,
	[SalesUnitWeight] [numeric](18, 0) NULL,
	[SalesVATGroup] [nvarchar](16) NOT NULL,
	[PurchaseUnit] [nvarchar](200) NOT NULL,
	[PurchaseItemsPerUnit] [numeric](18, 0) NOT NULL,
	[PurchaseUnitLength] [numeric](18, 0) NULL,
	[PurchaseUnitWidth] [numeric](18, 0) NULL,
	[PurchaseUnitHeight] [numeric](18, 0) NULL,
	[PurchaseUnitVolume] [numeric](18, 0) NULL,
	[PurchaseUnitWeight] [numeric](18, 0) NULL,
	[PurchaseVATGroup] [nvarchar](16) NULL,
	[InventoryUOM] [nvarchar](200) NOT NULL,
	[MOQ] [numeric](18, 0) NULL,
	[AttrCode0] [nvarchar](100) NULL,
	[AttrName0] [nvarchar](200) NULL,
	[AttrCode1] [nvarchar](100) NULL,
	[AttrName1] [nvarchar](200) NULL,
	[AttrCode2] [nvarchar](100) NULL,
	[AttrName2] [nvarchar](200) NULL,
	[AttrCode3] [nvarchar](100) NULL,
	[AttrName3] [nvarchar](200) NULL,
	[AttrCode4] [nvarchar](100) NULL,
	[AttrName4] [nvarchar](200) NULL,
	[AttrCode5] [nvarchar](100) NULL,
	[AttrName5] [nvarchar](200) NULL,
	[AttrCode6] [nvarchar](100) NULL,
	[AttrName6] [nvarchar](200) NULL,
	[AttrCode7] [nvarchar](100) NULL,
	[AttrName7] [nvarchar](200) NULL,
	[AttrCode8] [nvarchar](100) NULL,
	[AttrName8] [nvarchar](200) NULL,
	[AttrCode9] [nvarchar](100) NULL,
	[AttrName9] [nvarchar](200) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_PRODUCT] PRIMARY KEY CLUSTERED 
(
	[ItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RPO_DETAIL]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RPO_DETAIL](
	[CompanyCode] [nvarchar](30) NOT NULL,
	[DocNum] [nvarchar](40) NOT NULL,
	[LineNum] [nvarchar](40) NOT NULL,
	[DocType] [nchar](10) NOT NULL,
	[ProductCode] [nvarchar](40) NOT NULL,
	[Quantity] [numeric](17, 6) NOT NULL,
	[Price] [numeric](17, 6) NOT NULL,
	[Amount] [numeric](17, 6) NOT NULL,
	[UoM] [nvarchar](50) NOT NULL,
	[BranchCode] [nvarchar](30) NOT NULL,
	[WhsCode] [nvarchar](30) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_RPO_DETAIL] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DocNum] ASC,
	[LineNum] ASC,
	[DocType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RPO_HEADER]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RPO_HEADER](
	[CompanyCode] [nvarchar](30) NOT NULL,
	[DocNum] [nvarchar](40) NOT NULL,
	[RefNum] [nvarchar](40) NULL,
	[DocType] [nvarchar](10) NOT NULL,
	[PostingDate] [datetime] NOT NULL,
	[DeliveryDate] [datetime] NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[ShippingDate] [datetime] NOT NULL,
	[BranchCode] [nvarchar](30) NOT NULL,
	[VendorCode] [nvarchar](30) NOT NULL,
	[DocTotal] [numeric](17, 6) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[Status] [nvarchar](40) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_GPO_HEADER] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DocNum] ASC,
	[DocType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SO_DETAIL]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SO_DETAIL](
	[CompanyCode] [nvarchar](30) NOT NULL,
	[DocNum] [nvarchar](40) NOT NULL,
	[LineNum] [nvarchar](40) NOT NULL,
	[ProductCode] [nvarchar](40) NOT NULL,
	[Quantity] [numeric](18, 0) NOT NULL,
	[Price] [numeric](18, 0) NOT NULL,
	[Amount] [numeric](18, 0) NOT NULL,
	[UoM] [nvarchar](40) NOT NULL,
	[BranchCode] [nvarchar](30) NOT NULL,
	[WhsCode] [nvarchar](16) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_SO_DETAI] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DocNum] ASC,
	[LineNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SO_HEADER]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SO_HEADER](
	[CompanyCode] [nvarchar](30) NOT NULL,
	[DocNum] [nvarchar](40) NOT NULL,
	[BranchCode] [nvarchar](30) NOT NULL,
	[CustomerCode] [nvarchar](30) NOT NULL,
	[ShiptoCode] [nvarchar](30) NOT NULL,
	[SalesmanCode] [nvarchar](40) NOT NULL,
	[DeliveryDate] [datetime] NOT NULL,
	[TotalAmount] [numeric](18, 0) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[Status] [nvarchar](40) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_SO_HEADER_DMS] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DocNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SO_HEADER_CONFIRM]    Script Date: 8/3/2017 9:01:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SO_HEADER_CONFIRM](
	[CompanyCode] [nvarchar](40) NOT NULL,
	[DocNum] [nvarchar](40) NOT NULL,
	[ExtDocNum] [nvarchar](40) NOT NULL,
	[BranchCode] [nvarchar](40) NOT NULL,
	[CustomerCode] [nvarchar](40) NOT NULL,
	[ShiptoCode] [nvarchar](40) NOT NULL,
	[SalesmanCode] [nvarchar](40) NOT NULL,
	[DeliveryDate] [datetime] NOT NULL,
	[TotalAmount] [numeric](18, 0) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[Status] [nvarchar](40) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime] NULL,
	[SystemLog] [nvarchar](400) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_SO_HEADER_CONFIRM] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DocNum] ASC,
	[ExtDocNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
