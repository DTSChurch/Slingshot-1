--All Date Attribute Values

select 
[NameCounter] as [PersonId]
, [Profile] as [AttributeKey]
, CONVERT(VARCHAR(10),Min([Start]), 101) as [AttributeValue]
FROM [Shelby].[NAProfiles]
WHERE [Profile] IN ('LASTCN','ATSOSASO','ATSOSTSO','ACTBASE','ATSOSTSL','ATSOSASL'
	,'CNSOSASO','CNSOSTSO','CNRISKTR','CNRISKAR','ACTSOC','CNSOSTSL','CNSOSASL','CNRISKTK','CNRISKAK'
	,'CNRISKAI','FRCLABEL','CNRISKTS','ATSOSAON','CNRISKTI','CNRISKAS','ALLERG','CNSOSAON','BENVAID') and [Start] is not null AND [NameCounter] > 0
group by [NameCounter],[Profile]

UNION ALL

--All Lettered Out Dates
select 
[NameCounter] as [PersonId]
, 'LetteredOut' as [AttributeKey]
, CONVERT(VARCHAR(10),Min(Date4), 101) as [AttributeValue]
FROM [Shelby].[MBMst]
WHERE Date4 <> '' 
Group By [NameCounter]

UNION ALL

--All Lunch With Pastors Dates
select 
[NameCounter] as [PersonId]
, 'PastorLunch' as [AttributeKey]
, CONVERT(VARCHAR(10),Min(Date5), 101) as [AttributeValue]
FROM [Shelby].[MBMst]
WHERE Date5 <> '' 
Group By [NameCounter]

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
UNION ALL

--Memo
select 
[NameCounter] AS [PersonId]
, 'MEMOONPERSON' as [AttributeKey]
,  Memo as [AttributeValue]
from Shelby.vw_MBNames  n
where Memo != ''
UNION ALL

--Envelope Number
select 
[NameCounter] AS [PersonId]
, 'core_GivingEnvelopeNumber' as [AttributeKey]
,  Cast(EnvNu as varchar) as [AttributeValue]
from Shelby.NANames n
where EnvNu is not null and EnvNu > 0

UNION ALL

SELECT
NameCounter As PersonId,
'FirstVisit' As AttributeKey,
CONVERT(VARCHAR(10),ISNULL(MIN(VisitDate), GETDATE()), 101) as [AttributeValue]
FROM Shelby.MBMstDte 
GROUP BY NameCounter

UNION ALL

SELECT
NameCounter As PersonId,
'LastVisit' As AttributeKey,
CONVERT(VARCHAR(10),ISNULL(MAX(VisitDate), GETDATE()), 101) as [AttributeValue]
FROM Shelby.MBMstDte 
GROUP BY NameCounter
