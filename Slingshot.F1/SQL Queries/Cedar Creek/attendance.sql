SELECT
	ROW_NUMBER() OVER(ORDER BY Start_Date_Time) AS [AttendanceId]
	,A.Individual_ID AS [PersonId]
	,R.RLC_ID AS [GroupId]
	,'' AS [LocationId]
	--,CONCAT(DATEPART(dw, Start_Date_Time), DATEPART(HOUR, Start_Date_Time),DATEPART(MINUTE, Start_Date_Time)) AS [ScheduleId]
	,AA.ActivityScheduleID AS [ScheduleId]
	,'' AS [DeviceId]
	,Start_Date_Time AS [StartDateTime]
	,'' AS [EndDateTime]
	,'' AS [Note]
	,CASE
			WHEN AM.Activity_Name LIKE '%FN' THEN '1179974529'
			WHEN AM.Activity_Name LIKE '%PB' THEN '853354528'
			WHEN AM.Activity_Name LIKE '%ST' THEN '825227893'
			WHEN AM.Activity_Name LIKE '%WH' THEN '1294662989'
			WHEN AM.Activity_Name LIKE '%WT' THEN '782534649'
			WHEN AM.Activity_Name LIKE '%TOL' THEN '782534649'
			WHEN AM.Activity_Name LIKE 'PB%' THEN '853354528'
			WHEN AM.Activity_Name LIKE '%FIN' THEN '1179974529'
			WHEN AM.Activity_Name LIKE 'Whitehouse%' THEN '1294662989'
			WHEN AM.Activity_Name LIKE 'Perrysburg%' THEN '853354528'
			WHEN AM.Activity_Name LIKE 'Toledo Life Groups' THEN '782534649'
			WHEN AM.Activity_Name LIKE 'STOL%' THEN '825227893'
			WHEN AM.Activity_Name LIKE 'WTOL%' THEN '782534649'
			WHEN AM.Activity_Name LIKE 'WH%' THEN '1294662989'
			WHEN AM.Activity_Name LIKE 'FIN%' THEN '1179974529'
			WHEN AM.Ministry_Name = 'CedarVille PB' THEN '853354528'
			WHEN AM.Activity_Name LIKE 'WT%' THEN '782534649'
			ELSE ''
		END AS [CampusId]
FROM Attendance A 
INNER JOIN ActivityMinistry AM ON AM.Activity_ID = A.Activity_ID
INNER JOIN RLC R ON R.RLC_ID = A.RLC_ID
LEFT OUTER JOIN ActivityAssignment AA ON AA.Activity_ID = AM.Activity_ID
	AND AA.Ministry_ID = AM.Ministry_ID
	AND AA.RLC_ID = R.RLC_ID
	AND A.Individual_ID = AA.Individual_ID
ORDER BY Start_Date_Time