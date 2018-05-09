select
ROW_NUMBER() OVER(ORDER BY OrgStats.SGDate) AS [AttendanceId]
,[NameCounter] as PersonId
, Enrollments.SGOrgCounter as GroupId
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