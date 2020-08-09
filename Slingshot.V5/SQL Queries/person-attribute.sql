SELECT 
	-- Key --
	CASE 
	WHEN [Profile] = 'SCHOOL' THEN 'V5SCHOOL' 
	ELSE [Profile]
	END AS [Key],
	-- Name --
	[Descr] as [Name],
	-- Field Type --
	'Rock.Field.Types.DateFieldType' AS [FieldType],	
	'V5' AS [Category]
FROM [Shelby].[NAProfileTypes] 
WHERE [Profile] IN ('LASTCN','ATSOSASO','ATSOSTSO','ACTBASE','ATSOSTSL','ATSOSASL'
	,'CNSOSASO','CNSOSTSO','CNRISKTR','CNRISKAR','ACTSOC','CNSOSTSL','CNSOSASL','CNRISKTK','CNRISKAK'
	,'CNRISKAI','FRCLABEL','CNRISKTS','ATSOSAON','CNRISKTI','CNRISKAS','ALLERG','CNSOSAON','BENVAID')

UNION ALL
SELECT
-- Key --
'LastVisit' AS [Key],
	-- Name --
'Shelby Last Visit' AS Name,
	-- Field Type --
	'Rock.Field.Types.DateFieldType' AS [FieldType],
	'V5' AS Category

UNION ALL
SELECT
-- Key --
'FirstVisit' AS [Key],
	-- Name --
'Shelby First Visit' AS Name,
	-- Field Type --
	'Rock.Field.Types.DateFieldType' AS [FieldType],
	'V5' AS Category


--Text Fields
UNION ALL
SELECT 
	-- Key --
	[Profile] AS [Key],
	-- Name --
	[Descr] as [Name],
	-- Field Type --
	'Rock.Field.Types.TextFieldType' AS [FieldType],	
	'V5' AS [Category]
FROM [Shelby].[NAProfileTypes] 
WHERE [Profile] IN ('ALLERG')

UNION ALL

SELECT 
	-- Key --
	[Profile] AS [Key],
	-- Name --
	[Descr] as [Name],
	-- Field Type --
	'Rock.Field.Types.BooleanFieldType' AS [FieldType],	
	'V5' AS [Category]
FROM [Shelby].[NAProfileTypes] 
WHERE [Profile] IN ('1STHALF','2NDHALF','BIG','PLEDGEAB','THERES','10.45','9.00','CHOIR','ORDNDEAC'
	,'THFH','ABSW','GREETER','PEP','USHER','AWANA','ORCH','YOUTH','LIFE','HOST','SHUTTLE','PRAYER'
	,'FAITH','CUBBIE','COUNSELR')

UNION ALL

SELECT 	
	'MEMOONPERSON' AS [Key],	
	'Memo' as [Name],	
	'Rock.Field.Types.MemoFieldType' AS [FieldType],	
	'V5' AS [Category]
