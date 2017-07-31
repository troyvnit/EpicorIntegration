USE [EpicorIntegration]
GO
/****** Object:  Table [dbo].[ARINVOICE_DETAIL]    Script Date: 7/31/2017 1:49:47 PM ******/
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
/****** Object:  Table [dbo].[ARINVOICE_HEADER]    Script Date: 7/31/2017 1:49:47 PM ******/
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
/****** Object:  Table [dbo].[CUST_BALANCE]    Script Date: 7/31/2017 1:49:47 PM ******/
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
/****** Object:  Table [dbo].[CUST_OVER_DUE]    Script Date: 7/31/2017 1:49:47 PM ******/
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
/****** Object:  Table [dbo].[CUSTOMER]    Script Date: 7/31/2017 1:49:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CUSTOMER](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[Status] [char](1) NOT NULL,
	[CompanyCode] [nvarchar](30) NULL,
	[CustomerCode] [nvarchar](30) NULL,
	[CustomerName] [nvarchar](200) NULL,
	[ForeignName] [nvarchar](200) NULL,
	[Address1] [nvarchar](50) NULL,
	[Address2] [nvarchar](50) NULL,
	[Address3] [nvarchar](50) NULL,
	[Phone1] [nvarchar](40) NULL,
	[Phone2] [nvarchar](40) NULL,
	[Fax] [nvarchar](40) NULL,
	[ContactPerson] [nvarchar](180) NULL,
	[CreditLimit] [numeric](9, 0) NULL,
	[PriceListNum] [smallint] NULL,
	[Currency] [nvarchar](6) NULL,
	[Country] [nvarchar](6) NULL,
	[PaymentTerm] [nvarchar](4) NULL,
	[GroupTax] [nvarchar](16) NULL,
	[TaxCode] [nvarchar](100) NULL,
	[BusinessRegistrationCertificate] [nvarchar](100) NULL,
	[CustomerType] [nvarchar](3) NULL,
	[LegalName] [nvarchar](200) NULL,
	[GroupCode] [nvarchar](4) NULL,
	[ValidPayer] [bit] NOT NULL,
	[ValidSoldTo] [bit] NOT NULL,
	[ValidShipTo] [bit] NOT NULL,
	[TerritoryCode] [nvarchar](8) NULL,
	[SalesRepCode] [nvarchar](8) NULL,
	[AllowShipTo3] [bit] NOT NULL,
	[AllowOTS] [bit] NOT NULL,
	[PrintStatements] [bit] NOT NULL,
	[CreditHold] [bit] NOT NULL,
	[EstablishedDate] [datetime2](7) NULL,
	[CountryNum] [int] NULL,
	[Email] [nvarchar](50) NULL,
	[ConsolidateSO] [bit] NOT NULL,
 CONSTRAINT [PK_CUSTOMER] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PO_DETAIL]    Script Date: 7/31/2017 1:49:47 PM ******/
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
/****** Object:  Table [dbo].[PO_HEADER]    Script Date: 7/31/2017 1:49:47 PM ******/
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
/****** Object:  Table [dbo].[PRICE_LIST]    Script Date: 7/31/2017 1:49:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PRICE_LIST](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
	[PriceListNum] [nvarchar](50) NOT NULL,
	[ItemCode] [nvarchar](100) NOT NULL,
	[Price] [numeric](9, 0) NOT NULL,
	[Currency] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PRICE_LIST] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PRODUCT]    Script Date: 7/31/2017 1:49:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PRODUCT](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemCode] [nvarchar](100) NOT NULL,
	[ItemName] [nvarchar](200) NOT NULL,
	[ForeignName] [nvarchar](200) NOT NULL,
	[BarCode] [nvarchar](200) NULL,
	[Status] [char](1) NOT NULL,
	[SalesUnit] [nvarchar](200) NOT NULL,
	[SalesItemPerUnit] [numeric](9, 0) NOT NULL,
	[SalesUnitLength] [numeric](9, 0) NULL,
	[SalesUnitWidth] [numeric](9, 0) NULL,
	[SalesUnitHeight] [numeric](9, 0) NULL,
	[SalesUnitVolume] [numeric](9, 0) NULL,
	[SalesUnitWeight] [numeric](9, 0) NULL,
	[SalesVATGroup] [nvarchar](16) NOT NULL,
	[PurchaseUnit] [nvarchar](200) NOT NULL,
	[PurchaseItemsPerUnit] [numeric](9, 0) NOT NULL,
	[PurchaseUnitLength] [numeric](9, 0) NULL,
	[PurchaseUnitWidth] [numeric](9, 0) NULL,
	[PurchaseUnitHeight] [numeric](9, 0) NULL,
	[PurchaseUnitVolume] [numeric](9, 0) NULL,
	[PurchaseUnitWeight] [numeric](9, 0) NULL,
	[PurchaseVATGroup] [nvarchar](16) NULL,
	[InventoryUOM] [nvarchar](200) NOT NULL,
	[MOQ] [numeric](9, 0) NULL,
	[AttrCode0] [nvarchar](100) NULL,
	[AttrName0] [nvarchar](200) NULL,
	[AttrCode01] [nvarchar](100) NULL,
	[AttrName01] [nvarchar](200) NULL,
	[AttrCode02] [nvarchar](100) NULL,
	[AttrName02] [nvarchar](200) NULL,
	[AttrCode03] [nvarchar](100) NULL,
	[AttrName03] [nvarchar](200) NULL,
	[AttrCode04] [nvarchar](100) NULL,
	[AttrName04] [nvarchar](200) NULL,
	[AttrCode05] [nvarchar](100) NULL,
	[AttrName05] [nvarchar](200) NULL,
	[AttrCode06] [nvarchar](100) NULL,
	[AttrName06] [nvarchar](200) NULL,
	[AttrCode07] [nvarchar](100) NULL,
	[AttrName07] [nvarchar](200) NULL,
	[AttrCode08] [nvarchar](100) NULL,
	[AttrName08] [nvarchar](200) NULL,
	[AttrCode09] [nvarchar](100) NULL,
	[AttrName09] [nvarchar](200) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDateTime] [datetime2](7) NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedDateTime] [datetime2](7) NULL,
	[DMSFlag] [char](1) NOT NULL,
 CONSTRAINT [PK_PRODUCT] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
