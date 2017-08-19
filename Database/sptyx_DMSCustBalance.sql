USE [ERPAPPTRAIN]
GO

/****** Object:  StoredProcedure [dbo].[sptyx_DMSCustBalance]    Script Date: 8/19/2017 10:27:43 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sptyx_DMSCustBalance]              
                
/* Create by MinhNH on 30th July 2017                
   -- V01: Get Customer Balance  */                
                
AS             
         
          
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED            
SET NOCOUNT ON;                

SELECT  H.Company         
		, C.Custnum	
		, C.CustID           		
		, C.CreditLimit        
		, SUM(H.InvoiceBal) AS Balance    
		, getdate() AS CreateDate        
FROM erp.InvcHead H WITH (NOLOCK)       
INNER JOIN Customer C WITH (NOLOCK) ON H.Company = C.Company AND H.CustNum = C.CustNum         
WHERE H.UnpostedBal <> 0         
--  and H.Company = 'PMN'          
  AND LEFT(C.Custid,1) = '8'        
GROUP BY H.Company         
		, C.Custnum	
		, C.CustID           		
		, C.CreditLimit           
  
 --select * from xttyx_CustBalance 
 
GO


