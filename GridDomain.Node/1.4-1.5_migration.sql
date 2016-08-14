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