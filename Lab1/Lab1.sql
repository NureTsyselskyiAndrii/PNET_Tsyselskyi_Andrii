IF NOT EXISTS (
    SELECT name FROM sys.databases WHERE name = N'Shop'
)
    CREATE DATABASE Shop;
GO

-- 1.3.0

USE Shop;
GO

IF OBJECT_ID('dbo.Sales',       'U') IS NOT NULL DROP TABLE dbo.Sales;
IF OBJECT_ID('dbo.Goods',       'U') IS NOT NULL DROP TABLE dbo.Goods;
IF OBJECT_ID('dbo.Departments', 'U') IS NOT NULL DROP TABLE dbo.Departments;
GO

CREATE TABLE Departments (
    DEPT_ID  Decimal(4,0) Identity (1, 1)   NOT NULL,
    NAME     Varchar(20)   NULL,
    INFO     Varchar(40)   NULL
);

ALTER TABLE Departments
    ADD CONSTRAINT pk_DEPT_ID
    PRIMARY KEY CLUSTERED (DEPT_ID);

ALTER TABLE Departments
    ADD CONSTRAINT df_INFO
    DEFAULT (NULL) FOR INFO;
GO

CREATE TABLE Goods (
    GOOD_ID     Int   Identity (1, 1)  NOT NULL,
    NAME        Varchar(20)   NULL,
    PRICE       Float         NULL,
    QUANTITY    Integer       NULL,
    PRODUCER    Varchar(20)   NULL,
    DEPT_ID     Decimal(4,0)  NULL,
    DESCRIPTION NVarchar(50)  NULL
);

ALTER TABLE Goods
    ADD CONSTRAINT pk_GOOD_ID 
    PRIMARY KEY CLUSTERED (GOOD_ID);

ALTER TABLE Goods
    ADD CONSTRAINT fk_Goods_DEPT_ID
    FOREIGN KEY (DEPT_ID)
    REFERENCES Departments (DEPT_ID)
    ON UPDATE CASCADE
    ON DELETE SET NULL;
GO

CREATE TABLE Sales (
    SALE_ID   Int  Identity (1, 1)   NOT NULL,
    CHECK_NO  Int     NOT NULL,
    GOOD_ID   Int     NOT NULL,
    DATE_SALE Date    NOT NULL,
    QUANTITY  Integer NULL
);

ALTER TABLE Sales
    ADD CONSTRAINT pk_SALE_ID
    PRIMARY KEY CLUSTERED (SALE_ID);

ALTER TABLE Sales
    ADD CONSTRAINT fk_Sales_GOOD_ID
    FOREIGN KEY (GOOD_ID)
    REFERENCES Goods (GOOD_ID)
    ON UPDATE CASCADE
    ON DELETE NO ACTION;
GO

INSERT INTO Departments (NAME, INFO) VALUES
    (N'Продукти',    N'Харчові товари'),
    (N'Електроніка', N'Техніка та гаджети'),
    (N'Одяг',        N'Чоловічий та жіночий'),
    (N'Побутхімія',  N'Засоби для дому'),
    (N'Канцтовари',  N'Офісні та шкільні');
GO

INSERT INTO Goods (NAME, PRICE, QUANTITY, PRODUCER, DEPT_ID, DESCRIPTION) VALUES
    (N'Молоко',        28.50,  120, N'Галичина', 1, N'Молоко 2.5%, 1л'),
    (N'Хліб чорний',   22.00,   80, N'Київхліб', 1, N'Житній хліб 400г'),
    (N'Навушники',    850.00,   15, N'Samsung',  2, N'Бездротові BT 5.0'),
    (N'Футболка',     299.00,   40, N'H&M',      3, N'Бавовна 100%, XL'),
    (N'Порошок',      145.00,   55, N'Tide',     4, N'Автомат 1.5 кг'),
    (N'Зошит',         35.00,  200, N'Buromax',  5, N'96 аркушів, клітинка');
GO

INSERT INTO Sales (CHECK_NO, GOOD_ID, DATE_SALE, QUANTITY) VALUES
    (1001, 1, '2024-01-10', 3),
    (1001, 2, '2024-01-10', 2),
    (1002, 3, '2024-01-11', 1),
    (1003, 4, '2024-01-12', 2),
    (1003, 5, '2024-01-12', 1),
    (1004, 6, '2024-01-13', 5);
GO

SELECT * FROM Departments;

SELECT * FROM Goods;

SELECT * FROM Sales;

SELECT
    s.SALE_ID,
    s.CHECK_NO,
    s.DATE_SALE,
    g.NAME        AS Товар,
    s.QUANTITY    AS Кількість,
    g.PRICE       AS Ціна,
    g.PRICE * s.QUANTITY AS Сума,
    d.NAME        AS Відділ
