

Create Function [dbo].[sayStringToTable](@Delimeter varchar(1), @String Varchar(8000) )

RETURNS @Tablo TABLE (Kod varchar(1000)) AS
BEGIN
declare @Pos1 int 
declare @Pos2 int
declare @TempKod varchar (1000)

set @Pos1=0
set @Pos2=0

set @TempKod = ''
 SET @String = @String + @Delimeter
 WHILE @Pos1<Len(@String)
 BEGIN
  SET @Pos1 = CharIndex(@Delimeter, @String, @Pos1)
  SET @TempKod = Substring(@String,@Pos2,@Pos1-@Pos2)
  INSERT INTO @Tablo (Kod) VALUES (@TempKod)
  SET @Pos2 = @Pos1+1
  SET @Pos1 = @Pos1+1
 END
return
end
GO

CREATE TABLE [dbo].[gnl_group_user_definitions](
	[id] [uniqueidentifier] NOT NULL,
	[group_uid] [uniqueidentifier] NULL,
	[user_uid] [uniqueidentifier] NULL,
	[active] [bit] NULL,
	[is_admin] [bit] NULL,
	[is_user_admin] [bit] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [nchar](10) NULL,
 CONSTRAINT [PK_gnl_grup_kullanici_tanimlari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [gnl_group_user_definitions] ([id],[group_uid],[user_uid],[active],[is_admin],[is_user_admin],[inserted_by],[inserted_at],[updated_by],[updated_at])VALUES('3B06EA8F-5333-48C7-92DF-AFBA43BB0044','4403BC9D-E08D-4EDD-AD7B-AB1F030B70C8','A80B5F72-FEB2-4621-BE3F-99A55F59F9E9',1,1,0,'00000000-0000-0000-0000-000000000000','Jul 16 2012  2:01:21:233PM',NULL,NULL)

GO

CREATE TABLE [dbo].[gnl_user_groups](
	[group_uid] [uniqueidentifier] NOT NULL,
	[group_name] [nvarchar](1000) NULL,
	[active] [bit] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [nchar](10) NULL,
 CONSTRAINT [PK_gnl_kullanici_gruplari] PRIMARY KEY CLUSTERED 
(
	[group_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO [gnl_user_groups] ([group_uid],[group_name],[active],[inserted_by],[inserted_at],[updated_by],[updated_at])VALUES('4403BC9D-E08D-4EDD-AD7B-AB1F030B70C8','Survey',1,'00000000-0000-0000-0000-000000000000','Jul 16 2012  2:01:21:217PM','A80B5F72-FEB2-4621-BE3F-99A55F59F9E9',NULL)

GO

CREATE function [dbo].[sbr_anket_kullanici_gruplari] (@kullanici uniqueidentifier)
RETURNS TABLE 
AS
RETURN 
(
   select gnl_group_user_definitions.group_uid,gnl_user_groups.group_name,gnl_group_user_definitions.user_uid,gnl_group_user_definitions.is_admin,gnl_group_user_definitions.is_user_admin from gnl_group_user_definitions,gnl_user_groups 
where gnl_group_user_definitions.user_uid=@kullanici and gnl_group_user_definitions.group_uid=gnl_user_groups.group_uid
)
GO



CREATE FUNCTION [dbo].[Split]
(
	@RowData nvarchar(2000),
	@SplitOn nvarchar(5)
)  
RETURNS @RtnValue table 
(
	Id int identity(1,1),
	Data nvarchar(100)
) 
AS  
BEGIN 
	Declare @Cnt int
	Set @Cnt = 1

	While (Charindex(@SplitOn,@RowData)>0)
	Begin
		Insert Into @RtnValue (data)
		Select 
			Data = ltrim(rtrim(Substring(@RowData,1,Charindex(@SplitOn,@RowData)-1)))

		Set @RowData = Substring(@RowData,Charindex(@SplitOn,@RowData)+1,len(@RowData))
		Set @Cnt = @Cnt + 1
	End
	
	Insert Into @RtnValue (data)
	Select Data = ltrim(rtrim(@RowData))

	Return
END


GO






CREATE PROC [dbo].[SP_GENERATE_INSERTS]
(
	@table_name varchar(776),  		-- The table/view for which the INSERT statements will be generated using the existing data
	@target_table varchar(776) = NULL, 	-- Use this parameter to specify a different table name into which the data will be inserted
	@include_column_list bit = 1,		-- Use this parameter to include/ommit column list in the generated INSERT statement
	@from varchar(800) = NULL, 		-- Use this parameter to filter the rows based on a filter condition (using WHERE)
	@include_timestamp bit = 0, 		-- Specify 1 for this parameter, if you want to include the TIMESTAMP/ROWVERSION column's data in the INSERT statement
	@debug_mode bit = 0,			-- If @debug_mode is set to 1, the SQL statements constructed by this procedure will be printed for later examination
	@owner varchar(64) = NULL,		-- Use this parameter if you are not the owner of the table
	@ommit_images bit = 0,			-- Use this parameter to generate INSERT statements by omitting the 'image' columns
	@ommit_identity bit = 0,		-- Use this parameter to ommit the identity columns
	@top int = NULL,			-- Use this parameter to generate INSERT statements only for the TOP n rows
	@cols_to_include varchar(8000) = NULL,	-- List of columns to be included in the INSERT statement
	@cols_to_exclude varchar(8000) = NULL,	-- List of columns to be excluded from the INSERT statement
	@disable_constraints bit = 0,		-- When 1, disables foreign key constraints and enables them after the INSERT statements
	@ommit_computed_cols bit = 0		-- When 1, computed columns will not be included in the INSERT statement
	
)
AS
BEGIN

/***********************************************************************************************************
Procedure:	sp_generate_inserts  (Build 22) 
		(Copyright © 2002 Narayana Vyas Kondreddi. All rights reserved.)
                                          
Purpose:	To generate INSERT statements from existing data. 
		These INSERTS can be executed to regenerate the data at some other location.
		This procedure is also useful to create a database setup, where in you can 
		script your data along with your table definitions.

Written by:	Narayana Vyas Kondreddi
	        http://vyaskn.tripod.com

Acknowledgements:
		Divya Kalra	-- For beta testing
		Mark Charsley	-- For reporting a problem with scripting uniqueidentifier columns with NULL values
		Artur Zeygman	-- For helping me simplify a bit of code for handling non-dbo owned tables
		Joris Laperre   -- For reporting a regression bug in handling text/ntext columns

Tested on: 	SQL Server 7.0 and SQL Server 2000

Date created:	January 17th 2001 21:52 GMT

Date modified:	May 1st 2002 19:50 GMT

Email: 		vyaskn@hotmail.com

NOTE:		This procedure may not work with tables with too many columns.
		Results can be unpredictable with huge text columns or SQL Server 2000's sql_variant data types
		Whenever possible, Use @include_column_list parameter to ommit column list in the INSERT statement, for better results
		IMPORTANT: This procedure is not tested with internation data (Extended characters or Unicode). If needed
		you might want to convert the datatypes of character variables in this procedure to their respective unicode counterparts
		like nchar and nvarchar
		

Example 1:	To generate INSERT statements for table 'titles':
		
		EXEC sp_generate_inserts 'titles'

Example 2: 	To ommit the column list in the INSERT statement: (Column list is included by default)
		IMPORTANT: If you have too many columns, you are advised to ommit column list, as shown below,
		to avoid erroneous results
		
		EXEC sp_generate_inserts 'titles', @include_column_list = 0

Example 3:	To generate INSERT statements for 'titlesCopy' table from 'titles' table:

		EXEC sp_generate_inserts 'titles', 'titlesCopy'

Example 4:	To generate INSERT statements for 'titles' table for only those titles 
		which contain the word 'Computer' in them:
		NOTE: Do not complicate the FROM or WHERE clause here. It's assumed that you are good with T-SQL if you are using this parameter

		EXEC sp_generate_inserts 'titles', @from = "from titles where title like '%Computer%'"

Example 5: 	To specify that you want to include TIMESTAMP column's data as well in the INSERT statement:
		(By default TIMESTAMP column's data is not scripted)

		EXEC sp_generate_inserts 'titles', @include_timestamp = 1

Example 6:	To print the debug information:
  
		EXEC sp_generate_inserts 'titles', @debug_mode = 1

Example 7: 	If you are not the owner of the table, use @owner parameter to specify the owner name
		To use this option, you must have SELECT permissions on that table

		EXEC sp_generate_inserts Nickstable, @owner = 'Nick'

Example 8: 	To generate INSERT statements for the rest of the columns excluding images
		When using this otion, DO NOT set @include_column_list parameter to 0.

		EXEC sp_generate_inserts imgtable, @ommit_images = 1

Example 9: 	To generate INSERT statements excluding (ommiting) IDENTITY columns:
		(By default IDENTITY columns are included in the INSERT statement)

		EXEC sp_generate_inserts mytable, @ommit_identity = 1

Example 10: 	To generate INSERT statements for the TOP 10 rows in the table:
		
		EXEC sp_generate_inserts mytable, @top = 10

Example 11: 	To generate INSERT statements with only those columns you want:
		
		EXEC sp_generate_inserts titles, @cols_to_include = "'title','title_id','au_id'"

Example 12: 	To generate INSERT statements by omitting certain columns:
		
		EXEC sp_generate_inserts titles, @cols_to_exclude = "'title','title_id','au_id'"

Example 13:	To avoid checking the foreign key constraints while loading data with INSERT statements:
		
		EXEC sp_generate_inserts titles, @disable_constraints = 1

Example 14: 	To exclude computed columns from the INSERT statement:
		EXEC sp_generate_inserts MyTable, @ommit_computed_cols = 1
***********************************************************************************************************/

SET NOCOUNT ON

--Making sure user only uses either @cols_to_include or @cols_to_exclude
IF ((@cols_to_include IS NOT NULL) AND (@cols_to_exclude IS NOT NULL))
	BEGIN
		RAISERROR('Use either @cols_to_include or @cols_to_exclude. Do not use both the parameters at once',16,1)
		RETURN -1 --Failure. Reason: Both @cols_to_include and @cols_to_exclude parameters are specified
	END

--Making sure the @cols_to_include and @cols_to_exclude parameters are receiving values in proper format
IF ((@cols_to_include IS NOT NULL) AND (PATINDEX('''%''',@cols_to_include) = 0))
	BEGIN
		RAISERROR('Invalid use of @cols_to_include property',16,1)
		PRINT 'Specify column names surrounded by single quotes and separated by commas'
		PRINT 'Eg: EXEC sp_generate_inserts titles, @cols_to_include = "''title_id'',''title''"'
		RETURN -1 --Failure. Reason: Invalid use of @cols_to_include property
	END

IF ((@cols_to_exclude IS NOT NULL) AND (PATINDEX('''%''',@cols_to_exclude) = 0))
	BEGIN
		RAISERROR('Invalid use of @cols_to_exclude property',16,1)
		PRINT 'Specify column names surrounded by single quotes and separated by commas'
		PRINT 'Eg: EXEC sp_generate_inserts titles, @cols_to_exclude = "''title_id'',''title''"'
		RETURN -1 --Failure. Reason: Invalid use of @cols_to_exclude property
	END


--Checking to see if the database name is specified along wih the table name
--Your database context should be local to the table for which you want to generate INSERT statements
--specifying the database name is not allowed
IF (PARSENAME(@table_name,3)) IS NOT NULL
	BEGIN
		RAISERROR('Do not specify the database name. Be in the required database and just specify the table name.',16,1)
		RETURN -1 --Failure. Reason: Database name is specified along with the table name, which is not allowed
	END

--Checking for the existence of 'user table' or 'view'
--This procedure is not written to work on system tables
--To script the data in system tables, just create a view on the system tables and script the view instead

IF @owner IS NULL
	BEGIN
		IF ((OBJECT_ID(@table_name,'U') IS NULL) AND (OBJECT_ID(@table_name,'V') IS NULL)) 
			BEGIN
				RAISERROR('User table or view not found.',16,1)
				PRINT 'You may see this error, if you are not the owner of this table or view. In that case use @owner parameter to specify the owner name.'
				PRINT 'Make sure you have SELECT permission on that table or view.'
				RETURN -1 --Failure. Reason: There is no user table or view with this name
			END
	END
ELSE
	BEGIN
		IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @table_name AND (TABLE_TYPE = 'BASE TABLE' OR TABLE_TYPE = 'VIEW') AND TABLE_SCHEMA = @owner)
			BEGIN
				RAISERROR('User table or view not found.',16,1)
				PRINT 'You may see this error, if you are not the owner of this table. In that case use @owner parameter to specify the owner name.'
				PRINT 'Make sure you have SELECT permission on that table or view.'
				RETURN -1 --Failure. Reason: There is no user table or view with this name		
			END
	END

--Variable declarations
DECLARE		@Column_ID int, 		
		@Column_List varchar(8000), 
		@Column_Name varchar(128), 
		@Start_Insert varchar(786), 
		@Data_Type varchar(128), 
		@Actual_Values varchar(8000),	--This is the string that will be finally executed to generate INSERT statements
		@IDN varchar(128)		--Will contain the IDENTITY column's name in the table

--Variable Initialization
SET @IDN = ''
SET @Column_ID = 0
SET @Column_Name = ''
SET @Column_List = ''
SET @Actual_Values = ''

IF @owner IS NULL 
	BEGIN
		SET @Start_Insert = 'INSERT INTO ' + '[' + RTRIM(COALESCE(@target_table,@table_name)) + ']' 
	END
ELSE
	BEGIN
		SET @Start_Insert = 'INSERT ' + '[' + LTRIM(RTRIM(@owner)) + '].' + '[' + RTRIM(COALESCE(@target_table,@table_name)) + ']' 		
	END


--To get the first column's ID

SELECT	@Column_ID = MIN(ORDINAL_POSITION) 	
FROM	INFORMATION_SCHEMA.COLUMNS (NOLOCK) 
WHERE 	TABLE_NAME = @table_name AND
(@owner IS NULL OR TABLE_SCHEMA = @owner)



--Loop through all the columns of the table, to get the column names and their data types
WHILE @Column_ID IS NOT NULL
	BEGIN
		SELECT 	@Column_Name = QUOTENAME(COLUMN_NAME), 
		@Data_Type = DATA_TYPE 
		FROM 	INFORMATION_SCHEMA.COLUMNS (NOLOCK) 
		WHERE 	ORDINAL_POSITION = @Column_ID AND 
		TABLE_NAME = @table_name AND
		(@owner IS NULL OR TABLE_SCHEMA = @owner)



		IF @cols_to_include IS NOT NULL --Selecting only user specified columns
		BEGIN
			IF CHARINDEX( '''' + SUBSTRING(@Column_Name,2,LEN(@Column_Name)-2) + '''',@cols_to_include) = 0 
			BEGIN
				GOTO SKIP_LOOP
			END
		END

		IF @cols_to_exclude IS NOT NULL --Selecting only user specified columns
		BEGIN
			IF CHARINDEX( '''' + SUBSTRING(@Column_Name,2,LEN(@Column_Name)-2) + '''',@cols_to_exclude) <> 0 
			BEGIN
				GOTO SKIP_LOOP
			END
		END

		--Making sure to output SET IDENTITY_INSERT ON/OFF in case the table has an IDENTITY column
		IF (SELECT COLUMNPROPERTY( OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name),SUBSTRING(@Column_Name,2,LEN(@Column_Name) - 2),'IsIdentity')) = 1 
		BEGIN
			IF @ommit_identity = 0 --Determing whether to include or exclude the IDENTITY column
				SET @IDN = @Column_Name
			ELSE
				GOTO SKIP_LOOP			
		END
		
		--Making sure whether to output computed columns or not
		IF @ommit_computed_cols = 1
		BEGIN
			IF (SELECT COLUMNPROPERTY( OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name),SUBSTRING(@Column_Name,2,LEN(@Column_Name) - 2),'IsComputed')) = 1 
			BEGIN
				GOTO SKIP_LOOP					
			END
		END
		
		--Tables with columns of IMAGE data type are not supported for obvious reasons
		IF(@Data_Type in ('image'))
			BEGIN
				IF (@ommit_images = 0)
					BEGIN
						RAISERROR('Tables with image columns are not supported.',16,1)
						PRINT 'Use @ommit_images = 1 parameter to generate INSERTs for the rest of the columns.'
						PRINT 'DO NOT ommit Column List in the INSERT statements. If you ommit column list using @include_column_list=0, the generated INSERTs will fail.'
						RETURN -1 --Failure. Reason: There is a column with image data type
					END
				ELSE
					BEGIN
					GOTO SKIP_LOOP
					END
			END

		--Determining the data type of the column and depending on the data type, the VALUES part of
		--the INSERT statement is generated. Care is taken to handle columns with NULL values. Also
		--making sure, not to lose any data from flot, real, money, smallmomey, datetime columns
		SET @Actual_Values = @Actual_Values  +
		CASE 
			WHEN @Data_Type IN ('char','varchar','nchar','nvarchar') 
				THEN 
					'COALESCE('''''''' + REPLACE(RTRIM(' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'
			WHEN @Data_Type IN ('datetime','smalldatetime') 
				THEN 
					'COALESCE('''''''' + RTRIM(CONVERT(char,' + @Column_Name + ',109))+'''''''',''NULL'')'
			WHEN @Data_Type IN ('uniqueidentifier') 
				THEN  
					'COALESCE('''''''' + REPLACE(CONVERT(char(255),RTRIM(' + @Column_Name + ')),'''''''','''''''''''')+'''''''',''NULL'')'
			WHEN @Data_Type IN ('text','ntext') 
				THEN  
					'COALESCE('''''''' + REPLACE(CONVERT(char(8000),' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'					
			WHEN @Data_Type IN ('binary','varbinary') 
				THEN  
					'COALESCE(RTRIM(CONVERT(char,' + 'CONVERT(int,' + @Column_Name + '))),''NULL'')'  
			WHEN @Data_Type IN ('timestamp','rowversion') 
				THEN  
					CASE 
						WHEN @include_timestamp = 0 
							THEN 
								'''DEFAULT''' 
							ELSE 
								'COALESCE(RTRIM(CONVERT(char,' + 'CONVERT(int,' + @Column_Name + '))),''NULL'')'  
					END
			WHEN @Data_Type IN ('float','real','money','smallmoney')
				THEN
					'COALESCE(LTRIM(RTRIM(' + 'CONVERT(char, ' +  @Column_Name  + ',2)' + ')),''NULL'')' 
			ELSE 
				'COALESCE(LTRIM(RTRIM(' + 'CONVERT(char, ' +  @Column_Name  + ')' + ')),''NULL'')' 
		END   + '+' +  ''',''' + ' + '
		
		--Generating the column list for the INSERT statement
		SET @Column_List = @Column_List +  @Column_Name + ','	

		SKIP_LOOP: --The label used in GOTO

		SELECT 	@Column_ID = MIN(ORDINAL_POSITION) 
		FROM 	INFORMATION_SCHEMA.COLUMNS (NOLOCK) 
		WHERE 	TABLE_NAME = @table_name AND 
		ORDINAL_POSITION > @Column_ID AND
		(@owner IS NULL OR TABLE_SCHEMA = @owner)


	--Loop ends here!
	END

--To get rid of the extra characters that got concatenated during the last run through the loop
SET @Column_List = LEFT(@Column_List,len(@Column_List) - 1)
SET @Actual_Values = LEFT(@Actual_Values,len(@Actual_Values) - 6)

IF LTRIM(@Column_List) = '' 
	BEGIN
		RAISERROR('No columns to select. There should at least be one column to generate the output',16,1)
		RETURN -1 --Failure. Reason: Looks like all the columns are ommitted using the @cols_to_exclude parameter
	END

--Forming the final string that will be executed, to output the INSERT statements
IF (@include_column_list <> 0)
	BEGIN
		SET @Actual_Values = 
			'SELECT ' +  
			CASE WHEN @top IS NULL OR @top < 0 THEN '' ELSE ' TOP ' + LTRIM(STR(@top)) + ' ' END + 
			'''' + RTRIM(@Start_Insert) + 
			' ''+' + '''(' + RTRIM(@Column_List) +  '''+' + ''')''' + 
			' +''VALUES(''+ ' +  @Actual_Values  + '+'')''' + ' ' + 
			COALESCE(@from,' FROM ' + CASE WHEN @owner IS NULL THEN '' ELSE '[' + LTRIM(RTRIM(@owner)) + '].' END + '[' + rtrim(@table_name) + ']' + '(NOLOCK)')
	END
ELSE IF (@include_column_list = 0)
	BEGIN
		SET @Actual_Values = 
			'SELECT ' + 
			CASE WHEN @top IS NULL OR @top < 0 THEN '' ELSE ' TOP ' + LTRIM(STR(@top)) + ' ' END + 
			'''' + RTRIM(@Start_Insert) + 
			' '' +''VALUES(''+ ' +  @Actual_Values + '+'')''' + ' ' + 
			COALESCE(@from,' FROM ' + CASE WHEN @owner IS NULL THEN '' ELSE '[' + LTRIM(RTRIM(@owner)) + '].' END + '[' + rtrim(@table_name) + ']' + '(NOLOCK)')
	END	

--Determining whether to ouput any debug information
IF @debug_mode =1
	BEGIN
		PRINT '/*****START OF DEBUG INFORMATION*****'
		PRINT 'Beginning of the INSERT statement:'
		PRINT @Start_Insert
		PRINT ''
		PRINT 'The column list:'
		PRINT @Column_List
		PRINT ''
		PRINT 'The SELECT statement executed to generate the INSERTs'
		PRINT @Actual_Values
		PRINT ''
		PRINT '*****END OF DEBUG INFORMATION*****/'
		PRINT ''
	END
		
PRINT '--INSERTs generated by ''sp_generate_inserts'' stored procedure written by Vyas'
PRINT '--Build number: 22'
PRINT '--Problems/Suggestions? Contact Vyas @ vyaskn@hotmail.com'
PRINT '--http://vyaskn.tripod.com'
PRINT ''
PRINT 'SET NOCOUNT ON'
PRINT ''


--Determining whether to print IDENTITY_INSERT or not
IF (@IDN <> '')
	BEGIN
		PRINT 'SET IDENTITY_INSERT ' + QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + QUOTENAME(@table_name) + ' ON'
		PRINT 'GO'
		PRINT ''
	END


IF @disable_constraints = 1 AND (OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name, 'U') IS NOT NULL)
	BEGIN
		IF @owner IS NULL
			BEGIN
				SELECT 	'ALTER TABLE ' + QUOTENAME(COALESCE(@target_table, @table_name)) + ' NOCHECK CONSTRAINT ALL' AS '--Code to disable constraints temporarily'
			END
		ELSE
			BEGIN
				SELECT 	'ALTER TABLE ' + QUOTENAME(@owner) + '.' + QUOTENAME(COALESCE(@target_table, @table_name)) + ' NOCHECK CONSTRAINT ALL' AS '--Code to disable constraints temporarily'
			END

		PRINT 'GO'
	END

PRINT ''
PRINT 'PRINT ''Inserting values into ' + '[' + RTRIM(COALESCE(@target_table,@table_name)) + ']' + ''''


--All the hard work pays off here!!! You'll get your INSERT statements, when the next line executes!
EXEC (@Actual_Values)

PRINT 'PRINT ''Done'''
PRINT ''


IF @disable_constraints = 1 AND (OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name, 'U') IS NOT NULL)
	BEGIN
		IF @owner IS NULL
			BEGIN
				SELECT 	'ALTER TABLE ' + QUOTENAME(COALESCE(@target_table, @table_name)) + ' CHECK CONSTRAINT ALL'  AS '--Code to enable the previously disabled constraints'
			END
		ELSE
			BEGIN
				SELECT 	'ALTER TABLE ' + QUOTENAME(@owner) + '.' + QUOTENAME(COALESCE(@target_table, @table_name)) + ' CHECK CONSTRAINT ALL' AS '--Code to enable the previously disabled constraints'
			END

		PRINT 'GO'
	END

PRINT ''
IF (@IDN <> '')
	BEGIN
		PRINT 'SET IDENTITY_INSERT ' + QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + QUOTENAME(@table_name) + ' OFF'
		PRINT 'GO'
	END

PRINT 'SET NOCOUNT OFF'


SET NOCOUNT OFF
RETURN 0 --Success. We are done!
END


GO




CREATE PROCEDURE [dbo].[SP_GENERATE_INSERT_UPDATE_SCRIPTS] 
@objname nvarchar(776)
as

DECLARE @objid as int
	DECLARE @sysobj_type  as char(2)
    SELECT @objid = id, @sysobj_type = xtype from sysobjects where id = object_id(@objname)
DECLARE @colname as sysname
 SELECT @colname = name from syscolumns where id = @objid and colstat & 1 = 1
-- DISPLAY COLUMN IF TABLE / VIEW
if @sysobj_type in ('S ','U ','V ','TF','IF')
begin
-- SET UP NUMERIC TYPES: THESE WILL HAVE NON-BLANK PREC/SCALE
DECLARE @numtypes as  nvarchar(80);
DECLARE @avoidlength  as nvarchar(80);
SELECT  @numtypes = N'decimalreal,money,float,numeric,smallmoney'
SELECT  @avoidlength = N'int,smallint,float,datatime,smalldatetime,text,bit'
-- INFO FOR EACH COLUMN
CREATE TABLE #MyProc (pkey INT NOT NULL IDENTITY (1, 1),ID INT ,MyStatement NVARCHAR(4000))
INSERT INTO  #MyProc (ID, MyStatement)SELECT 1, 'alter PROCEDURE InsUpd_' + @objname + ' ' 
INSERT INTO  #MyProc (ID, MyStatement)SELECT 2, '@' + name + ' ' + type_name(xusertype) + ' ' + case when charindex(type_name(xtype),@avoidlength) > 0 then ''
        else case when charindex(type_name(xtype), @numtypes) <= 0 then '(' + convert(varchar(10), length) + ')' else '(' + case when charindex(type_name(xtype), @numtypes) > 0
        then convert(varchar(5),ColumnProperty(id, name, 'precision'))
        else '' end + case when charindex(type_name(xtype), @numtypes) > 0 then ',' else ' ' end + case when charindex(type_name(xtype), @numtypes) > 0
        then convert(varchar(5),OdbcScale(xtype,xscale))  else '' end + ')' end end + ', 'from syscolumns where id = @objid and number = 0 order by colid
    update #MyProc set MyStatement = Replace(MyStatement,', ',' ') where  pkey = (SELECT max(pkey) from #MyProc)
    INSERT INTO #MyProc (ID, MyStatement) SELECT 3, 'AS  BEGIN IF @' + @colname + ' <= 0  BEGIN'
    INSERT INTO #MyProc (ID, MyStatement)SELECT 3, 'INSERT INTO dbo.' + @objname + ' ('
    INSERT INTO #MyProc (ID, MyStatement)SELECT 4, '' + name + ','from syscolumns where id = @objid and number = 0 order by colid
    DELETE FROM #MyProc WHERE ID = 4 and MyStatement like '%' + @colname + '%'
        update #MyProc set MyStatement = Replace(MyStatement,',','') where pkey = (SELECT max(pkey) from #MyProc)
    INSERT INTO #MyProc (ID, MyStatement)  SELECT 5, ')'
       INSERT INTO #MyProc (ID, MyStatement) SELECT 6, 'VALUES ('
    INSERT INTO #MyProc (ID, MyStatement)SELECT 7, '@' + name + ','from syscolumns where id = @objid and number = 0 order by colid
    DELETE FROM #MyProc WHERE ID = 7 and MyStatement like '%' + @colname + '%' 
        update #MyProc set MyStatement = Replace(MyStatement,'@DateCreated,','GETDATE(),') where ID = 7 AND MyStatement like '%@DateCreated,'
		update #MyProc set MyStatement = Replace(MyStatement,'@DateModified,','GETDATE(),') where ID = 7 AND MyStatement like '%@DateModified,'
		update #MyProc set MyStatement = Replace(MyStatement,',','') where pkey = (SELECT max(pkey) from #MyProc)
    INSERT INTO #MyProc (ID, MyStatement)SELECT 8, ')SET @' + @colname + ' = @@IDENTITY SELECT @' + @colname + ' AS ' + @colname + 'END ELSE BEGIN'
    INSERT INTO #MyProc (ID, MyStatement) SELECT 9, ' '
    INSERT INTO #MyProc (ID, MyStatement) SELECT 10, 'UPDATE dbo.' + @objname + ' SET '
    INSERT INTO #MyProc (ID, MyStatement) SELECT 11, '' + name + ' = @' + name + ',' from syscolumns where id = @objid and number = 0 order by colid
    DELETE FROM #MyProc  WHERE ID = 11 and MyStatement like '%' + @colname + '%'
    DELETE FROM #MyProc  WHERE ID = 11 and MyStatement like '%DateCreated %'
     update #MyProc set MyStatement = Replace(MyStatement,'@DateModified,','GETDATE(),') where  ID = 11 AND MyStatement like '%@DateModified,'
    update #MyProc set MyStatement = Replace(MyStatement,',','') where  pkey = (SELECT max(pkey) from #MyProc)
    INSERT INTO #MyProc (ID, MyStatement) SELECT 12, ' WHERE ' + @colname + ' = @' + @colname + ' SELECT @' + @colname + ' AS ' + @colname + 'END END'
        SELECT MyStatement from #MyProc ORDER BY [ID] 
	End








GO


CREATE TABLE [dbo].[BaseAlertResource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UICulture] [varchar](10) NULL,
	[ResourceId] [varchar](50) NULL,
	[Resource] [varchar](500) NULL,
 CONSTRAINT [PK_BaseAlertResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT BaseAlertResource ON

GO

INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(1,'tr-TR','1','Operation Finished')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(2,'en-US','1','Operation Finished')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(3,'tr-TR','2','Delete Operation Finished')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(4,'en-US','2','Delete Operation Finished')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(5,'tr-TR','3','The e-mail address you entered has already been used. Please register with a different e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(6,'en-US','3','The e-mail address you entered has already been used. Please register with a different e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(7,'tr-TR','4','Registeration Operation Finished')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(8,'en-US','4','Registeration Operation Finished')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(9,'tr-TR','5','Please Login')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(10,'en-US','5','Please Login')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(11,'tr-TR','7','Your password has been changed with the new password you entered.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(12,'en-US','7','Your password has been changed with the new password you entered.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(13,'tr-TR','8','Old Password is Wrong')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(14,'en-US','8','Old Password is Wrong')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(15,'tr-TR','9','You entered the control text incorrectly. Please re-enter the Check Text.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(16,'en-US','9','You entered the control text incorrectly. Please re-enter the Check Text.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(17,'tr-TR','10','Please Fill End Date Greater Than Start Date')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(18,'en-US','10','Please Fill End Date Greater Than Start Date')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(19,'tr-TR','100','Please enter your e-mail address by filling out the form below to receive your e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(20,'en-US','100','Please enter your e-mail address by filling out the form below to receive your e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(21,'tr-TR','11','The e-mail address you entered for the purchase is available in the system. <br> You can login with your e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(22,'en-US','11','The e-mail address you entered for the purchase is available in the system. <br> You can login with your e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(23,'tr-TR','101','The activation e-mail has been sent to your e-mail address.Please go to your e-mail address and click on Activation link.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(24,'en-US','101','The activation e-mail has been sent to your e-mail address.Please go to your e-mail address and click on Activation link.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(25,'tr-TR','12','Login Register Login Username Password Remember Me Forgot your password?')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(26,'en-US','12','Login Register Login Username Password Remember Me Forgot your password?')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(27,'tr-TR','13','Previous Invite has been accepted.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(28,'en-US','13','Previous Invite has been accepted.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(29,'tr-TR','14','The e-mail address you entered is not registered in the system. Please try with a different e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(30,'en-US','14','The e-mail address you entered is not registered in the system. Please try with a different e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(31,'tr-TR','102','Purchasing has been performed.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(32,'en-US','102','Purchasing has been performed.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(33,'tr-TR','201','Please select Team.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(34,'en-US','201','Please select Team.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(35,'tr-TR','103','Your new e-mail address has been sent to you. If you want to reset your password, please click on the link provided by e-mail.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(36,'en-US','103','Your new e-mail address has been sent to you. If you want to reset your password, please click on the link provided by e-mail.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(37,'tr-TR','200','You are not authorized to create surveys. The authority to create questionnaires belongs to the team managers.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(38,'en-US','200','You are not authorized to create surveys. The authority to create questionnaires belongs to the team managers.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(39,'tr-TR','202','Your current membership and your right to use the Team has been filled. You can continue to create a questionnaire by purchasing the membership package.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(40,'en-US','202','Your current membership and your right to use the Team has been filled. You can continue to create a questionnaire by purchasing the membership package.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(41,'tr-TR','104','Purchasing has been performed. After you make the bank payment, your membership will be activated within 24 hours at the latest.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(42,'en-US','104','Purchasing has been performed. After you make the bank payment, your membership will be activated within 24 hours at the latest.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(43,'tr-TR','203','Your message has been sent successfully.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(44,'en-US','203','Your message has been sent successfully.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(45,'tr-TR','204','Your message could not be sent. Please try again.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(46,'en-US','204','Your message could not be sent. Please try again.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(47,'tr-TR','110','Please enter your company name.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(48,'en-US','110','Please enter your company name.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(49,'tr-TR','20','Please Enter Tax Office.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(50,'en-US','20','Please Enter Tax Office.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(51,'tr-TR','21','Please enter tax number.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(52,'en-US','21','Please enter tax number.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(53,'tr-TR','22','Activation could not be performed. Please click on the Activation link again at the e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(54,'en-US','22','Activation could not be performed. Please click on the Activation link again at the e-mail address.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(55,'tr-TR','23','You are still a member.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(56,'en-US','23','You are still a member.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(57,'tr-TR','24','Update Operation Finished')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(58,'en-US','24','Update Operation Finished')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(59,'tr-TR','25','Invitation Acceptance has not been performed. Please try again.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(60,'en-US','25','Invitation Acceptance has not been performed. Please try again.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(61,'tr-TR','26','The e-mail address you entered is different from the e-mail address you entered.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(62,'en-US','26','The e-mail address you entered is different from the e-mail address you entered.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(63,'tr-TR','27','Purchasing is not done.Please try again.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(64,'en-US','27','Purchasing is not done.Please try again.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(65,'tr-TR','28','Demand Amount can not be 0')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(66,'en-US','28','Demand Amount can not be 0')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(67,'tr-TR','29','Do not leave the subject of the charge.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(68,'en-US','29','Do not leave the subject of the charge.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(69,'tr-TR','30','Start Date cannot be greater than End Date.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(70,'en-US','30','Start Date cannot be greater than End Date.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(71,'tr-TR','31','Please Select User Title')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(72,'en-US','31','Please Select User Title')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(73,'tr-TR','32','Please Select Project')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(74,'tr-TR','33','Please Select Expense')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(75,'tr-TR','34','Please Select Currency')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(76,'tr-TR','35','Please select an advance type')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(77,'tr-TR','37','Please Select Currency')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(78,'tr-TR','38','Please select New Manager')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(79,'en-US','38','Please select New Manager')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(80,'tr-TR','39','The user has Open Avans on it. If you want to deactivate the user, please close these Advances.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(81,'tr-TR','40','There is Overhead Payable on User. If you want to make the user passive, pay these costs first.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(82,'tr-TR','41','User defined as Administrator. If you want to make the user passive, change the Administrator first from the Batch Admin Change screen.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(83,'tr-TR','42','Please Fill All Fields.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(84,'en-US','42','Please Fill All Fields.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(85,'tr-TR','45','Please Fill Document Field')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(86,'en-US','45','Please Fill Document Field')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(87,'tr-TR','46','Please Select Vendor')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(88,'en-US','46','Please Select Vendor')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(89,'tr-TR','47','Please Select Org. Unit')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(90,'en-US','47','Please Select Org. Unit')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(91,'tr-TR','48','Please Select Real Estate')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(92,'en-US','48','Please Select Real Estate')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(93,'tr-TR','49','Please Fill Stock Code,Quantity,Unit Price,Currency,Rate Of Exchange,Tax Group Field in Invoice Detail')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(94,'en-US','49','Please Fill Stock Code,Quantity,Unit Price,Currency,Rate Of Exchange,Tax Group Field in Invoice Detail')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(95,'tr-TR','205','Survey Start and End Dates Membership cannot be greater than your End date.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(96,'en-US','205','Survey Start and End Dates Membership cannot be greater than your End date.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(97,'tr-TR','50','Please Select Motor Vehicle .')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(98,'en-US','50','Please Select Motor Vehicle .')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(103,'tr-TR','ItemDefItemCodeFlds','Please Fill Item Code.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(104,'en-US','ItemDefItemCodeFlds','Please Fill Item Code.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(105,'tr-TR','ItemDefItemGroupFlds','Please Select Item Group.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(106,'en-US','ItemDefItemGroupFlds','Please Select Item Group.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(107,'tr-TR','206','In order to be able to publish the survey, the Survey End Date must be greater than the date of the day.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(108,'en-US','206','In order to be able to publish the survey, the Survey End Date must be greater than the date of the day.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(109,'tr-TR','55','Please Fill Policy No')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(110,'en-US','55','Please Fill Policy No')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(111,'tr-TR','56','Please Select Insurance Company.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(112,'en-US','56','Please Select Insurance Company.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(113,'tr-TR','57','Please Select Policy.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(114,'en-US','57','Please Select Policy.')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(115,'tr-TR','60','Colored Fields Must Be Filled!')
INSERT INTO [BaseAlertResource] ([Id],[UICulture],[ResourceId],[Resource])VALUES(116,'en-US','60','Colored Fields Must Be Filled!')
GO

SET IDENTITY_INSERT BaseAlertResource OFF

GO

CREATE TABLE [dbo].[BaseLoggedInUsers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [varchar](50) NULL,
	[user_uid] [uniqueidentifier] NULL,
	[user_name] [varchar](50) NULL,
	[user_name_surname] [varchar](50) NULL,
	[date] [datetime] NULL,
	[session_id] [varchar](50) NULL,
	[host_name] [varchar](50) NULL,
	[user_host_adres] [varchar](50) NULL,
	[browser] [varchar](50) NULL,
	[platform] [varchar](50) NULL,
	[version] [varchar](50) NULL,
 CONSTRAINT [PK_BaserLoggedInUsers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[gnl_kullanici_sifre_sifirlama](
	[id] [uniqueidentifier] NOT NULL,
	[sifre_sifirlanacak_email] [nvarchar](100) NULL,
	[sifirlama_istek_tarihi] [datetime] NULL,
	[sifirlama_key] [nvarchar](50) NULL,
	[sifirlama_kabul_edildi] [bit] NULL,
	[sifirlama_kabul_edilme_tarihi] [datetime] NULL,
	[sifre] [nvarchar](50) NULL,
 CONSTRAINT [PK_gnl_kullanici_sifre_sifirlama] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




CREATE TABLE [dbo].[gnl_mail_service](
	[id] [uniqueidentifier] NOT NULL,
	[eposta_to] [nvarchar](200) NULL,
	[subject] [nvarchar](2000) NULL,
	[body] [text] NULL,
	[eklemetarihi] [datetime] NULL,
	[gonderildi] [bit] NULL,
	[gonderimtarihi] [datetime] NULL,
	[anket_uid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_gnl_mail_service] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[gnl_message](
	[message_uid] [uniqueidentifier] NOT NULL,
	[related_message_uid] [uniqueidentifier] NULL,
	[message_subject] [nvarchar](2000) NOT NULL,
	[message] [nvarchar](max) NULL,
	[send_user_uid] [uniqueidentifier] NOT NULL,
	[sent_date] [datetime] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [datetime] NULL,
	[is_deleted] [bit] NULL,
	[deleted_by] [uniqueidentifier] NULL,
	[deleted_at] [datetime] NULL,
 CONSTRAINT [PK__gnl_ileti__41E3A924] PRIMARY KEY CLUSTERED 
(
	[message_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[gnl_message]  WITH CHECK ADD  CONSTRAINT [FK_Baseileti_Baseileti] FOREIGN KEY([related_message_uid])
REFERENCES [dbo].[gnl_message] ([message_uid])
GO

ALTER TABLE [dbo].[gnl_message] CHECK CONSTRAINT [FK_Baseileti_Baseileti]
GO


CREATE TABLE [dbo].[gnl_message_inbox](
	[message_uid] [uniqueidentifier] NOT NULL,
	[user_uid] [uniqueidentifier] NOT NULL,
	[recipient_type] [char](1) NOT NULL,
	[message_is_read] [bit] NOT NULL,
	[read_date] [datetime] NULL,
	[is_deleted] [bit] NULL,
	[deleted_by] [uniqueidentifier] NULL,
	[deleted_at] [datetime] NULL,
 CONSTRAINT [PK_gnl_ileti_gelen] PRIMARY KEY CLUSTERED 
(
	[message_uid] ASC,
	[user_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[gnl_message_inbox]  WITH CHECK ADD  CONSTRAINT [FK_BaseiletiInbox_Baseileti] FOREIGN KEY([message_uid])
REFERENCES [dbo].[gnl_message] ([message_uid])
GO

ALTER TABLE [dbo].[gnl_message_inbox] CHECK CONSTRAINT [FK_BaseiletiInbox_Baseileti]
GO


CREATE TABLE [dbo].[gnl_message_recipient](
	[message_uid] [uniqueidentifier] NOT NULL,
	[user_uid] [uniqueidentifier] NOT NULL,
	[recipient_rank] [smallint] NOT NULL,
	[recipient_type] [char](1) NOT NULL,
 CONSTRAINT [PK_gnl_ileti_alicilar] PRIMARY KEY CLUSTERED 
(
	[message_uid] ASC,
	[user_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[gnl_message_recipient]  WITH CHECK ADD  CONSTRAINT [FK_BaseiletiUserRelation_Baseileti] FOREIGN KEY([message_uid])
REFERENCES [dbo].[gnl_message] ([message_uid])
GO

ALTER TABLE [dbo].[gnl_message_recipient] CHECK CONSTRAINT [FK_BaseiletiUserRelation_Baseileti]
GO

CREATE TABLE [dbo].[gnl_notification](
	[notification_uid] [uniqueidentifier] NOT NULL,
	[notification_date] [datetime] NULL,
	[notification_subject] [nvarchar](4000) NULL,
	[notification] [text] NULL,
	[notification_created_uid] [uniqueidentifier] NULL,
	[notification_creation_date] [datetime] NULL,
	[notification_statu] [int] NULL,
 CONSTRAINT [PK_gnl_duyurular] PRIMARY KEY CLUSTERED 
(
	[notification_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[gnl_ref_cinsiyet](
	[cinsiyet_id] [int] NOT NULL,
	[cinsiyet] [nvarchar](50) NULL,
 CONSTRAINT [PK_gnl_ref_cinsiyet] PRIMARY KEY CLUSTERED 
(
	[cinsiyet_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [gnl_ref_cinsiyet] ([cinsiyet_id],[cinsiyet])VALUES(1,'Female')
INSERT INTO [gnl_ref_cinsiyet] ([cinsiyet_id],[cinsiyet])VALUES(2,'Male')

GO

CREATE TABLE [dbo].[gnl_ref_uyelik_odeme_sekli](
	[odeme_sekli_id] [int] NOT NULL,
	[odeme_sekli] [nvarchar](200) NULL,
 CONSTRAINT [PK_gnl_ref_uyelik_odeme_sekli] PRIMARY KEY CLUSTERED 
(
	[odeme_sekli_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [gnl_ref_uyelik_odeme_sekli] ([odeme_sekli_id],[odeme_sekli])VALUES(1,'Banka Havale')
INSERT INTO [gnl_ref_uyelik_odeme_sekli] ([odeme_sekli_id],[odeme_sekli])VALUES(2,'Kredi Kartı')

GO

CREATE TABLE [dbo].[gnl_ref_uyelik_tipleri](
	[uyelik_tipi_id] [int] NOT NULL,
	[uyelik_tipi] [nvarchar](1000) NULL,
 CONSTRAINT [PK_gnl_ref_uyelik_tipleri] PRIMARY KEY CLUSTERED 
(
	[uyelik_tipi_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [gnl_ref_uyelik_tipleri] ([uyelik_tipi_id],[uyelik_tipi])VALUES(1,'Free')
INSERT INTO [gnl_ref_uyelik_tipleri] ([uyelik_tipi_id],[uyelik_tipi])VALUES(2,'Ücretli Kullanıcı')

GO

CREATE TABLE [dbo].[gnl_suggest_to_friend](
	[id] [uniqueidentifier] NOT NULL,
	[suggesting_user_uid] [uniqueidentifier] NULL,
	[suggest_date] [datetime] NULL,
	[suggest_email] [nvarchar](200) NULL,
	[suggesting_message] [nvarchar](4000) NULL,
 CONSTRAINT [PK_gnl_arkadasima_oner] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[gnl_users](
	[user_uid] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](100) NULL,
	[second_name] [nvarchar](100) NULL,
	[surname] [nvarchar](100) NULL,
	[password] [nvarchar](50) NULL,
	[email] [nvarchar](200) NULL,
	[group_uid] [uniqueidentifier] NULL,
	[group_name] [nvarchar](500) NULL,
	[active] [bit] NULL,
	[activation_key] [nvarchar](50) NULL,
	[activation_ok] [bit] NULL,
	[is_system_admin] [bit] NULL,
	[dont_activate_purchase] [bit] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [nchar](10) NULL,
	[membership_conditions] [text] NULL,
	[survey_responsibilities] [text] NULL,
 CONSTRAINT [PK_BaseUsers] PRIMARY KEY CLUSTERED 
(
	[user_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

INSERT INTO [gnl_users] ([user_uid],[name],[second_name],[surname],[password],[email],[group_uid],[group_name],[active],[activation_key],[activation_ok],[is_system_admin],[dont_activate_purchase],[inserted_by],[inserted_at],[updated_by],[updated_at],[membership_conditions],[survey_responsibilities])VALUES('A80B5F72-FEB2-4621-BE3F-99A55F59F9E9','Demo',NULL,'User','/OqSD3QStdp74M9CuMk3WQ==','demouser@dcmteknoloji.com','4403BC9D-E08D-4EDD-AD7B-AB1F030B70C8','Survey',1,'0f739d85eda64520b858',1,1,NULL,'00000000-0000-0000-0000-000000000000',GETDATE(),'00000000-0000-0000-0000-000000000000',GETDATE(),'','')
GO

CREATE TABLE [dbo].[gnl_uye_kullanicilar](
	[id] [uniqueidentifier] NOT NULL,
	[user_uid] [uniqueidentifier] NULL,
	[ad] [nvarchar](100) NULL,
	[soyad] [nvarchar](100) NULL,
	[telefonu] [nvarchar](50) NULL,
	[cep_telefonu] [nvarchar](50) NULL,
	[adres] [text] NULL,
	[email] [nvarchar](200) NULL,
	[grup_uid] [uniqueidentifier] NULL,
	[grup_adi] [nvarchar](500) NULL,
	[dogum_tarihi] [datetime] NULL,
	[cinsiyet] [int] NULL,
	[sirket_adi] [nvarchar](500) NULL,
	[vergi_dairesi] [nvarchar](1000) NULL,
	[vergi_no] [nvarchar](200) NULL,
	[grup_sirket_tipi_id] [int] NULL,
	[aktif] [bit] NULL,
	[uye_baslangic_tarihi] [datetime] NULL,
	[uye_bitis_tarihi] [datetime] NULL,
	[son_odeme_tipi_id] [int] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [nchar](10) NULL,
	[kalan_anket_sayisi] [int] NULL,
 CONSTRAINT [PK_gnl_uye_kullanicilar] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

INSERT INTO [gnl_uye_kullanicilar] ([id],[user_uid],[ad],[soyad],[telefonu],[cep_telefonu],[adres],[email],[grup_uid],[grup_adi],[dogum_tarihi],[cinsiyet],[sirket_adi],[vergi_dairesi],[vergi_no],[grup_sirket_tipi_id],[aktif],[uye_baslangic_tarihi],[uye_bitis_tarihi],[son_odeme_tipi_id],[inserted_by],[inserted_at],[updated_by],[updated_at],[kalan_anket_sayisi])VALUES('73026C4C-54A0-4714-811A-35943510D150','A80B5F72-FEB2-4621-BE3F-99A55F59F9E9','Admin','Admin','','','Survey','demouser@dcmteknoloji.com','4403bc9d-e08d-4edd-ad7b-ab1f030b70c8','Survey',NULL,NULL,NULL,NULL,NULL,1,'True',getdate(),getdate(),1,'a80b5f72-feb2-4621-be3f-99a55f59f9e9',getdate(),'a80b5f72-feb2-4621-be3f-99a55f59f9e9',NULL,10000000) 

GO



CREATE TABLE [dbo].[gnl_uyelik_aktivasyon_mail_gonderi_tarihcesi](
	[id] [uniqueidentifier] NOT NULL,
	[paket_uid] [uniqueidentifier] NULL,
	[gonderilen_ad] [nvarchar](100) NULL,
	[gonderilen_soyad] [nvarchar](100) NULL,
	[gonderilen_email] [nvarchar](200) NULL,
	[gonderilme_tarihi] [datetime] NULL,
	[gonderen_user_uid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_sbr_uyelik_aktivasyon_mail_gonderi_tarihcesi] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi](
	[id] [uniqueidentifier] NOT NULL,
	[user_uid] [uniqueidentifier] NULL,
	[gonderilen_ad] [nvarchar](100) NULL,
	[gonderilen_soyad] [nvarchar](100) NULL,
	[gonderilen_email] [nvarchar](200) NULL,
	[gonderilme_tarihi] [datetime] NULL,
	[gonderen_user_uid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[gnl_uyelik_odeme_tipleri_tanimlari](
	[odeme_tipi_id] [int] NOT NULL,
	[uyelik_tip_id] [int] NULL,
	[odeme_tipi] [nvarchar](1000) NULL,
	[anket_sayisi] [int] NULL,
	[katilimci_sayisi] [int] NULL,
	[sure_ay] [int] NULL,
	[ucret] [decimal](18, 2) NULL,
 CONSTRAINT [PK_gnl_uyelik_odeme_tipleri_tanimlari] PRIMARY KEY CLUSTERED 
(
	[odeme_tipi_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [gnl_uyelik_odeme_tipleri_tanimlari] ([odeme_tipi_id],[uyelik_tip_id],[odeme_tipi],[anket_sayisi],[katilimci_sayisi],[sure_ay],[ucret])VALUES(1,1,'Free',10000000,10000000,NULL,NULL)

GO

CREATE TABLE [dbo].[gnl_uyelik_paket_alimlari](
	[id] [uniqueidentifier] NOT NULL,
	[user_uid] [uniqueidentifier] NULL,
	[grup_uid] [uniqueidentifier] NULL,
	[odeme_tipi_id] [int] NULL,
	[paket_alim_tarihi] [datetime] NULL,
	[paket_fiyati] [decimal](18, 2) NULL,
	[anket_sayisi] [int] NULL,
	[anket_basina_katilimci_sayisi] [int] NULL,
	[paket_suresi] [int] NULL,
	[uyelik_bitis_tarihi] [datetime] NULL,
	[telefonu] [nvarchar](50) NULL,
	[cep_telefonu] [nvarchar](50) NULL,
	[adres] [text] NULL,
	[sirket_adi] [nvarchar](500) NULL,
	[vergi_dairesi] [nvarchar](1000) NULL,
	[vergi_no] [nvarchar](200) NULL,
	[grup_sirket_tipi_id] [int] NULL,
	[odeme_sekli_id] [int] NULL,
	[aktive_edildi] [bit] NULL,
	[kabul_edildi] [bit] NULL,
	[UyelikKosullari] [text] NULL,
	[SurveyResponsibilities] [text] NULL,
	[fatura_kesildi] [bit] NULL,
	[islem_id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_gnl_uyelik_paket_alimlari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[log_mail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[kime] [nvarchar](500) NULL,
	[kimden] [nvarchar](500) NULL,
	[gonderen_aktif_user_name] [nvarchar](500) NULL,
	[tip] [nvarchar](50) NULL,
	[subject] [nvarchar](4000) NULL,
	[mesaj] [nvarchar](4000) NULL,
	[tarih] [datetime] NULL,
 CONSTRAINT [PK_log_mail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_anket](
	[anket_uid] [uniqueidentifier] NOT NULL,
	[grup_uid] [uniqueidentifier] NULL,
	[kullanici_uid] [uniqueidentifier] NULL,
	[anket_adi] [nvarchar](4000) NULL,
	[anket_goruntulenen_ad] [nvarchar](4000) NULL,
	[anket_goruntulenen_ad_ekle] [bit] NULL,
	[kategori_id] [int] NULL,
	[anket_tipi_id] [int] NULL,
	[anket_mesaji] [text] NULL,
	[anket_mesaji_ekle] [bit] NULL,
	[anket_sonuc_mesaji] [text] NULL,
	[anket_sonuc_mesaji_ekle] [bit] NULL,
	[baslangic_tarihi] [datetime] NULL,
	[bitis_tarihi] [datetime] NULL,
	[olusturma_tarihi] [datetime] NULL,
	[olusturan_kullanici_uid] [uniqueidentifier] NULL,
	[anket_durumu_id] [int] NULL,
	[anket_durumu_uid] [uniqueidentifier] NULL,
	[ip_kisitlamasi] [bit] NULL,
	[sayfadaki_soru_sayisi] [int] NULL,
	[soru_cevaplama_zorunlulugu] [int] NULL,
	[anket_tema_id] [int] NULL,
	[logo] [image] NULL,
	[bitis_tarihinde_anketi_kaldir] [bit] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_sbr_anket] PRIMARY KEY CLUSTERED 
(
	[anket_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[sbr_anket_davet](
	[davet_uid] [uniqueidentifier] NOT NULL,
	[davet_eden_kullanici_uid] [uniqueidentifier] NULL,
	[davet_edilen_grup_uid] [uniqueidentifier] NULL,
	[davet_edilen_email] [nvarchar](200) NULL,
	[davet_key] [nvarchar](50) NULL,
	[davet_tarihi] [datetime] NULL,
	[davet_kabul_edildi] [bit] NULL,
	[davet_kabul_edilme_tarihi] [datetime] NULL,
	[davet_kabul_eden_kullanici_uid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_sbr_anket_davet] PRIMARY KEY CLUSTERED 
(
	[davet_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_anket_durum_tarihcesi](
	[anket_durumu_uid] [uniqueidentifier] NOT NULL,
	[anket_uid] [uniqueidentifier] NULL,
	[anket_durumu_id] [int] NULL,
	[durumu_olusturan_kullanici] [uniqueidentifier] NULL,
	[durum_olusma_tarihi] [datetime] NULL,
	[durum_aciklamasi] [nvarchar](4000) NULL,
 CONSTRAINT [PK_sbr_anket_durum_tarihcesi] PRIMARY KEY CLUSTERED 
(
	[anket_durumu_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_anket_sorulari](
	[soru_uid] [uniqueidentifier] NOT NULL,
	[anket_uid] [uniqueidentifier] NULL,
	[soru_tipi_id] [int] NULL,
	[soru] [text] NULL,
	[soru_sira_no] [int] NULL,
	[soru_sitili_id] [int] NULL,
	[soru_siralama_sekli_id] [int] NULL,
	[soru_zorunlu] [bit] NULL,
	[soru_sayisal_ondalik] [bit] NULL,
	[soru_olusturma_tarihi] [datetime] NULL,
	[soru_olusturan_kullanici_uid] [uniqueidentifier] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [datetime] NULL,
	[soru_tek_satir] [bit] NULL,
 CONSTRAINT [PK_sbr_anket_sorulari] PRIMARY KEY CLUSTERED 
(
	[soru_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[sbr_anket_test_mail_gonderi_kisi_tarihcesi](
	[id] [uniqueidentifier] NOT NULL,
	[anket_uid] [uniqueidentifier] NULL,
	[gonderi_tarihi] [datetime] NULL,
	[gonderen_user_uid] [uniqueidentifier] NULL,
	[gonderilen_email_adresi] [nvarchar](500) NULL,
	[anket_test_key] [nvarchar](50) NULL,
	[anket_test_sonucu_id] [int] NULL,
	[anket_test_sonucu] [text] NULL,
 CONSTRAINT [PK_sbr_anket_test_mail_gonderi_kisi_tarihcesi] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_anket_yayinlama_acik_anket_aktivasyon](
	[id] [uniqueidentifier] NOT NULL,
	[anket_uid] [uniqueidentifier] NULL,
	[giris_key] [nvarchar](50) NULL,
	[anket_bitirildi] [bit] NULL,
	[anket_bitirilme_tarihi] [datetime] NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[versiyon] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_anket_yayinlama_acik_anket_aktivasyon] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sbr_anket_yayinlama_mail_gonderi_aktivasyon](
	[id] [uniqueidentifier] NOT NULL,
	[anket_uid] [uniqueidentifier] NULL,
	[tarihce_uid] [uniqueidentifier] NULL,
	[anket_gonderilen_ad] [nvarchar](100) NULL,
	[anket_gonderilen_soyad] [nvarchar](100) NULL,
	[anket_gonderilen_email] [nvarchar](200) NULL,
	[anket_gonderilme_tarihi] [datetime] NULL,
	[anket_gonderilme_key] [nvarchar](50) NULL,
	[ankete_girildi] [bit] NULL,
	[ankete_giris_tarihi] [datetime] NULL,
	[anket_bitirildi] [bit] NULL,
	[anket_bitirilme_tarihi] [datetime] NULL,
	[ankete_cevap_verildi] [bit] NULL,
	[ankete_cevap_verilme_tarihi] [datetime] NULL,
	[anket_mail_grubu_uid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_sbr_anket_yayinlama_mail_gonderi_aktivasyon] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi](
	[id] [uniqueidentifier] NOT NULL,
	[tarihce_uid] [uniqueidentifier] NULL,
	[gonderilen_kisi_ismi] [nvarchar](200) NULL,
	[gonderilen_email_adresi] [nvarchar](500) NULL,
 CONSTRAINT [PK_sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[sbr_anket_yayinlama_mail_gonderi_tarihcesi](
	[tarihce_uid] [uniqueidentifier] NOT NULL,
	[anket_uid] [uniqueidentifier] NULL,
	[gonderilen_mail_grubu_uid] [uniqueidentifier] NULL,
	[gonderi_tarihi] [datetime] NULL,
	[gonderen_user_uid] [uniqueidentifier] NULL,
	[gonderilen_mail_konusu] [nvarchar](4000) NULL,
	[gonderilen_mail] [text] NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_anket_yayinlama_mail_gonderi_tarihcesi] PRIMARY KEY CLUSTERED 
(
	[tarihce_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_mail_gruplari](
	[mail_grubu_uid] [uniqueidentifier] NOT NULL,
	[grup_uid] [uniqueidentifier] NULL,
	[mail_grubu_adi] [nvarchar](500) NULL,
	[olusturan_kullanici_uid] [uniqueidentifier] NULL,
	[olusturma_tarihi] [datetime] NULL,
	[is_deleted] [bit] NULL,
	[deleted_date] [datetime] NULL,
 CONSTRAINT [PK_sbr_mail_gruplari] PRIMARY KEY CLUSTERED 
(
	[mail_grubu_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sbr_mail_gruplari_dosyalari](
	[id] [uniqueidentifier] NOT NULL,
	[dosya_ekleyen_user_uid] [uniqueidentifier] NULL,
	[dosya_eklenma_tarihi] [datetime] NULL,
	[dosya_uzanti_adi] [nvarchar](50) NULL,
	[dosya] [image] NULL,
	[mail_grubu_uid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_sbr_mail_gruplari_dosyalari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[sbr_mail_gruplari_kullanici_listesi](
	[id] [uniqueidentifier] NOT NULL,
	[mail_grubu_uid] [uniqueidentifier] NULL,
	[ad] [nvarchar](200) NULL,
	[soyad] [nvarchar](200) NULL,
	[email] [nvarchar](500) NULL,
	[is_deleted] [bit] NULL,
	[deleted_date] [datetime] NULL,
	[olusturma_tarihi] [datetime] NULL,
	[olusturan_kullanici_uid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_sbr_mail_gruplari_kullanici_listesi] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_ref_anket_durumu](
	[anket_durumu_id] [int] NOT NULL,
	[anket_durumu] [nvarchar](500) NULL,
 CONSTRAINT [PK_sbr_ref_anket_durumu] PRIMARY KEY CLUSTERED 
(
	[anket_durumu_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_anket_durumu] ([anket_durumu_id],[anket_durumu])VALUES(0,'All')
INSERT INTO [sbr_ref_anket_durumu] ([anket_durumu_id],[anket_durumu])VALUES(1,'Open')
INSERT INTO [sbr_ref_anket_durumu] ([anket_durumu_id],[anket_durumu])VALUES(2,'Close')
INSERT INTO [sbr_ref_anket_durumu] ([anket_durumu_id],[anket_durumu])VALUES(3,'Archive')
INSERT INTO [sbr_ref_anket_durumu] ([anket_durumu_id],[anket_durumu])VALUES(4,'Live')

GO

CREATE TABLE [dbo].[sbr_ref_anket_kategori](
	[kategori_id] [int] NOT NULL,
	[kategori_kodu] [nvarchar](500) NULL,
	[kategori_adi] [nvarchar](1000) NULL,
 CONSTRAINT [PK_sbr_ref_anket_kategori] PRIMARY KEY CLUSTERED 
(
	[kategori_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_anket_kategori] ([kategori_id],[kategori_kodu],[kategori_adi])VALUES(1,'Customer','Customer')
INSERT INTO [sbr_ref_anket_kategori] ([kategori_id],[kategori_kodu],[kategori_adi])VALUES(2,'Quality','Quality')
INSERT INTO [sbr_ref_anket_kategori] ([kategori_id],[kategori_kodu],[kategori_adi])VALUES(3,'Education','Education')

GO

CREATE TABLE [dbo].[sbr_ref_anket_temalari](
	[anket_tema_id] [int] NOT NULL,
	[anket_tema_name] [nvarchar](200) NULL,
 CONSTRAINT [PK_sbr_ref_anket_temalari] PRIMARY KEY CLUSTERED 
(
	[anket_tema_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_anket_temalari] ([anket_tema_id],[anket_tema_name])VALUES(1,'AnketTema1.html')
INSERT INTO [sbr_ref_anket_temalari] ([anket_tema_id],[anket_tema_name])VALUES(2,'AnketTema2.html')
INSERT INTO [sbr_ref_anket_temalari] ([anket_tema_id],[anket_tema_name])VALUES(3,'AnketTema3.html')

GO

CREATE TABLE [dbo].[sbr_ref_anket_test_sonucu](
	[anket_test_sonucu_id] [int] NOT NULL,
	[anket_test_sonucu] [nvarchar](200) NULL,
 CONSTRAINT [PK_sbr_ref_anket_test_sonucu] PRIMARY KEY CLUSTERED 
(
	[anket_test_sonucu_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_anket_test_sonucu] ([anket_test_sonucu_id],[anket_test_sonucu])VALUES(1,'Positive')
INSERT INTO [sbr_ref_anket_test_sonucu] ([anket_test_sonucu_id],[anket_test_sonucu])VALUES(2,'Negative')

GO

CREATE TABLE [dbo].[sbr_ref_anket_tipi](
	[anket_tipi_id] [int] NOT NULL,
	[anket_tipi] [nvarchar](100) NULL,
 CONSTRAINT [PK_sbr_ref_anket_tipi] PRIMARY KEY CLUSTERED 
(
	[anket_tipi_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_anket_tipi] ([anket_tipi_id],[anket_tipi])VALUES(2,'Open Survey')

GO

CREATE TABLE [dbo].[sbr_ref_sablon_durumu](
	[sablon_durumu_id] [int] NOT NULL,
	[sablon_durumu] [nvarchar](500) NULL,
 CONSTRAINT [PK_sbr_ref_sablon_durumu] PRIMARY KEY CLUSTERED 
(
	[sablon_durumu_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_sablon_durumu] ([sablon_durumu_id],[sablon_durumu])VALUES(1,'Open')
INSERT INTO [sbr_ref_sablon_durumu] ([sablon_durumu_id],[sablon_durumu])VALUES(2,'Close')

GO


CREATE TABLE [dbo].[sbr_ref_soru_siralama_sekli](
	[soru_siralama_sekli_id] [int] NOT NULL,
	[soru_siralama_sekli] [nchar](10) NULL,
 CONSTRAINT [PK_sbr_ref_soru_siralama_sekli] PRIMARY KEY CLUSTERED 
(
	[soru_siralama_sekli_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_soru_siralama_sekli] ([soru_siralama_sekli_id],[soru_siralama_sekli])VALUES(1,'Yatay')
INSERT INTO [sbr_ref_soru_siralama_sekli] ([soru_siralama_sekli_id],[soru_siralama_sekli])VALUES(2,'Düşey')

GO

CREATE TABLE [dbo].[sbr_ref_soru_sitili](
	[soru_sitili_id] [int] NOT NULL,
	[soru_sitili] [nvarchar](500) NULL,
 CONSTRAINT [PK_sbr_ref_soru_sitili] PRIMARY KEY CLUSTERED 
(
	[soru_sitili_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_soru_sitili] ([soru_sitili_id],[soru_sitili])VALUES(1,'Sıralı')
INSERT INTO [sbr_ref_soru_sitili] ([soru_sitili_id],[soru_sitili])VALUES(2,'Liste')

GO

CREATE TABLE [dbo].[sbr_ref_soru_tipi](
	[soru_tipi_id] [int] NOT NULL,
	[soru_tipi_kodu] [nvarchar](1000) NULL,
	[soru_tipi_adi] [nvarchar](1000) NULL,
 CONSTRAINT [PK_sbr_ref_soru_tipi] PRIMARY KEY CLUSTERED 
(
	[soru_tipi_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(1,'Radio Button Select','Radio Button Select')
INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(2,'Check Box Multi Select','Check Box Multi Select')
INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(3,'Matrix','Matrix')
INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(4,'Text','Text')
INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(7,'Multiple Text','Multiple Text')
INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(8,'Numeric','Numeri')
INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(9,'Date','Date')
INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(10,'Phone','Phone')
INSERT INTO [sbr_ref_soru_tipi] ([soru_tipi_id],[soru_tipi_kodu],[soru_tipi_adi])VALUES(11,'E-Mail','E-Mail')

GO

CREATE TABLE [dbo].[sbr_sablon](
	[sablon_uid] [uniqueidentifier] NOT NULL,
	[grup_uid] [uniqueidentifier] NULL,
	[kullanici_uid] [uniqueidentifier] NULL,
	[sablon_adi] [nvarchar](4000) NULL,
	[kategori_id] [int] NULL,
	[soru_sayisal_ondalik] [bit] NULL,
	[olusturma_tarihi] [datetime] NULL,
	[olusturan_kullanici_uid] [uniqueidentifier] NULL,
	[sablon_durumu_id] [int] NULL,
	[sablon_durumu_uid] [uniqueidentifier] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_sbr_sablon] PRIMARY KEY CLUSTERED 
(
	[sablon_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_sablon_durum_tarihcesi](
	[sablon_durumu_uid] [uniqueidentifier] NOT NULL,
	[sablon_uid] [uniqueidentifier] NULL,
	[sablon_durumu_id] [int] NULL,
	[durumu_olusturan_kullanici] [uniqueidentifier] NULL,
	[durum_olusma_tarihi] [datetime] NULL,
	[durum_aciklamasi] [nvarchar](4000) NULL,
 CONSTRAINT [PK_sbr_sablon_durum_tarihcesi] PRIMARY KEY CLUSTERED 
(
	[sablon_durumu_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_soru_tipi_1_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_1_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_soru_tipi_10_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_10_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_sablon_soru_tipi_11_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_11_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_soru_tipi_2_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_2_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_soru_tipi_3_secenek_kolonlari](
	[soru_secenek_kolon_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_kolon_ad] [text] NULL,
	[soru_secenek_kolon_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_3_secenek_kolonlari] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_kolon_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_sablon_soru_tipi_3_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_3_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_sablon_soru_tipi_4_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_4_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_soru_tipi_5_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_5_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_soru_tipi_6_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_6_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_sablon_soru_tipi_7_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_7_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_soru_tipi_8_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_8_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_soru_tipi_9_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_sablon_soru_tipi_9_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_sablon_sorulari](
	[soru_uid] [uniqueidentifier] NOT NULL,
	[sablon_uid] [uniqueidentifier] NULL,
	[soru_tipi_id] [int] NULL,
	[soru] [text] NULL,
	[soru_sira_no] [int] NULL,
	[soru_sitili_id] [int] NULL,
	[soru_siralama_sekli_id] [int] NULL,
	[soru_zorunlu] [bit] NULL,
	[soru_sayisal_ondalik] [bit] NULL,
	[soru_olusturma_tarihi] [datetime] NULL,
	[soru_olusturan_kullanici_uid] [uniqueidentifier] NULL,
	[inserted_by] [uniqueidentifier] NULL,
	[inserted_at] [datetime] NULL,
	[updated_by] [uniqueidentifier] NULL,
	[updated_at] [datetime] NULL,
	[soru_tek_satir] [bit] NULL,
 CONSTRAINT [PK_sbr_sablon_sorulari] PRIMARY KEY CLUSTERED 
(
	[soru_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_1_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [bit] NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_cbr_soru_tipi_1_cavaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sbr_soru_tipi_1_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_1_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_10_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [nvarchar](50) NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_10_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_soru_tipi_10_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_10_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_soru_tipi_11_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [nvarchar](200) NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_11_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_11_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_11_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_2_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [bit] NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_2_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_2_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_2_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_3_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[soru_secenek_kolon_uid] [uniqueidentifier] NULL,
	[cevap] [bit] NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_3_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_3_secenek_kolonlari](
	[soru_secenek_kolon_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_kolon_ad] [text] NULL,
	[soru_secenek_kolon_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_3_secenek_kolonlari] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_kolon_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_3_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_3_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_4_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [text] NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_4_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_4_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_4_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_5_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [bit] NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_5_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_5_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_5_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_soru_tipi_6_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [bit] NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_6_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_6_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_6_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_soru_tipi_7_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [text] NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_7_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_7_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_7_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_soru_tipi_8_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [decimal](18, 2) NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_8_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_8_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_8_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[sbr_soru_tipi_9_cevaplari](
	[id] [uniqueidentifier] NOT NULL,
	[cevap_key] [nvarchar](50) NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_uid] [uniqueidentifier] NULL,
	[cevap] [datetime] NULL,
	[cevaplayan_ad] [nvarchar](100) NULL,
	[cevaplayan_soyad] [nvarchar](100) NULL,
	[cevaplayan_email] [nvarchar](200) NULL,
	[cavaplama_tarihi] [datetime] NULL,
	[session_id] [nvarchar](50) NULL,
	[host_name] [nvarchar](50) NULL,
	[user_host_adres] [nvarchar](50) NULL,
	[browser] [nvarchar](50) NULL,
	[platform] [nvarchar](50) NULL,
	[version] [nvarchar](50) NULL,
 CONSTRAINT [PK_sbr_soru_tipi_9_cevaplari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[sbr_soru_tipi_9_secenekleri](
	[soru_secenek_uid] [uniqueidentifier] NOT NULL,
	[soru_uid] [uniqueidentifier] NULL,
	[soru_secenek_ad] [text] NULL,
	[soru_secenek_sira_no] [int] NULL,
 CONSTRAINT [PK_sbr_soru_tipi_9_secenekleri] PRIMARY KEY CLUSTERED 
(
	[soru_secenek_uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

--Views--

CREATE FUNCTION [dbo].[gnl_ileti_alici_isimleri] 
(
	@ileti_uid uniqueidentifier,
	@alici_tipi char(1)
)
RETURNS nvarchar(2000)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @isimler nvarchar(2000)
DECLARE @isimler2 nvarchar(2000)
DECLARE @return nvarchar(2000)
	
	
	
	select @isimler=  
	  STUFF(
		(
		SELECT
		  ',' + ad +' '+soyad
		FROM dbo.gnl_uye_kullanicilar kullanicilar
		inner join dbo.gnl_ileti_alicilar alicilar
			on alicilar.ileti_uid=@ileti_uid
				and alici_tipi=@alici_tipi
				and alicilar.kullanici_uid = kullanicilar.user_uid
		FOR XML PATH('')
		), 1, 1, ''
	  ) 
	  
	 select @isimler2=
	  STUFF(
		(
		SELECT
		  ',' + ad +' '+soyad
		FROM dbo.gnl_kullanicilar kullanicilar
		inner join dbo.gnl_ileti_alicilar alicilar
			on alicilar.ileti_uid=@ileti_uid
				and alici_tipi=@alici_tipi
				and alicilar.kullanici_uid = kullanicilar.user_uid
		FOR XML PATH('')
		), 1, 1, ''
	  ) 
	
	IF (isnull(@isimler,'')='')
	BEGIN	
	set	@return =isnull(@isimler2,'')
	END
	ELSE
	BEGIN
	set	@return =isnull(@isimler,'')
	END
	
	RETURN  @return

END
GO

CREATE VIEW [dbo].[gnl_ileti_gelen_kutusu_v]
AS
SELECT        TOP (100) PERCENT gelen.message_uid, gelen.user_uid, gelen.recipient_type, gelen.message_is_read, gelen.read_date, gelen.is_deleted, ileti.message_subject, ileti.message, 
                         gonderen_kullanici.ad + ' ' + gonderen_kullanici.soyad AS gonderen_adi, ileti.sent_date, 'subject record' AS link_adi, CASE WHEN len(CAST(DATEPART(hour, ileti.sent_date) AS varchar(2))) = 2 THEN CAST(DATEPART(hour, 
                         ileti.sent_date) AS varchar(2)) ELSE '0' + CAST(DATEPART(hour, ileti.sent_date) AS varchar(2)) END + ':' + CASE WHEN LEN(CAST(DATEPART(minute, ileti.sent_date) AS varchar(2))) = 2 THEN CAST(DATEPART(minute, 
                         ileti.sent_date) AS varchar(2)) ELSE '0' + CAST(DATEPART(minute, ileti.sent_date) AS varchar(2)) END AS zamani, dbo.gnl_ileti_alici_isimleri(ileti.message_uid, 'T') AS to_alici_isimleri, 
                         CASE WHEN (gonderen_kullanici.ad IS NULL AND gonderen_kullanici.soyad IS NULL) THEN
                             (SELECT        ad + ' ' + soyad
                               FROM            gnl_users
                               WHERE        user_uid = ileti.send_user_uid) ELSE gonderen_kullanici.ad + ' ' + gonderen_kullanici.soyad END AS mesaj_ad
FROM            dbo.gnl_message AS ileti LEFT OUTER JOIN
                         dbo.gnl_message_inbox AS gelen ON ileti.message_uid = gelen.message_uid LEFT OUTER JOIN
                         dbo.gnl_uye_kullanicilar AS gonderen_kullanici ON ileti.send_user_uid = gonderen_kullanici.user_uid
ORDER BY ileti.sent_date DESC
GO





CREATE VIEW [dbo].[gnl_ileti_gelen_kutusu_groupby_v]
AS
SELECT        user_uid, 'Not Readed Messages (' + CONVERT(varchar(20), COUNT(*)) + ')' AS kayit
FROM            dbo.gnl_ileti_gelen_kutusu_v
WHERE        (ISNULL(is_deleted, 0) = 0) AND (ISNULL(message_is_read, 0) = 0)
GROUP BY user_uid
GO



CREATE view [dbo].[BaseLoggedInUsersList]
as
select top 100 percent 
	id
	,user_name as KullanıcıAdı
	,user_name_surname as AdSoyad
	,session_id as Session
	,host_name as Host
	,user_host_adres as UserHost
	,browser as Browser
	,platform as Platform
	,version as Version
	,convert(varchar(200),date,120) as Login
	,(select top 1 convert(varchar(200),date,120) from BaseLoggedInUsers table2 where table2.session_id=table1.session_id and table2.type='logout') as Logout 
from BaseLoggedInUsers table1
where table1.type='login'
order by table1.date desc, id



go


CREATE VIEW [dbo].[gnl_ileti_giden_kutusu_v]
AS
SELECT DISTINCT 
                         ileti.message_uid, ileti.message, gonderen.ad + ' ' + gonderen.soyad AS gonderen_adi, ileti.message_subject, ileti.message AS Expr1, ileti.sent_date, ileti.is_deleted, ileti.deleted_at, ileti.deleted_by, ileti.send_user_uid, 
                         CASE WHEN len(CAST(DATEPART(hour, ileti.sent_date) AS varchar(2))) = 2 THEN CAST(DATEPART(hour, ileti.sent_date) AS varchar(2)) ELSE '0' + CAST(DATEPART(hour, ileti.sent_date) AS varchar(2)) 
                         END + ':' + CASE WHEN LEN(CAST(DATEPART(minute, ileti.sent_date) AS varchar(2))) = 2 THEN CAST(DATEPART(minute, ileti.sent_date) AS varchar(2)) ELSE '0' + CAST(DATEPART(minute, ileti.sent_date) AS varchar(2)) 
                         END AS zamani, dbo.gnl_ileti_alici_isimleri(ileti.message_uid, 'T') AS to_alici_isimleri, dbo.gnl_ileti_alici_isimleri(ileti.message_uid, 'T') AS mesaj_ad, dbo.gnl_message_recipient.recipient_type
FROM            dbo.gnl_message AS ileti LEFT OUTER JOIN
                         dbo.gnl_uye_kullanicilar AS gonderen ON ileti.send_user_uid = gonderen.user_uid LEFT OUTER JOIN
                         dbo.gnl_message_recipient ON ileti.message_uid = dbo.gnl_message_recipient.message_uid
GO



CREATE VIEW [dbo].[gnl_kullanici_gruplari_v]
AS
SELECT        dbo.gnl_user_groups.group_uid, dbo.gnl_user_groups.group_uid AS Expr1, dbo.gnl_user_groups.active, dbo.gnl_group_user_definitions.user_uid, dbo.gnl_group_user_definitions.active AS aktif_kullanici, 
                         dbo.gnl_group_user_definitions.is_admin, dbo.gnl_group_user_definitions.is_user_admin, 
                         CASE WHEN is_admin = 1 THEN 'Manager' ELSE CASE WHEN is_user_admin = 1 THEN 'Member - Manager' ELSE 'Member' END END AS admin, dbo.gnl_users.name, dbo.gnl_users.surname, dbo.gnl_users.email
FROM            dbo.gnl_user_groups INNER JOIN
                         dbo.gnl_group_user_definitions ON dbo.gnl_user_groups.group_uid = dbo.gnl_group_user_definitions.group_uid LEFT OUTER JOIN
                         dbo.gnl_users ON dbo.gnl_group_user_definitions.user_uid = dbo.gnl_users.user_uid
GO


CREATE VIEW [dbo].[gnl_kullanicilar_v]
AS
SELECT        user_uid, name, surname, email, group_name, active, is_system_admin
FROM            dbo.gnl_users
GO

CREATE FUNCTION [dbo].[GetDateFaormatedString](@Date datetime,@show_hour bit)
RETURNS varchar(30)
AS
BEGIN
	declare @day	varchar(2)
	declare @month	varchar(2)
	declare @year	varchar(4)
	declare @hour varchar(2)
	declare @minute varchar(2)
	declare @result varchar(30)

    set @day = cast(day(@Date) as varchar(2))
    set @month = cast(month(@Date) as varchar(2))
    set @year = cast(year(@Date) as varchar(4))
    set @hour = cast(datepart(hour,@Date) as varchar(2))
	set @minute = cast(datepart(minute,@Date) as varchar(2))
	
	IF LEN(@day)=1
	BEGIN
	  set @day='0'+@day
	END
	
	IF LEN(@month)=1
	BEGIN
	  set @month='0'+@month
	END
	
	IF LEN(@hour)=1
	BEGIN
	  set @hour='0'+@hour
	END	
	
	IF LEN(@minute)=1
	BEGIN
	  set @minute='0'+@minute
	END
	
	IF @show_hour=1
	BEGIN
		set @result = @day+'.'+@month+'.'+@year+' '+@hour+':'+@minute
	END
	ELSE
	BEGIN
		set @result = @day+'.'+@month+'.'+@year
	END
	
	return @result
END



GO


CREATE VIEW [dbo].[gnl_paket_alimlari_v]
AS
SELECT     dbo.gnl_uyelik_paket_alimlari.id, dbo.gnl_uyelik_odeme_tipleri_tanimlari.odeme_tipi, dbo.GetDateFaormatedString(dbo.gnl_uyelik_paket_alimlari.paket_alim_tarihi, 0) AS paket_alim_tarihi_str, 
                      dbo.GetDateFaormatedString(dbo.gnl_uyelik_paket_alimlari.uyelik_bitis_tarihi, 0) AS uyelik_bitis_tarihi_str, dbo.gnl_uyelik_paket_alimlari.paket_fiyati, 
                      CAST(dbo.gnl_uyelik_paket_alimlari.paket_fiyati AS varchar(20)) + ' TL' AS paket_fiyati_str, dbo.gnl_uyelik_paket_alimlari.anket_sayisi, 
                      CAST(dbo.gnl_uyelik_paket_alimlari.anket_sayisi AS varchar(20)) + ' adet Anket' AS anket_sayisi_str, dbo.gnl_uyelik_paket_alimlari.anket_basina_katilimci_sayisi, 
                      CAST(dbo.gnl_uyelik_paket_alimlari.anket_basina_katilimci_sayisi AS varchar(20)) + ' Katılımcı ' AS anket_basina_katilimci_sayisi_str, dbo.gnl_uyelik_paket_alimlari.user_uid, 
                      dbo.gnl_uyelik_paket_alimlari.grup_uid, dbo.gnl_uyelik_paket_alimlari.paket_alim_tarihi, dbo.gnl_uyelik_paket_alimlari.uyelik_bitis_tarihi, dbo.gnl_uyelik_paket_alimlari.aktive_edildi, 
                      dbo.gnl_uyelik_paket_alimlari.odeme_sekli_id, 
                      CASE WHEN dbo.gnl_uyelik_paket_alimlari.odeme_sekli_id = 1 THEN 'Banka Havale' WHEN dbo.gnl_uyelik_paket_alimlari.odeme_sekli_id = 2 THEN 'Kredi Kartı' ELSE '' END AS odeme_sekli, 
                      dbo.gnl_uyelik_paket_alimlari.telefonu, dbo.gnl_uyelik_paket_alimlari.cep_telefonu, dbo.gnl_uyelik_paket_alimlari.adres, dbo.gnl_uyelik_paket_alimlari.sirket_adi, 
                      dbo.gnl_uyelik_paket_alimlari.vergi_dairesi, dbo.gnl_uyelik_paket_alimlari.vergi_no, dbo.gnl_uye_kullanicilar.ad, dbo.gnl_uye_kullanicilar.soyad, dbo.gnl_uye_kullanicilar.grup_adi, 
                      dbo.gnl_uyelik_paket_alimlari.fatura_kesildi, dbo.gnl_uyelik_paket_alimlari.islem_id
FROM         dbo.gnl_uye_kullanicilar RIGHT OUTER JOIN
                      dbo.gnl_uyelik_paket_alimlari ON dbo.gnl_uye_kullanicilar.user_uid = dbo.gnl_uyelik_paket_alimlari.user_uid LEFT OUTER JOIN
                      dbo.gnl_uyelik_odeme_tipleri_tanimlari ON dbo.gnl_uyelik_paket_alimlari.odeme_tipi_id = dbo.gnl_uyelik_odeme_tipleri_tanimlari.odeme_tipi_id
GO



CREATE VIEW [dbo].[gnl_uye_kullanicilar_v]
AS
SELECT     dbo.gnl_uye_kullanicilar.id, dbo.gnl_uye_kullanicilar.user_uid, dbo.gnl_uye_kullanicilar.ad, dbo.gnl_uye_kullanicilar.soyad, dbo.gnl_uye_kullanicilar.telefonu, 
                      dbo.gnl_uye_kullanicilar.cep_telefonu, dbo.gnl_uye_kullanicilar.adres, dbo.gnl_uye_kullanicilar.email, dbo.gnl_uye_kullanicilar.grup_adi, 
                      dbo.gnl_uye_kullanicilar.sirket_adi, dbo.gnl_uye_kullanicilar.vergi_dairesi, dbo.gnl_uye_kullanicilar.vergi_no, dbo.gnl_uye_kullanicilar.uye_baslangic_tarihi, 
                      dbo.gnl_uye_kullanicilar.uye_bitis_tarihi, dbo.gnl_uye_kullanicilar.son_odeme_tipi_id, dbo.gnl_uye_kullanicilar.kalan_anket_sayisi, dbo.gnl_uye_kullanicilar.aktif, 
                      dbo.gnl_uyelik_odeme_tipleri_tanimlari.odeme_tipi, dbo.gnl_uyelik_odeme_tipleri_tanimlari.anket_sayisi, dbo.gnl_uyelik_odeme_tipleri_tanimlari.ucret
FROM         dbo.gnl_uye_kullanicilar LEFT OUTER JOIN
                      dbo.gnl_uyelik_odeme_tipleri_tanimlari ON dbo.gnl_uye_kullanicilar.son_odeme_tipi_id = dbo.gnl_uyelik_odeme_tipleri_tanimlari.odeme_tipi_id
GO



CREATE VIEW [dbo].[sbr_anket_soru_v]
AS
SELECT     dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.anket_uid, dbo.sbr_anket.kullanici_uid, dbo.sbr_ref_soru_sitili.soru_sitili_id, 
                      dbo.sbr_ref_soru_sitili.soru_sitili, dbo.sbr_ref_anket_kategori.kategori_id, dbo.sbr_ref_soru_siralama_sekli.soru_siralama_sekli_id, 
                      dbo.sbr_ref_soru_siralama_sekli.soru_siralama_sekli, dbo.sbr_ref_soru_tipi.soru_tipi_id, dbo.sbr_ref_soru_tipi.soru_tipi_kodu, 
                      dbo.sbr_ref_anket_durumu.anket_durumu_id, dbo.sbr_ref_anket_durumu.anket_durumu, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket_sorulari.soru, 
                      dbo.sbr_anket.grup_uid, CASE WHEN LEN(CAST(dbo.sbr_anket_sorulari.soru AS varchar(4000))) 
                      < 50 THEN dbo.sbr_anket_sorulari.soru ELSE SUBSTRING(dbo.sbr_anket_sorulari.soru, 0, 50) + '...' END AS soru_kisa, dbo.sbr_anket_sorulari.soru_zorunlu, 
                      dbo.sbr_anket_sorulari.soru_sayisal_ondalik, dbo.sbr_anket_sorulari.soru_tek_satir
FROM         dbo.sbr_ref_soru_tipi RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_ref_soru_tipi.soru_tipi_id = dbo.sbr_anket_sorulari.soru_tipi_id LEFT OUTER JOIN
                      dbo.sbr_ref_soru_siralama_sekli ON dbo.sbr_anket_sorulari.soru_siralama_sekli_id = dbo.sbr_ref_soru_siralama_sekli.soru_siralama_sekli_id LEFT OUTER JOIN
                      dbo.sbr_ref_soru_sitili ON dbo.sbr_anket_sorulari.soru_sitili_id = dbo.sbr_ref_soru_sitili.soru_sitili_id LEFT OUTER JOIN
                      dbo.sbr_anket LEFT OUTER JOIN
                      dbo.sbr_ref_anket_durumu ON dbo.sbr_anket.anket_durumu_id = dbo.sbr_ref_anket_durumu.anket_durumu_id LEFT OUTER JOIN
                      dbo.sbr_ref_anket_kategori ON dbo.sbr_anket.kategori_id = dbo.sbr_ref_anket_kategori.kategori_id ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid
GO



CREATE VIEW [dbo].[sbr_anket_v]
AS
SELECT        dbo.sbr_anket.anket_uid, dbo.sbr_anket.kullanici_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket.anket_goruntulenen_ad, dbo.sbr_anket.kategori_id, dbo.sbr_anket.anket_mesaji, dbo.sbr_anket.baslangic_tarihi, 
                         dbo.sbr_anket.bitis_tarihi, dbo.sbr_anket.olusturma_tarihi, dbo.sbr_anket.olusturan_kullanici_uid, dbo.sbr_anket.anket_durumu_id, dbo.sbr_anket.anket_durumu_uid, dbo.sbr_ref_anket_durumu.anket_durumu, 
                         dbo.sbr_ref_anket_kategori.kategori_kodu, dbo.GetDateFaormatedString(dbo.sbr_anket.baslangic_tarihi, 0) AS baslangic_tarihi_str, dbo.GetDateFaormatedString(dbo.sbr_anket.bitis_tarihi, 0) AS bitis_tarihi_str, 
                         dbo.sbr_anket.grup_uid, dbo.gnl_user_groups.group_name, dbo.sbr_anket.anket_tipi_id, dbo.gnl_users.name + ' ' + dbo.gnl_users.surname AS olusturan_kullanici, dbo.gnl_users.email, dbo.sbr_ref_anket_tipi.anket_tipi, 
                         dbo.sbr_anket.sayfadaki_soru_sayisi, dbo.sbr_anket.soru_cevaplama_zorunlulugu, dbo.sbr_anket.anket_sonuc_mesaji
FROM            dbo.sbr_anket LEFT OUTER JOIN
                         dbo.sbr_ref_anket_tipi ON dbo.sbr_anket.anket_tipi_id = dbo.sbr_ref_anket_tipi.anket_tipi_id LEFT OUTER JOIN
                         dbo.gnl_users ON dbo.sbr_anket.olusturan_kullanici_uid = dbo.gnl_users.user_uid LEFT OUTER JOIN
                         dbo.gnl_user_groups ON dbo.sbr_anket.grup_uid = dbo.gnl_user_groups.group_uid LEFT OUTER JOIN
                         dbo.sbr_ref_anket_durumu ON dbo.sbr_anket.anket_durumu_id = dbo.sbr_ref_anket_durumu.anket_durumu_id LEFT OUTER JOIN
                         dbo.sbr_ref_anket_kategori ON dbo.sbr_anket.kategori_id = dbo.sbr_ref_anket_kategori.kategori_id
GO


CREATE VIEW [dbo].[sbr_anket_yayinlama_acik_anket_aktivasyon_v]
AS
SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
GO



CREATE VIEW [dbo].[sbr_anket_yayinlama_mail_gonderi_aktivasyon_v]
AS
SELECT     id, anket_uid, tarihce_uid, anket_gonderilen_ad, anket_gonderilen_soyad, anket_gonderilen_email, anket_gonderilme_tarihi, anket_gonderilme_key, ankete_girildi, 
                      ankete_giris_tarihi, anket_bitirildi, anket_bitirilme_tarihi, ankete_cevap_verildi, ankete_cevap_verilme_tarihi, anket_mail_grubu_uid, 
                      anket_gonderilen_ad + ' ' + anket_gonderilen_soyad AS anket_gonderilen_ismi
FROM         dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon
GO



CREATE VIEW [dbo].[sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi_v]
AS
SELECT     dbo.sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi.id, dbo.sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi.tarihce_uid, 
                      dbo.sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi.gonderilen_kisi_ismi, dbo.sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi.gonderilen_email_adresi, 
                      dbo.GetDateFaormatedString(dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi.gonderi_tarihi, 1) AS gonderi_tarihi_str
FROM         dbo.sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi ON 
                      dbo.sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi.tarihce_uid = dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi.tarihce_uid
GO



CREATE VIEW [dbo].[sbr_anket_yayinlama_mail_gonderi_tarihcesi_v]
AS
SELECT     dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi.tarihce_uid, dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi.anket_uid, dbo.sbr_mail_gruplari.mail_grubu_adi, 
                      dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi.gonderi_tarihi, dbo.GetDateFaormatedString(dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi.gonderi_tarihi, 1) 
                      AS gonderi_tarihi_str
FROM         dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi LEFT OUTER JOIN
                      dbo.sbr_mail_gruplari ON dbo.sbr_anket_yayinlama_mail_gonderi_tarihcesi.gonderilen_mail_grubu_uid = dbo.sbr_mail_gruplari.mail_grubu_uid
GO



CREATE VIEW [dbo].[sbr_davet_ettiklerim_v]
AS
SELECT     davet_uid, davet_edilen_email, dbo.GetDateFaormatedString(davet_tarihi, 1) AS davet_tarihi_str, davet_eden_kullanici_uid, davet_edilen_grup_uid, davet_key, 
                      davet_tarihi, davet_kabul_edildi, davet_kabul_edilme_tarihi, davet_kabul_eden_kullanici_uid
FROM         dbo.sbr_anket_davet
GO



CREATE VIEW [dbo].[sbr_sablon_soru_v]
AS
SELECT     dbo.sbr_sablon_sorulari.soru_uid, dbo.sbr_sablon_sorulari.sablon_uid, dbo.sbr_sablon.kullanici_uid, dbo.sbr_ref_soru_sitili.soru_sitili_id, 
                      dbo.sbr_ref_soru_sitili.soru_sitili, dbo.sbr_ref_anket_kategori.kategori_id, dbo.sbr_ref_soru_siralama_sekli.soru_siralama_sekli_id, 
                      dbo.sbr_ref_soru_siralama_sekli.soru_siralama_sekli, dbo.sbr_ref_soru_tipi.soru_tipi_id, dbo.sbr_ref_soru_tipi.soru_tipi_kodu, 
                      dbo.sbr_ref_sablon_durumu.sablon_durumu_id, dbo.sbr_ref_sablon_durumu.sablon_durumu, dbo.sbr_sablon_sorulari.soru_sira_no, dbo.sbr_sablon_sorulari.soru, 
                      dbo.sbr_sablon.grup_uid, CASE WHEN LEN(CAST(dbo.sbr_sablon_sorulari.soru AS varchar(4000))) 
                      < 50 THEN dbo.sbr_sablon_sorulari.soru ELSE SUBSTRING(dbo.sbr_sablon_sorulari.soru, 0, 50) + '...' END AS soru_kisa, dbo.sbr_sablon.soru_sayisal_ondalik, 
                      dbo.sbr_sablon_sorulari.soru_tek_satir, dbo.sbr_sablon_sorulari.soru_zorunlu
FROM         dbo.sbr_ref_soru_tipi RIGHT OUTER JOIN
                      dbo.sbr_sablon_sorulari ON dbo.sbr_ref_soru_tipi.soru_tipi_id = dbo.sbr_sablon_sorulari.soru_tipi_id LEFT OUTER JOIN
                      dbo.sbr_ref_soru_siralama_sekli ON dbo.sbr_sablon_sorulari.soru_siralama_sekli_id = dbo.sbr_ref_soru_siralama_sekli.soru_siralama_sekli_id LEFT OUTER JOIN
                      dbo.sbr_ref_soru_sitili ON dbo.sbr_sablon_sorulari.soru_sitili_id = dbo.sbr_ref_soru_sitili.soru_sitili_id LEFT OUTER JOIN
                      dbo.sbr_sablon LEFT OUTER JOIN
                      dbo.sbr_ref_sablon_durumu ON dbo.sbr_sablon.sablon_durumu_id = dbo.sbr_ref_sablon_durumu.sablon_durumu_id LEFT OUTER JOIN
                      dbo.sbr_ref_anket_kategori ON dbo.sbr_sablon.kategori_id = dbo.sbr_ref_anket_kategori.kategori_id ON 
                      dbo.sbr_sablon_sorulari.sablon_uid = dbo.sbr_sablon.sablon_uid
GO



create VIEW [dbo].[sbr_sablon_v]
AS
SELECT     dbo.sbr_sablon.sablon_uid, dbo.sbr_sablon.kullanici_uid, dbo.sbr_sablon.sablon_adi, dbo.sbr_sablon.kategori_id, 
                      dbo.sbr_sablon.olusturma_tarihi, dbo.sbr_sablon.olusturan_kullanici_uid, 
                      dbo.sbr_sablon.sablon_durumu_id, dbo.sbr_sablon.sablon_durumu_uid, dbo.sbr_ref_sablon_durumu.sablon_durumu, dbo.sbr_ref_anket_kategori.kategori_kodu, 
                      
                      dbo.sbr_sablon.grup_uid
FROM         dbo.sbr_sablon LEFT OUTER JOIN
                      dbo.sbr_ref_sablon_durumu ON dbo.sbr_sablon.sablon_durumu_id = dbo.sbr_ref_sablon_durumu.sablon_durumu_id LEFT OUTER JOIN
                      dbo.sbr_ref_anket_kategori ON dbo.sbr_sablon.kategori_id = dbo.sbr_ref_anket_kategori.kategori_id

GO


CREATE VIEW [dbo].[sbr_test_anket_gonderi_sonuclari_v]
AS
SELECT     dbo.sbr_anket_test_mail_gonderi_kisi_tarihcesi.id, dbo.sbr_anket_test_mail_gonderi_kisi_tarihcesi.anket_uid, 
                      dbo.sbr_anket_test_mail_gonderi_kisi_tarihcesi.gonderi_tarihi, dbo.GetDateFaormatedString(dbo.sbr_anket_test_mail_gonderi_kisi_tarihcesi.gonderi_tarihi, 1) 
                      AS gonderi_tarihi_str, dbo.sbr_anket_test_mail_gonderi_kisi_tarihcesi.gonderilen_email_adresi, dbo.sbr_anket_test_mail_gonderi_kisi_tarihcesi.anket_test_sonucu, 
                      dbo.sbr_ref_anket_test_sonucu.anket_test_sonucu AS anket_test_sonucu_durumu
FROM         dbo.sbr_anket_test_mail_gonderi_kisi_tarihcesi LEFT OUTER JOIN
                      dbo.sbr_ref_anket_test_sonucu ON dbo.sbr_anket_test_mail_gonderi_kisi_tarihcesi.anket_test_sonucu_id = dbo.sbr_ref_anket_test_sonucu.anket_test_sonucu_id
GO


create function [dbo].[AnketUidGetir](@soru_uid uniqueidentifier)
returns uniqueidentifier
as
begin
declare @result uniqueidentifier

SELECT @result = sbr_anket_sorulari.anket_uid
FROM  sbr_anket_sorulari
where sbr_anket_sorulari.soru_uid= @soru_uid      

return @result
end
GO



CREATE function [dbo].[Cevaplanan_secenek_kayit_sayisi](@anket_uid uniqueidentifier,@soru_uid uniqueidentifier,@soru_tipi_id int,@soru_secenek_uid uniqueidentifier)
returns int
as
begin
declare @result int
set @result=1

if(@soru_tipi_id=1)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_1_cevaplari,sbr_soru_tipi_1_secenekleri where dbo.sbr_soru_tipi_1_cevaplari.soru_uid=@soru_uid and dbo.sbr_soru_tipi_1_cevaplari.soru_uid=sbr_soru_tipi_1_secenekleri.soru_uid and dbo.sbr_soru_tipi_1_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_1_cevaplari.cevap = 1  and ( sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=2)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_2_cevaplari,sbr_soru_tipi_2_secenekleri where dbo.sbr_soru_tipi_2_cevaplari.soru_uid=@soru_uid and dbo.sbr_soru_tipi_2_cevaplari.soru_uid=sbr_soru_tipi_2_secenekleri.soru_uid and dbo.sbr_soru_tipi_2_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_2_cevaplari.cevap = 1  and ( sbr_soru_tipi_2_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_2_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=4)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_4_cevaplari where dbo.sbr_soru_tipi_4_cevaplari.soru_uid=@soru_uid  and ( sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=5)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_5_cevaplari,sbr_soru_tipi_5_secenekleri where dbo.sbr_soru_tipi_5_cevaplari.soru_uid=@soru_uid and dbo.sbr_soru_tipi_5_cevaplari.soru_uid=sbr_soru_tipi_5_secenekleri.soru_uid and dbo.sbr_soru_tipi_5_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_5_cevaplari.cevap = 1  and ( sbr_soru_tipi_5_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_5_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=6)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_6_cevaplari,sbr_soru_tipi_6_secenekleri where dbo.sbr_soru_tipi_6_cevaplari.soru_uid=@soru_uid and dbo.sbr_soru_tipi_6_cevaplari.soru_uid=sbr_soru_tipi_6_secenekleri.soru_uid and dbo.sbr_soru_tipi_6_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_6_cevaplari.cevap = 1  and ( sbr_soru_tipi_6_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_6_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=7)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_7_cevaplari,sbr_soru_tipi_7_secenekleri where dbo.sbr_soru_tipi_7_cevaplari.soru_uid=@soru_uid and dbo.sbr_soru_tipi_7_cevaplari.soru_uid=sbr_soru_tipi_7_secenekleri.soru_uid and dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid=@soru_secenek_uid and dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid=dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid  and ( sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=8)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_8_cevaplari where dbo.sbr_soru_tipi_8_cevaplari.soru_uid=@soru_uid  and ( sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=9)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_9_cevaplari where dbo.sbr_soru_tipi_9_cevaplari.soru_uid=@soru_uid  and ( sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=10)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_10_cevaplari where dbo.sbr_soru_tipi_10_cevaplari.soru_uid=@soru_uid  and ( sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end
else if  (@soru_tipi_id=11)
begin
set @result =  cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_11_cevaplari where dbo.sbr_soru_tipi_11_cevaplari.soru_uid=@soru_uid  and ( sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))
end

if (@result is null or @result =0)
begin
set @result =1
end

return @result
end

                      
GO


CREATE FUNCTION [dbo].[fnAppEmailCheck](@email VARCHAR(255)) 
RETURNS bit as BEGIN 
DECLARE @valid bit 
IF @email IS NOT NULL 
SET @email = LOWER(@email) 
SET @valid = 0 
IF @email like '[a-z,0-9,_,-]%@[a-z,0-9,_,-]%.[a-z]%' 
AND @email NOT like '%[,";:=~!#$%*?()+}{şŞğĞÜüçÇöÖ]%' 
AND @email NOT like '%@%@%' 
AND CHARINDEX('.@',@email) = 0 
AND CHARINDEX('..',@email) = 0 
AND CHARINDEX(',',@email) = 0 
AND RIGHT(@email,1) between 'a' AND 'z' 
SET @valid=1 
RETURN @valid 
END
GO





CREATE FUNCTION [dbo].[GetDateForCalender](@Date datetime,@IsPeriodical bit)
RETURNS datetime
AS
BEGIN
	declare @day	int
	declare @month	int
	declare @year	int
	declare @resultDate datetime

	
	set @resultDate = @Date

	IF(@IsPeriodical=1)
	BEGIN
		set @resultDate = dateadd(year,year(getdate())-year(@date), @date)
	END

	return @resultDate
END

GO


CREATE FUNCTION [dbo].[GetKullaniciBazliAnketCevaplari](@anket_uid uniqueidentifier,@soru_uid uniqueidentifier,@soru_tipi_id int,@email nvarchar(500))
RETURNS nvarchar(4000)
AS
BEGIN
	declare @result nvarchar(4000)

	IF @soru_tipi_id=1
	BEGIN
	  (SELECT  @result =   STUFF
														((SELECT     ' ' + cast(sbr_soru_tipi_1_secenekleri.soru_secenek_ad as nvarchar(4000)) 
                                                           FROM         dbo.sbr_soru_tipi_1_cevaplari,sbr_soru_tipi_1_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_1_cevaplari.soru_secenek_uid =sbr_soru_tipi_1_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_1_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_1_cevaplari.cevap=1
                                                           and dbo.sbr_soru_tipi_1_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_1_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF @soru_tipi_id=2
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(sbr_soru_tipi_2_secenekleri.soru_secenek_ad as nvarchar(4000)) +' ; '
                                                           FROM         dbo.sbr_soru_tipi_2_cevaplari,sbr_soru_tipi_2_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_2_cevaplari.soru_secenek_uid =sbr_soru_tipi_2_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_2_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_2_cevaplari.cevap=1
                                                           and dbo.sbr_soru_tipi_2_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_2_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF @soru_tipi_id=3
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(sbr_soru_tipi_3_secenekleri.soru_secenek_ad as nvarchar(4000))+' - '+cast(sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_ad as nvarchar(4000)) +' ; '
                                                           FROM         dbo.sbr_soru_tipi_3_cevaplari,sbr_soru_tipi_3_secenekleri,sbr_anket_sorulari,sbr_soru_tipi_3_secenek_kolonlari
                                                           WHERE     dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_uid =sbr_soru_tipi_3_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_3_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           AND sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_uid=sbr_soru_tipi_3_cevaplari.soru_secenek_kolon_uid
                                                           and dbo.sbr_soru_tipi_3_cevaplari.cevap=1
                                                           and dbo.sbr_soru_tipi_3_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_3_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF  @soru_tipi_id=4
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(dbo.sbr_soru_tipi_4_cevaplari.cevap as nvarchar(4000)) 
                                                           FROM         dbo.sbr_soru_tipi_4_cevaplari,sbr_soru_tipi_4_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_4_cevaplari.soru_secenek_uid =sbr_soru_tipi_4_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_4_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_4_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_4_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	
	ELSE IF @soru_tipi_id=5
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(sbr_soru_tipi_5_secenekleri.soru_secenek_ad as nvarchar(4000)) +','
                                                           FROM         dbo.sbr_soru_tipi_5_cevaplari,sbr_soru_tipi_5_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_5_cevaplari.soru_secenek_uid =sbr_soru_tipi_5_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_5_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_5_cevaplari.cevap=1
                                                           and dbo.sbr_soru_tipi_5_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_5_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF @soru_tipi_id=6
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(sbr_soru_tipi_6_secenekleri.soru_secenek_ad as nvarchar(4000)) +','
                                                           FROM         dbo.sbr_soru_tipi_6_cevaplari,sbr_soru_tipi_6_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_6_cevaplari.soru_secenek_uid =sbr_soru_tipi_6_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_6_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_6_cevaplari.cevap=1
                                                           and dbo.sbr_soru_tipi_6_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_6_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF @soru_tipi_id=7
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(sbr_soru_tipi_7_cevaplari.cevap as nvarchar(4000)) +' ; '
                                                           FROM         dbo.sbr_soru_tipi_7_cevaplari,sbr_soru_tipi_7_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid =sbr_soru_tipi_7_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_7_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_7_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_7_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF  @soru_tipi_id=8
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(dbo.sbr_soru_tipi_8_cevaplari.cevap as nvarchar(4000)) 
                                                           FROM         dbo.sbr_soru_tipi_8_cevaplari,sbr_soru_tipi_8_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_8_cevaplari.soru_secenek_uid =sbr_soru_tipi_8_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_8_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_8_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_8_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF  @soru_tipi_id=9
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + case when (dbo.sbr_soru_tipi_9_cevaplari.cevap is not null and rtrim(ltrim(dbo.sbr_soru_tipi_9_cevaplari.cevap))!='') then  cast(day(dbo.sbr_soru_tipi_9_cevaplari.cevap) as nvarchar(2)) + '/' +cast(month(dbo.sbr_soru_tipi_9_cevaplari.cevap) as nvarchar(2)) +'/'+cast(year(dbo.sbr_soru_tipi_9_cevaplari.cevap) as nvarchar(4)) end
                                                           FROM         dbo.sbr_soru_tipi_9_cevaplari,sbr_soru_tipi_9_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_9_cevaplari.soru_secenek_uid =sbr_soru_tipi_9_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_9_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_9_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_9_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF  @soru_tipi_id=10
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(dbo.sbr_soru_tipi_10_cevaplari.cevap as nvarchar(4000)) 
                                                           FROM         dbo.sbr_soru_tipi_10_cevaplari,sbr_soru_tipi_10_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_10_cevaplari.soru_secenek_uid =sbr_soru_tipi_10_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_10_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_10_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_10_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	ELSE IF  @soru_tipi_id=11
	BEGIN
	(SELECT @result = STUFF
														((SELECT     ' ' + cast(dbo.sbr_soru_tipi_11_cevaplari.cevap as nvarchar(4000)) 
                                                           FROM         dbo.sbr_soru_tipi_11_cevaplari,sbr_soru_tipi_11_secenekleri,sbr_anket_sorulari
                                                           WHERE     dbo.sbr_soru_tipi_11_cevaplari.soru_secenek_uid =sbr_soru_tipi_11_secenekleri.soru_secenek_uid 
                                                           and dbo.sbr_soru_tipi_11_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid
                                                           and dbo.sbr_soru_tipi_11_cevaplari.soru_uid= @soru_uid
                                                           and dbo.sbr_soru_tipi_11_cevaplari.cevaplayan_email=@email
                                                           and sbr_anket_sorulari.anket_uid=@anket_uid
                                                           FOR XML PATH('')), 1, 1, '')) 
	END
	return @result
END



GO






CREATE FUNCTION [dbo].[KatilimciSoruCevaplandiMi](@anket_uid uniqueidentifier,@cevap_key nvarchar(100))
RETURNS bit
AS
BEGIN
	declare @cevap_durumu_1 int 
	declare @cevap_durumu_2 int
	declare @cevap_durumu_3 int
	declare @cevap_durumu_4 int
	declare @cevap_durumu_5 int
	declare @cevap_durumu_6 int
	declare @cevap_durumu_7 int
	declare @cevap_durumu_8 int
	declare @cevap_durumu_9 int
	declare @cevap_durumu_10 int
	declare @cevap_durumu_11 int
	
	declare @result bit
	set @result=1
	
	  select @cevap_durumu_1 = count(*) from sbr_soru_tipi_1_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_1_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap=1 and cevap_key=@cevap_key and anket_uid=@anket_uid
	
	  select @cevap_durumu_2 = count(*) from sbr_soru_tipi_2_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_2_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap=1 and cevap_key=@cevap_key and anket_uid=@anket_uid
	  
	  select @cevap_durumu_3 = count(*) from sbr_soru_tipi_3_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_3_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap=1 and cevap_key=@cevap_key and anket_uid=@anket_uid
	  
	  select @cevap_durumu_4 = count(*) from sbr_soru_tipi_4_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_4_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap not like  ''  and cevap_key=@cevap_key and anket_uid=@anket_uid
	  
	  select @cevap_durumu_5 = count(*) from sbr_soru_tipi_5_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_5_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap=1 and cevap_key=@cevap_key and anket_uid=@anket_uid
	  
	  select @cevap_durumu_6 = count(*) from sbr_soru_tipi_6_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_6_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap=1 and cevap_key=@cevap_key and anket_uid=@anket_uid
	  
	  select @cevap_durumu_7 = count(*) from sbr_soru_tipi_7_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_7_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap not like  ''  and cevap_key=@cevap_key and anket_uid=@anket_uid
	
	  select @cevap_durumu_8 = count(*) from sbr_soru_tipi_8_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_8_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap >0 and cevap_key=@cevap_key and anket_uid=@anket_uid
	  
	  select @cevap_durumu_9 = count(*) from sbr_soru_tipi_9_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_9_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap is not null and cevap_key=@cevap_key and anket_uid=@anket_uid
    
	  select @cevap_durumu_10 = count(*) from sbr_soru_tipi_10_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_10_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap not like  ''  and cevap_key=@cevap_key and anket_uid=@anket_uid
	  
	  select @cevap_durumu_11 = count(*) from sbr_soru_tipi_11_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_11_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and cevap is not null and cevap_key=@cevap_key and anket_uid=@anket_uid
	 
	  
    IF (@cevap_durumu_1 is null or @cevap_durumu_1=0) and (@cevap_durumu_2 is null or @cevap_durumu_2=0) and (@cevap_durumu_3 is null or @cevap_durumu_3=0) and (@cevap_durumu_4 is null or @cevap_durumu_4=0) and (@cevap_durumu_5 is null or @cevap_durumu_5=0) and (@cevap_durumu_6 is null or @cevap_durumu_6=0) and (@cevap_durumu_7 is null or @cevap_durumu_7=0) and (@cevap_durumu_8 is null or @cevap_durumu_8=0) and (@cevap_durumu_9 is null or @cevap_durumu_9=0) and (@cevap_durumu_10 is null or @cevap_durumu_10=0) and (@cevap_durumu_11 is null or @cevap_durumu_11=0)
    BEGIN
       SET @result =0
    END	
	return @result
END
GO




CREATE FUNCTION [dbo].[MenuToShortString] 
(
	@MenuId int,
	@Record_uid uniqueidentifier 
)
RETURNS varchar(255)
AS
BEGIN
	DECLARE @retVal varchar(255);
	
	select @MenuId=IsNull(RealMenuId,@MenuId) from dbo.BaseMenu where MenuId=@MenuId;
	if @MenuId=2	     -- Kullanıcılar
		select @retVal=ad from loj_kullanici_xtb_v where kullaniciid=@Record_uid
	else if @MenuId=3	     -- Roller
		select @retVal=Role_Name from BaseRoleList where Role_uid=@Record_uid
	else if (@MenuId=5050 or @MenuId=5055 or  @MenuId=5060 or @MenuId=5510 or  @MenuId=5520 or  @MenuId=5530 or  @MenuId=5050 or  @MenuId=5032 or  @MenuId=5034 or  @MenuId=5036) -- riskli beyannameler
		select @retVal=musteriad+'-'+cast(referansno as varchar(20)) from crm_riskli_beyannameler_v where beyanname_uid=@Record_uid
	else if (@MenuId=5250 or @MenuId=5238 or @MenuId=5240 or @MenuId=5242 or @MenuId=5540 or @MenuId=5250 or @MenuId=5550 or @MenuId=5560 or @MenuId=5610 or @MenuId=5620 or @MenuId=5630 or @MenuId=5635 or @MenuId=5810  or @MenuId=5820  or @MenuId=5830  )  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_riskler_v where risk_uid=@Record_uid
	else if @MenuId=5270  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_riskler_v where risk_uid=@Record_uid
    else if @MenuId=5280  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_riskler_v where risk_uid=@Record_uid		
else if @MenuId=5200  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_riskler_v where risk_uid=@Record_uid				
else if @MenuId=5445  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_riskler_v where risk_uid=@Record_uid						
else if @MenuId=5450  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_riskler_v where risk_uid=@Record_uid								
else if (@MenuId=5460 or @MenuId=5640 or @MenuId=5650 or @MenuId=5260 or @MenuId=5710 or  @MenuId=5720 or  @MenuId=5730 or  @MenuId=5740 or  @MenuId=5750 or @MenuId=5760  or @MenuId=5840  or @MenuId=5850  or @MenuId=5860 )-- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_risk_yonlendirmeler_v where risk_yonlendirme_uid=@Record_uid										
else if @MenuId=5470  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_risk_yonlendirmeler_v where risk_yonlendirme_uid=@Record_uid												
else if @MenuId=7100  -- riskler
		select @retVal=firma_ad+'-'+gorusulen from crm_risk_gorusme_v where gorusme_uid=@Record_uid																
else if @MenuId=7200  -- riskler
		select @retVal=firma_ad+'-'+gorusulen from crm_risk_gorusme_v where gorusme_uid=@Record_uid																
else if @MenuId=7300  -- riskler
	select @retVal=firma_ad+'-'+gorusulen from crm_risk_gorusme_v where gorusme_uid=@Record_uid																
else if @MenuId=5455  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_risk_yonlendirmeler_v where risk_yonlendirme_uid=@Record_uid										
else if @MenuId=5453  -- riskler
		select @retVal=firma_ad+'-'+cast(referansno as varchar(20))+'-'+risk_ana_tipi_kodu from crm_risk_yonlendirmeler_v where risk_yonlendirme_uid=@Record_uid										
		
	else 
		--set @retVal = cast (@record_uid as varchar(255))
		set @retVal = '*'+(select Name From BaseMenu where MenuId=@menuId)+'*'
	

	return @retVal
END

GO



CREATE FUNCTION [dbo].[SoruCevaplandiMi](@soru_uid uniqueidentifier,@soru_tipi_id int,@cevap_key nvarchar(100))
RETURNS int
AS
BEGIN
	declare @cevap_durumu int 
	
	IF @soru_tipi_id=1
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_1_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_1_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_1_cevaplari.soru_uid=@soru_uid and cevap=1 and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
	ELSE IF @soru_tipi_id=2
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_2_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_2_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_2_cevaplari.soru_uid=@soru_uid and cevap=1 and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
	ELSE IF @soru_tipi_id=3
	BEGIN
	  select @cevap_durumu = min(kayit_sayisi)
	  from (select (select COUNT(*) from sbr_soru_tipi_3_secenek_kolonlari where sbr_soru_tipi_3_secenek_kolonlari.soru_uid=@soru_uid) -  count(*) as kayit_sayisi from sbr_soru_tipi_3_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_3_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_3_cevaplari.soru_uid=@soru_uid and cevap=0 and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1 group by sbr_soru_tipi_3_cevaplari.soru_secenek_uid) as table_1
	END
	ELSE IF @soru_tipi_id=4
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_4_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_4_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_4_cevaplari.soru_uid=@soru_uid and cevap not like  '' and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
	ELSE IF @soru_tipi_id=5
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_5_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_5_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_5_cevaplari.soru_uid=@soru_uid and cevap=1 and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
	ELSE IF @soru_tipi_id=6
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_6_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_6_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_6_cevaplari.soru_uid=@soru_uid and cevap=1 and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
    ELSE IF @soru_tipi_id=7
	BEGIN
	  select @cevap_durumu = min(kayit_sayisi)
	  from (select   count(*) - (select COUNT(*) from sbr_soru_tipi_7_secenekleri where sbr_soru_tipi_7_secenekleri.soru_uid=@soru_uid) + 1 as kayit_sayisi from sbr_soru_tipi_7_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_7_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_7_cevaplari.soru_uid=@soru_uid and cevap not like '' and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1 ) as table_1
	END
	ELSE IF @soru_tipi_id=8
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_8_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_8_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_8_cevaplari.soru_uid=@soru_uid and cevap >0 and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
	ELSE IF @soru_tipi_id=9
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_9_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_9_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_9_cevaplari.soru_uid=@soru_uid and cevap is not null and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
	ELSE IF @soru_tipi_id=10
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_10_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_10_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_10_cevaplari.soru_uid=@soru_uid and cevap not like  '' and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
	ELSE IF @soru_tipi_id=11
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_11_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_11_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_11_cevaplari.soru_uid=@soru_uid and (cevap  is not null) and cevap_key=@cevap_key and sbr_anket_sorulari.soru_zorunlu=1
	END
	
    IF @cevap_durumu is null or @cevap_durumu<0
    BEGIN
       SET @cevap_durumu =0
    END	
	return @cevap_durumu
END
GO


create FUNCTION [dbo].[SoruCevaplandiMiHepsi](@soru_uid uniqueidentifier,@soru_tipi_id int,@cevap_key nvarchar(100))
RETURNS int
AS
BEGIN
	declare @cevap_durumu int 
	
	IF @soru_tipi_id=1
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_1_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_1_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_1_cevaplari.soru_uid=@soru_uid and cevap=1 and cevap_key=@cevap_key 
	END
	ELSE IF @soru_tipi_id=2
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_2_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_2_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_2_cevaplari.soru_uid=@soru_uid and cevap=1 and cevap_key=@cevap_key 
	END
	ELSE IF @soru_tipi_id=3
	BEGIN
	  select @cevap_durumu = min(kayit_sayisi)
	  from (select (select COUNT(*) from sbr_soru_tipi_3_secenek_kolonlari where sbr_soru_tipi_3_secenek_kolonlari.soru_uid=@soru_uid) -  count(*) as kayit_sayisi from sbr_soru_tipi_3_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_3_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_3_cevaplari.soru_uid=@soru_uid and cevap=0 and cevap_key=@cevap_key group by sbr_soru_tipi_3_cevaplari.soru_secenek_uid) as table_1
	END
	ELSE IF @soru_tipi_id=4
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_4_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_4_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_4_cevaplari.soru_uid=@soru_uid and cevap not like  '' and cevap_key=@cevap_key 
	END
	ELSE IF @soru_tipi_id=5
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_5_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_5_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_5_cevaplari.soru_uid=@soru_uid and cevap=1 and cevap_key=@cevap_key 
	END
	ELSE IF @soru_tipi_id=6
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_6_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_6_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_6_cevaplari.soru_uid=@soru_uid and cevap=1 and cevap_key=@cevap_key 
	END
    ELSE IF @soru_tipi_id=7
	BEGIN
	  select @cevap_durumu = min(kayit_sayisi)
	  from (select   count(*) - (select COUNT(*) from sbr_soru_tipi_7_secenekleri where sbr_soru_tipi_7_secenekleri.soru_uid=@soru_uid) + 1 as kayit_sayisi from sbr_soru_tipi_7_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_7_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_7_cevaplari.soru_uid=@soru_uid and cevap not like '' and cevap_key=@cevap_key  ) as table_1
	END
	ELSE IF @soru_tipi_id=8
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_8_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_8_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_8_cevaplari.soru_uid=@soru_uid and cevap >0 and cevap_key=@cevap_key 
	END
	ELSE IF @soru_tipi_id=9
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_9_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_9_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_9_cevaplari.soru_uid=@soru_uid and cevap is not null and cevap_key=@cevap_key 
	END
	ELSE IF @soru_tipi_id=10
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_10_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_10_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_10_cevaplari.soru_uid=@soru_uid and cevap not like  '' and cevap_key=@cevap_key 
	END
	ELSE IF @soru_tipi_id=11
	BEGIN
	  select @cevap_durumu = count(*) from sbr_soru_tipi_11_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_11_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid and sbr_soru_tipi_11_cevaplari.soru_uid=@soru_uid and (cevap  is not null) and cevap_key=@cevap_key 
	END
	
    IF @cevap_durumu is null or @cevap_durumu<0
    BEGIN
       SET @cevap_durumu =0
    END	
	return @cevap_durumu
END
GO



CREATE function [dbo].[SoruTipi1CevapOraniGetir](@anket_uid uniqueidentifier,@soru_uid uniqueidentifier,@secenek_uid uniqueidentifier)
returns nvarchar(20)
as
begin
declare @result nvarchar(20)

SELECT @result = (cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_1_cevaplari where dbo.sbr_soru_tipi_1_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_1_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_1_cevaplari.cevap = 1 and ( sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)))

FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_1_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_1_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 1)
and dbo.sbr_anket.anket_uid=@anket_uid
and dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid=@secenek_uid
and dbo.sbr_soru_tipi_1_secenekleri.soru_uid=@soru_uid
return @result
end
GO



create function [dbo].[SoruTipi1CevapSayisiGetir](@anket_uid uniqueidentifier,@soru_uid uniqueidentifier,@secenek_uid uniqueidentifier)
returns nvarchar(20)
as
begin
declare @result nvarchar(20)

SELECT @result = cast((select COUNT(*) from dbo.sbr_soru_tipi_1_cevaplari where dbo.sbr_soru_tipi_1_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_1_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_1_cevaplari.cevap = 1 and ( sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as nvarchar(20))
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_1_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_1_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 1)
and dbo.sbr_anket.anket_uid=@anket_uid
and dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid=@secenek_uid
and dbo.sbr_soru_tipi_1_secenekleri.soru_uid=@soru_uid
return @result
end
GO



CREATE FUNCTION [dbo].[udf_birlestir] 
(@tableName varchar(200), 
	@displayfield varchar(2000),
	@filter varchar(2000)) 
    RETURNS VARCHAR(8000) AS BEGIN 

Declare @sSQL nvarchar(4000)
Declare @retValue nvarchar(4000)

select @sSQL='
SELECT
  STUFF(
    (
    SELECT
      '','' + $$DISPLAYFIELD$$
    FROM $$TABLENAME$$
    where $$FILTER$$
    FOR XML PATH('''')
    ), 1, 1, ''''
  ) As concatenated_string'
  
	set @sSQL=REPLACE(@sSQL,'$$TABLENAME$$', @tableName)
	set @sSQL=REPLACE(@sSQL,'$$DISPLAYFIELD$$', @displayfield)
	if @filter != ''
		set @sSQL=REPLACE(@sSQL,'$$FILTER$$', @filter)
	else
		set @sSQL=REPLACE(@sSQL,'$$FILTER$$', '1=1')
  
  exec sp_executesql @sSQL, N'@concatenated_string varchar(8000) output', @concatenated_string = @retValue OUTPUT
  return @retvalue;
  end
GO




CREATE FUNCTION [dbo].[udf_StripHTML]
(@HTMLText VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS
BEGIN
DECLARE @Start INT
DECLARE @End INT
DECLARE @Length INT
SET @Start = CHARINDEX('<',@HTMLText)
SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
SET @Length = (@End - @Start) + 1
WHILE @Start > 0
AND @End > 0
AND @Length > 0
BEGIN
SET @HTMLText = STUFF(@HTMLText,@Start,@Length,'')
SET @Start = CHARINDEX('<',@HTMLText)
SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
SET @Length = (@End - @Start) + 1
END
RETURN LTRIM(RTRIM(@HTMLText))
END
GO

--procedures--


CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_1_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)

SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi, CAST(dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_1_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_1_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_1_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_1_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_1_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_1_cevaplari.soru_uid = dbo.sbr_soru_tipi_1_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_1_cevaplari.cevap = 1 )
and dbo.sbr_soru_tipi_1_secenekleri.soru_uid=@soru_uid and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_1_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_ad as varchar(500)) 

END

GO


CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_10_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)

select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_10_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_10_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_10_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_10_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_10_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_10_cevaplari.soru_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_10_cevaplari.cevap is not null )
and (ltrim(rtrim(dbo.sbr_soru_tipi_10_cevaplari.cevap))) <>''
and dbo.sbr_soru_tipi_10_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_10_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Not Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_10_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_10_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_10_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_10_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_10_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_10_cevaplari.soru_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_10_cevaplari.cevap is null or ltrim(rtrim(dbo.sbr_soru_tipi_10_cevaplari.cevap)) ='')
and dbo.sbr_soru_tipi_10_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_10_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END

GO



CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_11_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)

select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_11_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_11_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_11_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_11_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_11_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_11_cevaplari.soru_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_11_cevaplari.cevap is not null )
and (ltrim(rtrim(dbo.sbr_soru_tipi_11_cevaplari.cevap))) <>''
and dbo.sbr_soru_tipi_11_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_11_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Not Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_11_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_11_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_11_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_11_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_11_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_11_cevaplari.soru_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_11_cevaplari.cevap is null or ltrim(rtrim(dbo.sbr_soru_tipi_11_cevaplari.cevap)) ='')
and dbo.sbr_soru_tipi_11_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_11_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END

GO


CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_2_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)

SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi, CAST(dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_2_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_2_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_2_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_2_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_2_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_2_cevaplari.soru_uid = dbo.sbr_soru_tipi_2_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_2_cevaplari.cevap = 1)
and dbo.sbr_soru_tipi_2_secenekleri.soru_uid=@soru_uid and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_2_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_ad as varchar(500)) 

END

GO




CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_3_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)

SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi,(select COUNT(*) from dbo.sbr_soru_tipi_3_cevaplari as b where b.soru_uid=dbo.sbr_anket_sorulari.soru_uid and b.soru_secenek_uid=dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid and b.cevap = 1 and b.cevap_key in (select #TempTable.giris_key from #TempTable where #TempTable.anket_uid=dbo.sbr_anket.anket_uid and #TempTable.anket_bitirildi=1)) as tum_cevap_sayisi,
             cast(cast(COUNT(*) as decimal(18,2))/cast((select COUNT(*) from dbo.sbr_soru_tipi_3_cevaplari where dbo.sbr_soru_tipi_3_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_3_cevaplari.cevap = 1) as decimal(18,2)) as decimal(18,2))*100 as cevap_orani,CAST(dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, 
                      dbo.sbr_anket_sorulari.soru_uid, CAST(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad, dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid, 
                      dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_uid,CAST(dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_ad AS varchar(500)) AS kolon_ad,
                      cast(COUNT(*) as varchar(20))+'/'+CAST((select COUNT(*) from dbo.sbr_soru_tipi_3_cevaplari as b where b.soru_uid=dbo.sbr_anket_sorulari.soru_uid and b.soru_secenek_uid=dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid and b.cevap = 1 and b.cevap_key in (select #TempTable.giris_key from #TempTable where #TempTable.anket_uid=dbo.sbr_anket.anket_uid and #TempTable.anket_bitirildi=1)) as varchar(20))+' '+CAST(cast(cast(cast(COUNT(*) as decimal(18,2))/cast((select COUNT(*) from dbo.sbr_soru_tipi_3_cevaplari where dbo.sbr_soru_tipi_3_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_3_cevaplari.cevap = 1 and sbr_soru_tipi_3_cevaplari.cevap_key in (select #TempTable.giris_key from #TempTable where #TempTable.anket_uid=dbo.sbr_anket.anket_uid and #TempTable.anket_bitirildi=1)) as decimal(18,2)) as decimal(18,2))*100 as decimal(18,0)) as varchar(10))+'%' as cevap_orani_str
FROM         dbo.sbr_soru_tipi_3_cevaplari RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_3_secenek_kolonlari ON 
                      dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_kolon_uid = dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_uid LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_3_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_3_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_3_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_3_cevaplari.soru_uid = dbo.sbr_soru_tipi_3_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_3_cevaplari.cevap = 1) 
and dbo.sbr_soru_tipi_3_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_3_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_ad as varchar(500)) ,dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid,CAST(dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_ad AS varchar(500)),dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_uid



END
GO



CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_4_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)

select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_4_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_4_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_4_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_4_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_4_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_4_cevaplari.soru_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_4_cevaplari.cevap is not null )
and (ltrim(rtrim(cast(dbo.sbr_soru_tipi_4_cevaplari.cevap as nvarchar(4000))))) <>''
and dbo.sbr_soru_tipi_4_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_4_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Not Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_4_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_4_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_4_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_4_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_4_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_4_cevaplari.soru_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_4_cevaplari.cevap is null or ltrim(rtrim(cast(dbo.sbr_soru_tipi_4_cevaplari.cevap as nvarchar(4000)))) ='')
and dbo.sbr_soru_tipi_4_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_4_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END

GO

CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_5_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN


CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)

SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi, CAST(dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_5_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_5_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_5_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_5_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_5_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_5_cevaplari.soru_uid = dbo.sbr_soru_tipi_5_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_5_cevaplari.cevap = 1)
and dbo.sbr_soru_tipi_5_secenekleri.soru_uid=@soru_uid and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_5_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_ad as varchar(500)) 

END

GO



CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_6_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)


SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi, CAST(dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_6_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_6_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_6_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_6_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_6_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_6_cevaplari.soru_uid = dbo.sbr_soru_tipi_6_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_6_cevaplari.cevap = 1)
and dbo.sbr_soru_tipi_6_secenekleri.soru_uid=@soru_uid and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_6_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_ad as varchar(500)) 

END

GO



CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_7_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)


select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_7_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_7_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_7_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_7_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_7_cevaplari.soru_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_7_cevaplari.cevap is not null )
and (ltrim(rtrim(cast(dbo.sbr_soru_tipi_7_cevaplari.cevap as nvarchar(4000))))) <>''
and dbo.sbr_soru_tipi_7_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_7_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Not Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_7_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_7_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_7_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_7_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_7_cevaplari.soru_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_7_cevaplari.cevap is null or ltrim(rtrim(cast(dbo.sbr_soru_tipi_7_cevaplari.cevap as nvarchar(4000)))) ='')
and dbo.sbr_soru_tipi_7_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_7_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END

GO



CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_8_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)


select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_8_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_8_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_8_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_8_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_8_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_8_cevaplari.soru_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_8_cevaplari.cevap is not null )
and dbo.sbr_soru_tipi_8_cevaplari.cevap<>0
and dbo.sbr_soru_tipi_8_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_8_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Not Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_8_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_8_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_8_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_8_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_8_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_8_cevaplari.soru_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_8_cevaplari.cevap is null or dbo.sbr_soru_tipi_8_cevaplari.cevap=0)
and dbo.sbr_soru_tipi_8_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_8_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END

GO




CREATE PROCEDURE [dbo].[sbr_acik_anket_soru_tipi_9_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN

CREATE TABLE #TempTable(
anket_uid uniqueidentifier,
giris_key nvarchar(50), 
anket_bitirildi bit)

insert into #TempTable SELECT DISTINCT anket_uid, giris_key, anket_bitirildi
FROM         dbo.sbr_anket_yayinlama_acik_anket_aktivasyon
where dbo.sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=dbo.AnketUidGetir(@soru_uid)


select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_9_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_9_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_9_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_9_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_9_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_9_cevaplari.soru_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_9_cevaplari.cevap is not null )
and dbo.sbr_soru_tipi_9_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_9_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Not Answered' as cevaplandi, CAST(dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_9_cevaplari LEFT OUTER JOIN
                      #TempTable ON 
                      dbo.sbr_soru_tipi_9_cevaplari.cevap_key = #TempTable.giris_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_9_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_9_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_9_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_9_cevaplari.soru_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_9_cevaplari.cevap is null)
and dbo.sbr_soru_tipi_9_secenekleri.soru_uid=@soru_uid 
and #TempTable.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_9_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END

GO



CREATE PROCEDURE [dbo].[sbr_admin_anket_listesi_raporu_sp]
 @anket_durumu_id int
AS
BEGIN

select * from sbr_anket_v where (@anket_durumu_id=0 or anket_durumu_id=@anket_durumu_id )  order by olusturma_tarihi desc

END
GO


CREATE PROCEDURE [dbo].[sbr_anket_soru_cevap_raporu_tumu_sp]
 @anket_uid uniqueidentifier
AS
BEGIN
select ROW_NUMBER() OVER(ORDER BY soru_uid) as 'RowNumber',* from (
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_ad as varchar(4000)) as secenek_ad, dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_1_cevaplari where dbo.sbr_soru_tipi_1_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_1_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_1_cevaplari.cevap = 1 and ( sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_1_cevaplari where dbo.sbr_soru_tipi_1_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_1_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_1_cevaplari.cevap = 1 and ( sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_1_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran

FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_1_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_1_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 1)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_ad as varchar(4000)), dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid
union 
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_ad as varchar(4000)) as secenek_ad, dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_2_cevaplari where dbo.sbr_soru_tipi_2_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_2_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_2_cevaplari.cevap = 1 and ( sbr_soru_tipi_2_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_2_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_2_cevaplari where dbo.sbr_soru_tipi_2_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_2_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_2_cevaplari.cevap = 1 and ( sbr_soru_tipi_2_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_2_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran


FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_2_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_2_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 2)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_ad as varchar(4000)), dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_uid
                      
union 
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_ad as varchar(4000)) as secenek_ad, dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_5_cevaplari where dbo.sbr_soru_tipi_5_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_5_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_5_cevaplari.cevap = 1 and ( sbr_soru_tipi_5_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_5_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_5_cevaplari where dbo.sbr_soru_tipi_5_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_5_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_5_cevaplari.cevap = 1 and ( sbr_soru_tipi_5_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_5_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_5_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_5_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 5)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_ad as varchar(4000)), dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_uid
                      
union 
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_ad as varchar(4000)) as secenek_ad, dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_6_cevaplari where dbo.sbr_soru_tipi_6_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_6_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_6_cevaplari.cevap = 1 and ( sbr_soru_tipi_6_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_6_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_6_cevaplari where dbo.sbr_soru_tipi_6_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_6_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_6_cevaplari.cevap = 1 and ( sbr_soru_tipi_6_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_6_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_6_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_6_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 6)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_ad as varchar(4000)), dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_uid
                      
union              
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Answered' as secenek_ad, dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_4_cevaplari where dbo.sbr_soru_tipi_4_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_4_cevaplari.cevap is not null and CAST(dbo.sbr_soru_tipi_4_cevaplari.cevap as varchar(4000))<>'') and ( sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_4_cevaplari where dbo.sbr_soru_tipi_4_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_4_cevaplari.cevap is not null and CAST(dbo.sbr_soru_tipi_4_cevaplari.cevap as varchar(4000))<>'') and ( sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran

FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_4_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 4)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid

union 
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Not Answered' as secenek_ad, dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_4_cevaplari where dbo.sbr_soru_tipi_4_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_4_cevaplari.cevap is null or CAST(dbo.sbr_soru_tipi_4_cevaplari.cevap as varchar(4000))='') and ( sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_4_cevaplari where dbo.sbr_soru_tipi_4_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_4_cevaplari.cevap is  null or CAST(dbo.sbr_soru_tipi_4_cevaplari.cevap as varchar(4000))='') and ( sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_4_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran

FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_4_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 4)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid
union
       SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad as varchar(4000))+' - Answered' as secenek_ad, dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_7_cevaplari where dbo.sbr_soru_tipi_7_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid and (dbo.sbr_soru_tipi_7_cevaplari.cevap is not null and CAST(dbo.sbr_soru_tipi_7_cevaplari.cevap as varchar(4000))<>'') and ( sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_7_cevaplari where dbo.sbr_soru_tipi_7_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid and (dbo.sbr_soru_tipi_7_cevaplari.cevap is not null and CAST(dbo.sbr_soru_tipi_7_cevaplari.cevap as varchar(4000))<>'') and ( sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_7_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 7)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad as varchar(4000)), dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid
union
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad as varchar(4000))+' - Not Answered' as secenek_ad, dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_7_cevaplari where dbo.sbr_soru_tipi_7_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid and (dbo.sbr_soru_tipi_7_cevaplari.cevap is null or CAST(dbo.sbr_soru_tipi_7_cevaplari.cevap as varchar(4000))='') and ( sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_7_cevaplari where dbo.sbr_soru_tipi_7_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid and (dbo.sbr_soru_tipi_7_cevaplari.cevap is null or CAST(dbo.sbr_soru_tipi_7_cevaplari.cevap as varchar(4000))='') and ( sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_7_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_7_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 7)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id, cast(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad as varchar(4000)), dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid
union    
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Answered' as secenek_ad, dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_8_cevaplari where dbo.sbr_soru_tipi_8_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_8_cevaplari.cevap is not null and dbo.sbr_soru_tipi_8_cevaplari.cevap <>0) and ( sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_8_cevaplari where dbo.sbr_soru_tipi_8_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_8_cevaplari.cevap is not null and dbo.sbr_soru_tipi_8_cevaplari.cevap <>0) and ( sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran


FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_8_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 8)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid

union 
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Not Answered' as secenek_ad, dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_8_cevaplari where dbo.sbr_soru_tipi_8_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_8_cevaplari.cevap is null and dbo.sbr_soru_tipi_8_cevaplari.cevap=0) and ( sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_8_cevaplari where dbo.sbr_soru_tipi_8_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_8_cevaplari.cevap is null or dbo.sbr_soru_tipi_8_cevaplari.cevap=0) and ( sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_8_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_8_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 8)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid
union
           
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Answered' as secenek_ad, dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid,

cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_9_cevaplari where dbo.sbr_soru_tipi_9_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_9_cevaplari.cevap is not null) and ( sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_9_cevaplari where dbo.sbr_soru_tipi_9_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_9_cevaplari.cevap is not null) and ( sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran



FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_9_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 9)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid

union 
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Not Answered' as secenek_ad, dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid,

cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_9_cevaplari where dbo.sbr_soru_tipi_9_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_9_cevaplari.cevap is  null) and ( sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_9_cevaplari where dbo.sbr_soru_tipi_9_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_9_cevaplari.cevap is null) and ( sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_9_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran

FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_9_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 9)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid
union
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Answered' as secenek_ad, dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_10_cevaplari where dbo.sbr_soru_tipi_10_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_10_cevaplari.cevap is not null and CAST(dbo.sbr_soru_tipi_10_cevaplari.cevap as varchar(4000))<>'') and ( sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_10_cevaplari where dbo.sbr_soru_tipi_10_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_10_cevaplari.cevap is not null and CAST(dbo.sbr_soru_tipi_10_cevaplari.cevap as varchar(4000))<>'') and ( sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_10_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 10)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid

union 
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Not Answered' as secenek_ad, dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_10_cevaplari where dbo.sbr_soru_tipi_10_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_10_cevaplari.cevap is null or CAST(dbo.sbr_soru_tipi_10_cevaplari.cevap as varchar(4000))='') and ( sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_10_cevaplari where dbo.sbr_soru_tipi_10_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_10_cevaplari.cevap is null or CAST(dbo.sbr_soru_tipi_10_cevaplari.cevap as varchar(4000))='') and ( sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_10_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_10_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 10)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid
union
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Answered' as secenek_ad, dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid,

cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_11_cevaplari where dbo.sbr_soru_tipi_11_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_11_cevaplari.cevap is not null and CAST(dbo.sbr_soru_tipi_11_cevaplari.cevap as varchar(4000))<>'') and ( sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_11_cevaplari where dbo.sbr_soru_tipi_11_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_11_cevaplari.cevap is not null and CAST(dbo.sbr_soru_tipi_11_cevaplari.cevap as varchar(4000))<>'') and ( sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran

FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_11_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 11)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid

union 
SELECT     dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)) as soru, 
                      dbo.sbr_anket_sorulari.soru_tipi_id, 'Not Answered' as secenek_ad, dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid,
cast(cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_11_cevaplari where dbo.sbr_soru_tipi_11_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_11_cevaplari.cevap is null or CAST(dbo.sbr_soru_tipi_11_cevaplari.cevap as varchar(4000))='') and ( sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))as decimal(18,2)) as int) as varchar(20)) cevap_sayisi,cast(cast(cast(cast((select COUNT(*) from dbo.sbr_soru_tipi_11_cevaplari where dbo.sbr_soru_tipi_11_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and (dbo.sbr_soru_tipi_11_cevaplari.cevap is null or CAST(dbo.sbr_soru_tipi_11_cevaplari.cevap as varchar(4000))='') and ( sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_acik_anket_aktivasyon.giris_key from sbr_anket_yayinlama_acik_anket_aktivasyon where sbr_anket_yayinlama_acik_anket_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_acik_anket_aktivasyon.anket_bitirildi=1) or  sbr_soru_tipi_11_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=@anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))) as decimal(18,2)) as decimal(18,2))/dbo.Cevaplanan_secenek_kayit_sayisi(@anket_uid,dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_tipi_id,dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid)*100 as decimal(18,2)) as varchar(20)) as cevap_oran

FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid LEFT OUTER JOIN
                      dbo.sbr_soru_tipi_11_secenekleri ON dbo.sbr_anket_sorulari.soru_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_uid
WHERE     (dbo.sbr_anket_sorulari.soru_tipi_id = 11)
and dbo.sbr_anket.anket_uid=@anket_uid
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid, dbo.sbr_anket_sorulari.soru_sira_no, dbo.sbr_anket.anket_adi, cast(dbo.sbr_anket_sorulari.soru as varchar(4000)), 
                      dbo.sbr_anket_sorulari.soru_tipi_id,  dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid
 ) as table1 order by soru_sira_no      
END

GO


CREATE PROCEDURE [dbo].[sbr_anket_soru_raporu_sp]
 @anket_uid uniqueidentifier
AS
BEGIN
SELECT   dbo.sbr_anket.anket_uid, dbo.sbr_anket_sorulari.soru_uid,dbo.sbr_anket_sorulari.soru_sira_no , dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru, dbo.sbr_anket_sorulari.soru_tipi_id
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_sorulari.anket_uid
Where      sbr_anket.anket_uid =@anket_uid
and dbo.sbr_anket_sorulari.soru_tipi_id=1
order by   dbo.sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_sira_no                      

END
GO



CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_1_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi, CAST(dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_1_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_1_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_1_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_1_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_1_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_1_cevaplari.soru_uid = dbo.sbr_soru_tipi_1_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_1_cevaplari.cevap = 1)
and dbo.sbr_soru_tipi_1_secenekleri.soru_uid=@soru_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_1_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_1_secenekleri.soru_secenek_ad as varchar(500)) 

END
GO



CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_10_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplandı' as cevaplandi, CAST(dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_10_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_10_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_10_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_10_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_10_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_10_cevaplari.soru_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_10_cevaplari.cevap is not null )
and (ltrim(rtrim(dbo.sbr_soru_tipi_10_cevaplari.cevap))) <>''
and dbo.sbr_soru_tipi_10_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_10_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplanmadı' as cevaplandi, CAST(dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_10_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_10_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_10_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_10_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_10_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_10_cevaplari.soru_uid = dbo.sbr_soru_tipi_10_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_10_cevaplari.cevap is null or ltrim(rtrim(dbo.sbr_soru_tipi_10_cevaplari.cevap)) ='')
and dbo.sbr_soru_tipi_10_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_10_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_10_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END
GO


CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_11_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplandı' as cevaplandi, CAST(dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_11_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_11_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_11_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_11_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_11_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_11_cevaplari.soru_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_11_cevaplari.cevap is not null )
and (ltrim(rtrim(dbo.sbr_soru_tipi_11_cevaplari.cevap))) <>''
and dbo.sbr_soru_tipi_11_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_11_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplanmadı' as cevaplandi, CAST(dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_11_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_11_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_11_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_11_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_11_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_11_cevaplari.soru_uid = dbo.sbr_soru_tipi_11_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_11_cevaplari.cevap is null or ltrim(rtrim(dbo.sbr_soru_tipi_11_cevaplari.cevap)) ='')
and dbo.sbr_soru_tipi_11_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_11_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_11_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END
GO

CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_2_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi, CAST(dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_2_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_2_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_2_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_2_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_2_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_2_cevaplari.soru_uid = dbo.sbr_soru_tipi_2_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_2_cevaplari.cevap = 1)
and dbo.sbr_soru_tipi_2_secenekleri.soru_uid=@soru_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_2_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_2_secenekleri.soru_secenek_ad as varchar(500)) 

END
GO

CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_3_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi,(select COUNT(*) from dbo.sbr_soru_tipi_3_cevaplari as b where b.soru_uid=dbo.sbr_anket_sorulari.soru_uid and b.soru_secenek_uid=dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid and b.cevap = 1 and b.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=dbo.sbr_anket.anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1)) as tum_cevap_sayisi,
             cast(cast(COUNT(*) as decimal(18,2))/cast((select COUNT(*) from dbo.sbr_soru_tipi_3_cevaplari where dbo.sbr_soru_tipi_3_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_3_cevaplari.cevap = 1  and sbr_soru_tipi_3_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=dbo.sbr_anket.anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1)) as decimal(18,2)) as decimal(18,2))*100 as cevap_orani,CAST(dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, 
                      dbo.sbr_anket_sorulari.soru_uid, CAST(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad, dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid, 
                      dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_uid,CAST(dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_ad AS varchar(500)) AS kolon_ad,
                      cast(COUNT(*) as varchar(20))+'/'+CAST((select COUNT(*) from dbo.sbr_soru_tipi_3_cevaplari as b where b.soru_uid=dbo.sbr_anket_sorulari.soru_uid and b.soru_secenek_uid=dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid and b.cevap = 1  and b.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=dbo.sbr_anket.anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))  as varchar(20))+' '+CAST(cast(cast(cast(COUNT(*) as decimal(18,2))/cast((select COUNT(*) from dbo.sbr_soru_tipi_3_cevaplari where dbo.sbr_soru_tipi_3_cevaplari.soru_uid=dbo.sbr_anket_sorulari.soru_uid and dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_uid=dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid and dbo.sbr_soru_tipi_3_cevaplari.cevap = 1  and sbr_soru_tipi_3_cevaplari.cevap_key in (select sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key from sbr_anket_yayinlama_mail_gonderi_aktivasyon where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=dbo.sbr_anket.anket_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1))  as decimal(18,2)) as decimal(18,2))*100 as decimal(18,0)) as varchar(10))+'%' as cevap_orani_str
FROM         dbo.sbr_soru_tipi_3_cevaplari RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_3_secenek_kolonlari ON 
                      dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_kolon_uid = dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_uid LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_3_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_3_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_3_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_3_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_3_cevaplari.soru_uid = dbo.sbr_soru_tipi_3_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_3_cevaplari.cevap = 1) 
and dbo.sbr_soru_tipi_3_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_3_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_ad as varchar(500)) ,dbo.sbr_soru_tipi_3_secenekleri.soru_secenek_uid,CAST(dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_ad AS varchar(500)),dbo.sbr_soru_tipi_3_secenek_kolonlari.soru_secenek_kolon_uid



END
GO

CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_4_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplandı' as cevaplandi, CAST(dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_4_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_4_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_4_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_4_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_4_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_4_cevaplari.soru_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_4_cevaplari.cevap is not null )
and (ltrim(rtrim(cast(dbo.sbr_soru_tipi_4_cevaplari.cevap as nvarchar(4000))))) <>''
and dbo.sbr_soru_tipi_4_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_4_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplanmadı' as cevaplandi, CAST(dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_4_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_4_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_4_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_4_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_4_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_4_cevaplari.soru_uid = dbo.sbr_soru_tipi_4_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_4_cevaplari.cevap is null or ltrim(rtrim(cast(dbo.sbr_soru_tipi_4_cevaplari.cevap as nvarchar(4000)))) ='')
and dbo.sbr_soru_tipi_4_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_4_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_4_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END
GO


CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_5_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi, CAST(dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_5_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_5_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_5_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_5_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_5_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_5_cevaplari.soru_uid = dbo.sbr_soru_tipi_5_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_5_cevaplari.cevap = 1)
and dbo.sbr_soru_tipi_5_secenekleri.soru_uid=@soru_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_5_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_5_secenekleri.soru_secenek_ad as varchar(500)) 

END
GO



CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_6_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid,dbo.sbr_anket_sorulari.soru_uid) AS 'RowNumber',COUNT(*) AS cevap_sayisi, CAST(dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_6_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_6_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_6_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_6_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_6_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_6_cevaplari.soru_uid = dbo.sbr_soru_tipi_6_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_6_cevaplari.cevap = 1)
and dbo.sbr_soru_tipi_6_secenekleri.soru_uid=@soru_uid and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_6_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_6_secenekleri.soru_secenek_ad as varchar(500)) 

END
GO


CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_7_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplandı' as cevaplandi, CAST(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_7_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_7_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_7_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_7_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_7_cevaplari.soru_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_7_cevaplari.cevap is not null )
and (ltrim(rtrim(cast(dbo.sbr_soru_tipi_7_cevaplari.cevap as nvarchar(4000))))) <>''
and dbo.sbr_soru_tipi_7_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_7_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplanmadı' as cevaplandi, CAST(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_7_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_7_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_7_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_7_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_7_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_7_cevaplari.soru_uid = dbo.sbr_soru_tipi_7_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_7_cevaplari.cevap is null or ltrim(rtrim(cast(dbo.sbr_soru_tipi_7_cevaplari.cevap as nvarchar(4000)))) ='')
and dbo.sbr_soru_tipi_7_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_7_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_7_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END
GO



CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_8_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplandı' as cevaplandi, CAST(dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_8_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_8_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_8_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_8_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_8_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_8_cevaplari.soru_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_8_cevaplari.cevap is not null )
and dbo.sbr_soru_tipi_8_cevaplari.cevap<>0
and dbo.sbr_soru_tipi_8_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_8_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplanmadı' as cevaplandi, CAST(dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_8_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_8_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_8_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_8_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_8_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_8_cevaplari.soru_uid = dbo.sbr_soru_tipi_8_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_8_cevaplari.cevap is null or dbo.sbr_soru_tipi_8_cevaplari.cevap=0)
and dbo.sbr_soru_tipi_8_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_8_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_8_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END
GO



CREATE PROCEDURE [dbo].[sbr_anket_soru_tipi_9_cevap_raporu_sp]
 @soru_uid uniqueidentifier
AS
BEGIN
select ROW_NUMBER() OVER(ORDER BY table1.anket_uid,table1.soru_uid) AS 'RowNumber',* from
(
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplandı' as cevaplandi, CAST(dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_9_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_9_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_9_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_9_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_9_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_9_cevaplari.soru_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_9_cevaplari.cevap is not null )
and dbo.sbr_soru_tipi_9_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_9_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_ad as varchar(500)) 
union
SELECT     COUNT(*) AS cevap_sayisi,'Cevaplanmadı' as cevaplandi, CAST(dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_ad AS varchar(500)) AS secenek_ad, dbo.sbr_anket.anket_uid, 
                      dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid , cast(dbo.sbr_anket_sorulari.soru AS varchar(500)) AS soru_ad
FROM         dbo.sbr_soru_tipi_9_cevaplari LEFT OUTER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON 
                      dbo.sbr_soru_tipi_9_cevaplari.cevap_key = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key RIGHT OUTER JOIN
                      dbo.sbr_soru_tipi_9_secenekleri RIGHT OUTER JOIN
                      dbo.sbr_anket_sorulari ON dbo.sbr_soru_tipi_9_secenekleri.soru_uid = dbo.sbr_anket_sorulari.soru_uid RIGHT OUTER JOIN
                      dbo.sbr_anket ON dbo.sbr_anket_sorulari.anket_uid = dbo.sbr_anket.anket_uid ON 
                      dbo.sbr_soru_tipi_9_cevaplari.soru_secenek_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_uid AND 
                      dbo.sbr_soru_tipi_9_cevaplari.soru_uid = dbo.sbr_soru_tipi_9_secenekleri.soru_uid
WHERE     (dbo.sbr_soru_tipi_9_cevaplari.cevap is null)
and dbo.sbr_soru_tipi_9_secenekleri.soru_uid=@soru_uid 
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1
group by dbo.sbr_anket.anket_uid, dbo.sbr_anket.anket_adi, dbo.sbr_anket_sorulari.soru_uid,cast(dbo.sbr_anket_sorulari.soru AS varchar(500)),dbo.sbr_soru_tipi_9_secenekleri.soru_uid,cast(dbo.sbr_soru_tipi_9_secenekleri.soru_secenek_ad as varchar(500)) 
) as table1
END
GO



CREATE PROCEDURE [dbo].[sbr_kullanici_bazli_anket_durum_raporu_sp]
 @anket_uid uniqueidentifier
AS
BEGIN

SELECT     dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.id,dbo.GetDateFaormatedString(dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_tarihi,1) as anket_gonderilme_tarihi, 
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_email, 
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_ad + ' ' + dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_soyad AS cevaplayan,
                       dbo.sbr_anket.anket_uid, dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.ankete_girildi, 
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.ankete_cevap_verildi, dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi, 
                      CASE WHEN dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.ankete_girildi = 1 THEN 'Evet' ELSE 'Hayır' END AS ankete_girildi_str, 
                      CASE WHEN dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.ankete_cevap_verildi = 1 THEN 'Evet' ELSE 'Hayır' END AS ankete_cevap_verildi_str, 
                      CASE WHEN dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi = 1 THEN 'Evet' ELSE 'Hayır' END AS anket_bitirildi_str
FROM         dbo.sbr_anket INNER JOIN
                      dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon ON dbo.sbr_anket.anket_uid = dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid
where    sbr_anket.anket_uid=@anket_uid
order by dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_ad,dbo.sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_soyad


END
GO


CREATE PROCEDURE [dbo].[sbr_kullanici_bazli_anket_raporu_sp]
 @anket_uid uniqueidentifier,
 @email varchar(500)		
AS
BEGIN

SELECT     sbr_anket_yayinlama_mail_gonderi_aktivasyon.tarihce_uid,sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_tarihi,sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi,sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirilme_tarihi,sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_email,sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_ad+' '+sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_soyad as cevaplayan, sbr_anket.anket_uid, sbr_anket.anket_adi, sbr_anket_sorulari.soru_uid, sbr_anket_sorulari.soru, sbr_anket_sorulari.soru_tipi_id,dbo.GetKullaniciBazliAnketCevaplari(sbr_anket.anket_uid,sbr_anket_sorulari.soru_uid,sbr_anket_sorulari.soru_tipi_id,@email) as cevap,
case when sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=1 then sbr_anket.anket_adi+' - Bitirildi ('+dbo.GetDateFaormatedString(sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirilme_tarihi,1)+')' else sbr_anket.anket_adi+' - Devam Ediyor' end as anket_adi_durumu,sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_ad+' '+sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_soyad as cavaplayan_gonderilme_tarihi
FROM       sbr_anket,sbr_anket_sorulari,sbr_anket_yayinlama_mail_gonderi_aktivasyon 
Where      sbr_anket.anket_uid = sbr_anket_sorulari.anket_uid
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=sbr_anket.anket_uid
and sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilen_email=@email
And    sbr_anket.anket_uid=@anket_uid
order by sbr_anket_sorulari.soru_sira_no 

END
GO



CREATE PROCEDURE [dbo].[sbr_kullanici_bazli_bitirme_raporu_sp]
 @anket_uid uniqueidentifier
AS
BEGIN

SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid) AS 'RowNumber',COUNT(*) as ankete_bitirme_sayisi,sbr_anket.anket_uid, sbr_anket.anket_adi,case when (sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi is null or sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=0) then 'Bitirilmedi' else 'Bitirildi' end as anket_bitirme_durumu
FROM       sbr_anket,sbr_anket_yayinlama_mail_gonderi_aktivasyon 
where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=sbr_anket.anket_uid
And    sbr_anket.anket_uid=@anket_uid
group by sbr_anket.anket_uid,sbr_anket.anket_adi,case when (sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi is null or sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_bitirildi=0) then 'Bitirilmedi' else 'Bitirildi' end
END
GO



CREATE PROCEDURE [dbo].[sbr_kullanici_bazli_cevaplanma_raporu_sp]
 @anket_uid uniqueidentifier
AS
BEGIN

SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid) AS 'RowNumber',COUNT(*) as ankete_cevaplama_sayisi,sbr_anket.anket_uid, sbr_anket.anket_adi,case when (dbo.KatilimciSoruCevaplandiMi(sbr_anket.anket_uid,sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key)=0) then 'Cevap Verilmedi' else 'Cevap Verildi' end as anket_cevaplanma_durumu
FROM       sbr_anket,sbr_anket_yayinlama_mail_gonderi_aktivasyon 
where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=sbr_anket.anket_uid
And    sbr_anket.anket_uid=@anket_uid
group by sbr_anket.anket_uid,sbr_anket.anket_adi,case when (dbo.KatilimciSoruCevaplandiMi(sbr_anket.anket_uid,sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_gonderilme_key)=0) then 'Cevap Verilmedi' else 'Cevap Verildi' end 
END
GO


CREATE PROCEDURE [dbo].[sbr_kullanici_bazli_giris_raporu_sp]
 @anket_uid uniqueidentifier
AS
BEGIN

SELECT     ROW_NUMBER() OVER(ORDER BY sbr_anket.anket_uid) AS 'RowNumber',sbr_anket.anket_uid,COUNT(*) as ankete_giris_sayisi, sbr_anket.anket_adi,case when (sbr_anket_yayinlama_mail_gonderi_aktivasyon.ankete_girildi is null or sbr_anket_yayinlama_mail_gonderi_aktivasyon.ankete_girildi=0) then 'Bakılmadı' else 'Bakıldı' end as anket_giris_durumu
FROM       sbr_anket,sbr_anket_yayinlama_mail_gonderi_aktivasyon 
where sbr_anket_yayinlama_mail_gonderi_aktivasyon.anket_uid=sbr_anket.anket_uid
And    sbr_anket.anket_uid=@anket_uid
group by sbr_anket.anket_uid,sbr_anket.anket_adi,case when (sbr_anket_yayinlama_mail_gonderi_aktivasyon.ankete_girildi is null or sbr_anket_yayinlama_mail_gonderi_aktivasyon.ankete_girildi=0) then 'Bakılmadı' else 'Bakıldı' end 
END

GO


--select * from CRMInterviewsList

CREATE PROCEDURE [dbo].[Search]
 @Content nvarchar(200),
 @Object varchar(200)		
AS
BEGIN

DECLARE @Column varchar(200)
DECLARE @Criteria varchar(4000)

DECLARE cur CURSOR FOR select name from syscolumns where Id =(select id from sysobjects where name =@Object) and (xtype=167 or xtype=231 or xtype=35 or xtype=99)
        OPEN cur
        FETCH NEXT FROM cur INTO @Column
		WHILE (@@FETCH_STATUS = 0) 
		BEGIN
		
		IF(@Criteria IS NOT NULL AND @Criteria<>'')
		BEGIN
			SET @Criteria =	@Criteria +' OR '+	'['+@Column+']'	+' LIKE ''%'+@Content+'%'''
		END
		ELSE
		BEGIN
			SET @Criteria = '['+@Column +']'	+' LIKE ''%'+@Content+'%'''
		END

			FETCH NEXT FROM cur INTO @Column
		END
		CLOSE cur
		DEALLOCATE cur
SET @Criteria =' ( '+@Criteria+' ) '
select @Criteria as Criteria
END



GO



