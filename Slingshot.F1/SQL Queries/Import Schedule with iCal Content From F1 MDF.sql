DECLARE @CategoryId as Int
DECLARE @ForeignKey as nvarchar(25)
DECLARE @Now as DateTime

SET @ForeignKey = 'F1'
SET @CategoryId = (Select Id From Category Where [Guid] = '153B61A4-2FAA-4364-B75A-AC2D8961D8B7') -- Event Scehdules Category
SET @Now = GetDate()

CREATE Table  _BEMA_Seervices_F1_SCHEDULE_IMPORT_TMP( Id INT, [Day] nvarchar(2)) 
INSERT INTO _BEMA_Seervices_F1_SCHEDULE_IMPORT_TMP (Id, [Day])
SELECT DISTINCT u.ACTIVITY_Schedule_ID
	, CASE u.[Day]
		WHEN 'WEEKLY_MONDAY' THEN 'MO'
		WHEN 'WEEKLY_TUESDAY' THEN 'TU'
		WHEN 'WEEKLY_WEDNESDAY' THEN 'WE'
		WHEN 'WEEKLY_THURSDAY' THEN 'TH'
		WHEN 'WEEKLY_FRIDAY' THEN 'FR'
		WHEN 'WEEKLY_SATURDAY' THEN 'SA'
		WHEN 'WEEKLY_SUNDAY' THEN 'SU'
	END
FROM FAITHBRIDGE.DBO.Activity_Schedule_Recurrences r
unpivot
	( 
		[Value]
		for [day] IN(WEEKLY_MONDAY, WEEKLY_TUESDAY, WEEKLY_WEDNESDAY, WEEKLY_THURSDAY, WEEKLY_FRIDAY, WEEKLY_SATURDAY, WEEKLY_SUNDAY)
	) u
	WHERE u.[Value] > 0
	and u.ACTIVITY_Schedule_ID is not null
;

Insert Into Schedule(
		[Name]
		, iCalendarContent
		, EffectiveStartDate
		, EffectiveEndDate
		, CategoryId
		, [Guid]
		, CreatedDateTime
		, ModifiedDateTime
		, ForeignKey
		, ForeignId
		, IsActive
	)
