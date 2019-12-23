--
-- Staging Area View definition for STG_PROFILER_CUST_MEMBERSHIP
-- Generated at 17/07/2019 12:57:31 PM
--

USE [100_Staging_Area]
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[STG_PROFILER_CUST_MEMBERSHIP]') AND type in (N'V'))
DROP VIEW [vedw].[STG_PROFILER_CUST_MEMBERSHIP]
GO
CREATE VIEW [vedw].[STG_PROFILER_CUST_MEMBERSHIP] AS 
WITH STG_CTE AS 
(
SELECT
   [CustomerID] AS [CustomerID],
   [Plan_Code] AS [Plan_Code],
   [Start_Date] AS [Start_Date],
   [End_Date] AS [End_Date],
   [Status] AS [Status],
   [Comment] AS [Comment],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CustomerID])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Plan_Code])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Start_Date])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[End_Date])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Status])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Comment])),'N/A')+'|'
   ) AS [HASH_FULL_RECORD]
FROM [000_Source].[dbo].[CUST_MEMBERSHIP]
),
PSA_CTE AS
(
SELECT
   A.HASH_FULL_RECORD AS HASH_FULL_RECORD,
   A.CustomerID AS CustomerID,
   A.Plan_Code AS Plan_Code,
   A.Start_Date AS Start_Date,
   A.End_Date AS End_Date,
   A.Status AS Status,
   A.Comment AS Comment
FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUST_MEMBERSHIP A
   JOIN (
        SELECT
            [CustomerID],            [Plan_Code],
            MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME
        FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUST_MEMBERSHIP
        GROUP BY
         CustomerID,         Plan_Code
        ) B ON
   A.CustomerID = B.CustomerID   AND
   A.Plan_Code = B.Plan_Code 
   AND
   A.LOAD_DATETIME = B.MAX_LOAD_DATETIME
WHERE CDC_OPERATION != 'Delete'
)
SELECT
  CASE 
     WHEN STG_CTE.[CustomerID] IS NULL 
     THEN PSA_CTE.[CustomerID] 
     ELSE STG_CTE.[CustomerID] 
  END AS [CustomerID], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Plan_Code] ELSE STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT END AS [Plan_Code], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Start_Date] ELSE STG_CTE.[Start_Date] END AS [Start_Date], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[End_Date] ELSE STG_CTE.[End_Date] END AS [End_Date], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Status] ELSE STG_CTE.[Status] COLLATE DATABASE_DEFAULT END AS [Status], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Comment] ELSE STG_CTE.[Comment] COLLATE DATABASE_DEFAULT END AS [Comment], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[HASH_FULL_RECORD] ELSE STG_CTE.[HASH_FULL_RECORD]  END AS [HASH_FULL_RECORD], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN 'Delete' WHEN PSA_CTE.[CustomerID] IS NULL THEN 'Insert' WHEN STG_CTE.CustomerID IS NOT NULL AND PSA_CTE.CustomerID IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' ELSE 'No Change' END AS [CDC_OPERATION],
  'PROFILER' AS RECORD_SOURCE,
  ROW_NUMBER() OVER 
    (ORDER BY 
      CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[CustomerID] ELSE STG_CTE.[CustomerID] END,
      CASE WHEN STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT IS NULL THEN PSA_CTE.[Plan_Code] ELSE STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT END
    ) AS SOURCE_ROW_ID,
  GETDATE() AS EVENT_DATETIME
FROM STG_CTE
FULL OUTER JOIN PSA_CTE ON 
PSA_CTE.CustomerID = STG_CTE.CustomerID
AND
PSA_CTE.Plan_Code = STG_CTE.Plan_Code COLLATE DATABASE_DEFAULT
WHERE 
(
  CASE 
     WHEN STG_CTE.[CustomerID] IS NULL THEN 'Delete' 
     WHEN PSA_CTE.[CustomerID] IS NULL THEN 'Insert' 
     WHEN STG_CTE.CustomerID IS NOT NULL AND PSA_CTE.CustomerID IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' 
     ELSE 'No Change' 
     END 
) != 'No Change'

GO


--
-- Staging Area View definition for STG_PROFILER_CUSTOMER_OFFER
-- Generated at 17/07/2019 12:57:31 PM
--

USE [100_Staging_Area]
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[STG_PROFILER_CUSTOMER_OFFER]') AND type in (N'V'))
DROP VIEW [vedw].[STG_PROFILER_CUSTOMER_OFFER]
GO
CREATE VIEW [vedw].[STG_PROFILER_CUSTOMER_OFFER] AS 
WITH STG_CTE AS 
(
SELECT
   [CustomerID] AS [CustomerID],
   [OfferID] AS [OfferID],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CustomerID])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[OfferID])),'N/A')+'|'
   ) AS [HASH_FULL_RECORD]
FROM [000_Source].[dbo].[CUSTOMER_OFFER]
),
PSA_CTE AS
(
SELECT
   A.HASH_FULL_RECORD AS HASH_FULL_RECORD,
   A.CustomerID AS CustomerID,
   A.OfferID AS OfferID
FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_OFFER A
   JOIN (
        SELECT
            [CustomerID],            [OfferID],
            MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME
        FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_OFFER
        GROUP BY
         CustomerID,         OfferID
        ) B ON
   A.CustomerID = B.CustomerID   AND
   A.OfferID = B.OfferID 
   AND
   A.LOAD_DATETIME = B.MAX_LOAD_DATETIME
WHERE CDC_OPERATION != 'Delete'
)
SELECT
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[CustomerID] ELSE STG_CTE.[CustomerID] END AS [CustomerID], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[OfferID] ELSE STG_CTE.[OfferID] END AS [OfferID], 
  
  CASE WHEN STG_CTE.[CustomerID] IS NULL 
    THEN PSA_CTE.[HASH_FULL_RECORD] 
    ELSE STG_CTE.[HASH_FULL_RECORD]  
  END AS [HASH_FULL_RECORD], 
 
  CASE 
      WHEN STG_CTE.[CustomerID] IS NULL THEN 'Delete'
      WHEN PSA_CTE.[CustomerID] IS NULL THEN 'Insert' 
      WHEN STG_CTE.CustomerID IS NOT NULL 
        AND PSA_CTE.CustomerID IS NOT NULL 
        AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD 
        THEN 'Change' ELSE 'No Change' 
  END AS [CDC_OPERATION],
  'PROFILER' AS RECORD_SOURCE,
  ROW_NUMBER() OVER 
    (ORDER BY 
      CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[CustomerID] ELSE STG_CTE.[CustomerID] END,
      CASE WHEN STG_CTE.[OfferID] IS NULL THEN PSA_CTE.[OfferID] ELSE STG_CTE.[OfferID] END
    ) AS SOURCE_ROW_ID,
  GETDATE() AS EVENT_DATETIME
FROM STG_CTE
FULL OUTER JOIN PSA_CTE ON 
PSA_CTE.CustomerID = STG_CTE.CustomerID
AND
PSA_CTE.OfferID = STG_CTE.OfferID
WHERE 
(
  CASE 
     WHEN STG_CTE.[CustomerID] IS NULL THEN 'Delete' 
     WHEN PSA_CTE.[CustomerID] IS NULL THEN 'Insert' 
     WHEN STG_CTE.CustomerID IS NOT NULL AND PSA_CTE.CustomerID IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' 
     ELSE 'No Change' 
     END 
) != 'No Change'

GO


--
-- Staging Area View definition for STG_PROFILER_CUSTOMER_PERSONAL
-- Generated at 17/07/2019 12:57:31 PM
--

USE [100_Staging_Area]
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[STG_PROFILER_CUSTOMER_PERSONAL]') AND type in (N'V'))
DROP VIEW [vedw].[STG_PROFILER_CUSTOMER_PERSONAL]
GO
CREATE VIEW [vedw].[STG_PROFILER_CUSTOMER_PERSONAL] AS 
WITH STG_CTE AS 
(
SELECT
   [CustomerID] AS [CustomerID],
   [Given] AS [Given],
   [Surname] AS [Surname],
   [Suburb] AS [Suburb],
   [State] AS [State],
   [Postcode] AS [Postcode],
   [Country] AS [Country],
   [Gender] AS [Gender],
   [DOB] AS [DOB],
   [Contact_Number] AS [Contact_Number],
   [Referee_Offer_Made] AS [Referee_Offer_Made],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CustomerID])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Given])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Surname])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Suburb])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[State])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Postcode])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Country])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Gender])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[DOB])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Contact_Number])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Referee_Offer_Made])),'N/A')+'|'
   ) AS [HASH_FULL_RECORD]
FROM [000_Source].[dbo].[CUSTOMER_PERSONAL]
),
PSA_CTE AS
(
SELECT
   A.HASH_FULL_RECORD AS HASH_FULL_RECORD,
   A.CustomerID AS CustomerID,
   A.Given AS Given,
   A.Surname AS Surname,
   A.Suburb AS Suburb,
   A.State AS State,
   A.Postcode AS Postcode,
   A.Country AS Country,
   A.Gender AS Gender,
   A.DOB AS DOB,
   A.Contact_Number AS Contact_Number,
   A.Referee_Offer_Made AS Referee_Offer_Made
FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_PERSONAL A
   JOIN (
        SELECT
            [CustomerID],
            MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME
        FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_PERSONAL
        GROUP BY
         CustomerID
        ) B ON
   A.CustomerID = B.CustomerID 
   AND
   A.LOAD_DATETIME = B.MAX_LOAD_DATETIME
WHERE CDC_OPERATION != 'Delete'
)
SELECT
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[CustomerID] ELSE STG_CTE.[CustomerID] END AS [CustomerID], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Given] ELSE STG_CTE.[Given] COLLATE DATABASE_DEFAULT END AS [Given], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Surname] ELSE STG_CTE.[Surname] COLLATE DATABASE_DEFAULT END AS [Surname], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Suburb] ELSE STG_CTE.[Suburb] COLLATE DATABASE_DEFAULT END AS [Suburb], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[State] ELSE STG_CTE.[State] COLLATE DATABASE_DEFAULT END AS [State], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Postcode] ELSE STG_CTE.[Postcode] COLLATE DATABASE_DEFAULT END AS [Postcode], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Country] ELSE STG_CTE.[Country] COLLATE DATABASE_DEFAULT END AS [Country], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Gender] ELSE STG_CTE.[Gender] COLLATE DATABASE_DEFAULT END AS [Gender], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[DOB] ELSE STG_CTE.[DOB] END AS [DOB], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Contact_Number] ELSE STG_CTE.[Contact_Number] END AS [Contact_Number], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[Referee_Offer_Made] ELSE STG_CTE.[Referee_Offer_Made] END AS [Referee_Offer_Made], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[HASH_FULL_RECORD] ELSE STG_CTE.[HASH_FULL_RECORD]  END AS [HASH_FULL_RECORD], 
  CASE WHEN STG_CTE.[CustomerID] IS NULL THEN 'Delete' WHEN PSA_CTE.[CustomerID] IS NULL THEN 'Insert' WHEN STG_CTE.CustomerID IS NOT NULL AND PSA_CTE.CustomerID IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' ELSE 'No Change' END AS [CDC_OPERATION],
  'PROFILER' AS RECORD_SOURCE,
  ROW_NUMBER() OVER 
    (ORDER BY 
      CASE WHEN STG_CTE.[CustomerID] IS NULL THEN PSA_CTE.[CustomerID] ELSE STG_CTE.[CustomerID] END
    ) AS SOURCE_ROW_ID,
  GETDATE() AS EVENT_DATETIME
