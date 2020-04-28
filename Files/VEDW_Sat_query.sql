-- 
-- Satellite View definition for SAT_CUSTOMER
-- Generated at 17/07/2019 12:58:27 PM
-- 

USE [150_Persistent_Staging_Area]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[SAT_CUSTOMER]') AND type in (N'V'))
DROP VIEW [vedw].[SAT_CUSTOMER]
go
CREATE VIEW [vedw].[SAT_CUSTOMER] AS  
SELECT
   HASHBYTES('MD5',
     ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID)),'NA')+'|'
   ) AS CUSTOMER_HSH,
   DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) AS LOAD_DATETIME,
   COALESCE ( LEAD ( DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) ) OVER
   		     (PARTITION BY 
   		          [CUSTOMER_ID]
   		      ORDER BY LOAD_DATETIME),
   CAST( '9999-12-31' AS DATETIME)) AS LOAD_END_DATETIME,
   CASE
      WHEN ( RANK() OVER (PARTITION BY 
           [CUSTOMER_ID]
          ORDER BY LOAD_DATETIME desc )) = 1
      THEN 'Y'
      ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   -1 AS ETL_INSERT_RUN_ID,
   -1 AS ETL_UPDATE_RUN_ID,
   CDC_OPERATION,
   SOURCE_ROW_ID,
   RECORD_SOURCE,
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),CDC_OPERATION)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),COUNTRY)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),DOB)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),GENDER)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Given)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),POSTCODE)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Referee_Offer_Made)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Suburb)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),SURNAME)),'NA')+'|'
   ) AS HASH_FULL_RECORD,
   COUNTRY AS COUNTRY,
   DOB AS DATE_OF_BIRTH,
   GENDER AS GENDER,
   Given AS GIVEN_NAME,
   POSTCODE AS POSTCODE,
   Referee_Offer_Made AS REFERRAL_OFFER_MADE_INDICATOR,
   Suburb AS SUBURB,
   SURNAME AS SURNAME,
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
         [CUSTOMER_ID]      ORDER BY 
         [CUSTOMER_ID],
         [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER
FROM 
   (
      SELECT 
         [LOAD_DATETIME],
         [EVENT_DATETIME],
         [RECORD_SOURCE],
         [SOURCE_ROW_ID],
         [CDC_OPERATION],
         [CUSTOMER_ID],
         [COUNTRY],
         [DOB],
         [GENDER],
         [Given],
         [POSTCODE],
         [Referee_Offer_Made],
         [Suburb],
         [SURNAME],
         COMBINED_VALUE,
         CASE 
           WHEN LAG(COMBINED_VALUE,1,0x00000000000000000000000000000000) OVER (PARTITION BY 
             [CUSTOMER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] DESC) = COMBINED_VALUE
           THEN 'Same'
           ELSE 'Different'
         END AS VALUE_CHANGE_INDICATOR,
         CASE 
           WHEN LAG([CDC_OPERATION],1,'') OVER (PARTITION BY 
             [CUSTOMER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [CDC_OPERATION]
           THEN 'Same'
           ELSE 'Different'
         END AS CDC_CHANGE_INDICATOR,
         CASE 
           WHEN LEAD([LOAD_DATETIME],1,'9999-12-31') OVER (PARTITION BY 
             [CUSTOMER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [LOAD_DATETIME]
           THEN 'Same'
           ELSE 'Different'
         END AS TIME_CHANGE_INDICATOR
      FROM
      (
        SELECT
          [LOAD_DATETIME],
          [EVENT_DATETIME],
          [RECORD_SOURCE],
          [SOURCE_ROW_ID],
          [CDC_OPERATION],
              CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID],
          [COUNTRY],
          [DOB],
          [GENDER],
          [Given],
          [POSTCODE],
          [Referee_Offer_Made],
          [Suburb],
          [SURNAME],
         HASHBYTES('MD5',
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[COUNTRY])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[DOB])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[GENDER])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Given])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[POSTCODE])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Referee_Offer_Made])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Suburb])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[SURNAME])),'NA')+'|'
         ) AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_CUSTOMER_PERSONAL
      WHERE 2=2
        UNION
        SELECT DISTINCT
          '1900-01-01' AS [LOAD_DATETIME],
          '1900-01-01' AS [EVENT_DATETIME],
          'Data Warehouse' AS [RECORD_SOURCE],
          0 AS [SOURCE_ROW_ID],
          'N/A' AS [CDC_OPERATION],
              CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID],
          NULL AS COUNTRY,
          NULL AS DOB,
          NULL AS GENDER,
          NULL AS Given,
          NULL AS POSTCODE,
          NULL AS Referee_Offer_Made,
          NULL AS Suburb,
          NULL AS SURNAME,
          0x00000000000000000000000000000000 AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_CUSTOMER_PERSONAL
   ) sub
) combined_value
WHERE 
  (VALUE_CHANGE_INDICATOR ='Different' and [CDC_OPERATION] in ('Insert', 'Change')) 
  OR
  (CDC_CHANGE_INDICATOR = 'Different' and TIME_CHANGE_INDICATOR = 'Different')