SELECT DISTINCT TOP 10
	Activity_Time_Name as [Name]
	,'BEGIN:VCALENDAR' + CHAR(13) + CHAR(10)
	+ 'VERSION:2.0' + CHAR(13) + CHAR(10)
	+ 'PRODID:-//ddaysoftware.com//NONSGML DDay.iCal 1.0//EN' + CHAR(13) + CHAR(10)
	+ 'BEGIN:VEVENT' + CHAR(13) + CHAR(10) 
	+ 'DTEND:' + FORMAT(Activity_End_Time, 'yyyyMMddTHHmmss')+ CHAR(13) + CHAR(10)
	+ 'DTSTAMP:' + FORMAT(@Now, 'yyyyMMddTHHmmssZ')+ CHAR(13) + CHAR(10)
	+ 'DTSTART:' + FORMAT(Activity_Start_Time, 'yyyyMMddTHHmmss')+ CHAR(13) + CHAR(10)
	+ IIF(ccb_r.ACTIVITY_RECURRENCE_ID is null,'','RRULE:' + CHAR(13) + CHAR(10))
	+ CASE RECURRENCE_PATTERN
	WHEN 1 THEN
		'RRULE:FREQ=DAILY'
		+ IIF(DAILY_EVERY_DAYS > 1,
				';INTERVAL=' 
				+ Cast(DAILY_EVERY_DAYS as varchar(2))
			,'')
		+ IIF(NO_END_DATE=0,
			IIF(END_AFTER_OCCURENCES > 0,
				';COUNT='
				+ CAST(END_AFTER_OCCURENCES as varchar(4))
			,';UNTIL='
			+ FORMAT(CAST(END_DATE as datetime),'yyyyMMddTHHmmssZ'))
		,'')
	WHEN 2 THEN
		'RRULE:FREQ=WEEKLY'
		+ IIF(WEEKLY_EVERY_WEEKS > 1,
				';INTERVAL=' 
				+ Cast(WEEKLY_EVERY_WEEKS as varchar(2))
			,'')
		+ IIF(NO_END_DATE=0,
			IIF(END_AFTER_OCCURENCES > 0,
				';COUNT='
				+ CAST(END_AFTER_OCCURENCES as varchar(4))
			,';UNTIL='
			+ FORMAT(CAST(END_DATE as datetime),'yyyyMMddTHHmmssZ'))
		,'')
		+ ';BYDAY='
		+ tmp.[Day]
	WHEN 3 THEN
		'RRULE:FREQ=MONTHLY' 
		+ IIF(MONTHLY_EVERY_MONTH > 1,
			';INTERVAL=' 
			+ Cast(MONTHLY_EVERY_MONTH as varchar(2)) 
		,'')
		+ IIF(NO_END_DATE=0,
			IIF(END_AFTER_OCCURENCES > 0,
				';COUNT='
				+ CAST(END_AFTER_OCCURENCES as varchar(4))
			,';UNTIL='
			+ FORMAT(CAST(END_DATE as datetime),'yyyyMMddTHHmmssZ'))
		,'')
		+ IIF(MONTHLY_DAY_MONTH > 0,
			';BYMONTHDAY=' + CAST(MONTHLY_DAY_MONTH as varchar(2))
			, IIF(MONTHLY_WEEK > 0,
				';BYDAY=' 
				+ CAST(MONTHLY_WEEK as varchar(2)) 
				+ CASE MONTHLY_DAY_OF_WEEK
					WHEN 1 THEN 'MO'
					WHEN 2 THEN 'TU'
					WHEN 3 THEN 'WE'
					WHEN 4 THEN 'TH'
					WHEN 5 THEN 'FR'
					WHEN 6 THEN 'SA'
					WHEN 7 THEN 'SU'
					END
			,'')
		)

	WHEN 4 THEN
		'RRULE:FREQ=YEARLY'
		+ IIF(NO_END_DATE=0,
			IIF(END_AFTER_OCCURENCES > 0,
				';COUNT='
				+ CAST(END_AFTER_OCCURENCES as varchar(4))
			,';UNTIL='
			+ FORMAT(CAST(END_DATE as datetime),'yyyyMMddTHHmmssZ')
			)
		,'')
		+ ';BYMONTH='
		+ CAST(YEARLY_EVERY_MONTH as varchar(2))
		+ IIF(YEARLY_DAY_MONTH > 0,
			';BYMONTHDAY='
			+ CAST(YEARLY_DAY_MONTH as varchar(2))
		,
			';BYDAY='
			+ CAST(YEARLY_WEEK as varchar(2))
			+ CASE YEARLY_DAY_OF_WEEK
				WHEN 1 THEN 'MO'
				WHEN 2 THEN 'TU'
				WHEN 3 THEN 'WE'
				WHEN 4 THEN 'TH'
				WHEN 5 THEN 'FR'
				WHEN 6 THEN 'SA'
				WHEN 7 THEN 'SU'
			END
		)
	END
    + CHAR(13) + CHAR(10)
	+ 'SEQUENCE:0' + CHAR(13) + CHAR(10)
	+ 'UID:' + LOWER(CAST(NEWID() as nvarchar(36))) + CHAR(13) + CHAR(10)
	+ 'END:VEVENT' + CHAR(13) + CHAR(10)
	+ 'END:VCALENDAR' as iCalendarContent
	, Activity_Start_Time as EffectiveStateDate
	, Activity_End_Time as EffectiveEndDate
	, @CategoryId as CategoryId
	, NEWID() as [Guid]
	, @Now as CreatedDateTime
	, @Now as ModifiedDateTime
	, @ForeignKey as ForeignKey
	, ccb_S.Activity_Schedule_ID as ForeignId
	, 1 as IsActive
FROM FAITHBRIDGE.dbo.Activity_Schedule CCB_S
LEFT Join FAITHBRIDGE.dbo.Activity_Schedule_Recurrences CCB_R on CCB_R.ACTIVITY_Schedule_ID = CCB_s.Activity_Schedule_ID
LEFT JOIN _BEMA_Seervices_F1_SCHEDULE_IMPORT_TMP Tmp on Tmp.Id = CCB_R.ACTIVITY_Schedule_ID
LEFT Join Schedule s on ForeignKey = @ForeignKey and ForeignId = ccb_S.Activity_Schedule_ID
Where s.Id is null

DROP Table _BEMA_Seervices_F1_SCHEDULE_IMPORT_TMP

/*
Delete From Schedule Where ForeignKey = 'F1'
*/