FROM STG_CTE
FULL OUTER JOIN PSA_CTE ON 
PSA_CTE.CustomerID = STG_CTE.CustomerID
WHERE 
(
  CASE 
     WHEN STG_CTE.[CustomerID] IS NULL THEN 'Delete' 
     WHEN PSA_CTE.[CustomerID] IS NULL THEN 'Insert' 
     WHEN STG_CTE.CustomerID IS NOT NULL AND PSA_CTE.CustomerID IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' 
     ELSE 'No Change' 
     END 
) != 'No Change'

GO


--
-- Staging Area View definition for STG_PROFILER_ESTIMATED_WORTH
-- Generated at 17/07/2019 12:57:31 PM
--

USE [100_Staging_Area]
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[STG_PROFILER_ESTIMATED_WORTH]') AND type in (N'V'))
DROP VIEW [vedw].[STG_PROFILER_ESTIMATED_WORTH]
GO
CREATE VIEW [vedw].[STG_PROFILER_ESTIMATED_WORTH] AS 
WITH STG_CTE AS 
(
SELECT
   [Plan_Code] AS [Plan_Code],
   CONVERT(DATETIME2(7),[Date_effective]) AS [Date_effective],
   [Value_Amount] AS [Value_Amount],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Plan_Code])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Date_effective])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Value_Amount])),'N/A')+'|'
   ) AS [HASH_FULL_RECORD]
FROM [000_Source].[dbo].[ESTIMATED_WORTH]
),
PSA_CTE AS
(
SELECT
   A.HASH_FULL_RECORD AS HASH_FULL_RECORD,
   A.Plan_Code AS Plan_Code,
   A.Date_effective AS Date_effective,
   A.Value_Amount AS Value_Amount
FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_ESTIMATED_WORTH A
   JOIN (
        SELECT
            [Plan_Code],            [Date_effective],
            MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME
        FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_ESTIMATED_WORTH
        GROUP BY
         Plan_Code,         Date_effective
        ) B ON
   A.Plan_Code = B.Plan_Code   AND
   A.Date_effective = B.Date_effective 
   AND
   A.LOAD_DATETIME = B.MAX_LOAD_DATETIME
WHERE CDC_OPERATION != 'Delete'
)
SELECT
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN PSA_CTE.[Plan_Code] ELSE STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT END AS [Plan_Code], 
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN PSA_CTE.[Date_effective] ELSE STG_CTE.[Date_effective] END AS [Date_effective], 
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN PSA_CTE.[Value_Amount] ELSE STG_CTE.[Value_Amount] END AS [Value_Amount], 
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN PSA_CTE.[HASH_FULL_RECORD] ELSE STG_CTE.[HASH_FULL_RECORD]  END AS [HASH_FULL_RECORD], 
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN 'Delete' WHEN PSA_CTE.[Plan_Code] IS NULL THEN 'Insert' WHEN STG_CTE.Plan_Code IS NOT NULL AND PSA_CTE.Plan_Code IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' ELSE 'No Change' END AS [CDC_OPERATION],
  'PROFILER' AS RECORD_SOURCE,
  ROW_NUMBER() OVER 
    (ORDER BY 
      CASE WHEN STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT IS NULL THEN PSA_CTE.[Plan_Code] ELSE STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT END,
      CASE WHEN STG_CTE.[Date_effective] IS NULL THEN PSA_CTE.[Date_effective] ELSE STG_CTE.[Date_effective] END
    ) AS SOURCE_ROW_ID,
  GETDATE() AS EVENT_DATETIME
FROM STG_CTE
FULL OUTER JOIN PSA_CTE ON 
PSA_CTE.Plan_Code = STG_CTE.Plan_Code COLLATE DATABASE_DEFAULT
AND
PSA_CTE.Date_effective = STG_CTE.Date_effective
WHERE 
(
  CASE 
     WHEN STG_CTE.[Plan_Code] IS NULL THEN 'Delete' 
     WHEN PSA_CTE.[Plan_Code] IS NULL THEN 'Insert' 
     WHEN STG_CTE.Plan_Code IS NOT NULL AND PSA_CTE.Plan_Code IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' 
     ELSE 'No Change' 
     END 
) != 'No Change'

GO


--
-- Staging Area View definition for STG_PROFILER_OFFER
-- Generated at 17/07/2019 12:57:31 PM
--

USE [100_Staging_Area]
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[STG_PROFILER_OFFER]') AND type in (N'V'))
DROP VIEW [vedw].[STG_PROFILER_OFFER]
GO
CREATE VIEW [vedw].[STG_PROFILER_OFFER] AS 
WITH STG_CTE AS 
(
SELECT
   [OfferID] AS [OfferID],
   [Offer_Long_Description] AS [Offer_Long_Description],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[OfferID])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Offer_Long_Description])),'N/A')+'|'
   ) AS [HASH_FULL_RECORD]
FROM [000_Source].[dbo].[OFFER]
),
PSA_CTE AS
(
SELECT
   A.HASH_FULL_RECORD AS HASH_FULL_RECORD,
   A.OfferID AS OfferID,
   A.Offer_Long_Description AS Offer_Long_Description
FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_OFFER A
   JOIN (
        SELECT
            [OfferID],
            MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME
        FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_OFFER
        GROUP BY
         OfferID
        ) B ON
   A.OfferID = B.OfferID 
   AND
   A.LOAD_DATETIME = B.MAX_LOAD_DATETIME
WHERE CDC_OPERATION != 'Delete'
)
SELECT
  CASE WHEN STG_CTE.[OfferID] IS NULL THEN PSA_CTE.[OfferID] ELSE STG_CTE.[OfferID] END AS [OfferID], 
  CASE WHEN STG_CTE.[OfferID] IS NULL THEN PSA_CTE.[Offer_Long_Description] ELSE STG_CTE.[Offer_Long_Description] COLLATE DATABASE_DEFAULT END AS [Offer_Long_Description], 
  CASE WHEN STG_CTE.[OfferID] IS NULL THEN PSA_CTE.[HASH_FULL_RECORD] ELSE STG_CTE.[HASH_FULL_RECORD]  END AS [HASH_FULL_RECORD], 
  CASE WHEN STG_CTE.[OfferID] IS NULL THEN 'Delete' WHEN PSA_CTE.[OfferID] IS NULL THEN 'Insert' WHEN STG_CTE.OfferID IS NOT NULL AND PSA_CTE.OfferID IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' ELSE 'No Change' END AS [CDC_OPERATION],
  'PROFILER' AS RECORD_SOURCE,
  ROW_NUMBER() OVER 
    (ORDER BY 
      CASE WHEN STG_CTE.[OfferID] IS NULL THEN PSA_CTE.[OfferID] ELSE STG_CTE.[OfferID] END
    ) AS SOURCE_ROW_ID,
  GETDATE() AS EVENT_DATETIME
FROM STG_CTE
FULL OUTER JOIN PSA_CTE ON 
PSA_CTE.OfferID = STG_CTE.OfferID
WHERE 
(
  CASE 
     WHEN STG_CTE.[OfferID] IS NULL THEN 'Delete' 
     WHEN PSA_CTE.[OfferID] IS NULL THEN 'Insert' 
     WHEN STG_CTE.OfferID IS NOT NULL AND PSA_CTE.OfferID IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' 
     ELSE 'No Change' 
     END 
) != 'No Change'

GO


--
-- Staging Area View definition for STG_PROFILER_PERSONALISED_COSTING
-- Generated at 17/07/2019 12:57:31 PM
--

USE [100_Staging_Area]
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[STG_PROFILER_PERSONALISED_COSTING]') AND type in (N'V'))
DROP VIEW [vedw].[STG_PROFILER_PERSONALISED_COSTING]
GO
CREATE VIEW [vedw].[STG_PROFILER_PERSONALISED_COSTING] AS 
WITH STG_CTE AS 
(
SELECT
   [Member] AS [Member],
   [Segment] AS [Segment],
   [Plan_Code] AS [Plan_Code],
   CONVERT(DATETIME2(7),[Date_effective]) AS [Date_effective],
   [Monthly_Cost] AS [Monthly_Cost],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Member])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Segment])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Plan_Code])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Date_effective])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Monthly_Cost])),'N/A')+'|'
   ) AS [HASH_FULL_RECORD]
FROM [000_Source].[dbo].[PERSONALISED_COSTING]
),
PSA_CTE AS
(
SELECT
   A.HASH_FULL_RECORD AS HASH_FULL_RECORD,
   A.Member AS Member,
   A.Segment AS Segment,
   A.Plan_Code AS Plan_Code,
   A.Date_effective AS Date_effective,
   A.Monthly_Cost AS Monthly_Cost
FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PERSONALISED_COSTING A
   JOIN (
        SELECT
            [Segment],            [Plan_Code],            [Date_effective],
            MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME
        FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PERSONALISED_COSTING
        GROUP BY
         Segment,         Plan_Code,         Date_effective
        ) B ON
   A.Segment = B.Segment   AND
   A.Plan_Code = B.Plan_Code   AND
   A.Date_effective = B.Date_effective 
   AND
   A.LOAD_DATETIME = B.MAX_LOAD_DATETIME
WHERE CDC_OPERATION != 'Delete'
)
SELECT
  CASE WHEN STG_CTE.[Segment] IS NULL THEN PSA_CTE.[Member] ELSE STG_CTE.[Member] END AS [Member], 
  CASE WHEN STG_CTE.[Segment] IS NULL THEN PSA_CTE.[Segment] ELSE STG_CTE.[Segment] COLLATE DATABASE_DEFAULT END AS [Segment], 
  CASE WHEN STG_CTE.[Segment] IS NULL THEN PSA_CTE.[Plan_Code] ELSE STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT END AS [Plan_Code], 
  CASE WHEN STG_CTE.[Segment] IS NULL THEN PSA_CTE.[Date_effective] ELSE STG_CTE.[Date_effective] END AS [Date_effective], 
  CASE WHEN STG_CTE.[Segment] IS NULL THEN PSA_CTE.[Monthly_Cost] ELSE STG_CTE.[Monthly_Cost] END AS [Monthly_Cost], 
  CASE WHEN STG_CTE.[Segment] IS NULL THEN PSA_CTE.[HASH_FULL_RECORD] ELSE STG_CTE.[HASH_FULL_RECORD]  END AS [HASH_FULL_RECORD], 
  CASE WHEN STG_CTE.[Segment] IS NULL THEN 'Delete' WHEN PSA_CTE.[Segment] IS NULL THEN 'Insert' WHEN STG_CTE.Segment IS NOT NULL AND PSA_CTE.Segment IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' ELSE 'No Change' END AS [CDC_OPERATION],
  'PROFILER' AS RECORD_SOURCE,
  ROW_NUMBER() OVER 
    (ORDER BY 
      CASE WHEN STG_CTE.[Segment] COLLATE DATABASE_DEFAULT IS NULL THEN PSA_CTE.[Segment] ELSE STG_CTE.[Segment] COLLATE DATABASE_DEFAULT END,
      CASE WHEN STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT IS NULL THEN PSA_CTE.[Plan_Code] ELSE STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT END,
      CASE WHEN STG_CTE.[Date_effective] IS NULL THEN PSA_CTE.[Date_effective] ELSE STG_CTE.[Date_effective] END
    ) AS SOURCE_ROW_ID,
  GETDATE() AS EVENT_DATETIME
