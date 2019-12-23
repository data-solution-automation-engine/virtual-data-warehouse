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


