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

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'JobId(����/����)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Job�ȼ�(0.ҵ��1.���ԣ�2.ϵͳ)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'JobLevel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Job����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'Description'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������(��������)(��:Newcats.JobManager.Host.dll)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'AssemblyName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����(���������ռ������)(��:Newcats.JobManager.Host.Manager.SystemJob)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'ClassName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'JobArgs'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cron���ʽ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CronExpression'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Cron���ʽ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CronExpressionDescription'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ϴ�����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'LastFireTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�´�����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'NextFireTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���д���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'FireCount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'״̬(0.ֹͣ��1.�����У�3.�����У�5.ֹͣ�У�7.�ȴ����£�9.�ȴ�����ִ��һ��)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'State'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�Ƿ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'Disabled'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CreateId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CreateName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'UpdateId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'UpdateName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobInfo', @level2type=N'COLUMN',@level2name=N'UpdateTime'
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

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'LogId(����/����)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'JobId(��JobInfo.Id)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'JobId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ִ��ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'FireTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ִ�г���ʱ������λ���룩' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'FireDuration'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ִ�н��(0.�ɹ���1.ʧ�ܣ�2.�쳣)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'FireState'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��־����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'Content'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'JobLog', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

--�����Ǿۼ�����(��Ψһ)
create index IX_JobLog_JobId on JobLog (JobId);