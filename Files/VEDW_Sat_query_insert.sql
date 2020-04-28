--
-- Satellite Insert Into statement for SAT_CUSTOMER
-- Generated at 11/08/2019 2:43:00 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[SAT_CUSTOMER]
   (
   [CDC_OPERATION],
   [COUNTRY],
   [CURRENT_RECORD_INDICATOR],
   [CUSTOMER_HSH],
   [DATE_OF_BIRTH],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [GENDER],
   [GIVEN_NAME],
   [HASH_FULL_RECORD],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [POSTCODE],
   [RECORD_SOURCE],
   [REFERRAL_OFFER_MADE_INDICATOR],
   [SOURCE_ROW_ID],
   [SUBURB],
   [SURNAME]
   )
SELECT 
   sat_view.[CDC_OPERATION],
   sat_view.[COUNTRY],
   sat_view.[CURRENT_RECORD_INDICATOR],
   sat_view.[CUSTOMER_HSH],
   sat_view.[DATE_OF_BIRTH],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   sat_view.[GENDER],
   sat_view.[GIVEN_NAME],
   sat_view.[HASH_FULL_RECORD],
   sat_view.[LOAD_DATETIME],
   sat_view.[LOAD_END_DATETIME],
   sat_view.[POSTCODE],
   sat_view.[RECORD_SOURCE],
   sat_view.[REFERRAL_OFFER_MADE_INDICATOR],
   sat_view.[SOURCE_ROW_ID],
   sat_view.[SUBURB],
   sat_view.[SURNAME]
FROM [vedw].SAT_CUSTOMER sat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[SAT_CUSTOMER] sat_table
 ON sat_view.[CUSTOMER_HSH] = sat_table.[CUSTOMER_HSH]
AND sat_view.[LOAD_DATETIME] = sat_table.[LOAD_DATETIME]
WHERE sat_table.CUSTOMER_HSH IS NULL

--
-- Satellite Insert Into statement for SAT_CUSTOMER_ADDITIONAL_DETAILS
-- Generated at 11/08/2019 2:43:00 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[SAT_CUSTOMER_ADDITIONAL_DETAILS]
   (
   [CDC_OPERATION],
   [CONTACT_NUMBER],
   [CURRENT_RECORD_INDICATOR],
   [CUSTOMER_HSH],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [HASH_FULL_RECORD],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [STATE]
   )
SELECT 
   sat_view.[CDC_OPERATION],
   sat_view.[CONTACT_NUMBER],
   sat_view.[CURRENT_RECORD_INDICATOR],
   sat_view.[CUSTOMER_HSH],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   sat_view.[HASH_FULL_RECORD],
   sat_view.[LOAD_DATETIME],
   sat_view.[LOAD_END_DATETIME],
   sat_view.[RECORD_SOURCE],
   sat_view.[SOURCE_ROW_ID],
   sat_view.[STATE]
FROM [vedw].SAT_CUSTOMER_ADDITIONAL_DETAILS sat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[SAT_CUSTOMER_ADDITIONAL_DETAILS] sat_table
 ON sat_view.[CUSTOMER_HSH] = sat_table.[CUSTOMER_HSH]
AND sat_view.[LOAD_DATETIME] = sat_table.[LOAD_DATETIME]
WHERE sat_table.CUSTOMER_HSH IS NULL

--
-- Satellite Insert Into statement for SAT_INCENTIVE_OFFER
-- Generated at 11/08/2019 2:43:00 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[SAT_INCENTIVE_OFFER]
   (
   [CDC_OPERATION],
   [CURRENT_RECORD_INDICATOR],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [HASH_FULL_RECORD],
   [INCENTIVE_OFFER_HSH],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [OFFER_DESCRIPTION],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID]
   )
SELECT 
   sat_view.[CDC_OPERATION],
   sat_view.[CURRENT_RECORD_INDICATOR],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   sat_view.[HASH_FULL_RECORD],
   sat_view.[INCENTIVE_OFFER_HSH],
   sat_view.[LOAD_DATETIME],
   sat_view.[LOAD_END_DATETIME],
   sat_view.[OFFER_DESCRIPTION],
   sat_view.[RECORD_SOURCE],
   sat_view.[SOURCE_ROW_ID]
FROM [vedw].SAT_INCENTIVE_OFFER sat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[SAT_INCENTIVE_OFFER] sat_table
 ON sat_view.[INCENTIVE_OFFER_HSH] = sat_table.[INCENTIVE_OFFER_HSH]
AND sat_view.[LOAD_DATETIME] = sat_table.[LOAD_DATETIME]
WHERE sat_table.INCENTIVE_OFFER_HSH IS NULL

