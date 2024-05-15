---------------ЗАПРЕТ НА ВСТАВКУ С ADMIN---------------------
CREATE TRIGGER i_tgLastName ON dbo.client
FOR INSERT 
AS
DECLARE @Name varchar(50)
SELECT @Name=client_last_name
FROM inserted
IF @Name='admin'
BEGIN 
PRINT 'ОШИБКА ПРИ ПОПЫТКЕ ВСТАВИТЬ ADMIN. ВЫ НЕ ИМЕЕТЕ ДОСТАТОЧНЫХ ПРАВ.'
ROLLBACK TRANSACTION
END;
-------------------------------------------------------------
INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number, is_admin)
VALUES('admin', 'A', 'A', 'A', 'A', 'A', 'A', 0);

---------------ЗАПРЕТ НА IS_ADMIN = 1---------------------
CREATE TRIGGER SetClientIsAdminFlag
ON client
AFTER INSERT
AS
BEGIN
    UPDATE client
    SET is_admin = 0
    FROM inserted
    WHERE client.id_client = inserted.id_client;
END;
-------------------------------------------------------------
INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number, is_admin)
VALUES('B', 'A', 'A', 'A', 'A', 'A', 'A', 1);

----БУДЕТ АВТОМАТИЧЕСКИ ПРОВЕРЯТЬ БАЛАНС БАНКОВСКОЙ КАРТЫ ПЕРЕД ВСТАВКОЙ НОВОЙ ТРАНЗАКЦИИ И ОТКЛОНЯТЬ ТРАНЗАКЦИЮ, ЕСЛИ БАЛАНС НЕДОСТАТОЧЕН-----
CREATE OR ALTER TRIGGER CheckTransactionBalance
ON transactions
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @ins TABLE (
        id_transaction int,
        transaction_type varchar(50),
        transaction_destination varchar(200),
        transaction_date date,
        transaction_number nchar(50),
        transaction_value money,
        id_bank_card int
    );
    INSERT INTO @ins
    SELECT id_transaction, transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card
    FROM inserted;
    IF EXISTS (
        SELECT 1
        FROM @ins i
        INNER JOIN bank_card bc ON i.id_bank_card = bc.id_bank_card
        WHERE i.transaction_value > bc.bank_card_balance
    )
    BEGIN
        RAISERROR('Недостаточно средств для выполнения операции.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;
    INSERT INTO transactions (id_transaction, transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card)
    SELECT id_transaction, transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card
    FROM @ins;
END;
-------------------------------------------------------------
INSERT INTO transactions (transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card) 
VALUES ('Payment', 'Recipient', GETDATE(), '1234567890', 100000, 12);
select * from transactions
select * from bank_card;
----------ТРИГГЕР ДЛЯ ПРОВЕРКИ СУММЫ КРЕДИТА ПЕРЕД ВСТАВКОЙ----------
CREATE OR ALTER TRIGGER CheckCreditSum
ON credits
FOR INSERT
AS
BEGIN
    DECLARE @credit_sum money;
    SELECT @credit_sum = credit_sum FROM inserted;
    IF @credit_sum <= 0
    BEGIN
        RAISERROR('Недостаточно средств для выполнения операции.', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
-------------------------------------------------

----------ТРИГГЕР ДЛЯ ПРОВЕРКИ ТИПА БАНКОВСКОЙ КАРТЫ ПЕРЕД ВСТАВКОЙ----------
CREATE OR ALTER TRIGGER CheckBankCardType
ON bank_card
FOR INSERT
AS
BEGIN
    DECLARE @bank_card_type nvarchar(50);
    SELECT @bank_card_type = bank_card_type FROM inserted;
    
    IF @bank_card_type NOT IN ('VISA', 'MasterCard')
    BEGIN
        RAISERROR('Тип карты не соответствует нормам.', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
------------------------------------------------------------
INSERT INTO bank_card (bank_card_type, bank_card_number, bank_card_cvv_code, bank_card_currency, bank_card_payment, bank_card_date, bank_card_pin, paymentSystem)
VALUES ('неVisa', '1111222233334444', '123', 'USD', 'Payment System', '2024-05-09', 1234, 'Visa');
--------------------ОБЯЗАТЕЛЬНЫЕ ПОЛЯ--------------------
CREATE OR ALTER TRIGGER CheckClientFields
ON client
FOR INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE client_last_name IS NULL
            OR client_first_name IS NULL
            OR client_middle_name IS NULL
            OR client_gender IS NULL
            OR client_password IS NULL
            OR client_email IS NULL
            OR client_phone_number IS NULL
    )
    BEGIN
        RAISERROR('Заполните обязательные поля.', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
---------------------------------------------------------------------------
select * from client;
INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number)
VALUES ('ало', 'ало', 'ало', 'ало', 'ало', 'ало@example.com', '155532131234');
---------------УНИКАЛЬНОСТЬ КАРТЫ---------------
CREATE OR ALTER TRIGGER CheckBankCardNumber
ON bank_card
FOR INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE bank_card_number IN (SELECT bank_card_number FROM bank_card)
    )
    BEGIN
        RAISERROR('Такая карта уже есть.', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
---------------------------------------------------------------------------
INSERT INTO bank_card (bank_card_type, bank_card_number, bank_card_cvv_code, bank_card_currency, bank_card_payment, bank_card_date, bank_card_pin, paymentSystem)
VALUES ('Visa', '1111222233334444', '123', 'USD', 'Payment System', '2024-05-09', 1234, 'Visa');
INSERT INTO bank_card (bank_card_type, bank_card_number, bank_card_cvv_code, bank_card_currency, bank_card_payment, bank_card_date, bank_card_pin, paymentSystem)
VALUES ('Visa', '1111222233334444', '123', 'USD', 'Payment System', '2024-05-09', 1234, 'Visa');
---------------УНИКАЛЬНОСТЬ ТЕЛЕФОНА---------------
CREATE OR ALTER TRIGGER CheckUniquePhoneNumber
ON client
FOR INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM client
        WHERE client_phone_number IN (SELECT client_phone_number FROM client)
        GROUP BY client_phone_number
        HAVING COUNT(*) > 1
    )
    BEGIN
        RAISERROR('Phone number must be unique.', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
------------------------------------------------------------
INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number)
VALUES ('курсачевский', 'курсач', 'курсанович', 'кп', 'mypassword123', 'кп@example.com', '1234567890');
INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number)
VALUES ('курсачевский2', 'курсач2', 'курсанович2', 'кп2', 'mypassword1232', 'кп2@example.com', '1234567890');
