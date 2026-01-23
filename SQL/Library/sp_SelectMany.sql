CREATE OR ALTER PROCEDURE dbo.sp_SelectMany(@Page INT, @PageSize INT)
AS
BEGIN
	SELECT COUNT(*) AS TotalCount
	FROM Books;

	SELECT *
	FROM Books
	ORDER BY Name
	OFFSET @PageSize*(@Page-1) ROWS
	FETCH NEXT @PageSize ROWS ONLY
	FOR XML PATH('Book'), ROOT('Books')
END
GO