USE EDW_100_Staging_Area;

DECLARE @STG_View_Name VARCHAR(100);
DECLARE @HSTG_View_Name VARCHAR(100);
DECLARE @ROW_COUNTER INT;
DECLARE @SQL VARCHAR(MAX)='';

--Metadata cursor; get the list of views
DECLARE Metadata_Cursor CURSOR STATIC LOCAL FOR
SELECT TABLE_NAME
FROM EDW_100_Staging_Area.INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE='VIEW'	AND SUBSTRING(TABLE_NAME,1,3)='STG' AND TABLE_NAME LIKE '%E5%'

--SET @SQL ='USE [EDW_150_EDW_History_Area];'
	  
OPEN Metadata_Cursor;
FETCH NEXT FROM Metadata_Cursor INTO  @STG_View_Name;

WHILE (@@FETCH_STATUS = 0)
BEGIN
	PRINT '--'+@STG_View_Name;
	PRINT 'GO';

	SET @HSTG_View_Name='H'+@STG_View_Name;
	
	SELECT @SQL = m.definition from sys.sql_modules m where m.object_id = object_id('dbo.'+@STG_View_Name, 'V') 	
		
	SET	@SQL = REPLACE(@SQL,'[dbo].[STG_','[dbo].[HSTG_'); -- Name change
	SET	@SQL = REPLACE(@SQL,'dbo.STG_','dbo.HSTG_'); -- Name change in another format
	
	SET	@SQL = REPLACE(@SQL,'from STG_','FROM HSTG_'); -- Name change

	SET @SQL = @SQL + ' GO';

	PRINT(@SQL) 
				
FETCH NEXT FROM Metadata_Cursor INTO  @STG_View_Name;
END;
	   
CLOSE Metadata_Cursor;
DEALLOCATE Metadata_Cursor;


