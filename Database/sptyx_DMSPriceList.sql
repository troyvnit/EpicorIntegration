USE [ERPAPPTRAIN]
GO

/****** Object:  StoredProcedure [dbo].[sptyx_DMSPriceList]    Script Date: 8/19/2017 10:27:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sptyx_DMSPriceList]              
                
/* Create by MinhNH on 9th Aug  2017                
   -- V01: Get Customer Price List  */                
                
AS             
         
          
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED            
SET NOCOUNT ON;              
SET FMTONLY OFF  

CREATE TABLE #PriceListTemp 
(
	Company nvarchar(10),
	PriceListNum nvarchar(10),
	PriceListDesc nvarchar(200),
	Startdate datetime,
	Enddate datetime,
	Partnum nvarchar(20),
	BasePrice float,
	BaseList int,
	sysRevID nvarchar(max)
)


DECLARE @Company nvarchar(10)
DECLARE @ListCode nvarchar(50)
DECLARE @ListDescription nvarchar(100)
DECLARE @Discount float
DECLARE @Startdate datetime
DECLARE @Enddate datetime

DECLARE db_cursor CURSOR FOR  

SELECT PLGrupBrk.Company
		, PLGrupBrk.ListCode
		, PriceLst.ListDescription
		, PriceLst.StartDate
		, PriceLst.EndDate
		, PLGrupBrk.DiscountPercent 
FROM ERP.PLGrupBrk PLGrupBrk
INNER JOIN erp.PriceLst PriceLst ON PLGrupBrk.Company = PriceLst.Company AND PLGrupBrk.ListCode = PriceLst.ListCode
GROUP BY PLGrupBrk.Company
		, PLGrupBrk.ListCode
		, PriceLst.ListDescription
		, PriceLst.StartDate
		, PriceLst.EndDate
		, PLGrupBrk.DiscountPercent 

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @Company, @ListCode, @ListDescription,@Startdate, @Enddate,@Discount 

WHILE @@FETCH_STATUS = 0   
BEGIN   
	INSERT INTO #PriceListTemp (Company, PriceListNum, PriceListDesc, StartDate, EndDate, Partnum, BasePrice, BaseList, sysRevID)
	SELECT @Company Company
			, @ListCode PriceListNum
			, @ListDescription PriceListDesc
			, @Startdate StartDate
			, @Enddate EndDate 	
			, Part.PartNum
			, BasePrice = Part.BasePrice * (100 - @Discount)
			, 0 BaseList
			, UPPER(sys.fn_sqlvarbasetostr(CONVERT(VARBINARY,Part.sysRevID))) sysRevID 
	FROM erp.PriceLst P
	INNER JOIN erp.PriceLstParts Part ON P.Company = Part.Company AND P.ListCode = Part.ListCode
	WHERE P.Company = @Company AND ListDescription like ('%Base') 
     
	FETCH NEXT FROM db_cursor INTO @Company, @ListCode, @ListDescription,@Startdate, @Enddate,@Discount     
END   

CLOSE db_cursor   
DEALLOCATE db_cursor

-- Get BasePrices List
SELECT P.Company
	, P.ListCode PriceListNum
	, P.ListDescription PriceListDesc
	, P.StartDate
	, P.EndDate 	
	, Part.PartNum
	, Part.BasePrice
	, 1 BaseList
	, UPPER(sys.fn_sqlvarbasetostr(CONVERT(VARBINARY,Part.sysRevID))) sysRevID 
FROM erp.PriceLst P
INNER JOIN erp.PriceLstParts Part ON P.Company = Part.Company AND P.ListCode = Part.ListCode
WHERE ListDescription like ('%Base')

UNION ALL

-- Get Prices List not Base
SELECT P.Company
	, P.ListCode PriceListNum
	, P.ListDescription PriceListDesc
	, P.StartDate
	, P.EndDate 	
	, Part.PartNum
	, Part.BasePrice
	, 0 BaseList
	, UPPER(sys.fn_sqlvarbasetostr(CONVERT(VARBINARY,Part.sysRevID))) sysRevID 
FROM erp.PriceLst P
INNER JOIN erp.PriceLstParts Part ON P.Company = Part.Company AND P.ListCode = Part.ListCode
WHERE ListDescription not like ('%Base')

UNION ALL

-- Get Prices List Discount

SELECT Company
	, PriceListNum
	, PriceListDesc
	, StartDate
	, EndDate 	
	, PartNum
	, BasePrice
	, BaseList
	, sysRevID
FROM #PriceListTemp


GO