UNION
SELECT 0x00000000000000000000000000000000,
'1900-01-01',
'9999-12-31',
'Y',
- 1,
- 1,
'N/A',
1,
'Data Warehouse',
'N',
0x00000000000000000000000000000000,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
1

GO

-- 
-- Satellite View definition for SAT_CUSTOMER_ADDITIONAL_DETAILS
-- Generated at 17/07/2019 12:58:28 PM
-- 

USE [150_Persistent_Staging_Area]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[SAT_CUSTOMER_ADDITIONAL_DETAILS]') AND type in (N'V'))
DROP VIEW [vedw].[SAT_CUSTOMER_ADDITIONAL_DETAILS]
go
CREATE VIEW [vedw].[SAT_CUSTOMER_ADDITIONAL_DETAILS] AS  
SELECT
   HASHBYTES('MD5',
     ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID)),'NA')+'|'
   ) AS CUSTOMER_HSH,
   DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) AS LOAD_DATETIME,
   COALESCE ( LEAD ( DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) ) OVER
   		     (PARTITION BY 
   		          [CUSTOMER_ID]
   		      ORDER BY LOAD_DATETIME),
   CAST( '9999-12-31' AS DATETIME)) AS LOAD_END_DATETIME,
   CASE
      WHEN ( RANK() OVER (PARTITION BY 
           [CUSTOMER_ID]
          ORDER BY LOAD_DATETIME desc )) = 1
      THEN 'Y'
      ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   -1 AS ETL_INSERT_RUN_ID,
   -1 AS ETL_UPDATE_RUN_ID,
   CDC_OPERATION,
   SOURCE_ROW_ID,
   RECORD_SOURCE,
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),CDC_OPERATION)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Contact_Number)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),State)),'NA')+'|'
   ) AS HASH_FULL_RECORD,
   Contact_Number AS CONTACT_NUMBER,
   State AS STATE,
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
         [CUSTOMER_ID]      ORDER BY 
         [CUSTOMER_ID],
         [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER
FROM 
   (
      SELECT 
         [LOAD_DATETIME],
         [EVENT_DATETIME],
         [RECORD_SOURCE],
         [SOURCE_ROW_ID],
         [CDC_OPERATION],
         [CUSTOMER_ID],
         [Contact_Number],
         [State],
         COMBINED_VALUE,
         CASE 
           WHEN LAG(COMBINED_VALUE,1,0x00000000000000000000000000000000) OVER (PARTITION BY 
             [CUSTOMER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] DESC) = COMBINED_VALUE
           THEN 'Same'
           ELSE 'Different'
         END AS VALUE_CHANGE_INDICATOR,
         CASE 
           WHEN LAG([CDC_OPERATION],1,'') OVER (PARTITION BY 
             [CUSTOMER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [CDC_OPERATION]
           THEN 'Same'
           ELSE 'Different'
         END AS CDC_CHANGE_INDICATOR,
         CASE 
           WHEN LEAD([LOAD_DATETIME],1,'9999-12-31') OVER (PARTITION BY 
             [CUSTOMER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [LOAD_DATETIME]
           THEN 'Same'
           ELSE 'Different'
         END AS TIME_CHANGE_INDICATOR
      FROM
      (
        SELECT
          [LOAD_DATETIME],
          [EVENT_DATETIME],
          [RECORD_SOURCE],
          [SOURCE_ROW_ID],
          [CDC_OPERATION],
              CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID],
          [Contact_Number],
          [State],
         HASHBYTES('MD5',
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Contact_Number])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[State])),'NA')+'|'
         ) AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_CUSTOMER_PERSONAL
        UNION
        SELECT DISTINCT
          '1900-01-01' AS [LOAD_DATETIME],
          '1900-01-01' AS [EVENT_DATETIME],
          'Data Warehouse' AS [RECORD_SOURCE],
          0 AS [SOURCE_ROW_ID],
          'N/A' AS [CDC_OPERATION],
              CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID],
          NULL AS Contact_Number,
          NULL AS State,
          0x00000000000000000000000000000000 AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_CUSTOMER_PERSONAL
   ) sub
) combined_value
WHERE 
  (VALUE_CHANGE_INDICATOR ='Different' and [CDC_OPERATION] in ('Insert', 'Change')) 
  OR
  (CDC_CHANGE_INDICATOR = 'Different' and TIME_CHANGE_INDICATOR = 'Different')
UNION
SELECT 0x00000000000000000000000000000000,
'1900-01-01',
'9999-12-31',
'Y',
- 1,
- 1,
'N/A',
1,
'Data Warehouse',
'N',
0x00000000000000000000000000000000,
NULL,
NULL,
1

GO

-- 
-- Satellite View definition for SAT_INCENTIVE_OFFER
-- Generated at 17/07/2019 12:58:28 PM
-- 

USE [150_Persistent_Staging_Area]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[SAT_INCENTIVE_OFFER]') AND type in (N'V'))
DROP VIEW [vedw].[SAT_INCENTIVE_OFFER]
go
CREATE VIEW [vedw].[SAT_INCENTIVE_OFFER] AS  
SELECT
   HASHBYTES('MD5',
     ISNULL(RTRIM(CONVERT(NVARCHAR(100),OFFER_ID)),'NA')+'|'
   ) AS INCENTIVE_OFFER_HSH,
   DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) AS LOAD_DATETIME,
   COALESCE ( LEAD ( DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) ) OVER
   		     (PARTITION BY 
   		          [OFFER_ID]
   		      ORDER BY LOAD_DATETIME),
   CAST( '9999-12-31' AS DATETIME)) AS LOAD_END_DATETIME,
   CASE
      WHEN ( RANK() OVER (PARTITION BY 
           [OFFER_ID]
          ORDER BY LOAD_DATETIME desc )) = 1
      THEN 'Y'
      ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   -1 AS ETL_INSERT_RUN_ID,
   -1 AS ETL_UPDATE_RUN_ID,
   CDC_OPERATION,
   SOURCE_ROW_ID,
   RECORD_SOURCE,
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),CDC_OPERATION)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Offer_Long_Description)),'NA')+'|'
   ) AS HASH_FULL_RECORD,
   Offer_Long_Description AS OFFER_DESCRIPTION,
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
         [OFFER_ID]      ORDER BY 
         [OFFER_ID],
         [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER
FROM 
   (
      SELECT 
         [LOAD_DATETIME],
         [EVENT_DATETIME],
         [RECORD_SOURCE],
         [SOURCE_ROW_ID],
         [CDC_OPERATION],
         [OFFER_ID],
         [Offer_Long_Description],
         COMBINED_VALUE,
         CASE 
           WHEN LAG(COMBINED_VALUE,1,0x00000000000000000000000000000000) OVER (PARTITION BY 
             [OFFER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] DESC) = COMBINED_VALUE
           THEN 'Same'
           ELSE 'Different'
         END AS VALUE_CHANGE_INDICATOR,
         CASE 
           WHEN LAG([CDC_OPERATION],1,'') OVER (PARTITION BY 
             [OFFER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [CDC_OPERATION]
           THEN 'Same'
           ELSE 'Different'
         END AS CDC_CHANGE_INDICATOR,
         CASE 
           WHEN LEAD([LOAD_DATETIME],1,'9999-12-31') OVER (PARTITION BY 
             [OFFER_ID]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [LOAD_DATETIME]
           THEN 'Same'
           ELSE 'Different'
         END AS TIME_CHANGE_INDICATOR
      FROM
      (
        SELECT
          [LOAD_DATETIME],
          [EVENT_DATETIME],
          [RECORD_SOURCE],
          [SOURCE_ROW_ID],
          [CDC_OPERATION],
              CAST([OfferID] AS NVARCHAR(100)) AS [OFFER_ID],
          [Offer_Long_Description],
         HASHBYTES('MD5',
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Offer_Long_Description])),'NA')+'|'
         ) AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_OFFER
      WHERE 4=4
        UNION
        SELECT DISTINCT
          '1900-01-01' AS [LOAD_DATETIME],
          '1900-01-01' AS [EVENT_DATETIME],
          'Data Warehouse' AS [RECORD_SOURCE],
          0 AS [SOURCE_ROW_ID],
          'N/A' AS [CDC_OPERATION],
              CAST([OfferID] AS NVARCHAR(100)) AS [OFFER_ID],
          NULL AS Offer_Long_Description,
          0x00000000000000000000000000000000 AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_OFFER
   ) sub
) combined_value
WHERE 
  (VALUE_CHANGE_INDICATOR ='Different' and [CDC_OPERATION] in ('Insert', 'Change')) 
  OR
  (CDC_CHANGE_INDICATOR = 'Different' and TIME_CHANGE_INDICATOR = 'Different')
UNION
SELECT 0x00000000000000000000000000000000,
'1900-01-01',
'9999-12-31',
'Y',
- 1,
- 1,
'N/A',
1,
'Data Warehouse',
'N',
0x00000000000000000000000000000000,
NULL,
1

GO

-- 
-- Satellite View definition for SAT_MEMBERSHIP_PLAN_DETAIL
-- Generated at 17/07/2019 12:58:31 PM
-- 

USE [150_Persistent_Staging_Area]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[SAT_MEMBERSHIP_PLAN_DETAIL]') AND type in (N'V'))
DROP VIEW [vedw].[SAT_MEMBERSHIP_PLAN_DETAIL]
go
CREATE VIEW [vedw].[SAT_MEMBERSHIP_PLAN_DETAIL] AS  
SELECT
   HASHBYTES('MD5',
     ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE)),'NA')+'|'+
     ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX)),'NA')+'|'
   ) AS MEMBERSHIP_PLAN_HSH,
   DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) AS LOAD_DATETIME,
   COALESCE ( LEAD ( DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) ) OVER
   		     (PARTITION BY 
   		          [PLAN_CODE],
   		          [PLAN_SUFFIX]
   		      ORDER BY LOAD_DATETIME),
   CAST( '9999-12-31' AS DATETIME)) AS LOAD_END_DATETIME,
   CASE
      WHEN ( RANK() OVER (PARTITION BY 
           [PLAN_CODE],
           [PLAN_SUFFIX]
          ORDER BY LOAD_DATETIME desc )) = 1
      THEN 'Y'
      ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   -1 AS ETL_INSERT_RUN_ID,
   -1 AS ETL_UPDATE_RUN_ID,
   CDC_OPERATION,
   SOURCE_ROW_ID,
   RECORD_SOURCE,
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),CDC_OPERATION)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Plan_Desc)),'NA')+'|'
   ) AS HASH_FULL_RECORD,
   Plan_Desc AS PLAN_DESCRIPTION,
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
         [PLAN_CODE],
         [PLAN_SUFFIX]      ORDER BY 
         [PLAN_CODE],
         [PLAN_SUFFIX],
         [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER
FROM 
   (
      SELECT 
         [LOAD_DATETIME],
         [EVENT_DATETIME],
         [RECORD_SOURCE],
         [SOURCE_ROW_ID],
         [CDC_OPERATION],
         [PLAN_CODE],
         [PLAN_SUFFIX],
         [Plan_Desc],
         COMBINED_VALUE,
         CASE 
           WHEN LAG(COMBINED_VALUE,1,0x00000000000000000000000000000000) OVER (PARTITION BY 
             [PLAN_CODE],
             [PLAN_SUFFIX]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] DESC) = COMBINED_VALUE
           THEN 'Same'
           ELSE 'Different'
         END AS VALUE_CHANGE_INDICATOR,
         CASE 
           WHEN LAG([CDC_OPERATION],1,'') OVER (PARTITION BY 
             [PLAN_CODE],
             [PLAN_SUFFIX]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [CDC_OPERATION]
           THEN 'Same'
           ELSE 'Different'
         END AS CDC_CHANGE_INDICATOR,
         CASE 
           WHEN LEAD([LOAD_DATETIME],1,'9999-12-31') OVER (PARTITION BY 
             [PLAN_CODE],
             [PLAN_SUFFIX]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [LOAD_DATETIME]
           THEN 'Same'
           ELSE 'Different'
         END AS TIME_CHANGE_INDICATOR
      FROM
      (
        SELECT
          [LOAD_DATETIME],
          [EVENT_DATETIME],
          [RECORD_SOURCE],
          [SOURCE_ROW_ID],
          [CDC_OPERATION],
                [Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
          [Plan_Desc],
         HASHBYTES('MD5',
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Plan_Desc])),'NA')+'|'
         ) AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_PLAN
      WHERE 11=11
        UNION
        SELECT DISTINCT
          '1900-01-01' AS [LOAD_DATETIME],
          '1900-01-01' AS [EVENT_DATETIME],
          'Data Warehouse' AS [RECORD_SOURCE],
          0 AS [SOURCE_ROW_ID],
          'N/A' AS [CDC_OPERATION],
                [Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
          NULL AS Plan_Desc,
          0x00000000000000000000000000000000 AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_PLAN
   ) sub
) combined_value
WHERE 
  (VALUE_CHANGE_INDICATOR ='Different' and [CDC_OPERATION] in ('Insert', 'Change')) 
  OR
  (CDC_CHANGE_INDICATOR = 'Different' and TIME_CHANGE_INDICATOR = 'Different')
