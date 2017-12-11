USE [EDW_100_Staging_Area]
GO

INSERT INTO [dbo].[STG_USERMANAGED_SEGMENT]
           ([ETL_INSERT_RUN_ID]
           ,[LOAD_DATETIME]
           ,[EVENT_DATETIME]
           ,[RECORD_SOURCE]
           ,[CDC_OPERATION]
           ,[HASH_FULL_RECORD]
           ,[Demographic_Segment_Code]
           ,[Demographic_Segment_Description])
     VALUES
	 (-1,GETDATE(), GETDATE(), 'Data Warehouse','Insert','N/A', 'LOW', 'Lower SES'),
	 (-1,GETDATE(), GETDATE(), 'Data Warehouse','Insert','N/A', 'MED', 'Medium SES'),
	 (-1,GETDATE(), GETDATE(), 'Data Warehouse','Insert','N/A', 'HIGH','High SES')
