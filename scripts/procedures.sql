use MobileBank;
--КОЛ-ВО ПРОЦЕДУР: 23;
SELECT 
    name AS [Procedure Name],
    create_date AS [Create Date],
    modify_date AS [Modify Date]
FROM sys.procedures
	WHERE is_ms_shipped = 0
ORDER BY name;
--DROP PROCEDURE CreateBankCard;
--DROP PROCEDURE CreateClient
--DROP PROCEDURE CreateClientPersonalAccount
--DROP PROCEDURE CreateClientService
--DROP PROCEDURE CreateCredit
--DROP PROCEDURE CreateTransaction
--DROP PROCEDURE DeleteBankCard
--DROP PROCEDURE DeleteClient
--DROP PROCEDURE DeleteClientPersonalAccount
--DROP PROCEDURE DeleteClientService
--DROP PROCEDURE DeleteCredit
--DROP PROCEDURE DeleteTransaction
--DROP PROCEDURE PopulateClients
--DROP PROCEDURE SaveBankCardDataToJson
--DROP PROCEDURE SaveClientDataToJson
--DROP PROCEDURE SaveClientPersonalAccountDataToJson
--DROP PROCEDURE SaveTableDataToJson
--DROP PROCEDURE UpdateBankCard
--DROP PROCEDURE UpdateClient
--DROP PROCEDURE UpdateClientPersonalAccount
--DROP PROCEDURE UpdateClientService
--DROP PROCEDURE UpdateCredit
--DROP PROCEDURE UpdateTransaction
-------------------------------------------------------------
-------------ДОБАВЛЕНИЕ КЛИЕНТА--------------------------------
CREATE OR ALTER PROCEDURE dbo.CreateClient
    @client_last_name nvarchar(50),
    @client_first_name nvarchar(50),
    @client_middle_name nvarchar(50),
    @client_gender nvarchar(50),
    @client_password nvarchar(50),
    @client_email nvarchar(50),
    @client_phone_number nchar(13),
    @is_admin bit
AS
BEGIN
    INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number, is_admin)
    VALUES (@client_last_name, @client_first_name, @client_middle_name, @client_gender, @client_password, @client_email, @client_phone_number, @is_admin)
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.CreateClient
    @client_last_name = 'Фамилия',
    @client_first_name = 'Имя',
    @client_middle_name = 'Отчество',
    @client_gender = 'Пол',
    @client_password = 'Пароль',
    @client_email = 'email@example.com',
    @client_phone_number = '1234567890123',
    @is_admin = 0;
