--йнк-бн тсмйжхи: 18
--DROP FUNCTION GetTransById;
--DROP FUNCTION GetTransByNumber;
--DROP FUNCTION SearchClientByLastName;
--DROP FUNCTION CalculateTotalBalance;
--DROP FUNCTION SearchByLastName;
--DROP FUNCTION GetClientsByGender;
--DROP FUNCTION GetRecentTransactions;
--DROP FUNCTION GetClientsWithCreditDebt;
--DROP FUNCTION GetLastTransactionForEachCard;
--DROP FUNCTION HasActiveCredit;
--DROP FUNCTION SearchClients;
--DROP FUNCTION GetTransactionTotalAmount;
--DROP FUNCTION GetClientCount;
--DROP FUNCTION GetBankCardCount;
--DROP FUNCTION GetTransactionCount;
--DROP FUNCTION GetCreditCount;
--DROP FUNCTION GetActiveServiceCount;
--DROP FUNCTION GetClientsWithoutActiveBankCards;

-----------------------онхяй рпюмгюйжхи он юидх(хяо)-------------------------
CREATE FUNCTION GetTransById(
	@bankCardId nvarchar(16)
	)
RETURNS TABLE
AS
RETURN
	(
	SELECT
		t.id_transaction,
		t.transaction_type,
		t.transaction_destination,
		t.transaction_date,
		t.transaction_number,
		t.transaction_value
	FROM
		transactions t
	WHERE
		t.id_bank_card = @bankCardId
)
------------------------пюанрняонянамнярэ----------------------
SELECT * FROM dbo.GetTransById('11');

---------------онхяй рпюмгюйжхи он мнлепс йюпрш(хяо)---------------
CREATE FUNCTION GetTransByNumber
(
	@bankCardNumber nvarchar(16)
)
RETURNS TABLE
AS
RETURN
(
	SELECT
		t.id_transaction,
		t.transaction_type,
		t.transaction_destination,
		t.transaction_date,
		t.transaction_number,
		t.transaction_value
	FROM
		transactions t
	WHERE
		t.id_bank_card = (SELECT id_bank_card FROM bank_card WHERE bank_card_number = @bankCardNumber)
);
------------------------пюанрняонянамнярэ----------------------
SELECT * FROM dbo.GetTransByNumber('4698256234918670');

----------------------онхяйю он хлемх(хяо)-------------------------
CREATE FUNCTION dbo.SearchClientByLastName
(
	@lastName NVARCHAR(50)
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		   c.client_first_name,
		   c.client_last_name,
		   c.client_middle_name,
		   bc.id_bank_card,
		   bc.bank_card_type, 
		   bc.bank_card_number, 
		   bc.bank_card_cvv_code, 
		   bc.bank_card_balance, 
		   bc.bank_card_currency, 
		   bc.bank_card_payment, 
		   bc.bank_card_date, 
		   bc.bank_card_pin,
		   bc.id_client
	FROM client AS c
	INNER JOIN bank_card AS bc ON c.id_client = bc.id_client
	WHERE c.client_last_name = @lastName
);
------------------------пюанрняонянамнярэ----------------------
SELECT * FROM dbo.SearchClientByLastName('хБЮМНБ');

-----------------япедмхи аюкюмя он бяел йюпрюл(хяо)+йспянп---------------
CREATE FUNCTION dbo.CalculateTotalBalance
(
	@clientId INT
)
RETURNS DECIMAL(18, 2)
AS
BEGIN
	DECLARE @totalBalance DECIMAL(18, 2)
	DECLARE @balance DECIMAL(18, 2)
	SET @totalBalance = 0
	DECLARE @cur CURSOR
	SET @cur = CURSOR FOR
	SELECT bank_card_balance
	FROM bank_card
	WHERE id_client = @clientId
	OPEN @cur
	FETCH NEXT FROM @cur INTO @balance
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @totalBalance = @totalBalance + @balance
		FETCH NEXT FROM @cur INTO @balance
	END
	CLOSE @cur
	DEALLOCATE @cur
	RETURN @totalBalance
END
------------------------пюанрняонянамнярэ----------------------
SELECT dbo.CalculateTotalBalance(1) as [нАЫХИ АЮКЮМЯ МЮ БЯЕУ ЙЮПРЮУ]

-----дкъ онхяйю он хлемх х ецн вюярэ(ме хяо) + хмдейя------ 
CREATE FUNCTION dbo.SearchByLastName
(
    @LastNameSearch VARCHAR(50)
)
RETURNS TABLE
AS
RETURN
(
    SELECT * 
    FROM client
    WHERE client_last_name LIKE '%' + @LastNameSearch + '%'
)
------------------------пюанрняонянамнярэ----------------------
SELECT * FROM dbo.SearchByLastName('хБЮ');

-------------------онхяй он цемдепс(х)----------------
CREATE FUNCTION dbo.GetClientsByGender
(
    @Gender NVARCHAR(50)
)
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM client
    WHERE client_gender = @Gender
)
------------------------пюанрняонянамнярэ----------------------
SELECT * FROM dbo.GetClientsByGender('ЛСФЯЙНИ');

----онксвемхе онякедмху N рпюмгюйжхи дкъ йюпрш(хяо):--------------
CREATE FUNCTION dbo.GetRecentTransactions
(
    @BankCardID INT,
    @NumTransactions INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT TOP (@NumTransactions) *
    FROM transactions
    WHERE id_bank_card = @BankCardID
    ORDER BY transaction_date DESC
)
------------------------пюанрняонянамнярэ----------------------
SELECT * FROM dbo.GetRecentTransactions(10, 5);

