
--SELECT 
--	 CONCAT(DATEPART(dw, Start_Date_Time), DATEPART(HOUR, Start_Date_Time),DATEPART(MINUTE, Start_Date_Time)) AS [Id]
--	,REPLACE(REPLACE(DATENAME(dw,Start_Date_Time) + ' ' + CONVERT(VARCHAR,CAST(Start_Date_Time AS time), 100),'PM','pm'),'AM','am') AS [Name]
--FROM Attendance
--GROUP BY CONCAT(DATEPART(dw, Start_Date_Time), DATEPART(HOUR, Start_Date_Time),DATEPART(MINUTE, Start_Date_Time)), REPLACE(REPLACE(DATENAME(dw,Start_Date_Time) + ' ' + CONVERT(VARCHAR,CAST(Start_Date_Time AS time), 100),'PM','pm'),'AM','am')

SELECT
	AA.ActivityScheduleID AS [Id]
	,AAS.Activity_Time_Name AS [Name]
FROM Attendance A 
INNER JOIN ActivityMinistry AM ON AM.Activity_ID = A.Activity_ID
INNER JOIN RLC R ON R.RLC_ID = A.RLC_ID
INNER JOIN ActivityAssignment AA ON AA.Activity_ID = AM.Activity_ID
	AND AA.Ministry_ID = AM.Ministry_ID
	AND AA.RLC_ID = R.RLC_ID
	AND A.Individual_ID = AA.Individual_ID
INNER JOIN  Activity_Schedule AAS ON AAS.Activity_Schedule_ID = AA.ActivityScheduleID
GROUP BY AA.ActivityScheduleID,AAS.Activity_Time_Name
ORDER BY AA.ActivityScheduleID