USE [master]
GO

/****** Object:  Database [TaxiOnline]    Script Date: 20.04.2014 11:00:04 ******/
CREATE DATABASE [TaxiOnline]
 ON  PRIMARY 
( NAME = N'TaxiOnline', FILENAME = N'C:\temp\DATA\TaxiOnline.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TaxiOnline_log', FILENAME = N'C:\temp\DATA\TaxiOnline_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [TaxiOnline] SET COMPATIBILITY_LEVEL = 110
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TaxiOnline].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [TaxiOnline] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [TaxiOnline] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [TaxiOnline] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [TaxiOnline] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [TaxiOnline] SET ARITHABORT OFF 
GO

ALTER DATABASE [TaxiOnline] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [TaxiOnline] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [TaxiOnline] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [TaxiOnline] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [TaxiOnline] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [TaxiOnline] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [TaxiOnline] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [TaxiOnline] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [TaxiOnline] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [TaxiOnline] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [TaxiOnline] SET  DISABLE_BROKER 
GO

ALTER DATABASE [TaxiOnline] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [TaxiOnline] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [TaxiOnline] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [TaxiOnline] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [TaxiOnline] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [TaxiOnline] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [TaxiOnline] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [TaxiOnline] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [TaxiOnline] SET  MULTI_USER 
GO

ALTER DATABASE [TaxiOnline] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [TaxiOnline] SET DB_CHAINING OFF 
GO

ALTER DATABASE [TaxiOnline] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [TaxiOnline] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [TaxiOnline] SET  READ_WRITE 
GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[Cities]    Script Date: 20.04.2014 11:01:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Cities](
	[Id] [uniqueidentifier] NOT NULL default newid(),
	[InitialLatitude] [float] NOT NULL,
	[InitialLongitude] [float] NOT NULL,
	[InitialZoom] [float] NOT NULL,
	[PhoneConstraintPattern] [nvarchar](300) NULL,
	[PhoneCorrectionPattern] [nvarchar](300) NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Cities] ADD  CONSTRAINT [DF_Cities_Id]  DEFAULT (newid()) FOR [Id]
GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[CityNames]    Script Date: 20.04.2014 11:01:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CityNames](
	[Id] [int] NOT NULL IDENTITY,
	[Name] [nvarchar](100) NULL,
	[CultureName] [nvarchar](8) NULL,
	[CityId] [uniqueidentifier] NOT NULL,
	[PhoneConstraintDescription] [nvarchar](500) NULL,
 CONSTRAINT [PK_CityNames] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CityNames]  WITH CHECK ADD  CONSTRAINT [FK_CityNames_Cities] FOREIGN KEY([CityId])
REFERENCES [dbo].[Cities] ([Id])
GO

