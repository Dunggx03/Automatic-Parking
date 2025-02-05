create database PARKING
use PARKING
go
create table CardUser
(
	ID int primary key identity(1,1),
	UName varchar(200) NOT NULL,
	Email varchar(200) NULL,
	Pass varchar(200) NOT NULL,
	Access varchar(100) NOT NULL
);

create table Card
(
	ID varchar(100) primary key,
	Vehicle varchar(100) NOT NULL,
	CheckIn time NULL,
	CheckOut time NULL,
	Money bigint NULL,
	UID int,
	foreign key (UID) references CardUser (ID)
);

set identity_insert [CardUser] ON;

insert into CardUser (ID, UName, Email, Pass, Access)
values (1, N'Admin', NULL, N'123', N'admin');

ALTER TABLE [PARKING].[dbo].[Card]
DROP COLUMN CheckIn;

ALTER TABLE [PARKING].[dbo].[Card]
DROP COLUMN CheckOut;

	CREATE TABLE [PARKING].[dbo].[CardLog] (
		LogID INT IDENTITY(1,1) PRIMARY KEY,
		CardID VARCHAR(100) NOT NULL,
		CheckIn DATETIME,
		CheckOut DATETIME,
		LogDate DATE NOT NULL DEFAULT GETDATE(),
		foreign key (CardID) references Card (ID)
	);
