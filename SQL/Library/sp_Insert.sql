CREATE OR ALTER PROCEDURE dbo.sp_InsertBook(
	@Name NVARCHAR(MAX), 
	@Author NVARCHAR(MAX) = NULL, 
	@Year INT = NULL, 
	@Category NVARCHAR(50) = NULL, 
	@Contents XML = NULL)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO Books(
		Name, 
		Author, 
		Year, 
		Category, 
		Contents) 
	VALUES(
		@Name,
		@Author, 
		@Year, 
		@Category, 
		@Contents)
END
GO