ALTER TABLE [dbo].[CityNames] CHECK CONSTRAINT [FK_CityNames_Cities]
GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[PersonAccounts]    Script Date: 21.04.2014 21:00:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PersonAccounts](
	[Id] [uniqueidentifier] NOT NULL default newid(),
	[PhoneNumber] [nvarchar](50) NULL,
	[SkypeNumber] [nvarchar](50) NULL,
 CONSTRAINT [PK_PersonAccounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[PersonsInfo]    Script Date: 04/22/2014 13:50:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PersonsInfo](
	[Id] [int] NOT NULL IDENTITY,
	[PersonId] [uniqueidentifier] NOT NULL,
	[CityId] [uniqueidentifier] NOT NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Altitude] [float] NULL,
 CONSTRAINT [PK_PersonsInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PersonsInfo]  WITH CHECK ADD  CONSTRAINT [FK_PersonsInfo_Cities] FOREIGN KEY([CityId])
REFERENCES [dbo].[Cities] ([Id])
GO

ALTER TABLE [dbo].[PersonsInfo] CHECK CONSTRAINT [FK_PersonsInfo_Cities]
GO

ALTER TABLE [dbo].[PersonsInfo]  WITH CHECK ADD  CONSTRAINT [FK_PersonsInfo_PersonAccounts] FOREIGN KEY([PersonId])
REFERENCES [dbo].[PersonAccounts] ([Id])
GO

ALTER TABLE [dbo].[PersonsInfo] CHECK CONSTRAINT [FK_PersonsInfo_PersonAccounts]
GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[PedestrianAccounts]    Script Date: 21.04.2014 21:05:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PedestrianAccounts](
	[Id] [int] NOT NULL IDENTITY,
	[PersonId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_PedestrianAccounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PedestrianAccounts]  WITH CHECK ADD  CONSTRAINT [FK_PedestrianAccounts_PersonAccounts] FOREIGN KEY([PersonId])
REFERENCES [dbo].[PersonAccounts] ([Id])
GO

ALTER TABLE [dbo].[PedestrianAccounts] CHECK CONSTRAINT [FK_PedestrianAccounts_PersonAccounts]
GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[PedestriansInfo]    Script Date: 21.04.2014 21:06:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PedestriansInfo](
	[Id] [int] NOT NULL IDENTITY,
	[PersonInfo] [int] NOT NULL,
 CONSTRAINT [PK_PedestriansInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PedestriansInfo]  WITH CHECK ADD  CONSTRAINT [FK_PedestriansInfo_PersonsInfo] FOREIGN KEY([PersonsInfo])
REFERENCES [dbo].[PersonsInfo] ([Id])
GO

ALTER TABLE [dbo].[PedestriansInfo] CHECK CONSTRAINT [FK_PedestriansInfo_PersonsInfo]
GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[DriverAccounts]    Script Date: 04/23/2014 14:31:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DriverAccounts](
	[Id] [int] NOT NULL IDENTITY,
	[PersonId] [uniqueidentifier] NOT NULL,
	[PersonName] [nvarchar](100) NULL,
	[CarColor] [nvarchar](50) NULL,
	[CarBrand] [nvarchar](50) NULL,
	[CarNumber] [nvarchar](50) NULL,
 CONSTRAINT [PK_DriverAccounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DriverAccounts]  WITH CHECK ADD  CONSTRAINT [FK_DriverAccounts_PersonAccounts] FOREIGN KEY([PersonId])
REFERENCES [dbo].[PersonAccounts] ([Id])
GO

ALTER TABLE [dbo].[DriverAccounts] CHECK CONSTRAINT [FK_DriverAccounts_PersonAccounts]
GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[DriversInfo]    Script Date: 04/23/2014 14:33:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DriversInfo](
	[Id] [int] NOT NULL IDENTITY,
	[PersonInfo] [int] NOT NULL,
 CONSTRAINT [PK_DriversInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DriversInfo]  WITH CHECK ADD  CONSTRAINT [FK_DriversInfo_PersonsInfo] FOREIGN KEY([PersonInfo])
REFERENCES [dbo].[PersonsInfo] ([Id])
GO

ALTER TABLE [dbo].[DriversInfo] CHECK CONSTRAINT [FK_DriversInfo_PersonsInfo]
GO

USE [TaxiOnline]
GO

ALTER TABLE [dbo].[PedestriansInfo]
ADD CONSTRAINT [FK_PedestriansInfo_PersonsInfo]
    FOREIGN KEY ([PersonInfo])
    REFERENCES [dbo].[PersonsInfo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

GO

/****** Object:  Table [dbo].[PedestrianRequests]    Script Date: 12.05.2014 08:06:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PedestrianRequests](
	[Id] [uniqueidentifier] NOT NULL default newid(),
	[Author] [int] NOT NULL,
	[Target] [int] NOT NULL,
	[Comment] [nvarchar](500) NULL,
 CONSTRAINT [PK_PedestrianRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [TaxiOnline]
GO

ALTER TABLE [dbo].[PedestrianRequests]  WITH CHECK ADD  CONSTRAINT [FK_PedestrianRequests_PedestriansInfo] FOREIGN KEY([Author])
REFERENCES [dbo].[PedestriansInfo] ([Id])
GO

ALTER TABLE [dbo].[PedestrianRequests] CHECK CONSTRAINT [FK_PedestrianRequests_PedestriansInfo]
GO

USE [TaxiOnline]
GO

ALTER TABLE [dbo].[PedestrianRequests]  WITH CHECK ADD  CONSTRAINT [FK_PedestrianRequests_DriversInfo] FOREIGN KEY([Target])
REFERENCES [dbo].[DriversInfo] ([Id])
GO

ALTER TABLE [dbo].[PedestrianRequests] CHECK CONSTRAINT [FK_PedestrianRequests_DriversInfo]
GO

USE [TaxiOnline]
GO

/****** Object:  Table [dbo].[DriverResponses]    Script Date: 13.05.2014 08:51:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DriverResponses](
	[Id] [uniqueidentifier] NOT NULL default newid(),
	[RequestId] [uniqueidentifier] NOT NULL,
	[IsAccepted] [bit] NULL,
 CONSTRAINT [PK_DriverResponses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DriverResponses]  WITH CHECK ADD CONSTRAINT [FK_DriverResponses_PedestrianRequests] FOREIGN KEY([RequestId])
REFERENCES [dbo].[PedestrianRequests] ([Id])
GO

ALTER TABLE [dbo].[DriverResponses] CHECK CONSTRAINT [FK_DriverResponses_PedestrianRequests]
GO

USE [TaxiOnline]
GO

/****** Object:  StoredProcedure [dbo].[AddCity]    Script Date: 05/08/2014 15:02:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[AddCity] @latitude float, @longitude float, @initzoom float, @englishName nvarchar(100) as
	declare @cityId table (Id uniqueidentifier);
	insert into Cities (InitialLatitude , InitialLongitude, InitialZoom) output inserted.Id into @cityId values (@latitude,@longitude,@initzoom);
	insert into CityNames( CityId, CultureName, Name) select id, 'en-US' as culname, @englishName as name from @cityId;

GO

USE [TaxiOnline]
GO

/****** Object:  StoredProcedure [dbo].[AddPedestrian]    Script Date: 05/08/2014 15:32:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[AddPedestrian] @phone nvarchar(50), @skype nvarchar(50), @cityEnglishName nvarchar(100) as
declare @personId table (personId uniqueidentifier);
declare @cityId uniqueidentifier;
select top 1 @cityId = Cities.Id from Cities inner join CityNames on Cities.Id = CityNames.CityId where CityNames.Name like '%' + @cityEnglishName + '%';
insert into PersonAccounts(PhoneNumber, SkypeNumber) output inserted.Id into @personId values (@phone, @skype);
declare @personInfoId table (personId int);
insert into PersonsInfo ( PersonId, CityId) output inserted.Id into @personInfoId select personId, @cityId as cityId from @personId;
declare @pedestrianId table (Id int);
insert into PedestrianAccounts (PersonId) output inserted.Id into @pedestrianId select personId from @personId;
insert into PedestriansInfo (PersonInfo) select personId from @personInfoId;

GO

USE [TaxiOnline]
GO

/****** Object:  StoredProcedure [dbo].[AddDriver]    Script Date: 05/08/2014 15:40:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[AddDriver] @phone nvarchar(50), @skype nvarchar(50), @cityEnglishName nvarchar(100), @personName nvarchar(100), @carColor nvarchar(50), @carBrand nvarchar(50), @carNumber nvarchar(50) as
declare @personId table (personId uniqueidentifier);
declare @cityId uniqueidentifier;
select top 1 @cityId = Cities.Id from Cities inner join CityNames on Cities.Id = CityNames.CityId where CityNames.Name like '%' + @cityEnglishName + '%';
insert into PersonAccounts(PhoneNumber, SkypeNumber) output inserted.Id into @personId values (@phone, @skype);
declare @personInfoId table (personId int);
insert into PersonsInfo ( PersonId, CityId) output inserted.Id into @personInfoId select personId, @cityId as cityId from @personId;
insert into DriverAccounts (PersonId, PersonName, CarColor, CarBrand, CarNumber) select personId, @personName as personName, @carColor as carColor, @carBrand as carBrand, @carNumber as carNumber from @personId;
insert into DriversInfo (PersonInfo) select personId from @personInfoId;

GO

USE [TaxiOnline]
GO

/****** Object:  StoredProcedure [dbo].[AddAcceptedRequest]    Script Date: 05/14/2014 09:31:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[AddAcceptedRequest]  @pedestrianPhone nvarchar(50), @driverName nvarchar(100), @comment nvarchar(500) as
declare @pedestrianId int;
select top 1 @pedestrianId = PedestriansInfo.Id from PedestriansInfo inner join PersonsInfo on PedestriansInfo.PersonInfo = PersonsInfo.Id inner join PersonAccounts on PersonsInfo.PersonId = PersonAccounts.Id where PersonAccounts.PhoneNumber like '%'+@pedestrianPhone+'%';
declare @driverId int;
select top 1 @driverId = DriversInfo.Id from DriversInfo inner join PersonsInfo on DriversInfo.PersonInfo = PersonsInfo.Id inner join PersonAccounts on PersonsInfo.PersonId = PersonAccounts.Id inner join DriverAccounts on PersonAccounts.Id = DriverAccounts.PersonId where DriverAccounts.PersonName like '%'+@driverName+'%';
declare @requestId table (Id uniqueidentifier);
insert into PedestrianRequests (Author, [Target], Comment) output inserted.Id into @requestId values (@pedestrianId, @driverId, @comment)
insert into DriverResponses (RequestId, IsAccepted) select Id, 1 as accepted from @requestId
GO


