-- This is the date we use to grab only recent groups
DECLARE @StartDate DateTime = '2019-01-01';

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
,'' AS [CampusId]
from
 Shelby.SGMstOrgAtt AttendanceDetails inner join
 Shelby.SGMstOrg Enrollments on AttendanceDetails.SGMstOrgCounter = Enrollments.Counter inner join
 Shelby.SGOrgGrpDat GroupStats on AttendanceDetails.SGOrgGrpDatCounter = GroupStats.Counter inner join
 Shelby.SGOrgDat OrgStats ON GroupStats.SGOrgDatCounter = OrgStats.Counter left join
 Shelby.SGOrg as Organizations on OrgStats.SGOrgCounter = Organizations.Counter
 where SGAttTyp = 'P' -- Grab only people marked Present --
	-- Grab only active groups in the date rage --
	AND GroupStats.SGOrgGrpCounter IN (
				SELECT	grp.[Counter] AS [Id]
					FROM [Shelby].[SGOrgGrp] grp
					LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
					LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]
					WHERE grp.WhenUpdated >= @StartDate
					Group By grp.[Counter],grp.[Descr],grp.[SGOrgLvlCounter],lvl.SGOrgCounter,org.Descr
				)
 order by AttendanceId
 