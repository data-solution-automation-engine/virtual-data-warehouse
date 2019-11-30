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