select * from client
-------------------------------------------------------------
------------ОБНОВЛЕНИЕ ИНФОРМАЦИИ О КЛИЕНТЕ--------------------
CREATE OR ALTER PROCEDURE dbo.UpdateClient
    @id_client int,
    @client_last_name nvarchar(50),
    @client_first_name nvarchar(50),
    @client_middle_name nvarchar(50),
    @client_gender nvarchar(50),
    @client_password nvarchar(50),
    @client_email nvarchar(50),
    @client_phone_number nchar(13),
    @is_admin bit
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM client WHERE id_client = @id_client)
    BEGIN
        RAISERROR ('Клиента с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        BEGIN TRANSACTION
        UPDATE client
        SET client_last_name = @client_last_name,
            client_first_name = @client_first_name,
            client_middle_name = @client_middle_name,
            client_gender = @client_gender,
            client_password = @client_password,
            client_email = @client_email,
            client_phone_number = @client_phone_number,
            is_admin = @is_admin
        WHERE id_client = @id_client
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.UpdateClient
	@id_client = 2209, 
    @client_last_name = 'Фамилия',
    @client_first_name = 'Имя',
    @client_middle_name = 'Отчество',
    @client_gender = 'Пол',
    @client_password = 'Пароль',
    @client_email = 'email@example.com',
    @client_phone_number = '1234567890123',
    @is_admin = 0;
select * from client
-------------------------------------------------------------
------------УДАЛЕНИЕ ИНФОРМАЦИИ О КЛИЕНТЕ--------------------
CREATE OR ALTER PROCEDURE dbo.DeleteClient
    @id_client int
AS
BEGIN
IF NOT EXISTS (SELECT 1 FROM client WHERE id_client = @id_client)
    BEGIN
        RAISERROR ('Клиента с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        BEGIN TRANSACTION
    DELETE FROM client 
		WHERE id_client = @id_client
		COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.DeleteClient @id_client = 2209;
-------------------------------------------------------------
--------------СОЗДАНИЕ БАНКОВСКОЙ КАРТЫ----------------------
select * from bank_card;
CREATE OR ALTER PROCEDURE dbo.CreateBankCard
    @bank_card_type nvarchar(50),
    @bank_card_number nvarchar(16),
    @bank_card_cvv_code nvarchar(3),
    @bank_card_balance money,
    @bank_card_currency nvarchar(10),
    @bank_card_payment nvarchar(50),
    @bank_card_date date,
    @bank_card_pin int,
    @id_client int,
    @paymentSystem nvarchar(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Client WHERE id_client = @id_client)
    BEGIN
        RAISERROR('Клиента с указанным идентификатором не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        INSERT INTO bank_card (bank_card_type, bank_card_number, bank_card_cvv_code, bank_card_balance, bank_card_currency, bank_card_payment, bank_card_date, bank_card_pin, id_client, paymentSystem)
        VALUES (@bank_card_type, @bank_card_number, @bank_card_cvv_code, @bank_card_balance, @bank_card_currency, @bank_card_payment, @bank_card_date, @bank_card_pin, @id_client, @paymentSystem)
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR('Нарушение ограничения внешнего ключа. Проверьте правильность идентификатора клиента.', 16, 1)
        END
        ELSE
        BEGIN
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
            DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
            DECLARE @ErrorState INT = ERROR_STATE()
            RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
        END
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.CreateBankCard
    @bank_card_type = 'дебетовая',
    @bank_card_number = '1234567891234567',
    @bank_card_cvv_code = '132',
    @bank_card_balance = 1000,
    @bank_card_currency = 'BYN',
    @bank_card_payment = 'VISA',
    @bank_card_date = '2024-05-09',
    @bank_card_pin = 1234,
    @id_client = 2220,
    @paymentSystem = 'VISA'
-------------------------------------------------------------
--------------ОБНОВЛЕНИЕ БАНКОВСКОЙ КАРТЫ----------------------
CREATE OR ALTER PROCEDURE dbo.UpdateBankCard
    @id_bank_card int,
    @bank_card_type nvarchar(50),
    @bank_card_number nvarchar(16),
    @bank_card_cvv_code nvarchar(3),
    @bank_card_balance money,
    @bank_card_currency nvarchar(10),
    @bank_card_payment nvarchar(50),
    @bank_card_date date,
    @bank_card_pin int,
    @id_client int,
    @paymentSystem nvarchar(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM bank_card WHERE id_bank_card = @id_bank_card)
    BEGIN
        RAISERROR('Карты с указанным идентификатором не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        UPDATE bank_card
        SET bank_card_type = @bank_card_type,
            bank_card_number = @bank_card_number,
            bank_card_cvv_code = @bank_card_cvv_code,
            bank_card_balance = @bank_card_balance,
            bank_card_currency = @bank_card_currency,
            bank_card_payment = @bank_card_payment,
            bank_card_date = @bank_card_date,
            bank_card_pin = @bank_card_pin,
            id_client = @id_client,
            paymentSystem = @paymentSystem
        WHERE id_bank_card = @id_bank_card
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.UpdateBankCard
    @id_bank_card = 22,
    @bank_card_type = 'кредитная',
    @bank_card_number = '8765432193547809',
    @bank_card_cvv_code = '123',
    @bank_card_balance = 1000,
    @bank_card_currency = 'BYN',
    @bank_card_payment = 'MasterCard',
    @bank_card_date = '2024-05-09',
    @bank_card_pin = 1234,
    @id_client = 2220,
    @paymentSystem = 'MasterCard'
-------------------------------------------------------------
--------------УДАЛЕНИЕ БАНКОВСКОЙ КАРТЫ----------------------
CREATE OR ALTER PROCEDURE dbo.DeleteBankCard
    @id_bank_card int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM bank_card WHERE id_bank_card = @id_bank_card)
    BEGIN
        RAISERROR('Карты с указанным идентификатором не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        DELETE FROM bank_card WHERE id_bank_card = @id_bank_card
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.DeleteBankCard @id_bank_card = 22;
-------------------------------------------------------------
------------СОЗДАНИЕ ТРАНЗАКЦИИ------------
CREATE OR ALTER PROCEDURE dbo.CreateTransaction
    @transaction_type varchar(50),
    @transaction_destination varchar(200),
    @transaction_date date,
    @transaction_number nchar(50),
    @transaction_value money,
    @id_bank_card int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM bank_card WHERE id_bank_card = @id_bank_card)
    BEGIN
        RAISERROR('Карты с указанным идентификатором не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        INSERT INTO transactions (transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card)
        VALUES (@transaction_type, @transaction_destination, @transaction_date, @transaction_number, @transaction_value, @id_bank_card)
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.CreateTransaction 
	@transaction_type = 'Payment', 
	@transaction_destination = 'Online Store', 
	@transaction_date = '2024-05-09', 
	@transaction_number = '1234567890', 
	@transaction_value = 100.00, 
	@id_bank_card = 23;
-------------------------------------------------------------
------------ОБНОВЛЕНИЕ ИНФОРМАЦИИ О ТРАНЗАКЦИИ------------
CREATE OR ALTER PROCEDURE dbo.UpdateTransaction
    @id_transaction int,
    @transaction_type varchar(50),
    @transaction_destination varchar(200),
    @transaction_date date,
    @transaction_number nchar(50),
    @transaction_value money,
    @id_bank_card int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM transactions WHERE id_transaction = @id_transaction)
    BEGIN
        RAISERROR ('Транзакция с таким ID не существует.', 16, 1)
        RETURN
    END
	BEGIN TRY
    UPDATE transactions
    SET transaction_type = @transaction_type,
        transaction_destination = @transaction_destination,
        transaction_date = @transaction_date,
        transaction_number = @transaction_number,
        transaction_value = @transaction_value,
        id_bank_card = @id_bank_card
    WHERE id_transaction = @id_transaction
	 END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.UpdateTransaction 
    @id_transaction = 1,
    @transaction_type = 'New Payment',
    @transaction_destination = 'Updated Online Store',
    @transaction_date = '2024-05-09',
    @transaction_number = '0987654321',
    @transaction_value = 200.00,
    @id_bank_card = 45;
-------------------------------------------------------------
------------УДАЛЕНИЕ ТРАНЗАКЦИИ------------
CREATE OR ALTER PROCEDURE dbo.DeleteTransaction
    @id_transaction int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM transactions WHERE id_transaction = @id_transaction)
    BEGIN
        RAISERROR ('Транзакция с таким ID не существует.', 16, 1)
        RETURN
    END
	BEGIN TRY
		DELETE FROM transactions WHERE id_transaction = @id_transaction
	END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
exec dbo.DeleteTransaction @id_transaction = 1;
------------СОЗДАНИЕ КРЕДИТА------------
CREATE OR ALTER PROCEDURE dbo.CreateCredit
    @credit_total_sum money,
    @credit_sum money,
    @credit_date date,
    @credit_status bit = 0,
    @repayment_date date = NULL,
    @repayment_sum money = NULL,
    @id_bank_card int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.bank_card WHERE id_bank_card = @id_bank_card)
    BEGIN
        RAISERROR ('Банковской карты с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        INSERT INTO credits (credit_total_sum, credit_sum, credit_date, credit_status, repayment_date, repayment_sum, id_bank_card)
        VALUES (@credit_total_sum, @credit_sum, @credit_date, @credit_status, @repayment_date, @repayment_sum, @id_bank_card)
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.CreateCredit 
@credit_total_sum = 10000, 
@credit_sum = 5000, 
@credit_date = '2024-05-09', 
@credit_status = 1, 
@repayment_date = '2024-06-09', 
@repayment_sum = 2500, 
@id_bank_card = 123456789;
------------ОБНОВЛЕНИЕ КРЕДИТА------------
CREATE OR ALTER PROCEDURE dbo.UpdateCredit
    @id_credit int,
    @credit_total_sum money,
    @credit_sum money,
    @credit_date date,
    @credit_status bit,
    @repayment_date date,
    @repayment_sum money,
    @id_bank_card int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM credits WHERE id_credit = @id_credit)
    BEGIN
        RAISERROR ('Кредит с таким ID не существует.', 16, 1)
        RETURN
    END
    IF NOT EXISTS (SELECT 1 FROM dbo.bank_card WHERE id_bank_card = @id_bank_card)
    BEGIN
        RAISERROR ('Банковской карты с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        UPDATE credits
        SET credit_total_sum = @credit_total_sum,
            credit_sum = @credit_sum,
            credit_date = @credit_date,
            credit_status = @credit_status,
            repayment_date = @repayment_date,
            repayment_sum = @repayment_sum,
            id_bank_card = @id_bank_card
        WHERE id_credit = @id_credit
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.UpdateCredit 
@id_credit = 1, 
@credit_total_sum = 15000, 
@credit_sum = 8000, 
@credit_date = '2024-05-09', 
@credit_status = 1, 
@repayment_date = '2024-07-09', 
@repayment_sum = 4000, 
@id_bank_card = 987654321;
------------УДАЛЕНИЕ КРЕДИТА------------
CREATE OR ALTER PROCEDURE dbo.DeleteCredit
    @id_credit int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM credits WHERE id_credit = @id_credit)
    BEGIN
        RAISERROR ('Кредита с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        DELETE FROM credits
        WHERE id_credit = @id_credit
   END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.DeleteCredit @id_credit = 1;
------------СОЗДАНИЕ КЛИЕНТ-СЕРВИСА------------
CREATE OR ALTER PROCEDURE dbo.CreateClientService
    @serviceName nvarchar(100),
    @serviceBalance money = 0,
    @serviceType nvarchar(100)
AS
BEGIN
    BEGIN TRY
        INSERT INTO clientServices (serviceName, serviceBalance, serviceType)
        VALUES (@serviceName, @serviceBalance, @serviceType)
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.CreateClientService @serviceName = 'Интернет', @serviceBalance = 0, @serviceType = 'Услуга связи'
------------ОБНОВЛЕНИЕ КЛИЕНТ-СЕРВИСА------------
CREATE OR ALTER PROCEDURE dbo.UpdateClientService
    @id_service int,
    @serviceName nvarchar(100),
    @serviceBalance money,
    @serviceType nvarchar(100)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM clientServices WHERE id_service = @id_service)
    BEGIN
        RAISERROR ('Клиент-сервис с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        UPDATE clientServices
        SET serviceName = @serviceName,
            serviceBalance = @serviceBalance,
            serviceType = @serviceType
        WHERE id_service = @id_service
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.UpdateClientService @id_service = 58, @serviceName = 'Телефония', @serviceBalance = 100, @serviceType = 'Услуга связи'
------------УДАЛЕНИЕ КЛИЕНТ-СЕРВИСА------------
CREATE OR ALTER PROCEDURE dbo.DeleteClientService
    @id_service int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM clientServices WHERE id_service = @id_service)
    BEGIN
        RAISERROR ('Клиент-сервис с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        DELETE FROM clientServices
        WHERE id_service = @id_service
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.DeleteClientService @id_service = 58;
------------СОЗДАНИЕ ПЕРСОНАЛЬНОГО АККАУНТА------------
CREATE OR ALTER PROCEDURE dbo.CreateClientPersonalAccount
    @personal_account nvarchar(10),
    @id_service int,
    @id_client int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.clientServices WHERE id_service = @id_service)
    BEGIN
        RAISERROR ('Клиент-сервис с таким ID не существует.', 16, 1)
        RETURN
    END
	BEGIN TRY
    INSERT INTO clientPersonalAccount (personal_account, id_service, id_client)
    VALUES (@personal_account, @id_service, @id_client)
END TRY
BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
    DECLARE @ErrorState INT = ERROR_STATE()

    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.CreateClientPersonalAccount @personal_account = '1234567890', @id_service = 1, @id_client = 2209;
------------ОБНОВЛЕНИЕ ПЕРСОНАЛЬНОГО АККАУНТА------------
CREATE OR ALTER PROCEDURE dbo.UpdateClientPersonalAccount
    @id_personal_account int,
    @personal_account nvarchar(10),
    @id_service int,
    @id_client int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM clientPersonalAccount WHERE id_personal_account = @id_personal_account)
    BEGIN
        RAISERROR ('Аккаунта с таким ID не существует.', 16, 1)
        RETURN
    END
    IF NOT EXISTS (SELECT 1 FROM dbo.clientServices WHERE id_service = @id_service)
    BEGIN
        RAISERROR ('Клиент-сервис с таким ID не существует.', 16, 1)
        RETURN
    END
    IF NOT EXISTS (SELECT 1 FROM dbo.client WHERE id_client = @id_client)
    BEGIN
        RAISERROR ('Клиент с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        UPDATE clientPersonalAccount
        SET personal_account = @personal_account,
            id_service = @id_service,
            id_client = @id_client
        WHERE id_personal_account = @id_personal_account
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.UpdateClientPersonalAccount @id_personal_account = 1, @personal_account = '9876543210', @id_service = 2, @id_client = 2;
------------УДАЛЕНИЕ ПЕРСОНАЛЬНОГО АККАУНТА------------
CREATE OR ALTER PROCEDURE dbo.DeleteClientPersonalAccount
    @id_personal_account int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM clientPersonalAccount WHERE id_personal_account = @id_personal_account)
    BEGIN
        RAISERROR ('Аккаунта с таким ID не существует.', 16, 1)
        RETURN
    END
    BEGIN TRY
        DELETE FROM clientPersonalAccount
        WHERE id_personal_account = @id_personal_account
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
------------РАБОТОСПОСОБНОСТЬ--------------
EXEC dbo.DeleteClientPersonalAccount @id_personal_account = 1