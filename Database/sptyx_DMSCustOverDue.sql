USE [ERPAPPTRAIN]
GO

/****** Object:  StoredProcedure [dbo].[sptyx_DMSCustOverDue]    Script Date: 8/19/2017 10:27:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sptyx_DMSCustOverDue]              
                
/* Create by MinhNH on 30th July 2017                
   -- V01: Get Customer Balance  */                
                
AS             
         
          
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED            
SET NOCOUNT ON;     
SET FMTONLY OFF           


CREATE TABLE #ARBalance        
(        
 Company  nvarchar(10),        
 Custnum  int,         
 CustID   NVARCHAR(20),   
 PaymentTerm nvarchar(100), 
 CreditLimit float,         
 Balance  float,  
 CreaditOnTime float,
 OverDue float,        
 CreateDate datetime
)        
        
-- Summary ARBalance        
INSERT INTO #ARBalance (Company,Custnum,CustID,PaymentTerm,CreditLimit,Balance,CreaditOnTime,OverDue,CreateDate)     
SELECT  H.Company         
		, C.Custnum	
		, C.CustID          
		, T.Description AS PaymentTerm 		
		, C.CreditLimit        
		, SUM(H.InvoiceBal) AS Balance   
		, CreaditOnTime = 0 
		, OverDue = 0
		, getdate() AS CreateDate        
FROM erp.InvcHead H WITH (NOLOCK)       
INNER JOIN Customer C WITH (NOLOCK) ON H.Company = C.Company AND H.CustNum = C.CustNum         
INNER JOIN erp.Terms T ON C.Company = T.Company AND C.TermsCode = T.TermsCode
WHERE H.UnpostedBal <> 0         
--  and H.Company = 'PMN'          
  AND LEFT(C.Custid,1) = '8'        
GROUP BY H.Company         
		, C.Custnum	
		, C.CustID           		
		, C.CreditLimit       
		, T.Description              

-- Caculate Balance ON TIME                        
UPDATE #ARBalance       
SET #ARBalance.CreaditOnTime = OnTime.CreaditOnTime                 
FROM ( SELECT H.Company ,
			  H.Custnum ,       
			  C.CustID ,          
			  SUM(H.InvoiceBal) AS CreaditOnTime              
	  FROM erp.InvcHead H        
	  INNER JOIN Customer C on H.Company = C.Company AND H.CustNum = C.CustNum          	  
	  WHERE H.UnpostedBal <> 0         
			--and H.Company = 'PMN'          
			AND H.DueDate > Getdate()  -- DUEDATE          
			AND LEFT(C.Custid,1) = '8'          
	  GROUP BY H.Company, H.Custnum,C.Custid) OnTime         
WHERE Ontime.Company = #ARBalance.Company AND Ontime.CustID = #ARBalance.CustID AND  Ontime.Custnum = #ARBalance.Custnum  

-- Caculate Balance OverDue
UPDATE #ARBalance SET OverDue = Balance - CreaditOnTime      

SELECT Company,Custnum,CustID,PaymentTerm,CreditLimit,Balance,OverDue,CreateDate
FROM #ARBalance

GO


