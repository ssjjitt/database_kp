use MobileBank
--------------гюонкмемхе рюакхж---------------------
CREATE OR ALTER PROCEDURE PopulateClients
AS
BEGIN
  DECLARE @i INT = 6001
  WHILE @i <= 8000
  BEGIN
    INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number, is_admin)
    VALUES ('Last Name ' + CAST(@i AS NVARCHAR(50)), 'First Name ' + CAST(@i AS NVARCHAR(50)), 'Middle Name ' + CAST(@i AS NVARCHAR(50)), 'Gender ' + CAST(@i AS NVARCHAR(50)), 'Password ' + CAST(@i AS NVARCHAR(50)), 'Email ' + CAST(@i AS NVARCHAR(50)), CAST(@i AS NCHAR(13)), CAST(@i % 2 AS BIT))

    SET @i = @i + 1
  END
END
----------------------------------------------------
EXEC PopulateClients;
--------------------------реярхпнбюмхе---------------------------
SET SHOWPLAN_ALL ON;
GO
SELECT client_last_name
FROM client
WHERE client_last_name = 'Last Name 6001';
GO
SET SHOWPLAN_ALL OFF;
GO
--------
SET STATISTICS TIME ON;
SELECT client_last_name
FROM client
WHERE client_last_name = 'Last Name 6001';
SET STATISTICS TIME OFF;
-----------
SELECT count(*) FROM CLIENT;