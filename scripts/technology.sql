-------------------“≈’ЌќЋќ√»я--------------------------
CREATE OR ALTER PROCEDURE SaveTableDataToJsonAndSend
    @tableName NVARCHAR(128),
    @recipients NVARCHAR(100),
    @subject NVARCHAR(255),
    @body NVARCHAR(MAX)
AS
BEGIN
    DECLARE @fileName NVARCHAR(128);
    SET @fileName = REPLACE(@tableName, ' ', '') + '.json';

    DECLARE @sql NVARCHAR(1000);
    SET @sql = 'bcp "SELECT * FROM ' + QUOTENAME(@tableName) + ' ' +
               'FOR JSON PATH, INCLUDE_NULL_VALUES" ' +
               'queryout "e:\' + @fileName + '" ' +
               '-c -S DESKTOP-F644G2I -d MobileBank -T';
    EXEC sys.XP_CMDSHELL @sql;

    DECLARE @attachment NVARCHAR(255);
    SET @attachment = 'e:\' + @fileName;

    -- ќтправка письма с вложением
    EXEC msdb.dbo.sp_send_dbmail
        @profile_name = 'database profile',
        @recipients = @recipients,
        @subject = @subject,
        @body = @body,
        @file_attachments = @attachment;
END;
----------------------------------------------------------------------------
EXEC SaveTableDataToJsonAndSend
    @tableName = 'client',
    @recipients = 'ssjjitt@mail.ru',
    @subject = 'JSON-файл с данными таблицы client',
    @body = '¬о вложении находитс€ JSON-файл с данными таблицы client.';