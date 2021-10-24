-- SELECT * FROM SYS.DATABASES
USE [master]
GO

DROP DATABASE IF EXISTS [DataservicesTest]

CREATE DATABASE [DataservicesTest]
GO

USE [DataservicesTest]

CREATE TABLE [dbo].[DimCustomerAttributes]
(
    [DimLatestCustomerAccountSKey] INT NULL,
    [FirstOrderReceiptID] INT NULL,
    [FirstOrderCustomerOrderReference] NVARCHAR(255) NULL,
    [FirstOrderDimOrderCreatedDateSKey] INT NULL,
    [FirstOrderDimOrderCreatedTimeSKey] INT NULL,
    [FirstOrderDimBilledDateSKey] INT NULL,
    [FirstOrderDimShippedDateSKey] INT NULL,
    [FirstOrderDimCustomerAgeSKey] INT NULL,
    [FirstOrderCustomerAge] INT NULL,
    [FirstOrderDimAffiliateSKey] INT NULL,
    [FirstOrderDimDiscountSKey] INT NULL,
    [Last24MonthsOrderCount] BIGINT NULL,
    [LastModifiedDateTime] DATETIME2 NULL
)

CREATE TABLE [dbo].[DimCustomerAccountAttributes]
(
    [DimCustomerAccountSKey] INT NULL,
    [FirstOrderReceiptID] INT NULL,
    [FirstOrderCustomerOrderReference] NVARCHAR(255) NULL,
    [FirstOrderDimOrderCreatedDateSKey] INT NULL,
    [FirstOrderDimOrderCreatedTimeSKey] INT NULL,
    [FirstOrderDimBilledDateSKey] INT NULL,
    [FirstOrderDimShippedDateSKey] INT NULL,
    [FirstOrderDimCustomerAgeSKey] INT NULL,
    [FirstOrderCustomerAge] INT NULL,
    [FirstOrderDimAffiliateSKey] INT NULL,
    [FirstOrderDimDiscountSKey] INT NULL,
    [Last24MonthsOrderCount] BIGINT NULL,
    [LastModifiedDateTime] DATETIME2 NULL
);

CREATE TABLE [dbo].[DimCustomerAccount]
(
    [DimCustomerAccountSKey] int,
    [AccountId] int,
    [AccountGUID] char(32),
    [AccountSiteId] int,
    [AccountSiteName] nvarchar(50),
    [AccountDisplayName] nvarchar(15),
    [AccountAge] int,
    [CustomerAge] int,
    [AccountGender] varchar(7),
    [AccountGenderSKey] int,
    [DimAccountRegisteredDateSKey] int,
    [IsAccountPremierCustomer] bit,
    [AccountPremierCustomer] varchar(11),
    [IsFirstTimeBuyer] bit,
    [FirstTimeBuyerAccount] varchar(20),
    [DimDatePremierStartSKey] int,
    [DimLatestCustomerAccountSKey] int,
    [IsSingleCustomerView] bit,
    [DimDefaultBillingAddressSKey] int,
    [DimDefaultShippingAddressSKey] int,
    [LastUsedBillingAddressSKey] int,
    [LastUsedShippingAddressSKey] int,
    [SCDValidFrom] datetime,
    [SCDValidTo] datetime,
    [IsCurrent] bit,
    [CreatedByLogID] int,
    [LastModifiedByLogID] int
)

GO

:r /workspace/database/PopulateDimCustomerAttributes.sql
GO

PRINT 'DATABASE SETUP COMPLETE :)'