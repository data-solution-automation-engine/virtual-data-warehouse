--
-- Link Insert Into statement for LNK_CUSTOMER_COSTING
-- Generated at 11/08/2019 2:43:05 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].dbo.LNK_CUSTOMER_COSTING
   (
   [CUSTOMER_COSTING_HSH],
   [CUSTOMER_HSH],
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [MEMBERSHIP_PLAN_HSH],
   [RECORD_SOURCE],
   [SEGMENT_HSH]
   )
SELECT 
   link_view.[CUSTOMER_COSTING_HSH],
   link_view.[CUSTOMER_HSH],
   -1 AS ETL_INSERT_RUN_ID,
   link_view.[LOAD_DATETIME],
   link_view.[MEMBERSHIP_PLAN_HSH],
   link_view.[RECORD_SOURCE],
   link_view.[SEGMENT_HSH]
FROM [vedw].LNK_CUSTOMER_COSTING link_view
LEFT OUTER JOIN 
 [200_Integration_Layer].dbo.LNK_CUSTOMER_COSTING link_table
 ON link_view.CUSTOMER_COSTING_HSH = link_table.CUSTOMER_COSTING_HSH
WHERE link_table.CUSTOMER_COSTING_HSH IS NULL

--
-- Link Insert Into statement for LNK_CUSTOMER_OFFER
-- Generated at 11/08/2019 2:43:05 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].dbo.LNK_CUSTOMER_OFFER
   (
   [CUSTOMER_HSH],
   [CUSTOMER_OFFER_HSH],
   [ETL_INSERT_RUN_ID],
   [INCENTIVE_OFFER_HSH],
   [LOAD_DATETIME],
   [RECORD_SOURCE]
   )
SELECT 
   link_view.[CUSTOMER_HSH],
   link_view.[CUSTOMER_OFFER_HSH],
   -1 AS ETL_INSERT_RUN_ID,
   link_view.[INCENTIVE_OFFER_HSH],
   link_view.[LOAD_DATETIME],
   link_view.[RECORD_SOURCE]
FROM [vedw].LNK_CUSTOMER_OFFER link_view
LEFT OUTER JOIN 
 [200_Integration_Layer].dbo.LNK_CUSTOMER_OFFER link_table
 ON link_view.CUSTOMER_OFFER_HSH = link_table.CUSTOMER_OFFER_HSH
WHERE link_table.CUSTOMER_OFFER_HSH IS NULL

--
-- Link Insert Into statement for LNK_MEMBERSHIP
-- Generated at 11/08/2019 2:43:05 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].dbo.LNK_MEMBERSHIP
   (
   [CUSTOMER_HSH],
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [MEMBERSHIP_HSH],
   [MEMBERSHIP_PLAN_HSH],
   [RECORD_SOURCE],
   [SALES_CHANNEL]
   )
SELECT 
   link_view.[CUSTOMER_HSH],
   -1 AS ETL_INSERT_RUN_ID,
   link_view.[LOAD_DATETIME],
   link_view.[MEMBERSHIP_HSH],
   link_view.[MEMBERSHIP_PLAN_HSH],
   link_view.[RECORD_SOURCE],
   link_view.[SALES_CHANNEL]
FROM [vedw].LNK_MEMBERSHIP link_view
LEFT OUTER JOIN 
 [200_Integration_Layer].dbo.LNK_MEMBERSHIP link_table
 ON link_view.MEMBERSHIP_HSH = link_table.MEMBERSHIP_HSH
WHERE link_table.MEMBERSHIP_HSH IS NULL

--
-- Link Insert Into statement for LNK_RENEWAL_MEMBERSHIP
-- Generated at 11/08/2019 2:43:05 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].dbo.LNK_RENEWAL_MEMBERSHIP
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [MEMBERSHIP_PLAN_HSH],
   [RECORD_SOURCE],
   [RENEWAL_MEMBERSHIP_HSH],
   [RENEWAL_PLAN_HSH]
   )
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   link_view.[LOAD_DATETIME],
   link_view.[MEMBERSHIP_PLAN_HSH],
   link_view.[RECORD_SOURCE],
   link_view.[RENEWAL_MEMBERSHIP_HSH],
   link_view.[RENEWAL_PLAN_HSH]
FROM [vedw].LNK_RENEWAL_MEMBERSHIP link_view
LEFT OUTER JOIN 
 [200_Integration_Layer].dbo.LNK_RENEWAL_MEMBERSHIP link_table
 ON link_view.RENEWAL_MEMBERSHIP_HSH = link_table.RENEWAL_MEMBERSHIP_HSH
WHERE link_table.RENEWAL_MEMBERSHIP_HSH IS NULL

