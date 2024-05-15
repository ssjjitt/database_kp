--E:\ÛÌËÍ\MobileBankDatabase
------------------› —œŒ–“ ¬ JSON----------------------------
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1;
RECONFIGURE;
---
SELECT * FROM client FOR JSON PATH, INCLUDE_NULL_VALUES
---œ–Œ—“Œ «¿œ–Œ—» 
DECLARE @sql varchar(1000)
SET @sql = 'bcp "SELECT * FROM client ' + 
    'FOR JSON PATH, INCLUDE_NULL_VALUES" ' +
    'queryout  "e:\MobileBankDatabase\client.json" ' + 
    '-c -S DESKTOP-F644G2I -d MobileBank -T'
EXEC sys.XP_CMDSHELL @sql
GO
-----------› —œŒ–“ œŒ œ¿–¿Ã≈“–”--------------
CREATE OR ALTER PROCEDURE SaveTableDataToJson
    @tableName NVARCHAR(128)
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
END;
------------–¿¡Œ“Œ—œŒ—Œ¡ÕŒ—“‹-------------------
EXEC SaveTableDataToJson @tableName = 'client';
-----------»ÃœŒ–“ »« CSV--------------
CREATE TABLE #csv_temp (
    id_service NVARCHAR(100),
	serviceName NVARCHAR(100),
	serviceBalance NVARCHAR(100),
	serviceType NVARCHAR(100)
);
--
bulk insert #csv_temp
from 'e:\ÛÌËÍ\MobileBankDatabase\importData.csv'
with (Fieldterminator = ',', RowTerminator = '\n', CODEPAGE = '1251')
--  
select * from #csv_temp;
------------–¿¡Œ“Œ—œŒ—Œ¡ÕŒ—“‹-------------------
INSERT INTO clientServices
SELECT CONVERT(nvarchar(100), serviceName), CONVERT(money, serviceBalance), CONVERT(nvarchar(100), serviceType)
FROM #csv_temp;