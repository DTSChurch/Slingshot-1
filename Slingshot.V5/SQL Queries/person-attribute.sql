SELECT 
	-- Key --
	CASE 
	WHEN [Profile] = 'SCHOOL' THEN 'V5SCHOOL' 
	ELSE [Profile]
	END AS [Key],
	-- Name --
	[Descr] as [Name],
	-- Field Type --
	'Rock.Field.Types.TextFieldType' AS [FieldType],
	-- Category --
	'V5' AS [Category]
FROM [Shelby].[NAProfileTypes]
WHERE [Profile] IN (
'2012',
'2013',
'2014',
--'ALLERG',
--'AUTODONE',
--'AUTONCNS',
'AUTONOND',
--'BENAPP',
--'BENDEN',
'CAREAUTO',
'CARECLN',
'CAREERND',
'CAREHOSP',
'CAREMEAL',
'GRADE1',
'GRADE10',
'GRADE11',
'GRADE12',
'GRADE2',
'GRADE3',
'GRADE4',
'GRADE5',
'GRADE6',
'GRADE7',
'GRADE8',
'GRADE9',
'GRADEK',
'GRADES',
'LASTCN',
'MAIL',
'MENATWRK',
--'NOTE1',
--'QDATA',
'SCHOOL',
'SLA3K',
'SLACH',
'SLAEC',
'SLAEL',
'SLD3K',
'SLDCH',
'SLDEC',
'SLDEL',
'SLL12',
'SLL35',
'SLL3YO',
'SLL4YO',
'SLL5K',
'SLLCH',
'SLLCOF',
'SLLEC',
'SLLM3K',
'SLLMEC',
'SLLMEL',
'SLLSUB',
'SLW12',
'SLW1YO',
'SLW2YO',
'SLW35',
'SLW3YO',
'SLW4YO',
'SLW5YO',
'SLWCH',
'SLWCOF',
'SLWINF',
'SLWK',
'SLWSUB',
--'SPECIA',
'STAFF'
)
UNION

SELECT
	'V5BaptismDate'
	, 'Baptism Date'
	, 'Rock.Field.Types.DateFieldType'
	, 'V5'