--
-- Satellite Insert Into statement for SAT_MEMBERSHIP_PLAN_DETAIL
-- Generated at 11/08/2019 2:43:00 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[SAT_MEMBERSHIP_PLAN_DETAIL]
   (
   [CDC_OPERATION],
   [CURRENT_RECORD_INDICATOR],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [HASH_FULL_RECORD],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [MEMBERSHIP_PLAN_HSH],
   [PLAN_DESCRIPTION],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID]
   )
SELECT 
   sat_view.[CDC_OPERATION],
   sat_view.[CURRENT_RECORD_INDICATOR],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   sat_view.[HASH_FULL_RECORD],
   sat_view.[LOAD_DATETIME],
   sat_view.[LOAD_END_DATETIME],
   sat_view.[MEMBERSHIP_PLAN_HSH],
   sat_view.[PLAN_DESCRIPTION],
   sat_view.[RECORD_SOURCE],
   sat_view.[SOURCE_ROW_ID]
FROM [vedw].SAT_MEMBERSHIP_PLAN_DETAIL sat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[SAT_MEMBERSHIP_PLAN_DETAIL] sat_table
 ON sat_view.[MEMBERSHIP_PLAN_HSH] = sat_table.[MEMBERSHIP_PLAN_HSH]
AND sat_view.[LOAD_DATETIME] = sat_table.[LOAD_DATETIME]
WHERE sat_table.MEMBERSHIP_PLAN_HSH IS NULL

--
-- Satellite Insert Into statement for SAT_MEMBERSHIP_PLAN_VALUATION
-- Generated at 11/08/2019 2:43:00 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[SAT_MEMBERSHIP_PLAN_VALUATION]
   (
   [CDC_OPERATION],
   [CURRENT_RECORD_INDICATOR],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [HASH_FULL_RECORD],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [MEMBERSHIP_PLAN_HSH],
   [PLAN_VALUATION_AMOUNT],
   [PLAN_VALUATION_DATE],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID]
   )
SELECT 
   sat_view.[CDC_OPERATION],
   sat_view.[CURRENT_RECORD_INDICATOR],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   sat_view.[HASH_FULL_RECORD],
   sat_view.[LOAD_DATETIME],
   sat_view.[LOAD_END_DATETIME],
   sat_view.[MEMBERSHIP_PLAN_HSH],
   sat_view.[PLAN_VALUATION_AMOUNT],
   sat_view.[PLAN_VALUATION_DATE],
   sat_view.[RECORD_SOURCE],
   sat_view.[SOURCE_ROW_ID]
FROM [vedw].SAT_MEMBERSHIP_PLAN_VALUATION sat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[SAT_MEMBERSHIP_PLAN_VALUATION] sat_table
 ON sat_view.[MEMBERSHIP_PLAN_HSH] = sat_table.[MEMBERSHIP_PLAN_HSH]
AND sat_view.[LOAD_DATETIME] = sat_table.[LOAD_DATETIME]
AND sat_view.[PLAN_VALUATION_DATE] = sat_table.[PLAN_VALUATION_DATE]
WHERE sat_table.MEMBERSHIP_PLAN_HSH IS NULL

--
-- Satellite Insert Into statement for SAT_SEGMENT
-- Generated at 11/08/2019 2:43:00 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].[dbo].[SAT_SEGMENT]
   (
   [CDC_OPERATION],
   [CURRENT_RECORD_INDICATOR],
   [ETL_INSERT_RUN_ID],
   [ETL_UPDATE_RUN_ID],
   [HASH_FULL_RECORD],
   [LOAD_DATETIME],
   [LOAD_END_DATETIME],
   [RECORD_SOURCE],
   [SEGMENT_DESCRIPTION],
   [SEGMENT_HSH],
   [SOURCE_ROW_ID]
   )
SELECT 
   sat_view.[CDC_OPERATION],
   sat_view.[CURRENT_RECORD_INDICATOR],
   -1 AS [ETL_INSERT_RUN_ID],
   -1 AS [ETL_UPDATE_RUN_ID],
   sat_view.[HASH_FULL_RECORD],
   sat_view.[LOAD_DATETIME],
   sat_view.[LOAD_END_DATETIME],
   sat_view.[RECORD_SOURCE],
   sat_view.[SEGMENT_DESCRIPTION],
   sat_view.[SEGMENT_HSH],
   sat_view.[SOURCE_ROW_ID]
FROM [vedw].SAT_SEGMENT sat_view
LEFT OUTER JOIN
   [200_Integration_Layer].[dbo].[SAT_SEGMENT] sat_table
 ON sat_view.[SEGMENT_HSH] = sat_table.[SEGMENT_HSH]
AND sat_view.[LOAD_DATETIME] = sat_table.[LOAD_DATETIME]
WHERE sat_table.SEGMENT_HSH IS NULL

