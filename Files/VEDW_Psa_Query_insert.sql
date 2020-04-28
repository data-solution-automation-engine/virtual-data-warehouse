--
-- PSA Insert Into statement for PSA_PROFILER_CUST_MEMBERSHIP
-- Generated at 11/08/2019 2:40:41 PM
--

USE Sandbox_VDW
GO

INSERT INTO [dbo].[PSA_PROFILER_CUST_MEMBERSHIP]
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
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
   -1 AS ETL_INSERT_RUN_ID,
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [Plan_Code],
   [Start_Date],
   [End_Date],
   [Status],
   [Comment]
FROM [vedw].PSA_PROFILER_CUST_MEMBERSHIP

--
-- PSA Insert Into statement for PSA_PROFILER_CUSTOMER_OFFER
-- Generated at 11/08/2019 2:40:41 PM
--

USE Sandbox_VDW
GO

INSERT INTO [dbo].[PSA_PROFILER_CUSTOMER_OFFER]
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [OfferID]
   )
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [CustomerID],
   [OfferID]
FROM [vedw].PSA_PROFILER_CUSTOMER_OFFER

--
-- PSA Insert Into statement for PSA_PROFILER_CUSTOMER_PERSONAL
-- Generated at 11/08/2019 2:40:41 PM
--

USE Sandbox_VDW
GO

INSERT INTO [dbo].[PSA_PROFILER_CUSTOMER_PERSONAL]
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
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
   -1 AS ETL_INSERT_RUN_ID,
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
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
FROM [vedw].PSA_PROFILER_CUSTOMER_PERSONAL

--
-- PSA Insert Into statement for PSA_PROFILER_ESTIMATED_WORTH
-- Generated at 11/08/2019 2:40:42 PM
--

USE Sandbox_VDW
GO

INSERT INTO [dbo].[PSA_PROFILER_ESTIMATED_WORTH]
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Plan_Code],
   [Date_effective],
   [Value_Amount]
   )
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Plan_Code],
   [Date_effective],
   [Value_Amount]
FROM [vedw].PSA_PROFILER_ESTIMATED_WORTH

--
-- PSA Insert Into statement for PSA_PROFILER_OFFER
-- Generated at 11/08/2019 2:40:42 PM
--

USE Sandbox_VDW
GO

INSERT INTO [dbo].[PSA_PROFILER_OFFER]
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [OfferID],
   [Offer_Long_Description]
   )
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [OfferID],
   [Offer_Long_Description]
FROM [vedw].PSA_PROFILER_OFFER

--
-- PSA Insert Into statement for PSA_PROFILER_PERSONALISED_COSTING
-- Generated at 11/08/2019 2:40:42 PM
--

USE Sandbox_VDW
GO

INSERT INTO [dbo].[PSA_PROFILER_PERSONALISED_COSTING]
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Member],
   [Segment],
   [Plan_Code],
   [Date_effective],
   [Monthly_Cost]
   )
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Member],
   [Segment],
   [Plan_Code],
   [Date_effective],
   [Monthly_Cost]
FROM [vedw].PSA_PROFILER_PERSONALISED_COSTING

--
-- PSA Insert Into statement for PSA_PROFILER_PLAN
-- Generated at 11/08/2019 2:40:42 PM
--

USE Sandbox_VDW
GO

INSERT INTO [dbo].[PSA_PROFILER_PLAN]
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Plan_Code],
   [Plan_Desc],
   [Renewal_Plan_Code]
   )
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Plan_Code],
   [Plan_Desc],
   [Renewal_Plan_Code]
FROM [vedw].PSA_PROFILER_PLAN

--
-- PSA Insert Into statement for PSA_USERMANAGED_SEGMENT
-- Generated at 11/08/2019 2:40:42 PM
--

USE Sandbox_VDW
GO

INSERT INTO [dbo].[PSA_USERMANAGED_SEGMENT]
   (
   [ETL_INSERT_RUN_ID],
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Demographic_Segment_Code],
   [Demographic_Segment_Description]
   )
SELECT 
   -1 AS ETL_INSERT_RUN_ID,
   [LOAD_DATETIME],
   [EVENT_DATETIME],
   [RECORD_SOURCE],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   [HASH_FULL_RECORD],
   [Demographic_Segment_Code],
   [Demographic_Segment_Description]
FROM [vedw].PSA_USERMANAGED_SEGMENT

