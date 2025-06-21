-- 1_App_Schema.sql
-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MiniAccountManagementSystemDB')
BEGIN
    CREATE DATABASE [MiniAccountManagementSystemDB];
    PRINT 'Database MiniAccountManagementSystemDB created successfully.';
END
GO

USE [MiniAccountManagementSystemDB];
GO

-- Create ChartOfAccounts table
CREATE TABLE ChartOfAccounts (
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    AccountCode VARCHAR(20) NOT NULL UNIQUE,
    AccountName NVARCHAR(100) NOT NULL,
    ParentAccountId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL,
    CONSTRAINT FK_ChartOfAccounts_Parent FOREIGN KEY (ParentAccountId) 
        REFERENCES ChartOfAccounts(AccountId)
);
GO

-- Create Vouchers table
CREATE TABLE Vouchers (
    VoucherId INT IDENTITY(1,1) PRIMARY KEY,
    VoucherDate DATE NOT NULL,
    VoucherType NVARCHAR(50) NOT NULL,
    ReferenceNo NVARCHAR(100),
    Narration NVARCHAR(500),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByUserId NVARCHAR(450),
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(450) NULL
);
GO

-- Create VoucherDetails table
CREATE TABLE VoucherDetails (
    VoucherDetailId INT IDENTITY(1,1) PRIMARY KEY,
    VoucherId INT NOT NULL,
    AccountId INT NOT NULL,
    DebitAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CreditAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_VoucherDetails_Voucher FOREIGN KEY (VoucherId) 
        REFERENCES Vouchers(VoucherId) ON DELETE CASCADE,
    CONSTRAINT FK_VoucherDetails_Account FOREIGN KEY (AccountId) 
        REFERENCES ChartOfAccounts(AccountId),
    CONSTRAINT CHK_DebitOrCredit CHECK (
        (DebitAmount > 0 AND CreditAmount = 0) OR 
        (CreditAmount > 0 AND DebitAmount = 0) OR
        (DebitAmount = 0 AND CreditAmount = 0)
    )
);
GO

-- Create VoucherDetailType type
CREATE TYPE dbo.VoucherDetailType AS TABLE (
    AccountId INT NOT NULL,
    DebitAmount DECIMAL(18, 2) NOT NULL,
    CreditAmount DECIMAL(18, 2) NOT NULL
);
GO

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX IX_ChartOfAccounts_ParentAccountId 
    ON ChartOfAccounts(ParentAccountId);
CREATE NONCLUSTERED INDEX IX_Vouchers_VoucherDate 
    ON Vouchers(VoucherDate);
CREATE NONCLUSTERED INDEX IX_VoucherDetails_VoucherId 
    ON VoucherDetails(VoucherId);
CREATE NONCLUSTERED INDEX IX_VoucherDetails_AccountId 
    ON VoucherDetails(AccountId);
GO