FROM STG_CTE
FULL OUTER JOIN PSA_CTE ON 
PSA_CTE.Segment = STG_CTE.Segment COLLATE DATABASE_DEFAULT
AND
PSA_CTE.Plan_Code = STG_CTE.Plan_Code COLLATE DATABASE_DEFAULT
AND
PSA_CTE.Date_effective = STG_CTE.Date_effective
WHERE 
(
  CASE 
     WHEN STG_CTE.[Segment] IS NULL THEN 'Delete' 
     WHEN PSA_CTE.[Segment] IS NULL THEN 'Insert' 
     WHEN STG_CTE.Segment IS NOT NULL AND PSA_CTE.Segment IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' 
     ELSE 'No Change' 
     END 
) != 'No Change'

GO


--
-- Staging Area View definition for STG_PROFILER_PLAN
-- Generated at 17/07/2019 12:57:31 PM
--

USE [100_Staging_Area]
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[STG_PROFILER_PLAN]') AND type in (N'V'))
DROP VIEW [vedw].[STG_PROFILER_PLAN]
GO
CREATE VIEW [vedw].[STG_PROFILER_PLAN] AS 
WITH STG_CTE AS 
(
SELECT
   [Plan_Code] AS [Plan_Code],
   [Plan_Desc] AS [Plan_Desc],
   [Renewal_Plan_Code] AS [Renewal_Plan_Code],
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Plan_Code])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Plan_Desc])),'N/A')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[Renewal_Plan_Code])),'N/A')+'|'
   ) AS [HASH_FULL_RECORD]
FROM [000_Source].[dbo].[PLAN]
),
PSA_CTE AS
(
SELECT
   A.HASH_FULL_RECORD AS HASH_FULL_RECORD,
   A.Plan_Code AS Plan_Code,
   A.Plan_Desc AS Plan_Desc,
   A.Renewal_Plan_Code AS Renewal_Plan_Code
FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PLAN A
   JOIN (
        SELECT
            [Plan_Code],
            MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME
        FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PLAN
        GROUP BY
         Plan_Code
        ) B ON
   A.Plan_Code = B.Plan_Code 
   AND
   A.LOAD_DATETIME = B.MAX_LOAD_DATETIME
WHERE CDC_OPERATION != 'Delete'
)
SELECT
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN PSA_CTE.[Plan_Code] ELSE STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT END AS [Plan_Code], 
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN PSA_CTE.[Plan_Desc] ELSE STG_CTE.[Plan_Desc] COLLATE DATABASE_DEFAULT END AS [Plan_Desc], 
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN PSA_CTE.[Renewal_Plan_Code] ELSE STG_CTE.[Renewal_Plan_Code] COLLATE DATABASE_DEFAULT END AS [Renewal_Plan_Code], 
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN PSA_CTE.[HASH_FULL_RECORD] ELSE STG_CTE.[HASH_FULL_RECORD]  END AS [HASH_FULL_RECORD], 
  CASE WHEN STG_CTE.[Plan_Code] IS NULL THEN 'Delete' WHEN PSA_CTE.[Plan_Code] IS NULL THEN 'Insert' WHEN STG_CTE.Plan_Code IS NOT NULL AND PSA_CTE.Plan_Code IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' ELSE 'No Change' END AS [CDC_OPERATION],
  'PROFILER' AS RECORD_SOURCE,
  ROW_NUMBER() OVER 
    (ORDER BY 
      CASE WHEN STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT IS NULL THEN PSA_CTE.[Plan_Code] ELSE STG_CTE.[Plan_Code] COLLATE DATABASE_DEFAULT END
    ) AS SOURCE_ROW_ID,
  GETDATE() AS EVENT_DATETIME
FROM STG_CTE
FULL OUTER JOIN PSA_CTE ON 
PSA_CTE.Plan_Code = STG_CTE.Plan_Code COLLATE DATABASE_DEFAULT
WHERE 
(
  CASE 
     WHEN STG_CTE.[Plan_Code] IS NULL THEN 'Delete' 
     WHEN PSA_CTE.[Plan_Code] IS NULL THEN 'Insert' 
     WHEN STG_CTE.Plan_Code IS NOT NULL AND PSA_CTE.Plan_Code IS NOT NULL AND STG_CTE.HASH_FULL_RECORD != PSA_CTE.HASH_FULL_RECORD THEN 'Change' 
     ELSE 'No Change' 
     END 
) != 'No Change'

GO




--
-- PSA View definition for PSA_PROFILER_CUST_MEMBERSHIP
-- Generated at 11/21/2014 11:52:15 AM
--

USE [100_Staging_Area]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[PSA_PROFILER_CUST_MEMBERSHIP]') AND type in (N'V'))
DROP VIEW [vedw].[PSA_PROFILER_CUST_MEMBERSHIP];
GO
CREATE VIEW [vedw].[PSA_PROFILER_CUST_MEMBERSHIP] AS 
SELECT 
  PSA_PROFILER_CUST_MEMBERSHIP_HSH,
  LOAD_DATETIME,
  EVENT_DATETIME,
  RECORD_SOURCE,
  SOURCE_ROW_ID,
  CDC_OPERATION,
  HASH_FULL_RECORD,
  CustomerID,
  Plan_Code,
  Start_Date,
  End_Date,
  Status,
  Comment,
  LKP_HASH_FULL_RECORD,
  LKP_CDC_OPERATION,
  KEY_ROW_NUMBER
FROM
(
  SELECT
    HASHBYTES('MD5',
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.CustomerID)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Plan_Code)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.SOURCE_ROW_ID)),'NA')+'|'+
       CONVERT(NVARCHAR(100),STG.[LOAD_DATETIME],126)+'|'
    ) AS PSA_PROFILER_CUST_MEMBERSHIP_HSH,
    STG.LOAD_DATETIME,
    STG.EVENT_DATETIME,
    STG.RECORD_SOURCE,
    STG.SOURCE_ROW_ID,
    STG.CDC_OPERATION,
    STG.HASH_FULL_RECORD,
    STG.CustomerID,
    STG.Plan_Code,
    STG.Start_Date,
    STG.End_Date,
    STG.Status,
    STG.Comment,
    COALESCE(maxsub.LKP_HASH_FULL_RECORD,'N/A') AS LKP_HASH_FULL_RECORD,
    COALESCE(maxsub.LKP_CDC_OPERATION,'N/A') AS LKP_CDC_OPERATION,
    CAST(ROW_NUMBER() OVER (PARTITION  BY 
       STG.CustomerID,       STG.Plan_Code,       STG.SOURCE_ROW_ID
    ORDER BY 
       STG.CustomerID,        STG.Plan_Code,        STG.SOURCE_ROW_ID, STG.LOAD_DATETIME) AS INT) AS KEY_ROW_NUMBER
  FROM [dbo].STG_PROFILER_CUST_MEMBERSHIP STG
  LEFT OUTER JOIN -- Prevent reprocessing
    [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUST_MEMBERSHIP HSTG
    ON
       HSTG.CustomerID = STG.CustomerID AND
       HSTG.Plan_Code = STG.Plan_Code AND
       HSTG.SOURCE_ROW_ID = STG.SOURCE_ROW_ID AND
       HSTG.LOAD_DATETIME=STG.LOAD_DATETIME
  LEFT OUTER JOIN -- max HSTG checksum
  (
    SELECT
      A.CustomerID,
      A.Plan_Code,
      A.SOURCE_ROW_ID,
      A.HASH_FULL_RECORD AS LKP_HASH_FULL_RECORD,
      A.CDC_OPERATION AS LKP_CDC_OPERATION
    FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUST_MEMBERSHIP A
    JOIN (
      SELECT 
        B.CustomerID,
        B.Plan_Code,
        B.SOURCE_ROW_ID,
        MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME 
      FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUST_MEMBERSHIP B
      GROUP BY 
       B.CustomerID,
       B.Plan_Code,
       B.SOURCE_ROW_ID
      ) C ON
        A.CustomerID = C.CustomerID AND
        A.Plan_Code = C.Plan_Code AND
        A.SOURCE_ROW_ID = C.SOURCE_ROW_ID AND
        A.LOAD_DATETIME = C.MAX_LOAD_DATETIME
  ) maxsub ON
     STG.CustomerID = maxsub.CustomerID AND
     STG.Plan_Code = maxsub.Plan_Code AND
     STG.SOURCE_ROW_ID = maxsub.SOURCE_ROW_ID 
  WHERE
    HSTG.CustomerID IS NULL -- prevent reprocessing
) sub
WHERE
(
  KEY_ROW_NUMBER=1
  AND
  (
    (HASH_FULL_RECORD!=LKP_HASH_FULL_RECORD)
    -- The checksums are different
    OR
    (HASH_FULL_RECORD=LKP_HASH_FULL_RECORD AND CDC_OPERATION!=LKP_CDC_OPERATION)
    -- The checksums are the same but the CDC is different
    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change
  )
)
OR
(
  -- It's not the most recent change in the set, so the record can be inserted as-is
  KEY_ROW_NUMBER!=1
)

GO

--
-- PSA View definition for PSA_PROFILER_CUSTOMER_OFFER
-- Generated at 11/21/2014 11:52:15 AM
--

USE [100_Staging_Area]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[PSA_PROFILER_CUSTOMER_OFFER]') AND type in (N'V'))
DROP VIEW [vedw].[PSA_PROFILER_CUSTOMER_OFFER];
GO
CREATE VIEW [vedw].[PSA_PROFILER_CUSTOMER_OFFER] AS 
SELECT 
  PSA_PROFILER_CUSTOMER_OFFER_HSH,
  LOAD_DATETIME,
  EVENT_DATETIME,
  RECORD_SOURCE,
  SOURCE_ROW_ID,
  CDC_OPERATION,
  HASH_FULL_RECORD,
  CustomerID,
  OfferID,
  LKP_HASH_FULL_RECORD,
  LKP_CDC_OPERATION,
  KEY_ROW_NUMBER
FROM
(
  SELECT
    HASHBYTES('MD5',
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.CustomerID)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.OfferID)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.SOURCE_ROW_ID)),'NA')+'|'+
       CONVERT(NVARCHAR(100),STG.[LOAD_DATETIME],126)+'|'
    ) AS PSA_PROFILER_CUSTOMER_OFFER_HSH,
    STG.LOAD_DATETIME,
    STG.EVENT_DATETIME,
    STG.RECORD_SOURCE,
    STG.SOURCE_ROW_ID,
    STG.CDC_OPERATION,
    STG.HASH_FULL_RECORD,
    STG.CustomerID,
    STG.OfferID,
    COALESCE(maxsub.LKP_HASH_FULL_RECORD,'N/A') AS LKP_HASH_FULL_RECORD,
    COALESCE(maxsub.LKP_CDC_OPERATION,'N/A') AS LKP_CDC_OPERATION,
    CAST(ROW_NUMBER() OVER (PARTITION  BY 
       STG.CustomerID,       STG.OfferID,       STG.SOURCE_ROW_ID
    ORDER BY 
       STG.CustomerID,        STG.OfferID,        STG.SOURCE_ROW_ID, STG.LOAD_DATETIME) AS INT) AS KEY_ROW_NUMBER
  FROM [dbo].STG_PROFILER_CUSTOMER_OFFER STG
  LEFT OUTER JOIN -- Prevent reprocessing
    [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_OFFER HSTG
    ON
       HSTG.CustomerID = STG.CustomerID AND
       HSTG.OfferID = STG.OfferID AND
       HSTG.SOURCE_ROW_ID = STG.SOURCE_ROW_ID AND
       HSTG.LOAD_DATETIME=STG.LOAD_DATETIME
  LEFT OUTER JOIN -- max HSTG checksum
  (
    SELECT
      A.CustomerID,
      A.OfferID,
      A.SOURCE_ROW_ID,
      A.HASH_FULL_RECORD AS LKP_HASH_FULL_RECORD,
      A.CDC_OPERATION AS LKP_CDC_OPERATION
    FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_OFFER A
    JOIN (
      SELECT 
        B.CustomerID,
        B.OfferID,
        B.SOURCE_ROW_ID,
        MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME 
      FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_OFFER B
      GROUP BY 
       B.CustomerID,
       B.OfferID,
       B.SOURCE_ROW_ID
      ) C ON
        A.CustomerID = C.CustomerID AND
        A.OfferID = C.OfferID AND
        A.SOURCE_ROW_ID = C.SOURCE_ROW_ID AND
        A.LOAD_DATETIME = C.MAX_LOAD_DATETIME
  ) maxsub ON
     STG.CustomerID = maxsub.CustomerID AND
     STG.OfferID = maxsub.OfferID AND
     STG.SOURCE_ROW_ID = maxsub.SOURCE_ROW_ID 
  WHERE
    HSTG.CustomerID IS NULL -- prevent reprocessing
) sub
WHERE
(
  KEY_ROW_NUMBER=1
  AND
  (
    (HASH_FULL_RECORD!=LKP_HASH_FULL_RECORD)
    -- The checksums are different
    OR
    (HASH_FULL_RECORD=LKP_HASH_FULL_RECORD AND CDC_OPERATION!=LKP_CDC_OPERATION)
    -- The checksums are the same but the CDC is different
    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change
  )
)
OR
(
  -- It's not the most recent change in the set, so the record can be inserted as-is
  KEY_ROW_NUMBER!=1
)

GO

--
-- PSA View definition for PSA_PROFILER_CUSTOMER_PERSONAL
-- Generated at 11/21/2014 11:52:15 AM
--

USE [100_Staging_Area]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[PSA_PROFILER_CUSTOMER_PERSONAL]') AND type in (N'V'))
DROP VIEW [vedw].[PSA_PROFILER_CUSTOMER_PERSONAL];
GO
CREATE VIEW [vedw].[PSA_PROFILER_CUSTOMER_PERSONAL] AS 
SELECT 
  PSA_PROFILER_CUSTOMER_PERSONAL_HSH,
  LOAD_DATETIME,
  EVENT_DATETIME,
  RECORD_SOURCE,
  SOURCE_ROW_ID,
  CDC_OPERATION,
  HASH_FULL_RECORD,
  CustomerID,
  Given,
  Surname,
  Suburb,
  State,
  Postcode,
  Country,
  Gender,
  DOB,
  Contact_Number,
  Referee_Offer_Made,
  LKP_HASH_FULL_RECORD,
  LKP_CDC_OPERATION,
  KEY_ROW_NUMBER
FROM
(
  SELECT
    HASHBYTES('MD5',
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.CustomerID)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.SOURCE_ROW_ID)),'NA')+'|'+
       CONVERT(NVARCHAR(100),STG.[LOAD_DATETIME],126)+'|'
    ) AS PSA_PROFILER_CUSTOMER_PERSONAL_HSH,
    STG.LOAD_DATETIME,
    STG.EVENT_DATETIME,
    STG.RECORD_SOURCE,
    STG.SOURCE_ROW_ID,
    STG.CDC_OPERATION,
    STG.HASH_FULL_RECORD,
    STG.CustomerID,
    STG.Given,
    STG.Surname,
    STG.Suburb,
    STG.State,
    STG.Postcode,
    STG.Country,
    STG.Gender,
    STG.DOB,
    STG.Contact_Number,
    STG.Referee_Offer_Made,
    COALESCE(maxsub.LKP_HASH_FULL_RECORD,'N/A') AS LKP_HASH_FULL_RECORD,
    COALESCE(maxsub.LKP_CDC_OPERATION,'N/A') AS LKP_CDC_OPERATION,
    CAST(ROW_NUMBER() OVER (PARTITION  BY 
       STG.CustomerID,       STG.SOURCE_ROW_ID
    ORDER BY 
       STG.CustomerID,        STG.SOURCE_ROW_ID, STG.LOAD_DATETIME) AS INT) AS KEY_ROW_NUMBER
  FROM [dbo].STG_PROFILER_CUSTOMER_PERSONAL STG
  LEFT OUTER JOIN -- Prevent reprocessing
    [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_PERSONAL HSTG
    ON
       HSTG.CustomerID = STG.CustomerID AND
       HSTG.SOURCE_ROW_ID = STG.SOURCE_ROW_ID AND
       HSTG.LOAD_DATETIME=STG.LOAD_DATETIME
  LEFT OUTER JOIN -- max HSTG checksum
  (
    SELECT
      A.CustomerID,
      A.SOURCE_ROW_ID,
      A.HASH_FULL_RECORD AS LKP_HASH_FULL_RECORD,
      A.CDC_OPERATION AS LKP_CDC_OPERATION
    FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_PERSONAL A
    JOIN (
      SELECT 
        B.CustomerID,
        B.SOURCE_ROW_ID,
        MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME 
      FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_CUSTOMER_PERSONAL B
      GROUP BY 
       B.CustomerID,
       B.SOURCE_ROW_ID
      ) C ON
        A.CustomerID = C.CustomerID AND
        A.SOURCE_ROW_ID = C.SOURCE_ROW_ID AND
        A.LOAD_DATETIME = C.MAX_LOAD_DATETIME
  ) maxsub ON
     STG.CustomerID = maxsub.CustomerID AND
     STG.SOURCE_ROW_ID = maxsub.SOURCE_ROW_ID 
  WHERE
    HSTG.CustomerID IS NULL -- prevent reprocessing
) sub
WHERE
(
  KEY_ROW_NUMBER=1
  AND
  (
    (HASH_FULL_RECORD!=LKP_HASH_FULL_RECORD)
    -- The checksums are different
    OR
    (HASH_FULL_RECORD=LKP_HASH_FULL_RECORD AND CDC_OPERATION!=LKP_CDC_OPERATION)
    -- The checksums are the same but the CDC is different
    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change
  )
)
OR
(
  -- It's not the most recent change in the set, so the record can be inserted as-is
  KEY_ROW_NUMBER!=1
)

GO

--
-- PSA View definition for PSA_PROFILER_ESTIMATED_WORTH
-- Generated at 11/21/2014 11:52:15 AM
--

USE [100_Staging_Area]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[PSA_PROFILER_ESTIMATED_WORTH]') AND type in (N'V'))
DROP VIEW [vedw].[PSA_PROFILER_ESTIMATED_WORTH];
GO
CREATE VIEW [vedw].[PSA_PROFILER_ESTIMATED_WORTH] AS 
SELECT 
  PSA_PROFILER_ESTIMATED_WORTH_HSH,
  LOAD_DATETIME,
  EVENT_DATETIME,
  RECORD_SOURCE,
  SOURCE_ROW_ID,
  CDC_OPERATION,
  HASH_FULL_RECORD,
  Plan_Code,
  Date_effective,
  Value_Amount,
  LKP_HASH_FULL_RECORD,
  LKP_CDC_OPERATION,
  KEY_ROW_NUMBER
FROM
(
  SELECT
    HASHBYTES('MD5',
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Plan_Code)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Date_effective)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.SOURCE_ROW_ID)),'NA')+'|'+
       CONVERT(NVARCHAR(100),STG.[LOAD_DATETIME],126)+'|'
    ) AS PSA_PROFILER_ESTIMATED_WORTH_HSH,
    STG.LOAD_DATETIME,
    STG.EVENT_DATETIME,
    STG.RECORD_SOURCE,
    STG.SOURCE_ROW_ID,
    STG.CDC_OPERATION,
    STG.HASH_FULL_RECORD,
    STG.Plan_Code,
    STG.Date_effective,
    STG.Value_Amount,
    COALESCE(maxsub.LKP_HASH_FULL_RECORD,'N/A') AS LKP_HASH_FULL_RECORD,
    COALESCE(maxsub.LKP_CDC_OPERATION,'N/A') AS LKP_CDC_OPERATION,
    CAST(ROW_NUMBER() OVER (PARTITION  BY 
       STG.Plan_Code,       STG.Date_effective,       STG.SOURCE_ROW_ID
    ORDER BY 
       STG.Plan_Code,        STG.Date_effective,        STG.SOURCE_ROW_ID, STG.LOAD_DATETIME) AS INT) AS KEY_ROW_NUMBER
  FROM [dbo].STG_PROFILER_ESTIMATED_WORTH STG
  LEFT OUTER JOIN -- Prevent reprocessing
    [150_Persistent_Staging_Area].dbo.PSA_PROFILER_ESTIMATED_WORTH HSTG
    ON
       HSTG.Plan_Code = STG.Plan_Code AND
       HSTG.Date_effective = STG.Date_effective AND
       HSTG.SOURCE_ROW_ID = STG.SOURCE_ROW_ID AND
       HSTG.LOAD_DATETIME=STG.LOAD_DATETIME
  LEFT OUTER JOIN -- max HSTG checksum
  (
    SELECT
      A.Plan_Code,
      A.Date_effective,
      A.SOURCE_ROW_ID,
      A.HASH_FULL_RECORD AS LKP_HASH_FULL_RECORD,
      A.CDC_OPERATION AS LKP_CDC_OPERATION
    FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_ESTIMATED_WORTH A
    JOIN (
      SELECT 
        B.Plan_Code,
        B.Date_effective,
        B.SOURCE_ROW_ID,
        MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME 
      FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_ESTIMATED_WORTH B
      GROUP BY 
       B.Plan_Code,
       B.Date_effective,
       B.SOURCE_ROW_ID
      ) C ON
        A.Plan_Code = C.Plan_Code AND
        A.Date_effective = C.Date_effective AND
        A.SOURCE_ROW_ID = C.SOURCE_ROW_ID AND
        A.LOAD_DATETIME = C.MAX_LOAD_DATETIME
  ) maxsub ON
     STG.Plan_Code = maxsub.Plan_Code AND
     STG.Date_effective = maxsub.Date_effective AND
     STG.SOURCE_ROW_ID = maxsub.SOURCE_ROW_ID 
  WHERE
    HSTG.Plan_Code IS NULL -- prevent reprocessing
) sub
WHERE
(
  KEY_ROW_NUMBER=1
  AND
  (
    (HASH_FULL_RECORD!=LKP_HASH_FULL_RECORD)
    -- The checksums are different
    OR
    (HASH_FULL_RECORD=LKP_HASH_FULL_RECORD AND CDC_OPERATION!=LKP_CDC_OPERATION)
    -- The checksums are the same but the CDC is different
    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change
  )
)
OR
(
  -- It's not the most recent change in the set, so the record can be inserted as-is
  KEY_ROW_NUMBER!=1
)

GO

--
-- PSA View definition for PSA_PROFILER_OFFER
-- Generated at 11/21/2014 11:52:15 AM
--

USE [100_Staging_Area]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[PSA_PROFILER_OFFER]') AND type in (N'V'))
DROP VIEW [vedw].[PSA_PROFILER_OFFER];
GO
CREATE VIEW [vedw].[PSA_PROFILER_OFFER] AS 
SELECT 
  PSA_PROFILER_OFFER_HSH,
  LOAD_DATETIME,
  EVENT_DATETIME,
  RECORD_SOURCE,
  SOURCE_ROW_ID,
  CDC_OPERATION,
  HASH_FULL_RECORD,
  OfferID,
  Offer_Long_Description,
  LKP_HASH_FULL_RECORD,
  LKP_CDC_OPERATION,
  KEY_ROW_NUMBER
FROM
(
  SELECT
    HASHBYTES('MD5',
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.OfferID)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.SOURCE_ROW_ID)),'NA')+'|'+
       CONVERT(NVARCHAR(100),STG.[LOAD_DATETIME],126)+'|'
    ) AS PSA_PROFILER_OFFER_HSH,
    STG.LOAD_DATETIME,
    STG.EVENT_DATETIME,
    STG.RECORD_SOURCE,
    STG.SOURCE_ROW_ID,
    STG.CDC_OPERATION,
    STG.HASH_FULL_RECORD,
    STG.OfferID,
    STG.Offer_Long_Description,
    COALESCE(maxsub.LKP_HASH_FULL_RECORD,'N/A') AS LKP_HASH_FULL_RECORD,
    COALESCE(maxsub.LKP_CDC_OPERATION,'N/A') AS LKP_CDC_OPERATION,
    CAST(ROW_NUMBER() OVER (PARTITION  BY 
       STG.OfferID,       STG.SOURCE_ROW_ID
    ORDER BY 
       STG.OfferID,        STG.SOURCE_ROW_ID, STG.LOAD_DATETIME) AS INT) AS KEY_ROW_NUMBER
  FROM [dbo].STG_PROFILER_OFFER STG
  LEFT OUTER JOIN -- Prevent reprocessing
    [150_Persistent_Staging_Area].dbo.PSA_PROFILER_OFFER HSTG
    ON
       HSTG.OfferID = STG.OfferID AND
       HSTG.SOURCE_ROW_ID = STG.SOURCE_ROW_ID AND
       HSTG.LOAD_DATETIME=STG.LOAD_DATETIME
  LEFT OUTER JOIN -- max HSTG checksum
  (
    SELECT
      A.OfferID,
      A.SOURCE_ROW_ID,
      A.HASH_FULL_RECORD AS LKP_HASH_FULL_RECORD,
      A.CDC_OPERATION AS LKP_CDC_OPERATION
    FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_OFFER A
    JOIN (
      SELECT 
        B.OfferID,
        B.SOURCE_ROW_ID,
        MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME 
      FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_OFFER B
      GROUP BY 
       B.OfferID,
       B.SOURCE_ROW_ID
      ) C ON
        A.OfferID = C.OfferID AND
        A.SOURCE_ROW_ID = C.SOURCE_ROW_ID AND
        A.LOAD_DATETIME = C.MAX_LOAD_DATETIME
  ) maxsub ON
     STG.OfferID = maxsub.OfferID AND
     STG.SOURCE_ROW_ID = maxsub.SOURCE_ROW_ID 
  WHERE
    HSTG.OfferID IS NULL -- prevent reprocessing
) sub
WHERE
(
  KEY_ROW_NUMBER=1
  AND
  (
    (HASH_FULL_RECORD!=LKP_HASH_FULL_RECORD)
    -- The checksums are different
    OR
    (HASH_FULL_RECORD=LKP_HASH_FULL_RECORD AND CDC_OPERATION!=LKP_CDC_OPERATION)
    -- The checksums are the same but the CDC is different
    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change
  )
)
OR
(
  -- It's not the most recent change in the set, so the record can be inserted as-is
  KEY_ROW_NUMBER!=1
)

GO

--
-- PSA View definition for PSA_PROFILER_PERSONALISED_COSTING
-- Generated at 11/21/2014 11:52:15 AM
--

USE [100_Staging_Area]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[PSA_PROFILER_PERSONALISED_COSTING]') AND type in (N'V'))
DROP VIEW [vedw].[PSA_PROFILER_PERSONALISED_COSTING];
GO
CREATE VIEW [vedw].[PSA_PROFILER_PERSONALISED_COSTING] AS 
SELECT 
  PSA_PROFILER_PERSONALISED_COSTING_HSH,
  LOAD_DATETIME,
  EVENT_DATETIME,
  RECORD_SOURCE,
  SOURCE_ROW_ID,
  CDC_OPERATION,
  HASH_FULL_RECORD,
  Member,
  Segment,
  Plan_Code,
  Date_effective,
  Monthly_Cost,
  LKP_HASH_FULL_RECORD,
  LKP_CDC_OPERATION,
  KEY_ROW_NUMBER
FROM
(
  SELECT
    HASHBYTES('MD5',
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Member)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Segment)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Plan_Code)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Date_effective)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.SOURCE_ROW_ID)),'NA')+'|'+
       CONVERT(NVARCHAR(100),STG.[LOAD_DATETIME],126)+'|'
    ) AS PSA_PROFILER_PERSONALISED_COSTING_HSH,
    STG.LOAD_DATETIME,
    STG.EVENT_DATETIME,
    STG.RECORD_SOURCE,
    STG.SOURCE_ROW_ID,
    STG.CDC_OPERATION,
    STG.HASH_FULL_RECORD,
    STG.Member,
    STG.Segment,
    STG.Plan_Code,
    STG.Date_effective,
    STG.Monthly_Cost,
    COALESCE(maxsub.LKP_HASH_FULL_RECORD,'N/A') AS LKP_HASH_FULL_RECORD,
    COALESCE(maxsub.LKP_CDC_OPERATION,'N/A') AS LKP_CDC_OPERATION,
    CAST(ROW_NUMBER() OVER (PARTITION  BY 
       STG.Member,       STG.Segment,       STG.Plan_Code,       STG.Date_effective,       STG.SOURCE_ROW_ID
    ORDER BY 
       STG.Member,        STG.Segment,        STG.Plan_Code,        STG.Date_effective,        STG.SOURCE_ROW_ID, STG.LOAD_DATETIME) AS INT) AS KEY_ROW_NUMBER
  FROM [dbo].STG_PROFILER_PERSONALISED_COSTING STG
  LEFT OUTER JOIN -- Prevent reprocessing
    [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PERSONALISED_COSTING HSTG
    ON
       HSTG.Member = STG.Member AND
       HSTG.Segment = STG.Segment AND
       HSTG.Plan_Code = STG.Plan_Code AND
       HSTG.Date_effective = STG.Date_effective AND
       HSTG.SOURCE_ROW_ID = STG.SOURCE_ROW_ID AND
       HSTG.LOAD_DATETIME=STG.LOAD_DATETIME
  LEFT OUTER JOIN -- max HSTG checksum
  (
    SELECT
      A.Member,
      A.Segment,
      A.Plan_Code,
      A.Date_effective,
      A.SOURCE_ROW_ID,
      A.HASH_FULL_RECORD AS LKP_HASH_FULL_RECORD,
      A.CDC_OPERATION AS LKP_CDC_OPERATION
    FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PERSONALISED_COSTING A
    JOIN (
      SELECT 
        B.Member,
        B.Segment,
        B.Plan_Code,
        B.Date_effective,
        B.SOURCE_ROW_ID,
        MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME 
      FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PERSONALISED_COSTING B
      GROUP BY 
       B.Member,
       B.Segment,
       B.Plan_Code,
       B.Date_effective,
       B.SOURCE_ROW_ID
      ) C ON
        A.Member = C.Member AND
        A.Segment = C.Segment AND
        A.Plan_Code = C.Plan_Code AND
        A.Date_effective = C.Date_effective AND
        A.SOURCE_ROW_ID = C.SOURCE_ROW_ID AND
        A.LOAD_DATETIME = C.MAX_LOAD_DATETIME
  ) maxsub ON
     STG.Member = maxsub.Member AND
     STG.Segment = maxsub.Segment AND
     STG.Plan_Code = maxsub.Plan_Code AND
     STG.Date_effective = maxsub.Date_effective AND
     STG.SOURCE_ROW_ID = maxsub.SOURCE_ROW_ID 
  WHERE
    HSTG.Member IS NULL -- prevent reprocessing
) sub
WHERE
(
  KEY_ROW_NUMBER=1
  AND
  (
    (HASH_FULL_RECORD!=LKP_HASH_FULL_RECORD)
    -- The checksums are different
    OR
    (HASH_FULL_RECORD=LKP_HASH_FULL_RECORD AND CDC_OPERATION!=LKP_CDC_OPERATION)
    -- The checksums are the same but the CDC is different
    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change
  )
)
OR
(
  -- It's not the most recent change in the set, so the record can be inserted as-is
  KEY_ROW_NUMBER!=1
)

GO

--
-- PSA View definition for PSA_PROFILER_PLAN
-- Generated at 11/21/2014 11:52:15 AM
--

USE [100_Staging_Area]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[PSA_PROFILER_PLAN]') AND type in (N'V'))
DROP VIEW [vedw].[PSA_PROFILER_PLAN];
GO
CREATE VIEW [vedw].[PSA_PROFILER_PLAN] AS 
SELECT 
  PSA_PROFILER_PLAN_HSH,
  LOAD_DATETIME,
  EVENT_DATETIME,
  RECORD_SOURCE,
  SOURCE_ROW_ID,
  CDC_OPERATION,
  HASH_FULL_RECORD,
  Plan_Code,
  Plan_Desc,
  Renewal_Plan_Code,
  LKP_HASH_FULL_RECORD,
  LKP_CDC_OPERATION,
  KEY_ROW_NUMBER
FROM
(
  SELECT
    HASHBYTES('MD5',
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Plan_Code)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.SOURCE_ROW_ID)),'NA')+'|'+
       CONVERT(NVARCHAR(100),STG.[LOAD_DATETIME],126)+'|'
    ) AS PSA_PROFILER_PLAN_HSH,
    STG.LOAD_DATETIME,
    STG.EVENT_DATETIME,
    STG.RECORD_SOURCE,
    STG.SOURCE_ROW_ID,
    STG.CDC_OPERATION,
    STG.HASH_FULL_RECORD,
    STG.Plan_Code,
    STG.Plan_Desc,
    STG.Renewal_Plan_Code,
    COALESCE(maxsub.LKP_HASH_FULL_RECORD,'N/A') AS LKP_HASH_FULL_RECORD,
    COALESCE(maxsub.LKP_CDC_OPERATION,'N/A') AS LKP_CDC_OPERATION,
    CAST(ROW_NUMBER() OVER (PARTITION  BY 
       STG.Plan_Code,       STG.SOURCE_ROW_ID
    ORDER BY 
       STG.Plan_Code,        STG.SOURCE_ROW_ID, STG.LOAD_DATETIME) AS INT) AS KEY_ROW_NUMBER
  FROM [dbo].STG_PROFILER_PLAN STG
  LEFT OUTER JOIN -- Prevent reprocessing
    [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PLAN HSTG
    ON
       HSTG.Plan_Code = STG.Plan_Code AND
       HSTG.SOURCE_ROW_ID = STG.SOURCE_ROW_ID AND
       HSTG.LOAD_DATETIME=STG.LOAD_DATETIME
  LEFT OUTER JOIN -- max HSTG checksum
  (
    SELECT
      A.Plan_Code,
      A.SOURCE_ROW_ID,
      A.HASH_FULL_RECORD AS LKP_HASH_FULL_RECORD,
      A.CDC_OPERATION AS LKP_CDC_OPERATION
    FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PLAN A
    JOIN (
      SELECT 
        B.Plan_Code,
        B.SOURCE_ROW_ID,
        MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME 
      FROM [150_Persistent_Staging_Area].dbo.PSA_PROFILER_PLAN B
      GROUP BY 
       B.Plan_Code,
       B.SOURCE_ROW_ID
      ) C ON
        A.Plan_Code = C.Plan_Code AND
        A.SOURCE_ROW_ID = C.SOURCE_ROW_ID AND
        A.LOAD_DATETIME = C.MAX_LOAD_DATETIME
  ) maxsub ON
     STG.Plan_Code = maxsub.Plan_Code AND
     STG.SOURCE_ROW_ID = maxsub.SOURCE_ROW_ID 
  WHERE
    HSTG.Plan_Code IS NULL -- prevent reprocessing
) sub
WHERE
(
  KEY_ROW_NUMBER=1
  AND
  (
    (HASH_FULL_RECORD!=LKP_HASH_FULL_RECORD)
    -- The checksums are different
    OR
    (HASH_FULL_RECORD=LKP_HASH_FULL_RECORD AND CDC_OPERATION!=LKP_CDC_OPERATION)
    -- The checksums are the same but the CDC is different
    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change
  )
)
OR
(
  -- It's not the most recent change in the set, so the record can be inserted as-is
  KEY_ROW_NUMBER!=1
)

GO

--
-- PSA View definition for PSA_USERMANAGED_SEGMENT
-- Generated at 11/21/2014 11:52:15 AM
--

USE [100_Staging_Area]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[PSA_USERMANAGED_SEGMENT]') AND type in (N'V'))
DROP VIEW [vedw].[PSA_USERMANAGED_SEGMENT];
GO
CREATE VIEW [vedw].[PSA_USERMANAGED_SEGMENT] AS 
SELECT 
  PSA_USERMANAGED_SEGMENT_HSH,
  LOAD_DATETIME,
  EVENT_DATETIME,
  RECORD_SOURCE,
  SOURCE_ROW_ID,
  CDC_OPERATION,
  HASH_FULL_RECORD,
  Demographic_Segment_Code,
  Demographic_Segment_Description,
  LKP_HASH_FULL_RECORD,
  LKP_CDC_OPERATION,
  KEY_ROW_NUMBER
FROM
(
  SELECT
    HASHBYTES('MD5',
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.SOURCE_ROW_ID)),'NA')+'|'+
       ISNULL(RTRIM(CONVERT(NVARCHAR(100),STG.Demographic_Segment_Code)),'NA')+'|'+
       CONVERT(NVARCHAR(100),STG.[LOAD_DATETIME],126)+'|'
    ) AS PSA_USERMANAGED_SEGMENT_HSH,
    STG.LOAD_DATETIME,
    STG.EVENT_DATETIME,
    STG.RECORD_SOURCE,
    STG.SOURCE_ROW_ID,
    STG.CDC_OPERATION,
    STG.HASH_FULL_RECORD,
    STG.Demographic_Segment_Code,
    STG.Demographic_Segment_Description,
    COALESCE(maxsub.LKP_HASH_FULL_RECORD,'N/A') AS LKP_HASH_FULL_RECORD,
    COALESCE(maxsub.LKP_CDC_OPERATION,'N/A') AS LKP_CDC_OPERATION,
    CAST(ROW_NUMBER() OVER (PARTITION  BY 
       STG.SOURCE_ROW_ID,       STG.Demographic_Segment_Code
    ORDER BY 
       STG.SOURCE_ROW_ID,        STG.Demographic_Segment_Code, STG.LOAD_DATETIME) AS INT) AS KEY_ROW_NUMBER
  FROM [dbo].STG_USERMANAGED_SEGMENT STG
  LEFT OUTER JOIN -- Prevent reprocessing
    [150_Persistent_Staging_Area].dbo.PSA_USERMANAGED_SEGMENT HSTG
    ON
       HSTG.SOURCE_ROW_ID = STG.SOURCE_ROW_ID AND
       HSTG.Demographic_Segment_Code = STG.Demographic_Segment_Code AND
       HSTG.LOAD_DATETIME=STG.LOAD_DATETIME
  LEFT OUTER JOIN -- max HSTG checksum
  (
    SELECT
      A.SOURCE_ROW_ID,
      A.Demographic_Segment_Code,
      A.HASH_FULL_RECORD AS LKP_HASH_FULL_RECORD,
      A.CDC_OPERATION AS LKP_CDC_OPERATION
    FROM [150_Persistent_Staging_Area].dbo.PSA_USERMANAGED_SEGMENT A
    JOIN (
      SELECT 
        B.SOURCE_ROW_ID,
        B.Demographic_Segment_Code,
        MAX(LOAD_DATETIME) AS MAX_LOAD_DATETIME 
      FROM [150_Persistent_Staging_Area].dbo.PSA_USERMANAGED_SEGMENT B
      GROUP BY 
       B.SOURCE_ROW_ID,
       B.Demographic_Segment_Code
      ) C ON
        A.SOURCE_ROW_ID = C.SOURCE_ROW_ID AND
        A.Demographic_Segment_Code = C.Demographic_Segment_Code AND
        A.LOAD_DATETIME = C.MAX_LOAD_DATETIME
  ) maxsub ON
     STG.SOURCE_ROW_ID = maxsub.SOURCE_ROW_ID AND
     STG.Demographic_Segment_Code = maxsub.Demographic_Segment_Code 
  WHERE
    HSTG.SOURCE_ROW_ID IS NULL -- prevent reprocessing
) sub
WHERE
(
  KEY_ROW_NUMBER=1
  AND
  (
    (HASH_FULL_RECORD!=LKP_HASH_FULL_RECORD)
    -- The checksums are different
    OR
    (HASH_FULL_RECORD=LKP_HASH_FULL_RECORD AND CDC_OPERATION!=LKP_CDC_OPERATION)
    -- The checksums are the same but the CDC is different
    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change
  )
)
OR
(
  -- It's not the most recent change in the set, so the record can be inserted as-is
  KEY_ROW_NUMBER!=1
)

GO



--
-- Hub View definition for HUB_CUSTOMER
-- Generated at 10/07/2019 1:18:43 PM
--

USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[HUB_CUSTOMER]') AND type in (N'V'))
DROP VIEW [vedw].[HUB_CUSTOMER]
GO
CREATE VIEW [vedw].[HUB_CUSTOMER] AS  
SELECT hub.*
FROM(  
SELECT
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CUSTOMER_ID])),'NA')+'|'
  ) AS CUSTOMER_HSH,
  -1 AS ETL_INSERT_RUN_ID,
  MIN(LOAD_DATETIME) AS LOAD_DATETIME,
  RECORD_SOURCE,
    CONVERT(NVARCHAR(100),[CUSTOMER_ID]) AS CUSTOMER_ID,
  ROW_NUMBER() OVER (PARTITION  BY
      [CUSTOMER_ID]
  ORDER BY 
      MIN(LOAD_DATETIME)
  ) AS ROW_NR
FROM
(
  SELECT 
        CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_CUSTOMER_PERSONAL
  WHERE
     [CustomerID] IS NOT NULL 
    AND 1=1
  GROUP BY 
        [CustomerID],
    RECORD_SOURCE
UNION
  SELECT 
        CAST([Member] AS NVARCHAR(100)) AS [CUSTOMER_ID],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_PERSONALISED_COSTING
  WHERE
     [Member] IS NOT NULL 
  GROUP BY 
        [Member],
    RECORD_SOURCE
UNION
  SELECT 
        CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_CUST_MEMBERSHIP
  WHERE
     [CustomerID] IS NOT NULL 
    AND 15=15
  GROUP BY 
        [CustomerID],
    RECORD_SOURCE
UNION
  SELECT 
        CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_CUSTOMER_OFFER
  WHERE
     [CustomerID] IS NOT NULL 
    AND 5=5
  GROUP BY 
        [CustomerID],
    RECORD_SOURCE
) HUB_selection
GROUP BY
  [CUSTOMER_ID],
  RECORD_SOURCE
) hub
WHERE ROW_NR = 1
UNION
SELECT 0x00000000000000000000000000000000,
- 1,
'1900-01-01',
'Data Warehouse',
'Unknown',
1 AS ROW_NR

GO

--
-- Hub View definition for HUB_INCENTIVE_OFFER
-- Generated at 10/07/2019 1:18:49 PM
--

USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[HUB_INCENTIVE_OFFER]') AND type in (N'V'))
DROP VIEW [vedw].[HUB_INCENTIVE_OFFER]
GO
CREATE VIEW [vedw].[HUB_INCENTIVE_OFFER] AS  
SELECT hub.*
FROM(
SELECT
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),[OFFER_ID])),'NA')+'|'
  ) AS INCENTIVE_OFFER_HSH,
  -1 AS ETL_INSERT_RUN_ID,
  MIN(LOAD_DATETIME) AS LOAD_DATETIME,
  RECORD_SOURCE,
    CONVERT(NVARCHAR(100),[OFFER_ID]) AS OFFER_ID,
  ROW_NUMBER() OVER (PARTITION  BY
      [OFFER_ID]
  ORDER BY 
      MIN(LOAD_DATETIME)
  ) AS ROW_NR
FROM
(
  SELECT 
        CAST([OfferID] AS NVARCHAR(100)) AS [OFFER_ID],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_OFFER
  WHERE
     [OfferID] IS NOT NULL 
    AND 3=3
  GROUP BY 
        [OfferID],
    RECORD_SOURCE
UNION
  SELECT 
        CAST([OfferID] AS NVARCHAR(100)) AS [OFFER_ID],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_CUSTOMER_OFFER
  WHERE
     [OfferID] IS NOT NULL 
    AND 6=6
  GROUP BY 
        [OfferID],
    RECORD_SOURCE
) HUB_selection
GROUP BY
  [OFFER_ID],
  RECORD_SOURCE
) hub
WHERE ROW_NR = 1
UNION
SELECT 0x00000000000000000000000000000000,
- 1,
'1900-01-01',
'Data Warehouse',
'Unknown',
1 AS ROW_NR

GO

--
-- Hub View definition for HUB_MEMBERSHIP_PLAN
-- Generated at 10/07/2019 1:18:51 PM
--

USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[HUB_MEMBERSHIP_PLAN]') AND type in (N'V'))
DROP VIEW [vedw].[HUB_MEMBERSHIP_PLAN]
GO
CREATE VIEW [vedw].[HUB_MEMBERSHIP_PLAN] AS  
SELECT hub.*
FROM(
SELECT
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),[PLAN_CODE])),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),[PLAN_SUFFIX])),'NA')+'|'
  ) AS MEMBERSHIP_PLAN_HSH,
  -1 AS ETL_INSERT_RUN_ID,
  MIN(LOAD_DATETIME) AS LOAD_DATETIME,
  RECORD_SOURCE,
    CONVERT(NVARCHAR(100),[PLAN_CODE]) AS PLAN_CODE,
    CONVERT(NVARCHAR(100),[PLAN_SUFFIX]) AS PLAN_SUFFIX,
  ROW_NUMBER() OVER (PARTITION  BY
      [PLAN_CODE],
      [PLAN_SUFFIX]
  ORDER BY 
      MIN(LOAD_DATETIME)
  ) AS ROW_NR
