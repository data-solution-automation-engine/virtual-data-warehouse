--
-- Link Satellite Insert Into statement for LSAT_CUSTOMER_COSTING
-- Generated at 11/08/2019 2:43:11 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[LSAT_CUSTOMER_COSTING]
   (
   [CDC_OPERATION],
   [COSTING_EFFECTIVE_DATE],
   [CURRENT_RECORD_INDICATOR],
   [CUSTOMER_COSTING_HSH],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [HASH_FULL_RECORD],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [PERSONAL_MONTHLY_COST],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID]
   )
SELECT 
   lsat_view.[CDC_OPERATION],
   lsat_view.[COSTING_EFFECTIVE_DATE],
   lsat_view.[CURRENT_RECORD_INDICATOR],
   lsat_view.[CUSTOMER_COSTING_HSH],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   lsat_view.[HASH_FULL_RECORD],
   lsat_view.[LOAD_DATETIME],
   lsat_view.[LOAD_END_DATETIME],
   lsat_view.[PERSONAL_MONTHLY_COST],
   lsat_view.[RECORD_SOURCE],
   lsat_view.[SOURCE_ROW_ID]
FROM [vedw].LSAT_CUSTOMER_COSTING lsat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[LSAT_CUSTOMER_COSTING] lsat_table
 ON lsat_view.[CUSTOMER_COSTING_HSH] = lsat_table.[CUSTOMER_COSTING_HSH]
AND lsat_view.[LOAD_DATETIME] = lsat_table.[LOAD_DATETIME]
AND lsat_view.[COSTING_EFFECTIVE_DATE] = lsat_table.[COSTING_EFFECTIVE_DATE]
WHERE lsat_table.CUSTOMER_COSTING_HSH IS NULL

--
-- Link Satellite Insert Into statement for LSAT_CUSTOMER_OFFER
-- Generated at 11/08/2019 2:43:11 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[LSAT_CUSTOMER_OFFER]
   (
   [CDC_OPERATION],
   [CURRENT_RECORD_INDICATOR],
   [CUSTOMER_OFFER_HSH],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [HASH_FULL_RECORD],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID]
   )
SELECT 
   lsat_view.[CDC_OPERATION],
   lsat_view.[CURRENT_RECORD_INDICATOR],
   lsat_view.[CUSTOMER_OFFER_HSH],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   lsat_view.[HASH_FULL_RECORD],
   lsat_view.[LOAD_DATETIME],
   lsat_view.[LOAD_END_DATETIME],
   lsat_view.[RECORD_SOURCE],
   lsat_view.[SOURCE_ROW_ID]
FROM [vedw].LSAT_CUSTOMER_OFFER lsat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[LSAT_CUSTOMER_OFFER] lsat_table
 ON lsat_view.[CUSTOMER_OFFER_HSH] = lsat_table.[CUSTOMER_OFFER_HSH]
AND lsat_view.[LOAD_DATETIME] = lsat_table.[LOAD_DATETIME]
WHERE lsat_table.CUSTOMER_OFFER_HSH IS NULL

--
-- Link Satellite Insert Into statement for LSAT_MEMBERSHIP
-- Generated at 11/08/2019 2:43:11 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[LSAT_MEMBERSHIP]
   (
   [CDC_OPERATION],
   [CURRENT_RECORD_INDICATOR],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [HASH_FULL_RECORD],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [MEMBERSHIP_END_DATE],
   [MEMBERSHIP_HSH],
   [MEMBERSHIP_START_DATE],
   [MEMBERSHIP_STATUS],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID]
   )
SELECT 
   lsat_view.[CDC_OPERATION],
   lsat_view.[CURRENT_RECORD_INDICATOR],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   lsat_view.[HASH_FULL_RECORD],
   lsat_view.[LOAD_DATETIME],
   lsat_view.[LOAD_END_DATETIME],
   lsat_view.[MEMBERSHIP_END_DATE],
   lsat_view.[MEMBERSHIP_HSH],
   lsat_view.[MEMBERSHIP_START_DATE],
   lsat_view.[MEMBERSHIP_STATUS],
   lsat_view.[RECORD_SOURCE],
   lsat_view.[SOURCE_ROW_ID]
FROM [vedw].LSAT_MEMBERSHIP lsat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[LSAT_MEMBERSHIP] lsat_table
 ON lsat_view.[MEMBERSHIP_HSH] = lsat_table.[MEMBERSHIP_HSH]
AND lsat_view.[LOAD_DATETIME] = lsat_table.[LOAD_DATETIME]
WHERE lsat_table.MEMBERSHIP_HSH IS NULL

