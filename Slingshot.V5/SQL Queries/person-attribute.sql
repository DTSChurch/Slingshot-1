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
WHERE [Profile] IN ('1-VISIT','2-VISIT')
UNION ALL
SELECT 
	-- Key --
	CASE 
	WHEN [Profile] = 'SCHOOL' THEN 'V5SCHOOL' 
	ELSE [Profile]
	END AS [Key],
	-- Name --
	[Descr] as [Name],
	-- Field Type --
	'Rock.Field.Types.BooleanFieldType' AS [FieldType],	
	'V5' AS [Category]
FROM [Shelby].[NAProfileTypes] 
WHERE [Profile] IN ('18VFAITH','18VGREET','RNOWLIST')
UNION ALL
SELECT 	
	'MEMOONPERSON' AS [Key],	
	'Memo' as [Name],	
	'Rock.Field.Types.MemoFieldType' AS [FieldType],	
	'V5' AS [Category]
