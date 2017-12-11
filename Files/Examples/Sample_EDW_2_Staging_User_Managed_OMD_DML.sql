USE [EDW_100_Staging_Area]
GO

INSERT INTO [dbo].[STG_USERMANAGED_SEGMENT]
           ([OMD_INSERT_MODULE_INSTANCE_ID]
           ,[OMD_INSERT_DATETIME]
           ,[OMD_EVENT_DATETIME]
           ,[OMD_RECORD_SOURCE]
           ,[OMD_CDC_OPERATION]
           ,[OMD_HASH_FULL_RECORD]
           ,[Demographic_Segment_Code]
           ,[Demographic_Segment_Description])
     VALUES
	 (-1,GETDATE(), GETDATE(), 'Data Warehouse','Insert','N/A', 'LOW', 'Lower SES'),
	 (-1,GETDATE(), GETDATE(), 'Data Warehouse','Insert','N/A', 'MED', 'Medium SES'),
	 (-1,GETDATE(), GETDATE(), 'Data Warehouse','Insert','N/A', 'HIGH','High SES')
