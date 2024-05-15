--------------------ÊËÈÅÍÒÛ È ÊÀÐÒÛ----------------------
CREATE VIEW ClientBankInfo AS
SELECT c.id_client, c.client_last_name, c.client_first_name, c.client_middle_name, c.client_gender, b.id_bank_card, b.bank_card_number, b.bank_card_balance
FROM client AS c
JOIN bank_card AS b ON c.id_client = b.id_client;
-------------------------------------------------------------
SELECT * FROM ClientBankInfo;
-----------------ÈÍÔÎ Î ÊÐÅÄÈÒÀÕ---------------------------
CREATE VIEW ClientCreditInfo AS
SELECT c.id_client, c.client_last_name, c.client_first_name, c.client_middle_name, SUM(cr.credit_sum) AS total_credit_sum
FROM client AS c
JOIN bank_card AS b ON c.id_client = b.id_client
JOIN credits AS cr ON b.id_bank_card = cr.id_bank_card
GROUP BY c.id_client, c.client_last_name, c.client_first_name, c.client_middle_name;
-------------------------------------------------------------
SELECT * FROM ClientCreditInfo;
-----------------ÀÊÒÈÂÍÛÅ ÓÑËÓÃÈ---------------------------
CREATE VIEW ClientServiceInfo AS
SELECT c.id_client, c.client_last_name, c.client_first_name, c.client_middle_name, COUNT(cs.id_service) AS active_service_count
FROM client AS c
JOIN clientPersonalAccount AS cpa ON c.id_client = cpa.id_client
JOIN clientServices AS cs ON cpa.id_service = cs.id_service
WHERE cs.serviceBalance > 0
GROUP BY c.id_client, c.client_last_name, c.client_first_name, c.client_middle_name;
-------------------------------------------------------------
SELECT * FROM ClientServiceInfo;