FROM Sales s
JOIN Goods       g ON s.GOOD_ID = g.GOOD_ID
JOIN Departments d ON g.DEPT_ID = d.DEPT_ID
ORDER BY s.DATE_SALE, s.CHECK_NO;
GO

-- 1.3.1

SELECT CONVERT(char, GETDATE(), 101) AS CurrentDate;
GO

-- 1.3.2.1

IF OBJECT_ID('dbo.usp_insert_depd', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_insert_depd;
GO
CREATE PROCEDURE usp_insert_depd
    @new_name VARCHAR(20),
    @new_info VARCHAR(40)
AS
BEGIN
    INSERT INTO Departments VALUES (@new_name, @new_info);
END
GO

EXEC usp_insert_depd @new_name = N'Іграшки', @new_info = N'Дитячі товари';
SELECT * FROM Departments;
GO

-- 1.3.2.2

IF OBJECT_ID('dbo.usr_ins_sale', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usr_ins_sale;
GO
CREATE PROCEDURE usr_ins_sale
    @GoodId   INT,
    @Check_no INT,
    @Quantity SMALLINT
AS
BEGIN
    DECLARE @MyOutput INT;
    SET @MyOutput = (
        SELECT COUNT(Quantity)
        FROM Sales
        WHERE GOOD_ID = @GoodId AND CHECK_NO = @Check_no
    );

    IF (@MyOutput > 0)
    BEGIN
        UPDATE Sales
            SET QUANTITY = QUANTITY - @Quantity
        WHERE GOOD_ID = @GoodId AND CHECK_NO = @Check_no;

        PRINT 'Check_no #' + LTRIM(CAST(@Check_no AS VARCHAR(10)));
        PRINT 'Good #'     + LTRIM(CAST(@GoodId   AS VARCHAR(10)));
    END

    IF (@MyOutput = 0)
    BEGIN
        DECLARE @PEX INT;
        SET @PEX = (SELECT COUNT(*) FROM Goods WHERE GOOD_ID = @GoodId);

        IF @PEX = 0
        BEGIN
            PRINT 'There is no product with number #'
                  + LTRIM(CAST(@GoodId AS VARCHAR(10)));
            RETURN 0;
        END
        ELSE
        BEGIN
            INSERT INTO Sales (CHECK_NO, GOOD_ID, QUANTITY, DATE_SALE)
            VALUES (@Check_no, @GoodId, @Quantity, GETDATE());
        END
    END
END
GO

EXEC usr_ins_sale @GoodId = 2, @Check_no = 1005, @Quantity = 3;

EXEC usr_ins_sale @GoodId = 99, @Check_no = 1006, @Quantity = 1;
SELECT * FROM Sales;
GO

-- 1.3.3.1

IF OBJECT_ID('dbo.LESS_THAN_AVG', 'FN') IS NOT NULL
    DROP FUNCTION dbo.LESS_THAN_AVG;
GO
CREATE FUNCTION dbo.LESS_THAN_AVG()
RETURNS INT
AS
BEGIN
    DECLARE @ROW_COUNT INT;
    SET @ROW_COUNT = (
        SELECT COUNT(*)
        FROM Goods
        WHERE PRICE < (SELECT AVG(PRICE) FROM Goods)
    );
    RETURN @ROW_COUNT;
END
GO

SELECT dbo.LESS_THAN_AVG() AS Товарів_дешевших_за_середню;
GO

-- 1.3.3.2

IF OBJECT_ID('dbo.Sum_Price_in_Dept', 'IF') IS NOT NULL
    DROP FUNCTION dbo.Sum_Price_in_Dept;
GO
CREATE FUNCTION dbo.Sum_Price_in_Dept()
RETURNS TABLE
AS
RETURN
(
    SELECT
        d.DEPT_ID,
        d.NAME,
        SUM(g.PRICE * g.QUANTITY) AS Sum_Price
    FROM Departments d
    INNER JOIN Goods g ON d.DEPT_ID = g.DEPT_ID
    GROUP BY d.DEPT_ID, d.NAME
);
GO

SELECT * FROM dbo.Sum_Price_in_Dept();
GO

-- 1.3.4

IF OBJECT_ID('dbo.SalesLogs', 'U') IS NOT NULL
    DROP TABLE dbo.SalesLogs;
GO
CREATE TABLE dbo.SalesLogs (
    Id         INT      NOT NULL IDENTITY(1,1),
    SaleId     INT      NOT NULL,
    ModifyDate DATETIME NOT NULL,
    CONSTRAINT PK_SALELOGS PRIMARY KEY (Id)
);
GO

IF OBJECT_ID('dbo.tr_SalesLoging', 'TR') IS NOT NULL
    DROP TRIGGER dbo.tr_SalesLoging;
GO
CREATE TRIGGER dbo.tr_SalesLoging
ON dbo.Sales
AFTER UPDATE
AS
BEGIN
    INSERT INTO dbo.SalesLogs (SaleId, ModifyDate)
    VALUES ((SELECT SALE_ID FROM inserted), GETDATE());
END
GO

UPDATE Sales SET QUANTITY = 10 WHERE SALE_ID = 1;
SELECT * FROM dbo.SalesLogs;
GO


-- 1.3.5

USE Shop;
GO

-- 1

IF OBJECT_ID('dbo.fn_GetCheckByProducer', 'TF') IS NOT NULL
    DROP FUNCTION dbo.fn_GetCheckByProducer;
GO

CREATE FUNCTION dbo.fn_GetCheckByProducer
(
    @Producer VARCHAR(20)
)
RETURNS @Result TABLE (CHECK_NO INT, TOTAL_QTY INT)
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM Goods g
        JOIN Sales s ON g.GOOD_ID = s.GOOD_ID
        WHERE g.PRODUCER = @Producer
    )
    BEGIN
        INSERT INTO @Result VALUES (-1, 0);
        RETURN;
    END

    ;WITH CTE AS (
        SELECT
            s.CHECK_NO,
            SUM(s.QUANTITY) AS TOTAL_QTY
        FROM Sales s
        JOIN Goods g ON s.GOOD_ID = g.GOOD_ID
        WHERE g.PRODUCER = @Producer
        GROUP BY s.CHECK_NO
    ),
    MaxCTE AS (
        SELECT MAX(TOTAL_QTY) AS MAX_QTY FROM CTE
    )
    INSERT INTO @Result
    SELECT c.CHECK_NO, c.TOTAL_QTY
    FROM CTE c
    JOIN MaxCTE m ON c.TOTAL_QTY = m.MAX_QTY;

    RETURN;
END;
GO

DECLARE @Producer VARCHAR(20) = N'Галичина';
DECLARE @HasResult BIT = 0;

SELECT @HasResult = 1 FROM dbo.fn_GetCheckByProducer(@Producer) WHERE CHECK_NO <> -1;

IF @HasResult = 1
    SELECT CHECK_NO, TOTAL_QTY AS [Кількість товарів]
    FROM dbo.fn_GetCheckByProducer(@Producer);
ELSE
    PRINT N'Відсутня інформація';
GO

DECLARE @Producer2 VARCHAR(20) = N'НеіснуючийВиробник';
DECLARE @HasResult2 BIT = 0;
SELECT @HasResult2 = 1 FROM dbo.fn_GetCheckByProducer(@Producer2) WHERE CHECK_NO <> -1;
IF @HasResult2 = 0
    PRINT N'Відсутня інформація';
GO


-- 6

IF OBJECT_ID('dbo.sp_UpdateDeptInfo', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_UpdateDeptInfo;
GO

CREATE PROCEDURE dbo.sp_UpdateDeptInfo
    @DeptId DECIMAL(4,0)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TopProducer VARCHAR(20);

    SELECT TOP 1 @TopProducer = PRODUCER
    FROM Goods
    WHERE DEPT_ID = @DeptId
      AND PRODUCER IS NOT NULL
    GROUP BY PRODUCER
    ORDER BY COUNT(DISTINCT GOOD_ID) DESC;

    IF @TopProducer IS NULL
    BEGIN
        PRINT N'Відсутня інформація про відділ або товари.';
        RETURN;
    END

    DECLARE @InfoText VARCHAR(40);
    SET @InfoText = CONVERT(VARCHAR(10), GETDATE(), 120) + N' ' + @TopProducer;

    UPDATE Departments
    SET INFO = @InfoText
    WHERE DEPT_ID = @DeptId;

    PRINT N'INFO оновлено: ' + @InfoText;
END;
GO

EXEC dbo.sp_UpdateDeptInfo @DeptId = 1;
SELECT DEPT_ID, NAME, INFO FROM Departments WHERE DEPT_ID = 1;
GO

-- 11

IF OBJECT_ID('dbo.fn_SalesByEveryIthGood', 'TF') IS NOT NULL
    DROP FUNCTION dbo.fn_SalesByEveryIthGood;
GO

CREATE FUNCTION dbo.fn_SalesByEveryIthGood
(
    @k FLOAT
)
RETURNS @Result TABLE (
    SALE_ID    INT,
    CHECK_NO   INT,
    GOOD_ID    INT,
    GOOD_NAME  VARCHAR(20),
    PRODUCER   VARCHAR(20),
    DATE_SALE  DATE,
    QUANTITY   INT,
    TOTAL_SUM  FLOAT
)
AS
BEGIN
    DECLARE @i INT = 10;

    ;WITH NumberedGoods AS (
        SELECT
            GOOD_ID,
            NAME,
            PRICE,
            PRODUCER,
            ROW_NUMBER() OVER (ORDER BY GOOD_ID) AS RowNum
        FROM Goods
    ),
    IthGoods AS (
        SELECT GOOD_ID, NAME, PRICE, PRODUCER
        FROM NumberedGoods
        WHERE RowNum % @i = 0
    )
    INSERT INTO @Result
    SELECT
        s.SALE_ID,
        s.CHECK_NO,
        s.GOOD_ID,
        g.NAME,
        g.PRODUCER,
        s.DATE_SALE,
        s.QUANTITY,
        ROUND(s.QUANTITY * g.PRICE, 2) AS TOTAL_SUM
    FROM Sales s
    JOIN IthGoods g ON s.GOOD_ID = g.GOOD_ID
    WHERE s.QUANTITY * g.PRICE < @k;

    RETURN;
END;
GO

INSERT INTO Goods (NAME, PRICE, QUANTITY, PRODUCER, DEPT_ID) VALUES
(N'Товар7',  50.00, 10, N'Виробник1', 1),
(N'Товар8',  60.00, 10, N'Виробник2', 2),
(N'Товар9',  70.00, 10, N'Виробник3', 3),
(N'Товар10', 80.00, 10, N'Виробник4', 4);

INSERT INTO Sales (CHECK_NO, GOOD_ID, DATE_SALE, QUANTITY) VALUES
(1010, 10, '2024-02-01', 2);

SELECT * FROM dbo.fn_SalesByEveryIthGood(500.00);
GO


-- 16

IF OBJECT_ID('dbo.tr_ProtectLastGood', 'TR') IS NOT NULL
    DROP TRIGGER dbo.tr_ProtectLastGood;
GO

CREATE TRIGGER dbo.tr_ProtectLastGood
ON dbo.Goods
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM inserted i
            WHERE i.QUANTITY = 0
              AND EXISTS (
                    SELECT 1
                    FROM Goods g2
                    WHERE g2.DEPT_ID = i.DEPT_ID
                      AND g2.GOOD_ID <> i.GOOD_ID
                    HAVING COUNT(*) = 0
                )
        )
        BEGIN
            RAISERROR (
                N'Помилка: не можна зменшити кількість до 0 — це єдиний товар у відділі.',
                16, 1
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END
    END

    IF NOT EXISTS (SELECT 1 FROM inserted)
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM deleted d
            WHERE NOT EXISTS (
                SELECT 1
                FROM Goods g2
                WHERE g2.DEPT_ID = d.DEPT_ID
                  AND g2.GOOD_ID <> d.GOOD_ID
            )
        )
        BEGIN
            RAISERROR (
                N'Помилка: не можна видалити останній товар у відділі.',
                16, 1
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END
    END
END;
GO

BEGIN TRY
    UPDATE Goods SET QUANTITY = 0 WHERE GOOD_ID = 6;
    PRINT 'UPDATE дозволено (несподівано).';
END TRY
BEGIN CATCH
    PRINT 'ОЧІКУВАНА ПОМИЛКА UPDATE: ' + ERROR_MESSAGE();
END CATCH;
GO

BEGIN TRY
    DELETE FROM Goods WHERE GOOD_ID = 6;
    PRINT 'DELETE дозволено (несподівано).';
END TRY
BEGIN CATCH
    PRINT 'ОЧІКУВАНА ПОМИЛКА DELETE: ' + ERROR_MESSAGE();
END CATCH;
GO

BEGIN TRY
    UPDATE Goods SET QUANTITY = 0 WHERE GOOD_ID = 1;
    PRINT 'UPDATE дозволено (очікувано — інші товари у відділі є).';
    UPDATE Goods SET QUANTITY = 120 WHERE GOOD_ID = 1;
END TRY
BEGIN CATCH
    PRINT 'ПОМИЛКА: ' + ERROR_MESSAGE();
END CATCH;
GO

BEGIN TRY
    DELETE FROM Sales WHERE GOOD_ID = 1;
    DELETE FROM Goods WHERE GOOD_ID = 1;
    PRINT 'DELETE дозволено (очікувано — інший товар у відділі є).';
END TRY
BEGIN CATCH
    PRINT 'ПОМИЛКА: ' + ERROR_MESSAGE();
END CATCH;
GO