USE [NewcatsDB20170627]
GO

/****** Object:  Table [dbo].[JobInfo]    Script Date: 2018/9/25 12:20:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JobInfo]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobLevel] [int] NULL,
	[Name] [nvarchar](256) NULL,
	[Description] [nvarchar](512) NULL,
	[AssemblyName] [nvarchar](256) NULL,
	[ClassName] [nvarchar](256) NULL,
	[JobArgs] [nvarchar](512) NULL,
	[CronExpression] [nvarchar](64) NULL,
	[CronExpressionDescription] [nvarchar](256) NULL,
	[LastFireTime] [datetime] NULL,
	[NextFireTime] [datetime] NULL,
	[FireCount] [int] NOT NULL,
	[State] [int] NOT NULL,
	[Disabled] [bit] NOT NULL,
	[CreateId] [bigint] NULL,
	[CreateName] [nvarchar](64) NULL,
	[CreateTime] [datetime] NULL,
	[UpdateId] [bigint] NULL,
	[UpdateName] [nvarchar](64) NULL,
	[UpdateTime] [datetime] NULL,
	CONSTRAINT [PK_JobInfo] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'JobId(主键/自增)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Job等级(0.业务，1.测试，2.系统)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'JobLevel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Job名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'Description'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序集名称(所属程序集)(例:Newcats.JobManager.Host.exe)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'AssemblyName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类名(完整命名空间的类名)(例:Newcats.JobManager.Host.Manager.SystemJob)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'ClassName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'JobArgs'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cron表达式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CronExpression'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cron表达式描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CronExpressionDescription'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上次运行时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'LastFireTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下次运行时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'NextFireTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运行次数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'FireCount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态(0.停止，1.运行中，3.启动中，5.停止中，7.等待更新，9.等待立即执行一次)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'State'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否禁用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'Disabled'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CreateId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CreateName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后更新人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'UpdateId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后更新人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'UpdateName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后更新时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'UpdateTime'
GO





USE [NewcatsDB20170627]
GO

/****** Object:  Table [dbo].[JobLog]    Script Date: 2018/9/25 12:20:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JobLog]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[FireTime] [datetime] NULL,
	[FireDuration] [decimal](18, 2) NOT NULL,
	[FireState] [int] NOT NULL,
	[Content] [nvarchar](1024) NULL,
	[CreateTime] [datetime] NULL,
	CONSTRAINT [PK_JobLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'LogId(主键/自增)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'JobId(表JobInfo.Id)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'JobId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'执行时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'FireTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'执行持续时长' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'FireDuration'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'执行结果(0.成功，1.失败，2.异常)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'FireState'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'Content'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO