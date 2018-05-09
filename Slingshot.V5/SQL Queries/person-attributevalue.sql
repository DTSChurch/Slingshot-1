
SELECT
	distinct 
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
		WHEN ([Comment] IS NULL OR [Comment] = '') AND ([Start] IS NOT NULL) THEN ISNULL(REPLACE(REPLACE(dbo.KeepSafeCharacters([Comment], 1),CHAR(13),''),CHAR(10),''),'TRUE')
		ELSE ISNULL(REPLACE(REPLACE([Comment], CHAR(13), ''), CHAR(10), ''),'TRUE')
	END AS [AttributeValue]
FROM [Shelby].[NAProfiles]
where [Counter] NOT IN (15476,15477,15479,28065,28066,28067) --get rid of the repeats, specifically for BCC
UNION ALL

--Membership Date
select 
[NameCounter] AS [PersonId]
, 'MembershipDate' as [AttributeKey]
,  CONVERT(VARCHAR(10),ISNULL(DateReceived, GETDATE()), 101) as [AttributeValue]
from Shelby.vw_MBNames
where DateReceived is not null and MemberStatus = 'Member'
UNION ALL

--Baptism Date
select 
[NameCounter] AS [PersonId]
, 'BaptismDate' as [AttributeKey]
,  CONVERT(VARCHAR(10),ISNULL(DateReceived, GETDATE()), 101) as [AttributeValue]
from Shelby.vw_MBNames
where HowReceived = 'Baptism' and DateReceived is not null
UNION ALL


--Baptized Here
select 
[NameCounter] AS [PersonId]
, 'BaptizedHere' as [AttributeKey]
,  '1' as [AttributeValue]
from Shelby.vw_MBNames
where HowReceived = 'Baptism' and DateReceived is not null