---------йкхемрш й йнрнпшл опхдср йнккейрнпш(хяо):----------------
CREATE FUNCTION dbo.GetClientsWithCreditDebt()
RETURNS TABLE
AS
RETURN
(
    SELECT c.*
    FROM client c
    INNER JOIN bank_card b ON c.id_client = b.id_client
    INNER JOIN credits cr ON b.id_bank_card = cr.id_bank_card
    WHERE cr.credit_status = 1 AND cr.repayment_sum > 0
)
------------------------пюанрняонянамнярэ----------------------
SELECT * FROM dbo.GetClientsWithCreditDebt();

---------онякедмъъ рпюмгюйжхъ дкъ йюфдни йюпрш(хяо):----------------
CREATE FUNCTION dbo.GetLastTransactionForEachCard (@ClientID int)
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM
    (
        SELECT t.*, ROW_NUMBER() OVER (PARTITION BY t.id_bank_card ORDER BY t.transaction_date DESC) AS RowNum
        FROM transactions t
        WHERE t.id_bank_card IN (
            SELECT b.id_bank_card
            FROM bank_card b
            WHERE b.id_client = @ClientID
        )
    ) AS Subquery
    WHERE RowNum = 1
)
------------------------пюанрняонянамнярэ----------------------
SELECT * FROM dbo.GetLastTransactionForEachCard(1);

-------------тсмйжхъ дкъ опнбепйх юйрхбмнцн йпедхрю(хяо)-----------
CREATE FUNCTION dbo.HasActiveCredit(@clientId INT)
RETURNS BIT
AS
BEGIN
    DECLARE @hasActiveCredit BIT;
    IF EXISTS (
        SELECT 1
        FROM credits
        WHERE id_bank_card IN (
            SELECT id_bank_card
            FROM bank_card
            WHERE id_client = @clientId
        ) AND credit_status = 1
    )
    BEGIN
        SET @hasActiveCredit = 1;
    END
    ELSE
    BEGIN
        SET @hasActiveCredit = 0;
    END
    RETURN @hasActiveCredit;
END
------------------------пюанрняонянамнярэ----------------------
SELECT dbo.HasActiveCredit(1) AS HasActiveCredit;

---тсмйжхъ онхяйю он меяйнкэйхл оюпюлерпюл(хлъ,тюлхкхъ,йюпрю)(хяо)----
CREATE FUNCTION SearchClients
(
    @searchString NVARCHAR(100)
)
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM client
    WHERE client_first_name LIKE '%' + @searchString + '%'
        OR client_last_name LIKE '%' + @searchString + '%'
        OR EXISTS (
            SELECT 1
            FROM bank_card
            WHERE bank_card.id_client = client.id_client
              AND (
                  bank_card.bank_card_number LIKE '%' + @searchString + '%'
                  OR CAST(client.id_client AS NVARCHAR(50)) = @searchString
              )
        )
);
SELECT * FROM dbo.SearchClients('хБЮ');
SELECT * FROM dbo.SearchClients(46);

---тсмйжхъ дкъ онксвемхъ наыеи ясллш рпюмгюйжхи дкъ нопедекеммни аюмйнбяйни йюпрш(хяо)----
CREATE FUNCTION dbo.GetTransactionTotalAmount(@id_bank_card INT)
RETURNS MONEY
AS
BEGIN
    DECLARE @TotalAmount MONEY;
    SELECT @TotalAmount = SUM(transaction_value) FROM transactions WHERE id_bank_card = @id_bank_card;
    RETURN @TotalAmount;
END;
------------------------пюанрняонянамнярэ----------------------
SELECT dbo.GetTransactionTotalAmount(11);

---------------тсмйжхъ ондяверю наыецн йнкхвеярбю йкхемрнб-------------
CREATE FUNCTION dbo.GetClientCount()
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM client;
    RETURN @Count;
END;
------------------------пюанрняонянамнярэ----------------------
SELECT dbo.GetClientCount();

---тсмйжхъ дкъ ондяверю наыецн йнкхвеярбю аюмйнбяйху йюпр----
CREATE FUNCTION dbo.GetBankCardCount()
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM bank_card;
    RETURN @Count;
END;
------------------------пюанрняонянамнярэ----------------------
SELECT dbo.GetBankCardCount();

---тсмйжхъ дкъ ондяверю наыецн йнкхвеярбю рпюмгюйжхи----
CREATE FUNCTION dbo.GetTransactionCount()
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM transactions;
    RETURN @Count;
END;
------------------------пюанрняонянамнярэ----------------------
SELECT dbo.GetTransactionCount();

---тсмйжхъ дкъ ондяверю наыецн йнкхвеярбю йпедхрнб----
CREATE FUNCTION dbo.GetCreditCount()
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM credits;
    RETURN @Count;
END;
------------------------пюанрняонянамнярэ----------------------
SELECT dbo.GetCreditCount();

---тсмйжхъ дкъ ондяверю наыецн йнкхвеярбю юйрхбмшу йкхемряйху сяксц----
CREATE FUNCTION dbo.GetActiveServiceCount()
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM clientServices WHERE serviceBalance > 0;
    RETURN @Count;
END;
------------------------пюанрняонянамнярэ----------------------
SELECT dbo.GetActiveServiceCount();

---тсмйжхъ дкъ онксвемхъ яохяйю йкхемрнб, с йнрнпшу мер юйрхбмшу аюмйнбяйху йюпр----
CREATE FUNCTION dbo.GetClientsWithoutActiveBankCards()
RETURNS TABLE
AS
RETURN
SELECT * FROM client WHERE id_client NOT IN (SELECT DISTINCT id_client FROM bank_card);
-------------------------------------------------------------
SELECT * from GetClientsWithoutActiveBankCards();