FROM
(
  SELECT 
          [Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_PLAN
  WHERE
     [Plan_Code] IS NOT NULL 
    AND 10=10
  GROUP BY 
        [Plan_Code],
    RECORD_SOURCE
UNION
  SELECT 
          [Renewal_Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_PLAN
  WHERE
     [Renewal_Plan_Code] IS NOT NULL 
    AND 1=1
  GROUP BY 
        [Renewal_Plan_Code],
    RECORD_SOURCE
UNION
  SELECT 
          [Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_PERSONALISED_COSTING
  WHERE
     [Plan_Code] IS NOT NULL 
    AND 18=18
  GROUP BY 
        [Plan_Code],
    RECORD_SOURCE
UNION
  SELECT 
          [Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_ESTIMATED_WORTH
  WHERE
     [Plan_Code] IS NOT NULL 
    AND 12=12
  GROUP BY 
        [Plan_Code],
    RECORD_SOURCE
UNION
  SELECT 
          [Plan_Code] AS [PLAN_CODE],
    'XYZ' AS [PLAN_SUFFIX],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_CUST_MEMBERSHIP
  WHERE
     [Plan_Code] IS NOT NULL 
    AND 14=14
  GROUP BY 
        [Plan_Code],
    RECORD_SOURCE
) HUB_selection
GROUP BY
  [PLAN_CODE],
  [PLAN_SUFFIX],
  RECORD_SOURCE
) hub
WHERE ROW_NR = 1
UNION
SELECT 0x00000000000000000000000000000000,
- 1,
'1900-01-01',
'Data Warehouse',
'Unknown',
'Unknown',
1 AS ROW_NR

GO

--
-- Hub View definition for HUB_SEGMENT
-- Generated at 10/07/2019 1:19:02 PM
--

USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[HUB_SEGMENT]') AND type in (N'V'))
DROP VIEW [vedw].[HUB_SEGMENT]
GO
CREATE VIEW [vedw].[HUB_SEGMENT] AS  
SELECT hub.*
FROM(
SELECT
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),[SEGMENT_CODE])),'NA')+'|'
  ) AS SEGMENT_HSH,
  -1 AS ETL_INSERT_RUN_ID,
  MIN(LOAD_DATETIME) AS LOAD_DATETIME,
  RECORD_SOURCE,
    CONVERT(NVARCHAR(100),[SEGMENT_CODE]) AS SEGMENT_CODE,
  ROW_NUMBER() OVER (PARTITION  BY
      [SEGMENT_CODE]
  ORDER BY 
      MIN(LOAD_DATETIME)
  ) AS ROW_NR
FROM
(
  SELECT 
    ISNULL([Segment], '') +  'TEST'  AS [SEGMENT_CODE],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_PROFILER_PERSONALISED_COSTING
  WHERE
    ISNULL([Segment], '') +  'TEST'  != '' 
  GROUP BY 
    ISNULL([Segment], '') +  'TEST' ,
    RECORD_SOURCE
UNION
  SELECT 
    ISNULL([Demographic_Segment_Code], '') +  'TEST'  AS [SEGMENT_CODE],
       RECORD_SOURCE,
       MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM dbo.PSA_USERMANAGED_SEGMENT
  WHERE
    ISNULL([Demographic_Segment_Code], '') +  'TEST'  != '' 
    AND 8=8
  GROUP BY 
    ISNULL([Demographic_Segment_Code], '') +  'TEST' ,
    RECORD_SOURCE
) HUB_selection
GROUP BY
  [SEGMENT_CODE],
  RECORD_SOURCE
) hub
WHERE ROW_NR = 1
UNION
SELECT 0x00000000000000000000000000000000,
- 1,
'1900-01-01',
'Data Warehouse',
'Unknown',
1 AS ROW_NR

GO

--
-- Link Satellite View definition - regular history for LSAT_CUSTOMER_COSTING
-- Generated at 17/07/2019 1:01:43 PM
--

USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[LSAT_CUSTOMER_COSTING]') AND type in (N'V'))
DROP VIEW [vedw].[LSAT_CUSTOMER_COSTING]

GO

CREATE VIEW [vedw].[LSAT_CUSTOMER_COSTING] AS  
SELECT
   HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),SEGMENT_CODE3)),'NA')+'|'
  ) AS CUSTOMER_COSTING_HSH,
   LOAD_DATETIME AS LOAD_DATETIME,
          [COSTING_EFFECTIVE_DATE],
   COALESCE ( LEAD ( [LOAD_DATETIME] ) OVER
   		     (PARTITION BY 
              PLAN_CODE1,
              PLAN_SUFFIX1,
              CUSTOMER_ID2,
              SEGMENT_CODE3,
              COSTING_EFFECTIVE_DATE
   		     ORDER BY [LOAD_DATETIME]),
   CAST( '9999-12-31' AS DATETIME)) AS LOAD_END_DATETIME,
   CASE
      WHEN ( RANK() OVER (PARTITION BY 
            PLAN_CODE1,
            PLAN_SUFFIX1,
            CUSTOMER_ID2,
            SEGMENT_CODE3,
         COSTING_EFFECTIVE_DATE
          ORDER BY [LOAD_DATETIME] desc )) = 1
      THEN 'Y'
      ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   [RECORD_SOURCE],
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   -1 AS ETL_INSERT_RUN_ID,
   -1 AS ETL_UPDATE_RUN_ID,
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   PERSONAL_MONTHLY_COST,
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
         PLAN_CODE1,
         PLAN_SUFFIX1,
         CUSTOMER_ID2,
         SEGMENT_CODE3,
         COSTING_EFFECTIVE_DATE
      ORDER BY 
         PLAN_CODE1,
         PLAN_SUFFIX1,
         CUSTOMER_ID2,
         SEGMENT_CODE3,
         COSTING_EFFECTIVE_DATE,
         [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER,
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CDC_OPERATION])),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE1)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX1)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID2)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),SEGMENT_CODE3)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),COSTING_EFFECTIVE_DATE)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),PERSONAL_MONTHLY_COST)),'NA')+'|'
   ) AS HASH_FULL_RECORD
FROM 
(
  SELECT DISTINCT
    [LOAD_DATETIME],
    [RECORD_SOURCE],
    [SOURCE_ROW_ID],
    [CDC_OPERATION],
      [Plan_Code] AS [PLAN_CODE1],
    'XYZ' AS [PLAN_SUFFIX1],
    CAST([Member] AS NVARCHAR(100)) AS [CUSTOMER_ID2],
ISNULL([Segment], '') +  'TEST'  AS [SEGMENT_CODE3]
    ,[Date_effective] AS [COSTING_EFFECTIVE_DATE],
    Monthly_Cost AS PERSONAL_MONTHLY_COST
  FROM dbo.PSA_PROFILER_PERSONALISED_COSTING
  UNION
  SELECT DISTINCT
    '1900-01-01' AS [LOAD_DATETIME],
    'Data Warehouse' AS [RECORD_SOURCE],
     0 AS [SOURCE_ROW_ID],
    'N/A' AS [CDC_OPERATION],
      [Plan_Code] AS [PLAN_CODE1],
    'XYZ' AS [PLAN_SUFFIX1],
    CAST([Member] AS NVARCHAR(100)) AS [CUSTOMER_ID2],
ISNULL([Segment], '') +  'TEST'  AS [SEGMENT_CODE3],
    Date_effective AS [COSTING_EFFECTIVE_DATE],
    NULL AS [PERSONAL_MONTHLY_COST]
  FROM dbo.PSA_PROFILER_PERSONALISED_COSTING
) sub

--
-- Link Satellite View definition - regular history for LSAT_MEMBERSHIP
-- Generated at 17/07/2019 1:01:46 PM
--
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[LSAT_MEMBERSHIP]') AND type in (N'V'))
DROP VIEW [vedw].[LSAT_MEMBERSHIP]
go
CREATE VIEW [vedw].[LSAT_MEMBERSHIP] AS  
SELECT
   HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),SALES_CHANNEL)),'NA')+'|'
  ) AS MEMBERSHIP_HSH,
   LOAD_DATETIME AS LOAD_DATETIME,
   COALESCE ( LEAD ( [LOAD_DATETIME] ) OVER
   		     (PARTITION BY 
              CUSTOMER_ID1,
              PLAN_CODE2,
              PLAN_SUFFIX2,
              SALES_CHANNEL
   		     ORDER BY [LOAD_DATETIME]),
   CAST( '9999-12-31' AS DATETIME)) AS LOAD_END_DATETIME,
   CASE
      WHEN ( RANK() OVER (PARTITION BY 
            CUSTOMER_ID1,
            PLAN_CODE2,
            PLAN_SUFFIX2,
            SALES_CHANNEL
          ORDER BY [LOAD_DATETIME] desc )) = 1
      THEN 'Y'
      ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   [RECORD_SOURCE],
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   -1 AS ETL_INSERT_RUN_ID,
   -1 AS ETL_UPDATE_RUN_ID,
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   MEMBERSHIP_END_DATE,
   MEMBERSHIP_START_DATE,
   MEMBERSHIP_STATUS,
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
         CUSTOMER_ID1,
         PLAN_CODE2,
         PLAN_SUFFIX2,
         SALES_CHANNEL
      ORDER BY 
         CUSTOMER_ID1,
         PLAN_CODE2,
         PLAN_SUFFIX2,
         SALES_CHANNEL,
         [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER,
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CDC_OPERATION])),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID1)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE2)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX2)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),MEMBERSHIP_END_DATE)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),MEMBERSHIP_START_DATE)),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),MEMBERSHIP_STATUS)),'NA')+'|'
   ) AS HASH_FULL_RECORD
FROM 
(
  SELECT DISTINCT
    [LOAD_DATETIME],
    [RECORD_SOURCE],
    [SOURCE_ROW_ID],
    [CDC_OPERATION],
    CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID1],
      [Plan_Code] AS [PLAN_CODE2],
    'XYZ' AS [PLAN_SUFFIX2]
    ,[Status] AS [SALES_CHANNEL],
    End_Date AS MEMBERSHIP_END_DATE,
    Start_Date AS MEMBERSHIP_START_DATE,
    Status AS MEMBERSHIP_STATUS
  FROM dbo.PSA_PROFILER_CUST_MEMBERSHIP
  WHERE 17=17
  UNION
  SELECT DISTINCT
    '1900-01-01' AS [LOAD_DATETIME],
    'Data Warehouse' AS [RECORD_SOURCE],
     0 AS [SOURCE_ROW_ID],
    'N/A' AS [CDC_OPERATION],
    CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID1],
      [Plan_Code] AS [PLAN_CODE2],
    'XYZ' AS [PLAN_SUFFIX2],
    [Status] AS [SALES_CHANNEL],
    NULL AS [MEMBERSHIP_END_DATE],
    NULL AS [MEMBERSHIP_START_DATE],
    NULL AS [MEMBERSHIP_STATUS]
  FROM dbo.PSA_PROFILER_CUST_MEMBERSHIP
) sub

--
-- Link Satellite View definition - Driving Key for LSAT_CUSTOMER_OFFER
-- Generated at 17/07/2019 1:01:52 PM
--


GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[LSAT_CUSTOMER_OFFER]') AND type in (N'V'))
DROP VIEW [vedw].[LSAT_CUSTOMER_OFFER]
go
CREATE VIEW [vedw].[LSAT_CUSTOMER_OFFER] AS  
SELECT
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CUSTOMER_ID1])),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[OFFER_ID2])),'NA')+'|'
   ) AS CUSTOMER_OFFER_HSH,
   --[CUSTOMER_ID1],
   --[OFFER_ID2],
   LOAD_DATETIME AS LOAD_DATETIME,
   COALESCE ( LEAD ( LOAD_DATETIME ) OVER
      (PARTITION BY 
       [CUSTOMER_ID1]
   	  ORDER BY LOAD_DATETIME),
      CAST( '9999-12-31' AS DATETIME)
   ) AS LOAD_END_DATETIME,
   CASE 
     WHEN ( LEAD ( LOAD_DATETIME ) OVER
      (PARTITION BY 
       [CUSTOMER_ID1]
   	  ORDER BY LOAD_DATETIME)
      ) IS NULL
     THEN 'Y' ELSE 'N'
   END AS CURRENT_RECORD_INDICATOR,
   [RECORD_SOURCE],
    CASE
      WHEN [CDC_OPERATION] = 'Delete' THEN 'Y'
      ELSE 'N'
    END AS [DELETED_RECORD_INDICATOR],
   [SOURCE_ROW_ID],
   [CDC_OPERATION],
   CAST(
      ROW_NUMBER() OVER (PARTITION  BY 
          [CUSTOMER_ID1]
         ORDER BY 
          [CUSTOMER_ID1],
          [LOAD_DATETIME]) AS INT)
   AS ROW_NUMBER,
   HASHBYTES('MD5',
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CDC_OPERATION])),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[CUSTOMER_ID1])),'NA')+'|'+
      ISNULL(RTRIM(CONVERT(NVARCHAR(100),[OFFER_ID2])),'NA')+'|'
   ) AS HASH_FULL_RECORD
