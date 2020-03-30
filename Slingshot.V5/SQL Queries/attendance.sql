select
AttendanceDetails.Counter as [AttendanceId]
,[NameCounter] as PersonId
, GroupStats.SGOrgGrpCounter as GroupId
,'' AS [LocationId]
,'' AS [ScheduleId]
,'' AS [DeviceId]
, OrgStats.SGDate AS [StartDateTime]
,'' AS [EndDateTime]
,'' AS [Note]
,1 AS [CampusId]
from
 Shelby.SGMstOrgAtt AttendanceDetails inner join
 Shelby.SGMstOrg Enrollments on AttendanceDetails.SGMstOrgCounter = Enrollments.Counter inner join
 Shelby.SGOrgGrpDat GroupStats on AttendanceDetails.SGOrgGrpDatCounter = GroupStats.Counter inner join
 Shelby.SGOrgDat OrgStats ON GroupStats.SGOrgDatCounter = OrgStats.Counter left join
 Shelby.SGOrg as Organizations on OrgStats.SGOrgCounter = Organizations.Counter
 where 
 Organizations.Descr like '%' and SGAttTyp = 'P'
 order by AttendanceId
 