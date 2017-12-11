SELECT MINUTES_TO_WORK_THIS_WEEK, (MINUTES_TO_WORK_THIS_WEEK/60) AS HOURS_TO_WORK_THIS_WEEK
FROM (
select 
case
	WHEN
		DATEDIFF(
		   mi,
		   GETDATE(),
		   dateadd(hour,17,cast(CAST(getdate() as DATE) as datetime)))<=0
	THEN 0
	ELSE
		DATEDIFF(
		   mi,
		   GETDATE(),
		   dateadd(hour,17,cast(CAST(getdate() as DATE) as datetime))) 
	END
	+
	CASE
		WHEN 6-datepart(WEEKDAY,GETDATE())<0
		THEN 0
		ELSE 6-datepart(WEEKDAY,GETDATE())
	END*480
	as MINUTES_TO_WORK_THIS_WEEK
) bla