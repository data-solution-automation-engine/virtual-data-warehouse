USE EDW_100_Staging_Area

-- Test set generator
DECLARE @TEST_SET_COUNTER INT = 5;
DECLARE @TEST_ROW_COUNTER INT = 0;

DECLARE @TABLE_NAME VARCHAR(1000);
DECLARE @COLUMN_NAME VARCHAR(1000);
DECLARE @DATA_TYPE VARCHAR(1000);
DECLARE @CHARACTER_MAXIMUM_LENGTH INT;
DECLARE @NUMERIC_PRECISION INT;
DECLARE @NUMERIC_SCALE INT;
DECLARE @INITIAL_SQL VARCHAR(MAX);
DECLARE @COLUMN_SQL VARCHAR(MAX);
DECLARE @COLUMN_VALUE VARCHAR(100);
DECLARE @COLUMN_VALUE_CHARACTER VARCHAR(100);
DECLARE @COLUMN_VALUE_NUMERIC INT;
DECLARE @COLUMN_VALUE_DATETIME DATETIME2(7);
DECLARE @ROWCOUNTER INT;

-- Randomizer values
DECLARE @LENGTH INT;
DECLARE @RANDOM_CODE INT;

-- Overall loop
DECLARE Test_Overall_Cursor CURSOR FOR 
	SELECT	TABLE_NAME
	FROM INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_TYPE='BASE TABLE' 
	AND TABLE_NAME NOT LIKE '%USERMANAGED%' 
	--AND TABLE_NAME='STG_PROFILER_CUST_MEMBERSHIP'

	OPEN Test_Overall_Cursor
	FETCH NEXT FROM Test_Overall_Cursor INTO @TABLE_NAME

		WHILE @@FETCH_STATUS=0
		BEGIN

		-- Reset cursor values and counters
		SET @ROWCOUNTER=0;
		SET @INITIAL_SQL='';
		SET @TEST_ROW_COUNTER=0;
		
		PRINT '-- Creating testcases for '+@TABLE_NAME

		WHILE @TEST_ROW_COUNTER < @TEST_SET_COUNTER
		BEGIN
		  SET @TEST_ROW_COUNTER=@TEST_ROW_COUNTER+1
		  PRINT '-- Testcase '+cast(@TEST_ROW_COUNTER as varchar(100));

		  SET @INITIAL_SQL = 
			'INSERT INTO [dbo].['+@TABLE_NAME+']'+CHAR(13)+
			'([ETL_INSERT_RUN_ID]'+CHAR(13)+
			',[EVENT_DATETIME]'+CHAR(13)+
			',[RECORD_SOURCE]'+CHAR(13)+
			',[CDC_OPERATION]'+CHAR(13)+
			',[HASH_FULL_RECORD]'+CHAR(13)

		  -- Create the insert into statement
		  DECLARE Attribute_Cursor CURSOR FOR 
			SELECT	TABLE_NAME, 
					COLUMN_NAME 
			FROM INFORMATION_SCHEMA.COLUMNS t
			WHERE  t.TABLE_NAME = @TABLE_NAME
			  AND t.COLUMN_NAME NOT IN 
			 (
			 'ETL_INSERT_RUN_ID',
			 'LOAD_DATETIME',
			 'EVENT_DATETIME',
			 'RECORD_SOURCE',
			 'SOURCE_ROW_ID',
			 'CDC_OPERATION',
			 'HASH_FULL_RECORD'
			 ) 
			 ORDER BY ORDINAL_POSITION

			OPEN Attribute_Cursor

			FETCH NEXT FROM Attribute_Cursor INTO @TABLE_NAME, @COLUMN_NAME

			WHILE @@FETCH_STATUS=0
			BEGIN
				SET @INITIAL_SQL = @INITIAL_SQL + ','+@COLUMN_NAME+CHAR(13)
				FETCH NEXT FROM Attribute_Cursor INTO @TABLE_NAME, @COLUMN_NAME
			END

			CLOSE Attribute_Cursor
			DEALLOCATE Attribute_Cursor

	      -- Create the VALUES statement
		  DECLARE Test_Cursor CURSOR FOR 
			SELECT	TABLE_NAME, 
					COLUMN_NAME, 
					DATA_TYPE, 
					CHARACTER_MAXIMUM_LENGTH, 
					NUMERIC_PRECISION, 
					NUMERIC_SCALE
			 FROM INFORMATION_SCHEMA.COLUMNS t
			 WHERE  t.TABLE_NAME = @TABLE_NAME
			  AND t.COLUMN_NAME NOT IN 
			 (
			 'ETL_INSERT_RUN_ID',
			 'LOAD_DATETIME',
			 'EVENT_DATETIME',
			 'RECORD_SOURCE',
			 'SOURCE_ROW_ID',
			 'CDC_OPERATION',
			 'HASH_FULL_RECORD'
			 ) 
			 ORDER BY ORDINAL_POSITION

			OPEN Test_Cursor
			SET @COLUMN_SQL='VALUES (-1, GETDATE(), ''Testcases'', ''Insert'', ''N/A'', '

			FETCH NEXT FROM Test_Cursor INTO @TABLE_NAME, @COLUMN_NAME, @DATA_TYPE, @CHARACTER_MAXIMUM_LENGTH, @NUMERIC_PRECISION, @NUMERIC_SCALE

			WHILE @@FETCH_STATUS=0
			BEGIN
				SET @ROWCOUNTER = @ROWCOUNTER+1;
				--PRINT @ROWCOUNTER;					

				-- Build the unique random string for each attribute
				SET @LENGTH = ROUND(@CHARACTER_MAXIMUM_LENGTH/5*RAND(),0);
				SET @COLUMN_VALUE_CHARACTER = '';
				WHILE @LENGTH > 0 BEGIN
					SET @LENGTH = @LENGTH - 1;
					SET @RANDOM_CODE  = ROUND(32*RAND(),0) - 6;
					SET @COLUMN_VALUE_CHARACTER = @COLUMN_VALUE_CHARACTER + CHAR(ASCII('a')+@RANDOM_CODE -1);
				END 

				-- Build the unique random string for each attribute
				SET @LENGTH = ROUND(@NUMERIC_PRECISION*RAND(),0);
				SET @COLUMN_VALUE_NUMERIC = '';
				WHILE @LENGTH > 0 BEGIN
					SET @LENGTH = @LENGTH - 1;
					SET @RANDOM_CODE  = @NUMERIC_PRECISION*RAND()*10/RAND();
					SET @COLUMN_VALUE_NUMERIC = @COLUMN_VALUE_NUMERIC+@RANDOM_CODE;
				END 

				-- Build the unique random date/time for each attribute
				SET @COLUMN_VALUE_DATETIME = NULL;
				SET @COLUMN_VALUE_DATETIME = DATEADD(day, DATEDIFF(day, 0, GETDATE()) - 1 - FLOOR(RAND(CAST(NEWID() AS binary(4))) * 365.25 * 90), 0)

				-- Attribute random value specifically for each datatype
				IF (@DATA_TYPE='varchar' or @DATA_TYPE='char')
					SET @COLUMN_VALUE = @COLUMN_VALUE_CHARACTER
				ELSE IF (@DATA_TYPE ='numeric' OR @DATA_TYPE='int')
					SET @COLUMN_VALUE = @COLUMN_VALUE_NUMERIC
				ELSE IF @DATA_TYPE='datetime2'
					SET @COLUMN_VALUE = @COLUMN_VALUE_DATETIME
				ELSE PRINT 'Unknown data type '+@DATA_TYPE

				-- Construct the VALUES statement
				SET @COLUMN_SQL = @COLUMN_SQL++''''+@COLUMN_VALUE+''', ';
				 

				FETCH NEXT FROM Test_Cursor INTO @TABLE_NAME, @COLUMN_NAME, @DATA_TYPE, @CHARACTER_MAXIMUM_LENGTH, @NUMERIC_PRECISION, @NUMERIC_SCALE

			END

			-- Remove the trailing comma
			SET @COLUMN_SQL = SUBSTRING(@COLUMN_SQL, 1,LEN(@COLUMN_SQL)-1)+');'
			SET @INITIAL_SQL = SUBSTRING(@INITIAL_SQL, 1,LEN(@INITIAL_SQL)-1)+')';
			-- Print to console
			PRINT @INITIAL_SQL
			PRINT @COLUMN_SQL;
			PRINT ''

			CLOSE Test_Cursor
			DEALLOCATE Test_Cursor
		END

	FETCH NEXT FROM Test_Overall_Cursor INTO @TABLE_NAME

END

CLOSE Test_Overall_Cursor
DEALLOCATE Test_Overall_Cursor


