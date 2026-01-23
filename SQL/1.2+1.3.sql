
--Задача 1.2

--В переменную @XML типа XML записываем результат выборки из таблицы
DECLARE @XML XML = (SELECT *
FROM T
FOR XML PATH('Row'), ROOT('Data'));

--Выводим XML документ
SELECT @XML


--Задача 1.3

--Выбираем данные из имеющегося XML документа и филтруем по StatusId != 3
SELECT 
    T.row.value('(Id)[1]', 'INT') AS Id,
    T.row.value('(Code)[1]', 'NVARCHAR(50)') AS Code,
    T.row.value('(Name)[1]', 'NVARCHAR(50)') AS Name,
    T.row.value('(StatusId)[1]', 'INT') AS StatusId
FROM @XML.nodes('/Data/Row') AS T(row)
WHERE T.row.value('(StatusId)[1]', 'INT') != 3;
