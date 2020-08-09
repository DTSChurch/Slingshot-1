 
SELECT 
	-- PersonId --
	n.[NameCounter] AS [PersonId],
	-- Id -- 
		CASE
		WHEN REPLACE(REPLACE(p1.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p1.Counter
		WHEN REPLACE(REPLACE(p2.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p2.Counter
		WHEN REPLACE(REPLACE(p3.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p3.Counter
		WHEN REPLACE(REPLACE(p4.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p4.Counter
		WHEN REPLACE(REPLACE(p5.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p5.Counter
		WHEN REPLACE(REPLACE(p6.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p6.Counter
		WHEN REPLACE(REPLACE(p7.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p7.Counter
		WHEN REPLACE(REPLACE(p8.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p8.Counter
		WHEN REPLACE(REPLACE(p9.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN p9.Counter
		END AS [Id],
	-- NoteType --
	CASE
	--SELECT REPLACE(REPLACE(p1.[Comment], CHAR(13), ''), CHAR(10), '')
		WHEN REPLACE(REPLACE(p1.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'Allergy Note'
		WHEN REPLACE(REPLACE(p2.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'General Note'
		WHEN REPLACE(REPLACE(p3.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'General Note'
		WHEN REPLACE(REPLACE(p4.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'Medical Note'

		WHEN REPLACE(REPLACE(p5.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'CMHost Note'
		WHEN REPLACE(REPLACE(p6.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'Benevolence Note'
		WHEN REPLACE(REPLACE(p7.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'Benevolence Note'
		WHEN REPLACE(REPLACE(p8.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'Auto Note'
		WHEN REPLACE(REPLACE(p9.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'Auto Note'
		ELSE ''
	END AS [NoteType],
	-- Caption --
	'' AS [Caption],
	-- IsAlert --
	CASE
		WHEN REPLACE(REPLACE(p1.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'TRUE'
		WHEN REPLACE(REPLACE(p2.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p3.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p4.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'TRUE'

		WHEN REPLACE(REPLACE(p5.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p6.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p7.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p8.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p9.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		ELSE ''
	END AS [IsAlert],
	-- IsPrivate --
	CASE
		WHEN REPLACE(REPLACE(p1.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p2.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p3.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'TRUE'
		WHEN REPLACE(REPLACE(p4.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'

		WHEN REPLACE(REPLACE(p5.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p6.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p7.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p8.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		WHEN REPLACE(REPLACE(p9.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'FALSE'
		ELSE ''
	END as [IsPrivateNote],
	-- Text --
	CASE
		WHEN REPLACE(REPLACE(p1.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN REPLACE(REPLACE(p1.[Comment], CHAR(13), ''), CHAR(10), '')
		WHEN REPLACE(REPLACE(p2.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN REPLACE(REPLACE(p2.[Comment], CHAR(13), ''), CHAR(10), '')
		WHEN REPLACE(REPLACE(p3.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN 'See Shelby'
		WHEN REPLACE(REPLACE(p4.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN REPLACE(REPLACE(p4.[Comment], CHAR(13), ''), CHAR(10), '')
		
		WHEN REPLACE(REPLACE(p5.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN REPLACE(REPLACE(p5.[Comment], CHAR(13), ''), CHAR(10), '')
		WHEN REPLACE(REPLACE(p6.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN REPLACE(REPLACE(p6.[Comment], CHAR(13), ''), CHAR(10), '')
		WHEN REPLACE(REPLACE(p7.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN REPLACE(REPLACE(p7.[Comment], CHAR(13), ''), CHAR(10), '')
		WHEN REPLACE(REPLACE(p8.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN REPLACE(REPLACE(p8.[Comment], CHAR(13), ''), CHAR(10), '')
		WHEN REPLACE(REPLACE(p9.[Comment], CHAR(13), ''), CHAR(10), '') IS NOT NULL THEN REPLACE(REPLACE(p9.[Comment], CHAR(13), ''), CHAR(10), '')
		ELSE ''
	END as [Text],
	-- DateTime --
	'' AS [DateTime],
	-- CreateByPersonId --
	'' AS [CreatedByPersonId]

FROM [Shelby].[NANames] n
LEFT OUTER JOIN [Shelby].[NAProfiles] p1 ON n.[NameCounter] = p1.[NameCounter] AND p1.[Profile] = 'ALLERG'
LEFT OUTER JOIN [Shelby].[NAProfiles] p2 ON n.[NameCounter] = p2.[NameCounter] AND p2.[Profile] = 'NOTE1'
LEFT OUTER JOIN [Shelby].[NAProfiles] p3 ON n.[NameCounter] = p3.[NameCounter] AND p3.[Profile] = 'QDATA'
LEFT OUTER JOIN [Shelby].[NAProfiles] p4 ON n.[NameCounter] = p4.[NameCounter] AND p4.[Profile] = 'SPECIA'
LEFT OUTER JOIN [Shelby].[NAProfiles] p5 ON n.[NameCounter] = p5.[NameCounter] AND p5.[Profile] = 'CM24H'
LEFT OUTER JOIN [Shelby].[NAProfiles] p6 ON n.[NameCounter] = p6.[NameCounter] AND p6.[Profile] = 'BENDEN'
LEFT OUTER JOIN [Shelby].[NAProfiles] p7 ON n.[NameCounter] = p7.[NameCounter] AND p7.[Profile] = 'BENAPP'
LEFT OUTER JOIN [Shelby].[NAProfiles] p8 ON n.[NameCounter] = p8.[NameCounter] AND p8.[Profile] = 'AUTONCNS'
LEFT OUTER JOIN [Shelby].[NAProfiles] p9 ON n.[NameCounter] = p9.[NameCounter] AND p9.[Profile] = 'AUTODONE'

WHERE 
-- Limiting to ONLY the 9 Note Types
p1.[Comment] IS NOT NULL OR
p2.[Comment] IS NOT NULL OR
p3.[Comment] IS NOT NULL OR
p4.[Comment] IS NOT NULL OR
p5.[Comment] IS NOT NULL OR
p6.[Comment] IS NOT NULL OR
p7.[Comment] IS NOT NULL OR
p8.[Comment] IS NOT NULL OR
p9.[Comment] IS NOT NULL

--ORDER BY [PersonId]

UNION ALL 

SELECT
 L.NameCounter AS PersonId,
 MBMstLifCounter AS Id,
 L1.Descr As NoteType,
 '' AS Caption,
 'FALSE' AS IsAlert,
 'FALSE' AS IsPrivateNote,
 isnull(W1a.Descr, case W1b.TitleCounter when -1 then W1b.LastName else W1b.FirstMiddle + ' ' + W1b.LastName end) As Text,
 L.WhenSetup AS DateTime,
 '' AS [CreatedByPersonId]
from
 Shelby.MBMstLif as L inner join
 Shelby.MBLif as L1 on L.MBLifCounter = L1.MBLifCounter left join
 Shelby.MBTextPicks as W1a on L.Who1Counter = W1a.Counter left join
 Shelby.NANames as W1b on L.Who1NameCounter = W1b.NameCounter left join
 Shelby.MBTextPicks as W2a on L.Who2Counter = W2a.Counter left join
 Shelby.NANames as W2b on L.Who2NameCounter = W2b.NameCounter left join
 Shelby.MBTextPicks as Lo on L.LocationCounter = Lo.Counter left join
 Shelby.MBTextPicks as CE on L.CongregationEnrolledCounter = CE.Counter left join
 Shelby.MBTextPicks as T on L.TypeCounter = T.Counter