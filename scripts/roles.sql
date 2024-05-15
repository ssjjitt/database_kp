CREATE LOGIN admin_bank_login WITH PASSWORD = 'admin_bank';
CREATE LOGIN user_bank_login WITH PASSWORD = 'user_bank';
CREATE USER admin_bank FOR LOGIN admin_bank_login;
CREATE USER user_bank FOR LOGIN user_bank_login;
CREATE ROLE administrator_role;
CREATE ROLE user_role;
ALTER ROLE administrator_role ADD MEMBER admin_bank;
ALTER ROLE user_role ADD MEMBER user_bank;

------------------------–¿¡Œ“Œ—œŒ—Œ¡ÕŒ—“‹----------------------
EXECUTE AS USER = 'admin_bank';
	SELECT * FROM dbo.GetTransById('11');
REVERT;

EXECUTE AS USER = 'user_bank';
	SELECT * FROM dbo.GetTransById('11');
	select * from client;
REVERT;
------------------------√–¿Õ“€----------------------
GRANT EXECUTE ON dbo.SaveTableDataToJson TO administrator_role;
GRANT EXECUTE ON dbo.CreateCredit TO administrator_role;
GRANT EXECUTE ON dbo.CreateTransaction TO administrator_role;
GRANT EXECUTE ON dbo.DeleteBankCard TO administrator_role;
GRANT EXECUTE ON dbo.DeleteClient TO administrator_role;
GRANT EXECUTE ON dbo.DeleteClientPersonalAccount TO administrator_role;
GRANT EXECUTE ON dbo.DeleteClientService TO administrator_role;
GRANT EXECUTE ON dbo.DeleteCredit TO administrator_role;
GRANT EXECUTE ON dbo.DeleteTransaction TO administrator_role;
GRANT EXECUTE ON dbo.PopulateClients TO administrator_role;
GRANT EXECUTE ON dbo.SaveBankCardDataToJson TO administrator_role;
GRANT EXECUTE ON dbo.SaveClientDataToJson TO administrator_role;
GRANT EXECUTE ON dbo.SaveClientPersonalAccountDataToJson TO administrator_role;
GRANT EXECUTE ON dbo.CreateBankCard TO administrator_role;
GRANT EXECUTE ON dbo.CreateClient TO administrator_role;
GRANT EXECUTE ON dbo.CreateClientPersonalAccount TO administrator_role;
GRANT EXECUTE ON dbo.CreateClientService TO administrator_role;
GRANT EXECUTE ON dbo.UpdateBankCard TO administrator_role;
GRANT EXECUTE ON dbo.UpdateClient TO administrator_role;
GRANT EXECUTE ON dbo.UpdateClientPersonalAccount TO administrator_role;
GRANT EXECUTE ON dbo.UpdateClientService TO administrator_role;
GRANT EXECUTE ON dbo.UpdateCredit TO administrator_role;
GRANT EXECUTE ON dbo.UpdateTransaction TO administrator_role;

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.client TO administrator_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.bank_card TO administrator_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.credits TO administrator_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.transactions TO administrator_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.clientServices TO administrator_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.clientPersonalAccount TO administrator_role;

GRANT SELECT ON dbo.GetTransById TO administrator_role;
GRANT SELECT ON dbo.GetTransByNumber TO administrator_role;
GRANT SELECT ON dbo.SearchClientByLastName TO administrator_role;
GRANT EXECUTE ON dbo.CalculateTotalBalance TO administrator_role;
GRANT SELECT ON dbo.SearchByLastName TO administrator_role;
GRANT SELECT ON dbo.GetClientsByGender TO administrator_role;
GRANT SELECT ON dbo.GetRecentTransactions TO administrator_role;
GRANT SELECT ON dbo.GetClientsWithCreditDebt TO administrator_role;
GRANT SELECT ON dbo.GetLastTransactionForEachCard TO administrator_role;
GRANT EXECUTE ON dbo.HasActiveCredit TO administrator_role;
GRANT SELECT ON dbo.SearchClients TO administrator_role;
GRANT EXECUTE ON dbo.GetTransactionTotalAmount TO administrator_role;
GRANT EXECUTE ON dbo.GetClientCount TO administrator_role;
GRANT EXECUTE ON dbo.GetBankCardCount TO administrator_role;
GRANT EXECUTE ON dbo.GetTransactionCount TO administrator_role;
GRANT EXECUTE ON dbo.GetCreditCount TO administrator_role;
GRANT EXECUTE ON dbo.GetActiveServiceCount TO administrator_role;
GRANT SELECT ON dbo.GetClientsWithoutActiveBankCards TO administrator_role;
GRANT SELECT ON dbo.GetTransById TO administrator_role;

GRANT SELECT ON dbo.client TO user_role;
GRANT SELECT ON dbo.bank_card TO user_role;
GRANT SELECT ON dbo.credits TO user_role;
GRANT SELECT ON dbo.transactions TO user_role;
GRANT SELECT ON dbo.clientServices TO user_role;
GRANT SELECT ON dbo.clientPersonalAccount TO user_role;