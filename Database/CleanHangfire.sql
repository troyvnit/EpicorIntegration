USE [EpicorIntegrationHangfire]
GO

DELETE FROM [HangFire].[Set]
GO
DELETE FROM [HangFire].[JobQueue]
GO
DELETE FROM [HangFire].[JobParameter]
GO
DELETE FROM [HangFire].[Job]
GO