FROM 
(
  SELECT 
    [LOAD_DATETIME],
    [RECORD_SOURCE],
    [SOURCE_ROW_ID],
    [CDC_OPERATION],
        [CustomerID] AS [CUSTOMER_ID1],
    [OfferID] AS [OFFER_ID2],
    LAG([OfferID], 1, '0') OVER (PARTITION BY [CustomerID] ORDER BY LOAD_DATETIME) AS PREVIOUS_FOLLOWER_KEY1
  FROM dbo.PSA_PROFILER_CUSTOMER_OFFER
  WHERE 7=7
  AND NOT (SOURCE_ROW_ID>1 AND CDC_OPERATION = 'Delete')
  UNION
  SELECT 
    [LOAD_DATETIME],
    [RECORD_SOURCE],
    [SOURCE_ROW_ID],
    [CDC_OPERATION],
    [CUSTOMER_ID1],
    [OFFER_ID2],
    '0' AS PREVIOUS_FOLLOWER_KEY0
  FROM (
    SELECT
    '1900-01-01' AS [LOAD_DATETIME],
    'Data Warehouse' AS [RECORD_SOURCE],
    0 AS [SOURCE_ROW_ID],
    'N/A' AS [CDC_OPERATION],
        [CustomerID] AS [CUSTOMER_ID1],
    [OfferID] AS [OFFER_ID2],
    DENSE_RANK() OVER (PARTITION  BY 
        [CustomerID]
    ORDER BY [LOAD_DATETIME], [CustomerID] ASC) ROWVERSION
  FROM dbo.PSA_PROFILER_CUSTOMER_OFFER
  ) dummysub
  WHERE ROWVERSION=1
) sub
WHERE OFFER_ID2 != PREVIOUS_FOLLOWER_KEY1
-- ORDER BY
--  CUSTOMER_ID1,
--  [LOAD_DATETIME]

--
-- Link View definition for LNK_CUSTOMER_COSTING
-- Generated at 17/07/2019 1:00:52 PM
--
GO
USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[LNK_CUSTOMER_COSTING]') AND type in (N'V'))
DROP VIEW [vedw].[LNK_CUSTOMER_COSTING]
GO

CREATE VIEW [vedw].[LNK_CUSTOMER_COSTING] AS  
SELECT link.*
FROM
(
SELECT
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),SEGMENT_CODE3)),'NA')+'|'
  ) AS CUSTOMER_COSTING_HSH,
  -1 AS ETL_INSERT_RUN_ID,
  MIN(LOAD_DATETIME) AS LOAD_DATETIME,
  RECORD_SOURCE,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX1)),'NA')+'|'
  ) AS MEMBERSHIP_PLAN_HSH,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID2)),'NA')+'|'                                       
  ) AS CUSTOMER_HSH,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),SEGMENT_CODE3)),'NA')+'|'
  ) AS SEGMENT_HSH,
  ROW_NUMBER() OVER (PARTITION  BY
      [PLAN_CODE1],
      [PLAN_SUFFIX1],
      [CUSTOMER_ID2],
      [SEGMENT_CODE3]
  ORDER BY 
      MIN(LOAD_DATETIME)
  ) AS ROW_NR
FROM
(


  SELECT 
    [Plan_Code] AS [PLAN_CODE1],
    'XYZ' AS [PLAN_SUFFIX1],
    CAST([Member] AS NVARCHAR(100)) AS [CUSTOMER_ID2],
   ISNULL([Segment], '') +  'TEST'  AS [SEGMENT_CODE3],     RECORD_SOURCE,
    MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM [150_Persistent_Staging_Area].[dbo].[PSA_PROFILER_PERSONALISED_COSTING]
  WHERE
  [Plan_Code] IS NOT NULL AND
 [Member] IS NOT NULL AND
  ISNULL([Segment], '') +  'TEST'  != '' 
  GROUP BY 
    [Plan_Code],
    [Member],
ISNULL([Segment], '') +  'TEST' ,     RECORD_SOURCE



) LINK_selection
GROUP BY
  [PLAN_CODE1],
  [PLAN_SUFFIX1],
  [CUSTOMER_ID2],
  [SEGMENT_CODE3],
  [RECORD_SOURCE]
) link
WHERE ROW_NR=1

--
-- Link View definition for LNK_CUSTOMER_OFFER
-- Generated at 17/07/2019 1:01:00 PM
--
GO
USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[LNK_CUSTOMER_OFFER]') AND type in (N'V'))
DROP VIEW [vedw].[LNK_CUSTOMER_OFFER]
GO

CREATE VIEW [vedw].[LNK_CUSTOMER_OFFER] AS  
SELECT link.*
FROM
(
SELECT
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),OFFER_ID2)),'NA')+'|'
  ) AS CUSTOMER_OFFER_HSH,
  -1 AS ETL_INSERT_RUN_ID,
  MIN(LOAD_DATETIME) AS LOAD_DATETIME,
  RECORD_SOURCE,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID1)),'NA')+'|'
  ) AS CUSTOMER_HSH,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),OFFER_ID2)),'NA')+'|'
  ) AS INCENTIVE_OFFER_HSH,
  ROW_NUMBER() OVER (PARTITION BY
      [CUSTOMER_ID1],
      [OFFER_ID2]
  ORDER BY 
      MIN(LOAD_DATETIME)
  ) AS ROW_NR
FROM
(



  SELECT 
    CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID1],
    CAST([OfferID] AS NVARCHAR(100)) AS [OFFER_ID2],
    RECORD_SOURCE,
    MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM [150_Persistent_Staging_Area].[dbo].[PSA_PROFILER_CUSTOMER_OFFER]
  WHERE
    [CustomerID] IS NOT NULL AND
    [OfferID] IS NOT NULL 
    AND 7=7
  GROUP BY 
    [CustomerID],
    [OfferID],     
    RECORD_SOURCE
    
    
    
) LINK_selection
GROUP BY
  [CUSTOMER_ID1],
  [OFFER_ID2],
  [RECORD_SOURCE]
) link
WHERE ROW_NR=1

--
-- Link View definition for LNK_MEMBERSHIP
-- Generated at 17/07/2019 1:01:00 PM
--
GO
USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[LNK_MEMBERSHIP]') AND type in (N'V'))
DROP VIEW [vedw].[LNK_MEMBERSHIP]
GO

CREATE VIEW [vedw].[LNK_MEMBERSHIP] AS  
SELECT link.*
FROM
(
SELECT
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),SALES_CHANNEL)),'NA')+'|'
  ) AS MEMBERSHIP_HSH,
  -1 AS ETL_INSERT_RUN_ID,
  MIN(LOAD_DATETIME) AS LOAD_DATETIME,
  RECORD_SOURCE,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),CUSTOMER_ID1)),'NA')+'|'
  ) AS CUSTOMER_HSH,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX2)),'NA')+'|'
  ) AS MEMBERSHIP_PLAN_HSH,
  SALES_CHANNEL,
  ROW_NUMBER() OVER (PARTITION  BY
      [CUSTOMER_ID1],
      [PLAN_CODE2],
      [PLAN_SUFFIX2],
      [SALES_CHANNEL]
  ORDER BY 
      MIN(LOAD_DATETIME)
  ) AS ROW_NR
FROM
(
  SELECT 
       CAST([CustomerID] AS NVARCHAR(100)) AS [CUSTOMER_ID1],
      [Plan_Code] AS [PLAN_CODE2],
    'XYZ' AS [PLAN_SUFFIX2],     RECORD_SOURCE,
    [Status] AS [SALES_CHANNEL],
    MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM [150_Persistent_Staging_Area].[dbo].[PSA_PROFILER_CUST_MEMBERSHIP]
  WHERE
  [CustomerID] IS NOT NULL AND
 [Plan_Code] IS NOT NULL 
    AND 16=16
  GROUP BY 
    [CustomerID],
    [Plan_Code],     [Status],
    RECORD_SOURCE
) LINK_selection
GROUP BY
  [CUSTOMER_ID1],
  [PLAN_CODE2],
  [PLAN_SUFFIX2],
  [SALES_CHANNEL],
  [RECORD_SOURCE]
) link
WHERE ROW_NR=1

--
-- Link View definition for LNK_RENEWAL_MEMBERSHIP
-- Generated at 17/07/2019 1:01:01 PM
--
GO
USE [150_Persistent_Staging_Area]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[vedw].[LNK_RENEWAL_MEMBERSHIP]') AND type in (N'V'))
DROP VIEW [vedw].[LNK_RENEWAL_MEMBERSHIP]
GO

CREATE VIEW [vedw].[LNK_RENEWAL_MEMBERSHIP] AS  
SELECT link.*
FROM
(
SELECT
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX2)),'NA')+'|'
  ) AS RENEWAL_MEMBERSHIP_HSH,
  -1 AS ETL_INSERT_RUN_ID,
  MIN(LOAD_DATETIME) AS LOAD_DATETIME,
  RECORD_SOURCE,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE1)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX1)),'NA')+'|'
  ) AS MEMBERSHIP_PLAN_HSH,
  HASHBYTES('MD5',
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_CODE2)),'NA')+'|'+
    ISNULL(RTRIM(CONVERT(NVARCHAR(100),PLAN_SUFFIX2)),'NA')+'|'
  ) AS RENEWAL_PLAN_HSH,
  ROW_NUMBER() OVER (PARTITION  BY
      [PLAN_CODE1],
      [PLAN_SUFFIX1],
      [PLAN_CODE2],
      [PLAN_SUFFIX2]
  ORDER BY 
      MIN(LOAD_DATETIME)
  ) AS ROW_NR
FROM
(
  SELECT 
         [Plan_Code] AS [PLAN_CODE1],
    'XYZ' AS [PLAN_SUFFIX1],
      [Renewal_Plan_Code] AS [PLAN_CODE2],
    'XYZ' AS [PLAN_SUFFIX2],     RECORD_SOURCE,
    MIN(LOAD_DATETIME) AS LOAD_DATETIME
  FROM [150_Persistent_Staging_Area].[dbo].[PSA_PROFILER_PLAN]
  WHERE
  [Plan_Code] IS NOT NULL AND
 [Renewal_Plan_Code] IS NOT NULL 
    AND 1=1
  GROUP BY 
    [Plan_Code],
    [Renewal_Plan_Code],     RECORD_SOURCE
) LINK_selection
GROUP BY
  [PLAN_CODE1],
  [PLAN_SUFFIX1],
  [PLAN_CODE2],
  [PLAN_SUFFIX2],
  [RECORD_SOURCE]
) link
WHERE ROW_NR=1

-- 
-- Satellite View definition for SAT_CUSTOMER
-- Generated at 17/07/2019 12:58:27 PM
-- 
GO
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