UNION
SELECT 0x00000000000000000000000000000000,
'1900-01-01',
'9999-12-31',
'Y',
- 1,
- 1,
'N/A',
1,
'Data Warehouse',
'N',
0x00000000000000000000000000000000,
NULL,
1

GO

-- 
-- Satellite View definition for SAT_MEMBERSHIP_PLAN_VALUATION
-- Generated at 17/07/2019 12:58:34 PM
-- 

USE [150_Persistent_Staging_Area]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[SAT_MEMBERSHIP_PLAN_VALUATION]') AND type in (N'V'))
DROP VIEW [vedw].[SAT_MEMBERSHIP_PLAN_VALUATION]
go
CREATE VIEW [vedw].[SAT_MEMBERSHIP_PLAN_VALUATION] AS  
SELECT
   HASHBYTES('MD5',
     ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE)),'NA')+'|'+
     ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX)),'NA')+'|'
   ) AS MEMBERSHIP_PLAN_HSH,
   DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) AS LOAD_DATETIME,
   COALESCE ( LEAD ( DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) ) OVER
   		     (PARTITION BY 
   		          [PLAN_CODE],
   		          [PLAN_SUFFIX],
              Date_effective
   		      ORDER BY LOAD_DATETIME),
   CAST( '9999-12-31' AS DATETIME)) AS LOAD_END_DATETIME,
   CASE
      WHEN ( RANK() OVER (PARTITION BY 
           [PLAN_CODE],
           [PLAN_SUFFIX],
         Date_effective
          ORDER BY LOAD_DATETIME desc )) = 1
      THEN 'Y'
      ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   -1 AS ETL_INSERT_RUN_ID,
   -1 AS ETL_UPDATE_RUN_ID,
   CDC_OPERATION,
   SOURCE_ROW_ID,
   RECORD_SOURCE,
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),CDC_OPERATION)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Date_effective)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Value_Amount)),'NA')+'|'
   ) AS HASH_FULL_RECORD,
   Date_effective AS PLAN_VALUATION_DATE,
   Value_Amount AS PLAN_VALUATION_AMOUNT,
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
         [PLAN_CODE],
         [PLAN_SUFFIX],
         Date_effective      ORDER BY 
         [PLAN_CODE],
         [PLAN_SUFFIX],
         Date_effective,
         [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER
FROM 
   (
      SELECT 
         [LOAD_DATETIME],
         [EVENT_DATETIME],
         [RECORD_SOURCE],
         [SOURCE_ROW_ID],
         [CDC_OPERATION],
         [PLAN_CODE],
         [PLAN_SUFFIX],
         [Date_effective],
         [Value_Amount],
         COMBINED_VALUE,
         CASE 
           WHEN LAG(COMBINED_VALUE,1,0x00000000000000000000000000000000) OVER (PARTITION BY 
             [PLAN_CODE],
             [PLAN_SUFFIX],
             [Date_effective]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] DESC) = COMBINED_VALUE
           THEN 'Same'
           ELSE 'Different'
         END AS VALUE_CHANGE_INDICATOR,
         CASE 
           WHEN LAG([CDC_OPERATION],1,'') OVER (PARTITION BY 
             [PLAN_CODE],
             [PLAN_SUFFIX],
             [Date_effective]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [CDC_OPERATION]
           THEN 'Same'
           ELSE 'Different'
         END AS CDC_CHANGE_INDICATOR,
         CASE 
           WHEN LEAD([LOAD_DATETIME],1,'9999-12-31') OVER (PARTITION BY 
             [PLAN_CODE],
             [PLAN_SUFFIX],
             [Date_effective]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [LOAD_DATETIME]
           THEN 'Same'
           ELSE 'Different'
         END AS TIME_CHANGE_INDICATOR
      FROM
      (
        SELECT
          [LOAD_DATETIME],
          [EVENT_DATETIME],
          [RECORD_SOURCE],
          [SOURCE_ROW_ID],
          [CDC_OPERATION],
                [Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
          [Date_effective],
          [Value_Amount],
         HASHBYTES('MD5',
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Date_effective])),'NA')+'|'+
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Value_Amount])),'NA')+'|'
         ) AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_ESTIMATED_WORTH
      WHERE 13=13
        UNION
        SELECT DISTINCT
          '1900-01-01' AS [LOAD_DATETIME],
          '1900-01-01' AS [EVENT_DATETIME],
          'Data Warehouse' AS [RECORD_SOURCE],
          0 AS [SOURCE_ROW_ID],
          'N/A' AS [CDC_OPERATION],
                [Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
          Date_effective,
          NULL AS Value_Amount,
          0x00000000000000000000000000000000 AS COMBINED_VALUE
        FROM dbo.PSA_PROFILER_ESTIMATED_WORTH
   ) sub
) combined_value
WHERE 
  (VALUE_CHANGE_INDICATOR ='Different' and [CDC_OPERATION] in ('Insert', 'Change')) 
  OR
  (CDC_CHANGE_INDICATOR = 'Different' and TIME_CHANGE_INDICATOR = 'Different')
