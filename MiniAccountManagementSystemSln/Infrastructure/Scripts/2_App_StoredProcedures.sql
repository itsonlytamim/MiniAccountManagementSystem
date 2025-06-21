-- 2_App_StoredProcedures.sql

/****** Stored Procedure: sp_ManageChartOfAccounts ******/
USE [MiniAccountManagementSystemDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_ManageChartOfAccounts]    Script Date: 6/20/2025 9:37:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[sp_ManageChartOfAccounts]
    @Action NVARCHAR(10), @AccountId INT=NULL, @AccountCode VARCHAR(20)=NULL, @AccountName NVARCHAR(100)=NULL, @ParentAccountId INT=NULL
AS BEGIN SET NOCOUNT ON;
    IF @Action='CREATE' BEGIN INSERT INTO ChartOfAccounts(AccountCode,AccountName,ParentAccountId)VALUES(@AccountCode,@AccountName,@ParentAccountId);SELECT SCOPE_IDENTITY();END
    ELSE IF @Action='UPDATE' BEGIN UPDATE ChartOfAccounts SET AccountCode=@AccountCode,AccountName=@AccountName,ParentAccountId=@ParentAccountId WHERE AccountId=@AccountId;END
    ELSE IF @Action='DELETE' BEGIN IF NOT EXISTS(SELECT 1 FROM VoucherDetails WHERE AccountId=@AccountId)BEGIN DELETE FROM ChartOfAccounts WHERE AccountId=@AccountId;END ELSE BEGIN RAISERROR('Cannot delete account; it has related vouchers.',16,1);RETURN;END END
    ELSE IF @Action='GETBYID' BEGIN SELECT c.AccountId,c.AccountCode,c.AccountName,p.AccountCode AS ParentAccountId FROM ChartOfAccounts c LEFT JOIN ChartOfAccounts p ON c.ParentAccountId=p.AccountId WHERE c.AccountId=@AccountId; END
    ELSE IF @Action='GETALL' BEGIN WITH AccountHierarchy AS(SELECT AccountId,AccountCode,AccountName,ParentAccountId,0 AS Level FROM ChartOfAccounts WHERE ParentAccountId IS NULL UNION ALL SELECT c.AccountId,c.AccountCode,c.AccountName,c.ParentAccountId,h.Level+1 FROM ChartOfAccounts c INNER JOIN AccountHierarchy h ON c.ParentAccountId=h.AccountId)SELECT AccountId,AccountCode,AccountName,ParentAccountId,Level FROM AccountHierarchy ORDER BY AccountName;END
END
GO

/****** Stored Procedure: sp_SaveVoucher ******/
CREATE OR ALTER PROCEDURE [dbo].[sp_SaveVoucher]
    @VoucherDate DATE, @VoucherType NVARCHAR(50), @ReferenceNo NVARCHAR(100), @Narration NVARCHAR(500), @CreatedByUserId NVARCHAR(450), @VoucherDetails dbo.VoucherDetailType READONLY
AS BEGIN SET NOCOUNT ON;BEGIN TRANSACTION;
    DECLARE @TotalDebit DECIMAL(18,2)=(SELECT SUM(DebitAmount)FROM @VoucherDetails);DECLARE @TotalCredit DECIMAL(18,2)=(SELECT SUM(CreditAmount)FROM @VoucherDetails);
    IF @TotalDebit<>@TotalCredit BEGIN ROLLBACK TRANSACTION;RAISERROR('Debit and Credit totals must be equal.',16,1);RETURN;END
    INSERT INTO Vouchers(VoucherDate,VoucherType,ReferenceNo,Narration,CreatedByUserId)VALUES(@VoucherDate,@VoucherType,@ReferenceNo,@Narration,@CreatedByUserId);
    DECLARE @NewVoucherId INT=SCOPE_IDENTITY();
    INSERT INTO VoucherDetails(VoucherId,AccountId,DebitAmount,CreditAmount)SELECT @NewVoucherId,AccountId,DebitAmount,CreditAmount FROM @VoucherDetails;
    COMMIT TRANSACTION;SELECT @NewVoucherId;
END
GO
