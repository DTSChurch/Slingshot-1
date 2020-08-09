-- This is the date we use to grab only recent groups
DECLARE @StartDate DateTime = '2019-01-01';
DECLARE @GroupOffset int = '30000';

--Imported normal group attendance
select
AttendanceDetails.Counter as [AttendanceId]
,[NameCounter] as PersonId
, GroupStats.SGOrgGrpCounter + @GroupOffset as GroupId
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
					WHERE grp.WhenSetup >= @StartDate
					Group By grp.[Counter],grp.[Descr],grp.[SGOrgLvlCounter],lvl.SGOrgCounter,org.Descr
				)
 --order by AttendanceId

 UNION ALL
 
-- CONNECT GROUP ATTENDANCE
select
AttendanceDetails.Counter as [AttendanceId]
,[NameCounter] as PersonId
, GroupStats.SGOrgGrpCounter + @GroupOffset as GroupId
,'' AS [LocationId]
,'' AS [ScheduleId]
,'' AS [DeviceId]
, OrgStats.SGDate AS [StartDateTime]
,'' AS [EndDateTime]
,'' AS [Note]
,'' AS [CampusId]
from
 Shelby.SGMstOrgAtt AttendanceDetails inner join
 Shelby.SGMstOrg Enrollments on AttendanceDetails.SGMstOrgCounter = Enrollments.Counter AND Enrollments.Enrolled <0 inner join
 Shelby.SGOrgGrpDat GroupStats on AttendanceDetails.SGOrgGrpDatCounter = GroupStats.Counter inner join
 Shelby.SGOrgDat OrgStats ON GroupStats.SGOrgDatCounter = OrgStats.Counter left join
 Shelby.SGOrg as Organizations on OrgStats.SGOrgCounter = Organizations.Counter
 where SGAttTyp = 'P' -- Grab only people marked Present --
	-- Grab only active groups in the date rage --
	AND GroupStats.SGOrgGrpCounter IN (
				5,7,9,46,50,122,169,170,337,392,428,704,1101,1658,2042,2737,2763,3106,3708,3795,3811,3937,4052,4582,4799,4822,5213,5214,5286,5294,5496,5705,5766,5993,6052)
 order by AttendanceId
 