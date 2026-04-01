/*
* FILE         : DatabaseSchema.sql
* PROJECT      : A04-MVC
* PROGRAMMER   : Brandy
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : SQL Server database schema for home inventory catalog system.
*                Creates tables for users, categories, and items with seed data.
*/

SET NOCOUNT ON
GO

USE master
GO

-- Drop database if it exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'mvc')
BEGIN
    ALTER DATABASE [mvc] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [mvc]
END
GO

-- Create new database
CREATE DATABASE [mvc]
GO

USE [mvc]
GO

SET DATEFORMAT ymd
GO

-- Create Category table
IF OBJECT_ID('dbo.Category', 'U') IS NOT NULL
    DROP TABLE dbo.Category
GO

CREATE TABLE dbo.Category
(
    CategoryID INT NOT NULL,
    CategoryName VARCHAR(50),
    CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (CategoryID)
);
GO

-- Create LoginInfo table
IF OBJECT_ID('dbo.LoginInfo', 'U') IS NOT NULL
    DROP TABLE dbo.LoginInfo
GO

CREATE TABLE dbo.LoginInfo
(
    UserID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Username VARCHAR(25) UNIQUE,
    Password VARCHAR(50)
);
GO

-- Create ItemCatalog table
IF OBJECT_ID('dbo.ItemCatalog', 'U') IS NOT NULL
    DROP TABLE dbo.ItemCatalog
GO

CREATE TABLE dbo.ItemCatalog
(
    UserID BIGINT NOT NULL,
    Item VARCHAR(80),
    Price FLOAT,
    DateBought DATE,
    CategoryID INT,
    CONSTRAINT FK_ItemCatalog_Category FOREIGN KEY (CategoryID) REFERENCES dbo.Category(CategoryID),
    CONSTRAINT FK_ItemCatalog_LoginInfo FOREIGN KEY (UserID) REFERENCES dbo.LoginInfo(UserID)
);
GO

-- Create function for user-specific catalog view
IF OBJECT_ID('dbo.UserViewCatalog', 'TF') IS NOT NULL
    DROP FUNCTION dbo.UserViewCatalog
GO

CREATE FUNCTION dbo.UserViewCatalog(@UserID BIGINT)
RETURNS TABLE
AS
RETURN
(
    SELECT
        i.Item,
        i.Price,
        i.DateBought,
        c.CategoryID,
        c.CategoryName
    FROM dbo.ItemCatalog AS i
    INNER JOIN dbo.Category AS c ON i.CategoryID = c.CategoryID
    WHERE i.UserID = @UserID
);
GO

-- Insert seed data into LoginInfo
INSERT INTO dbo.LoginInfo (Username, Password)
VALUES
    ('Rodrigo', 'Password'),
    ('Jules', 'Password'),
    ('Brandy', 'Password'),
    ('Negin', 'Password');
GO

-- Insert seed data into Category
INSERT INTO dbo.Category (CategoryID, CategoryName)
VALUES
    (1, 'Furniture'),
    (2, 'Electronics'),
    (3, 'Appliances'),
    (4, 'Clothing'),
    (5, 'Decor & Bedding'),
    (6, 'Kitchenwares'),
    (7, 'Tools & Equipment'),
    (8, 'Valuables & Collectables'),
    (9, 'Personal Health')
GO

-- Insert seed data into ItemCatalog for User ID 1
INSERT INTO dbo.ItemCatalog (UserID, Item, Price, DateBought, CategoryID)
VALUES
    (1, 'Couch', 999.99, '2000-11-11', 1),
    (1, 'Bed Frame', 666.66, '2000-11-05', 1),
    (1, 'Dining Table', 300.00, '1990-05-20', 1),
    (1, 'Dining Chairs', 250.00, '1990-05-20', 1),
    (1, 'TV', 888.88, '2010-01-01', 2),
    (1, 'Refridgerator', 1500.00, '2009-03-13', 3),
    (1, 'Toaster Oven', 49.99, '2005-10-29', 3),
    (1, 'Stove', 899.99, '2000-10-11', 3),
    (1, 'Cookware Set', 455.00, '2008-09-25', 6),
    (1, 'Winter Jacket', 400.00, '2007-12-21', 4),
    (1, 'Leather Boots', 210.99, '2006-07-05', 4),
    (1, 'Bedding Set', 235.50, '2004-11-02', 5),
    (1, 'Drill Set', 199.99, '2007-09-22', 7),
    (1, 'Pills', 61.39, '2011-09-09', 9),
    (1, 'Baby toys', 400.50, '2011-10-10', 8)
GO

-- Insert seed data into ItemCatalog for User ID 2
INSERT INTO dbo.ItemCatalog (UserID, Item, Price, DateBought, CategoryID)
VALUES
    (2, 'Couch', 999.99, '2000-11-11', 1),
    (2, 'Bed Frame', 666.66, '2000-11-05', 1),
    (2, 'Dining Table', 300.00, '1990-05-20', 1),
    (2, 'Dining Chairs', 250.00, '1990-05-20', 1),
    (2, 'TV', 888.88, '2010-01-01', 2),
    (2, 'Refridgerator', 1500.00, '2009-03-13', 3),
    (2, 'Toaster Oven', 49.99, '2005-10-29', 3),
    (2, 'Stove', 899.99, '2000-10-11', 3),
    (2, 'Cookware Set', 455.00, '2008-09-22', 6),
    (2, 'Blazer Jacket', 200.00, '2008-02-18', 4),
    (2, 'Running Shoes', 122.99, '2009-11-12', 4),
    (2, 'Entry Rug', 200.99, '2007-04-01', 5),
    (2, 'Bedding Set', 235.50, '2004-11-02', 5),
    (2, 'Drill Set', 199.99, '2007-09-22', 7),
    (2, 'Vitamins', 85.49, '2011-05-11', 9),
    (2, 'Food', 256.44, '2011-11-11', 8)
GO

PRINT 'Database schema created successfully.'
