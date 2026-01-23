CREATE OR ALTER PROCEDURE dbo.sp_EditBook(
	@Id INT,
	@Name NVARCHAR(MAX) = NULL,
	@Author NVARCHAR(MAX) = NULL,
	@Year INT = NULL,
	@Category NVARCHAR(100) = NULL,
	@Contents XML = NULL)
AS
BEGIN
	SET NOCOUNT ON

	SET XACT_ABORT ON

	BEGIN TRY
		BEGIN TRANSACTION

			--Блокируем строку для обновления
			UPDATE Books WITH(UPDLOCK, ROWLOCK)
			SET 
				--Если null оставляем предыдущее значение
				Name = ISNULL(@Name, Name),
				Author = ISNULL(@Author, Author),
				Year = ISNULL(@Year, Year),
				Category = ISNULL(@Category, Category),
				Contents = ISNULL(@Contents, Contents)
			WHERE Id = @Id

			IF @@ROWCOUNT = 0
				RAISERROR('Книга с Id: %d', 16, 1, @Id)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION

		SELECT ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO