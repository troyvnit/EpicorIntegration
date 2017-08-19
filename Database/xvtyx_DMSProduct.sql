USE [ERPAPPTRAIN]
GO

/****** Object:  View [dbo].[xvtyx_DMSProduct]    Script Date: 8/19/2017 10:29:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[xvtyx_DMSProduct]  
AS  
SELECT P.Company
		, Partnum ItemCode
		, PartDescription ItemName
		, InActive [Status]
		, SalesUM SalesUnit
		, 1 SalesItemsPerUnit 
		, PartLength SalesUnitLength
		, PartWidth SalesUnitWidth
		, PartHeight SalesUnitHeight
		, NetVolume SalesUnitVolume
		, NetWeight SalesUnitWeight
		, P.TaxCatID SalesVATGroup 
		, PUM PurchaseUnit
		, IUM InventoryUOM
		, P.ProdCode AttrCode0 -- Product Group
		, ProdGrup.[Description] AttrName0
		, P.ClassID AttrCode1 -- Prodcut Class
		, Partclass.[Description] AttrName1 
		, P.ShortChar05 AttrCode2 -- Cong dung
		, CDung.Character01 AttrName2
		, P.Character02 AttrCode3 -- Vat nuoi
		, VNuoi.Character01 AttrName3
		, P.Character03 AttrCode4 -- Dang bao che
		, BChe.Character01 AttrName4
		, P.Character04 AttrCode5 -- Cachdung
		, CachDung.Character01 AttrName5
		, P.Character05 AttrCode6 -- Nha Cung Cap
		, NCC.Character01 AttrName6
		, P.ShortChar02 AttrCode7 -- Nhom Hang
		, NhomHang.Character01 AttrName7
		, P.ShortChar03 AttrCode8 -- Quy Cach
		, QuyCach.Character01 AttrName8
		, '' AttrCode9 -- User Future
		, '' AttrName9 -- User Future
		, P.sysRevID
		, P.UD_SysRevID
FROM Part P
LEFT JOIN erp.ProdGrup ProdGrup ON P.Company = ProdGrup.Company AND P.ProdCode = ProdGrup.ProdCode
LEFT JOIN erp.Partclass Partclass ON P.Company = Partclass.Company AND P.ClassID = Partclass.ClassID
LEFT JOIN ice.UD10 CDung ON P.Company = CDung.Company AND P.ShortChar05 = CDung.Key1 -- Cong Dung
LEFT JOIN ice.UD11 VNuoi ON P.Company = VNuoi.Company AND P.Character02 = VNuoi.Key1 -- Vat Nuoi
LEFT JOIN ice.UD12 BChe ON P.Company = BChe.Company AND P.Character03 = BChe.Key1 -- Dang bao che
LEFT JOIN ice.UD13 CachDung ON P.Company = CachDung.Company AND P.Character04 = Cachdung.Key1 -- Lieu dung --> Cachdung
LEFT JOIN ice.UD14 NCC ON P.Company = NCC.Company AND P.Character05 = NCC.Key1 -- Cong dung chi tiet --> Nha Cung Cap
LEFT JOIN ice.UD07 NhomHang ON P.Company = Nhomhang.Company AND P.ShortChar02 = NhomHang.Key1  --Ham Luong --> Nhom Hang
LEFT JOIN ice.UD08 QuyCach ON P.Company = QuyCach.Company AND P.ShortChar03 = QuyCach.Key1  --Thanh Phan --> QuyCach
--WHERE P.company in ('GRV','PMN') and P.Partnum = '820001-TEST'





--select Key1, Character01 from ice.UD09 where Company = 'PMN' -- Chung loai
 

GO


