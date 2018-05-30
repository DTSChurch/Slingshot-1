/*
select * from Shelby.MBMst
where NameCounter = 8043
select * from Shelby.MBMstDte
where NameCounter = 8043 order by VisitDate desc
*/


select
ROW_NUMBER() OVER(ORDER BY OrgStats.SGDate) AS [AttendanceId]
,[NameCounter] as PersonId
, GroupStats.SGOrgGrpCounter as GroupId
,'' AS [LocationId]
,'' AS [ScheduleId]
,'' AS [DeviceId]
, OrgStats.SGDate AS [StartDateTime]
,'' AS [EndDateTime]
,'' AS [Note]
,'' AS [CampusId]
from
 Shelby.SGMstOrgAtt AttendanceDetails inner join
 Shelby.SGMstOrg Enrollments on AttendanceDetails.SGMstOrgCounter = Enrollments.Counter inner join
 Shelby.SGOrgGrpDat GroupStats on AttendanceDetails.SGOrgGrpDatCounter = GroupStats.Counter inner join
 Shelby.SGOrgDat OrgStats ON GroupStats.SGOrgDatCounter = OrgStats.Counter left join
 Shelby.SGOrg as Organizations on OrgStats.SGOrgCounter = Organizations.Counter
 where 
 Organizations.Descr like '%' and SGAttTyp = 'P' 
 UNION ALL

 select
	ROW_NUMBER() OVER(ORDER BY VTable.VisitDate) + 300375 AS [AttendanceId]
	,[NameCounter] as PersonId
	, 1 as GroupId
	,'' AS [LocationId]
	,'' AS [ScheduleId]
	,'' AS [DeviceId]
	, VTable.VisitDate AS [StartDateTime]
	,'' AS [EndDateTime]
	,'' AS [Note]
	,'' AS [CampusId]
from Shelby.MBMstDte VTable

