--
-- Staging Area Insert Into statement for STG_PROFILER_CUST_MEMBERSHIP
-- Generated at 11/08/2019 2:40:27 PM
-- The Row ID and Load Date/Time will be created upon insert as default values
--

--USE [100_Staging_Area]
GO

TRUNCATE TABLE [dbo].[STG_PROFILER_CUST_MEMBERSHIP]
GO

INSERT INTO [dbo].[STG_PROFILER_CUST_MEMBERSHIP]
   (
   [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [Plan_Code],
   [Start_Date],
   [End_Date],
   [Status],
   [Comment]
   )
SELECT 
   -1 AS [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [Plan_Code],
   [Start_Date],
   [End_Date],
   [Status],
   [Comment]
FROM [vedw].[STG_PROFILER_CUST_MEMBERSHIP]

--
-- Staging Area Insert Into statement for STG_PROFILER_CUSTOMER_OFFER
-- Generated at 11/08/2019 2:40:27 PM
-- The Row ID and Load Date/Time will be created upon insert as default values
--

--USE [100_Staging_Area]
GO

TRUNCATE TABLE [dbo].[STG_PROFILER_CUSTOMER_OFFER]
GO

INSERT INTO [dbo].[STG_PROFILER_CUSTOMER_OFFER]
   (
   [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [OfferID]
   )
SELECT 
   -1 AS [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [OfferID]
FROM [vedw].[STG_PROFILER_CUSTOMER_OFFER]

--
-- Staging Area Insert Into statement for STG_PROFILER_CUSTOMER_PERSONAL
-- Generated at 11/08/2019 2:40:27 PM
-- The Row ID and Load Date/Time will be created upon insert as default values
--

--USE [100_Staging_Area]
GO

TRUNCATE TABLE [dbo].[STG_PROFILER_CUSTOMER_PERSONAL]
GO

INSERT INTO [dbo].[STG_PROFILER_CUSTOMER_PERSONAL]
   (
   [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [Given],
   [Surname],
   [Suburb],
   [State],
   [Postcode],
   [Country],
   [Gender],
   [DOB],
   [Contact_Number],
   [Referee_Offer_Made]
   )
SELECT 
   -1 AS [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [Given],
   [Surname],
   [Suburb],
   [State],
   [Postcode],
   [Country],
   [Gender],
   [DOB],
   [Contact_Number],
   [Referee_Offer_Made]
FROM [vedw].[STG_PROFILER_CUSTOMER_PERSONAL]

--
-- Staging Area Insert Into statement for STG_PROFILER_ESTIMATED_WORTH
-- Generated at 11/08/2019 2:40:28 PM
-- The Row ID and Load Date/Time will be created upon insert as default values
--

--USE [100_Staging_Area]
GO

TRUNCATE TABLE [dbo].[STG_PROFILER_ESTIMATED_WORTH]
GO

INSERT INTO [dbo].[STG_PROFILER_ESTIMATED_WORTH]
   (
   [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Plan_Code],
   [Date_effective],
   [Value_Amount]
   )
SELECT 
   -1 AS [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Plan_Code],
   [Date_effective],
   [Value_Amount]
FROM [vedw].[STG_PROFILER_ESTIMATED_WORTH]

--
-- Staging Area Insert Into statement for STG_PROFILER_OFFER
-- Generated at 11/08/2019 2:40:28 PM
-- The Row ID and Load Date/Time will be created upon insert as default values
--

--USE [100_Staging_Area]
GO

TRUNCATE TABLE [dbo].[STG_PROFILER_OFFER]
GO

INSERT INTO [dbo].[STG_PROFILER_OFFER]
   (
   [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [OfferID],
   [Offer_Long_Description]
   )
SELECT 
   -1 AS [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [OfferID],
   [Offer_Long_Description]
FROM [vedw].[STG_PROFILER_OFFER]

--
-- Staging Area Insert Into statement for STG_PROFILER_PERSONALISED_COSTING
-- Generated at 11/08/2019 2:40:28 PM
-- The Row ID and Load Date/Time will be created upon insert as default values
--

--USE [100_Staging_Area]
GO

TRUNCATE TABLE [dbo].[STG_PROFILER_PERSONALISED_COSTING]
GO

INSERT INTO [dbo].[STG_PROFILER_PERSONALISED_COSTING]
   (
   [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Member],
   [Segment],
   [Plan_Code],
   [Date_effective],
   [Monthly_Cost]
   )
SELECT 
   -1 AS [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Member],
   [Segment],
   [Plan_Code],
   [Date_effective],
   [Monthly_Cost]
FROM [vedw].[STG_PROFILER_PERSONALISED_COSTING]

--
-- Staging Area Insert Into statement for STG_PROFILER_PLAN
-- Generated at 11/08/2019 2:40:28 PM
-- The Row ID and Load Date/Time will be created upon insert as default values
--

--USE [100_Staging_Area]
GO

TRUNCATE TABLE [dbo].[STG_PROFILER_PLAN]
GO

INSERT INTO [dbo].[STG_PROFILER_PLAN]
   (
   [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Plan_Code],
   [Plan_Desc],
   [Renewal_Plan_Code]
   )
SELECT 
   -1 AS [ETL_INSERT_RUN_ID],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Plan_Code],
   [Plan_Desc],
   [Renewal_Plan_Code]
FROM [vedw].[STG_PROFILER_PLAN]

