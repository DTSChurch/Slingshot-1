SELECT
ROW_NUMBER() OVER(ORDER BY Start_Date_Time) AS [AttendanceId]
,Individual_ID AS [PersonId]
,RLC_ID AS [GroupId]
,'' AS [LocationId]
,'' AS [ScheduleId]
,'' AS [DeviceId]
,Start_Date_Time AS [StartDateTime]
,'' AS [EndDateTime]
,'' AS [Note]
,'' AS [CampusId]
FROM Attendance
WHERE Individual_ID IS NOT NULL
ORDER BY Start_Date_Time