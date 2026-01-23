--Задача 2.1

CREATE OR ALTER PROCEDURE dbo.sp_TransferMoney(@N1 INT, @N2 INT, @S MONEY)
AS 
BEGIN 
	--Не выводим количество затронутых строк
	SET NOCOUNT ON

	--Откат транзакции при любых ошибках
	SET XACT_ABORT ON

	BEGIN TRY
		--Валидируем указанную сумму перевода без транзакции
		IF @S <= 0
			RAISERROR('Значение суммы перевода 0 или меньше', 16, 1)

		--Валидируем счета отправителя и получателя без транзакции
		IF @N1 = @N2
			RAISERROR('Перевод на счет отправителя', 16, 2)
		

		BEGIN TRANSACTION

			--Счета для блокировки в одном порядке по возрастанию номера счета для предотвращения дедлока
			DECLARE @minN INT = CASE WHEN @N1 < @N2 THEN @N1 ELSE @N2 END;
			DECLARE @maxN INT = CASE WHEN @N1 < @N2 THEN @N2 ELSE @N1 END;

			--Блокируем и проверяем существование обоих счетов
			IF NOT EXISTS(
				SELECT 1 FROM dbo.T WITH (UPDLOCK, ROWLOCK) 
				WHERE N = @minN
			)
				RAISERROR('Счет %d не существует', 16, 3, @minN);

			IF NOT EXISTS(
				SELECT 1 FROM dbo.T WITH (UPDLOCK, ROWLOCK) 
				WHERE N = @maxN
			)
				RAISERROR('Счет %d не существует', 16, 4, @maxN);

			--Проверяем баланс счета отправителя
			IF (SELECT T.S FROM T WHERE T.N = @N1) < @S
				RAISERROR('Недостаточно средств на счете %d', 16, 5, @N1)

			--Апдейт счета отправителя
			UPDATE T
			SET S = S - @S
			WHERE N = @N1

			--Апдейт счета получателя
			UPDATE T
			SET S = S + @S
			WHERE N = @N2

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		--Проверяем, есть ли активные транзакции и откатываем если есть
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION

		SELECT ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO