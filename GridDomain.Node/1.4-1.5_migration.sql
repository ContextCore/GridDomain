-- helper function to convert between DATETIME2 and BIGINT as .NET ticks
-- taken from: http://stackoverflow.com/questions/7386634/convert-sql-server-datetime-object-to-bigint-net-ticks
GO
CREATE FUNCTION [dbo].[Ticks] (@dt DATETIME)
RETURNS BIGINT
WITH SCHEMABINDING
AS
BEGIN 
DECLARE @year INT = DATEPART(yyyy, @dt)
DECLARE @month INT = DATEPART(mm, @dt)
DECLARE @day INT = DATEPART(dd, @dt)
DECLARE @hour INT = DATEPART(hh, @dt)
DECLARE @min INT = DATEPART(mi, @dt)
DECLARE @sec INT = DATEPART(ss, @dt)

DECLARE @days INT =
    CASE @month - 1
        WHEN 0 THEN 0
        WHEN 1 THEN 31
        WHEN 2 THEN 59
        WHEN 3 THEN 90
        WHEN 4 THEN 120
        WHEN 5 THEN 151
        WHEN 6 THEN 181
        WHEN 7 THEN 212
        WHEN 8 THEN 243
        WHEN 9 THEN 273
        WHEN 10 THEN 304
        WHEN 11 THEN 334
        WHEN 12 THEN 365
    END
    IF  @year % 4 = 0 AND (@year % 100  != 0 OR (@year % 100 = 0 AND @year % 400 = 0)) AND @month > 2 BEGIN
        SET @days = @days + 1
    END
RETURN CONVERT(bigint, 
    ((((((((@year - 1) * 365) + ((@year - 1) / 4)) - ((@year - 1) / 100)) + ((@year - 1) / 400)) + @days) + @day) - 1) * 864000000000) +
    ((((@hour * 3600) + CONVERT(bigint, @min) * 60) + CONVERT(bigint, @sec)) * 10000000) + (CONVERT(bigint, DATEPART(ms, @dt)) * CONVERT(bigint,10000));

END;

-- helper function to convert between BIGINT as .NET ticks to DateTime2 for diagnostics
-- taken from: http://stackoverflow.com/questions/2313236/convert-net-ticks-to-sql-server-datetime
GO
CREATE FUNCTION NetFxUtcTicksToDateTime
(
   @Ticks bigint
)
RETURNS datetime
AS
BEGIN

-- First, we will convert the ticks into a datetime value with UTC time
DECLARE @BaseDate datetime;
SET @BaseDate = '01/01/1900';

DECLARE @NetFxTicksFromBaseDate bigint;
SET @NetFxTicksFromBaseDate = @Ticks - 599266080000000000;
-- The numeric constant is the number of .Net Ticks between the System.DateTime.MinValue (01/01/0001) and the SQL Server datetime base date (01/01/1900)

DECLARE @DaysFromBaseDate int;
SET @DaysFromBaseDate = @NetFxTicksFromBaseDate / 864000000000;
-- The numeric constant is the number of .Net Ticks in a single day.

DECLARE @TimeOfDayInTicks bigint;
SET @TimeOfDayInTicks = @NetFxTicksFromBaseDate - @DaysFromBaseDate * 864000000000;

DECLARE @TimeOfDayInMilliseconds int;
SET @TimeOfDayInMilliseconds = @TimeOfDayInTicks / 10000;
-- A Tick equals to 100 nanoseconds which is 0.0001 milliseconds

DECLARE @UtcDate datetime;
SET @UtcDate = DATEADD(ms, @TimeOfDayInMilliseconds, DATEADD(d, @DaysFromBaseDate, @BaseDate));
-- The @UtcDate is already useful. If you need the time in UTC, just return this value.

-- Now, some magic to get the local time
RETURN @UtcDate + GETDATE() - GETUTCDATE();
END
GO



ALTER TABLE Journal ADD Timestamp_tmp BIGINT NULL;

GO


UPDATE Journal SET Timestamp_tmp = dbo.Ticks(Timestamp);
DROP INDEX [IX_Journal_Timestamp] ON [dbo].[Journal]
ALTER TABLE Journal DROP COLUMN Timestamp;
ALTER TABLE Journal ALTER COLUMN Timestamp_tmp BIGINT NOT NULL;
EXEC sp_RENAME 'Journal.Timestamp_tmp' , 'Timestamp', 'COLUMN';
GO
CREATE NONCLUSTERED INDEX [IX_Journal_Timestamp] ON [dbo].[Journal]
(
	[Timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE Journal ADD Tags NVARCHAR(100) NULL;