UNION
SELECT 0x00000000000000000000000000000000,
'1900-01-01',
'9999-12-31',
'Y',
- 1,
- 1,
'N/A',
1,
'Data Warehouse',
'N',
0x00000000000000000000000000000000,
CAST('1900-01-01' AS DATE),
NULL,
1

GO

-- 
-- Satellite View definition for SAT_SEGMENT
-- Generated at 17/07/2019 12:58:37 PM
-- 

USE [150_Persistent_Staging_Area]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[SAT_SEGMENT]') AND type in (N'V'))
DROP VIEW [vedw].[SAT_SEGMENT]
go
CREATE VIEW [vedw].[SAT_SEGMENT] AS  
SELECT
   HASHBYTES('MD5',
     ISNULL(RTRIM(CONVERT(NVARCHAR(100),SEGMENT_CODE)),'NA')+'|'
   ) AS SEGMENT_HSH,
   DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) AS LOAD_DATETIME,
   COALESCE ( LEAD ( DATEADD(mcs,[SOURCE_ROW_ID],LOAD_DATETIME) ) OVER
   		     (PARTITION BY 
   		          [SEGMENT_CODE]
   		      ORDER BY LOAD_DATETIME),
   CAST( '9999-12-31' AS DATETIME)) AS LOAD_END_DATETIME,
   CASE
      WHEN ( RANK() OVER (PARTITION BY 
           [SEGMENT_CODE]
          ORDER BY LOAD_DATETIME desc )) = 1
      THEN 'Y'
      ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   -1 AS ETL_INSERT_RUN_ID,
   -1 AS ETL_UPDATE_RUN_ID,
   CDC_OPERATION,
   SOURCE_ROW_ID,
   RECORD_SOURCE,
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),CDC_OPERATION)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),Demographic_Segment_Description)),'NA')+'|'
   ) AS HASH_FULL_RECORD,
   Demographic_Segment_Description AS SEGMENT_DESCRIPTION,
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
         [SEGMENT_CODE]      ORDER BY 
         [SEGMENT_CODE],
         [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER
FROM 
   (
      SELECT 
         [LOAD_DATETIME],
         [EVENT_DATETIME],
         [RECORD_SOURCE],
         [SOURCE_ROW_ID],
         [CDC_OPERATION],
         [SEGMENT_CODE],
         [Demographic_Segment_Description],
         COMBINED_VALUE,
         CASE 
           WHEN LAG(COMBINED_VALUE,1,0x00000000000000000000000000000000) OVER (PARTITION BY 
             [SEGMENT_CODE]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] DESC) = COMBINED_VALUE
           THEN 'Same'
           ELSE 'Different'
         END AS VALUE_CHANGE_INDICATOR,
         CASE 
           WHEN LAG([CDC_OPERATION],1,'') OVER (PARTITION BY 
             [SEGMENT_CODE]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [CDC_OPERATION]
           THEN 'Same'
           ELSE 'Different'
         END AS CDC_CHANGE_INDICATOR,
         CASE 
           WHEN LEAD([LOAD_DATETIME],1,'9999-12-31') OVER (PARTITION BY 
             [SEGMENT_CODE]
             ORDER BY [LOAD_DATETIME] ASC, [EVENT_DATETIME] ASC, [CDC_OPERATION] ASC) = [LOAD_DATETIME]
           THEN 'Same'
           ELSE 'Different'
         END AS TIME_CHANGE_INDICATOR
      FROM
      (
        SELECT
          [LOAD_DATETIME],
          [EVENT_DATETIME],
          [RECORD_SOURCE],
          [SOURCE_ROW_ID],
          [CDC_OPERATION],
          ISNULL([Demographic_Segment_Code], '') +  'TEST'  AS [SEGMENT_CODE],
          [Demographic_Segment_Description],
         HASHBYTES('MD5',
             ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Demographic_Segment_Description])),'NA')+'|'
         ) AS COMBINED_VALUE
        FROM dbo.PSA_USERMANAGED_SEGMENT
      WHERE 9=9
        UNION
        SELECT DISTINCT
          '1900-01-01' AS [LOAD_DATETIME],
          '1900-01-01' AS [EVENT_DATETIME],
          'Data Warehouse' AS [RECORD_SOURCE],
          0 AS [SOURCE_ROW_ID],
          'N/A' AS [CDC_OPERATION],
          ISNULL([Demographic_Segment_Code], '') +  'TEST'  AS [SEGMENT_CODE],
          NULL AS Demographic_Segment_Description,
          0x00000000000000000000000000000000 AS COMBINED_VALUE
        FROM dbo.PSA_USERMANAGED_SEGMENT
   ) sub
) combined_value
WHERE 
  (VALUE_CHANGE_INDICATOR ='Different' and [CDC_OPERATION] in ('Insert', 'Change')) 
  OR
  (CDC_CHANGE_INDICATOR = 'Different' and TIME_CHANGE_INDICATOR = 'Different')
UNION
SELECT 0x00000000000000000000000000000000,
'1900-01-01',
'9999-12-31',
'Y',
- 1,
- 1,
'N/A',
1,
'Data Warehouse',
'N',
0x00000000000000000000000000000000,
NULL,
1

GO

