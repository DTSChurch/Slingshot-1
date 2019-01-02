SELECT DISTINCT
	-- PersonId --
	[NameCounter] AS [PersonId],
	-- AttributeKey --
	CASE
	WHEN [Profile] = 'SCHOOL' THEN 'V5SCHOOL'
	ELSE [Profile]
	END as [AttributeKey],
	-- AttributeValue --
	--REPLACE(ad.Adr1, ',', ' ')
	CASE
		WHEN ([Comment] IS NULL OR [Comment] = '') AND ([Start] IS NOT NULL) THEN CONVERT(varchar(10), [Start], 126)
		ELSE ISNULL(REPLACE(REPLACE([Comment], CHAR(13), ''), CHAR(10), ''),'TRUE')
	END AS [AttributeValue]
FROM [Shelby].[NAProfiles]
WHERE [Profile] IN (
'2012',
'2013',
'2014',
--'ALLERG', -- We have imported this into an Allergy Note
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
/*  -- We have imported the folloewing into Rock's Graduation Year
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
*/

'GRADES',
'LASTCN',
'MAIL',
'MENATWRK',
--'NOTE1', -- We have imported this into a General Note
--'QDATA', -- We have imported this into a General Note (Set as Private)
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
--'SPECIA', -- We have imported this into a Medical Note
'STAFF'
)

UNION ALL

SElECT
 d.[NameCounter] as PersonId
 , 'V5BaptismDate' as AttributeKey
 , CONVERT(varchar(10), d.Date1, 126) as AttributeValue
FROM
Shelby.MBMst d
WHERE d.Date1 is not null

 
