--
-- Hub Insert Into statement for HUB_CUSTOMER
-- Generated at 11/08/2019 2:42:50 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].dbo.HUB_CUSTOMER
(
   [CUSTOMER_HSH],
   [CUSTOMER_ID],
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [RECORD_SOURCE]
)
SELECT 
   hub_view.[CUSTOMER_HSH],
   hub_view.[CUSTOMER_ID],
   -1 AS ETL_INSERT_RUN_ID,
   hub_view.[LOAD_DATETIME],
   hub_view.[RECORD_SOURCE]
FROM [vedw].HUB_CUSTOMER hub_view
LEFT OUTER JOIN 
 [200_Integration_Layer].dbo.HUB_CUSTOMER hub_table
 ON hub_view.CUSTOMER_HSH = hub_table.CUSTOMER_HSH
WHERE hub_table.CUSTOMER_HSH IS NULL

--
-- Hub Insert Into statement for HUB_INCENTIVE_OFFER
-- Generated at 11/08/2019 2:42:50 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].dbo.HUB_INCENTIVE_OFFER
(
   [ETL_INSERT_RUN_ID],
   [INCENTIVE_OFFER_HSH],
   [LOAD_DATETIME],
   [OFFER_ID],
   [RECORD_SOURCE]
)
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   hub_view.[INCENTIVE_OFFER_HSH],
   hub_view.[LOAD_DATETIME],
   hub_view.[OFFER_ID],
   hub_view.[RECORD_SOURCE]
FROM [vedw].HUB_INCENTIVE_OFFER hub_view
LEFT OUTER JOIN 
 [200_Integration_Layer].dbo.HUB_INCENTIVE_OFFER hub_table
 ON hub_view.INCENTIVE_OFFER_HSH = hub_table.INCENTIVE_OFFER_HSH
WHERE hub_table.INCENTIVE_OFFER_HSH IS NULL

--
-- Hub Insert Into statement for HUB_MEMBERSHIP_PLAN
-- Generated at 11/08/2019 2:42:50 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].dbo.HUB_MEMBERSHIP_PLAN
(
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [MEMBERSHIP_PLAN_HSH],
   [PLAN_CODE],
   [PLAN_SUFFIX],
   [RECORD_SOURCE]
)
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   hub_view.[LOAD_DATETIME],
   hub_view.[MEMBERSHIP_PLAN_HSH],
   hub_view.[PLAN_CODE],
   hub_view.[PLAN_SUFFIX],
   hub_view.[RECORD_SOURCE]
FROM [vedw].HUB_MEMBERSHIP_PLAN hub_view
LEFT OUTER JOIN 
 [200_Integration_Layer].dbo.HUB_MEMBERSHIP_PLAN hub_table
 ON hub_view.MEMBERSHIP_PLAN_HSH = hub_table.MEMBERSHIP_PLAN_HSH
WHERE hub_table.MEMBERSHIP_PLAN_HSH IS NULL

--
-- Hub Insert Into statement for HUB_SEGMENT
-- Generated at 11/08/2019 2:42:50 PM
--

USE [150_Persistent_Staging_Area]
GO

INSERT INTO [200_Integration_Layer].dbo.HUB_SEGMENT
(
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [RECORD_SOURCE],
   [SEGMENT_CODE],
   [SEGMENT_HSH]
)
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   hub_view.[LOAD_DATETIME],
   hub_view.[RECORD_SOURCE],
   hub_view.[SEGMENT_CODE],
   hub_view.[SEGMENT_HSH]
FROM [vedw].HUB_SEGMENT hub_view
LEFT OUTER JOIN 
 [200_Integration_Layer].dbo.HUB_SEGMENT hub_table
 ON hub_view.SEGMENT_HSH = hub_table.SEGMENT_HSH
WHERE hub_table.SEGMENT_HSH IS NULL

