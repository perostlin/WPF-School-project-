create table Color
(
	ID int identity(1,1) not null primary key,
	Color nvarchar(50)
);

GO

create table VehicleType
(
	ID int identity(1,1) not null primary key,
	[Type] nvarchar(50)
);

GO

create table ModelYear
(
	ID int identity(1,1) not null primary key,
	[Year] int
);

GO

create table FuelType
(
	ID int identity(1,1) not null primary key,
	[Type] nvarchar(50)
);

GO

create table Vehicle
(
	ID uniqueidentifier not null primary key,
	RegNo varchar(6) not null unique,
	OriginalMileage int not null,
	Desription nvarchar(75) null,

	ColorID int foreign key references Color(ID) null,
	VehicleTypeID int foreign key references VehicleType(ID) not null,
	ModelYearID int foreign key references ModelYear(ID) null,
	FuelTypeID int foreign key references FuelType(ID) not null,
);

GO

create table [User]
(
	ID uniqueidentifier primary key not null,
	Username nvarchar(50) not null,
	[Password] nvarchar(250) not null,
	Salt nvarchar(50) not null,
	IsAdmin bit not null
);

GO

create table ReportDriverJournal
(
	ID uniqueidentifier primary key not null,
	[Date] datetime not null,
	Milage int not null,
	FuelAmount decimal not null,
	PricePerUnit decimal not null,
	TotalPrice decimal not null,
	ChauffeurID uniqueidentifier foreign key references [User](ID) not null,
	VehicleID uniqueidentifier foreign key references Vehicle(ID) not null,
	FuelTypeID int foreign key references FuelType(ID) not null,
);

GO

create table TypeOfCost
(
	ID int identity(1,1) primary key not null,
	[Type] nvarchar(75) not null,
);

GO

create table RefuelingDriverJournal
(
	ID uniqueidentifier primary key not null,
	[Date] datetime not null,
	Cost decimal not null,
	Comment nvarchar(50) null,
	VehicleID uniqueidentifier foreign key references Vehicle(ID) not null,
	TypeOfCostID int foreign key references TypeOfCost(ID) not null
);

GO

insert into [User](ID, Username, [Password], Salt, IsAdmin)
values(NEWID(),'administrator', 'zyBRhfhd3rnSCWV6qo1RiYnXOfMNgpZBD4Yy4L3k45w=', 'Wv2npLfBv6x7bL9kq4N83w==', 1);

GO

insert into VehicleType([Type]) values('Sedan');
insert into VehicleType([Type]) values('Kombi');
insert into VehicleType([Type]) values('Stretch limo');
insert into VehicleType([Type]) values('Minibuss');

GO

insert into FuelType([Type]) values('El');
insert into FuelType([Type]) values('Bensin');
insert into FuelType([Type]) values('Diesel');
insert into FuelType([Type]) values('Hybrid Bensin/El');
insert into FuelType([Type]) values('Hybrid Etanol/Bensin');
insert into FuelType([Type]) values('Bio- och Naturgas');
insert into FuelType([Type]) values('Etanol');
insert into FuelType([Type]) values('Vätgas');

GO

insert into ModelYear([Year]) values(1990);
insert into ModelYear([Year]) values(1991);
insert into ModelYear([Year]) values(1992);
insert into ModelYear([Year]) values(1993);
insert into ModelYear([Year]) values(1994);
insert into ModelYear([Year]) values(1995);
insert into ModelYear([Year]) values(1996);
insert into ModelYear([Year]) values(1997);
insert into ModelYear([Year]) values(1998);
insert into ModelYear([Year]) values(1999);
insert into ModelYear([Year]) values(2000);
insert into ModelYear([Year]) values(2001);
insert into ModelYear([Year]) values(2002);
insert into ModelYear([Year]) values(2003);
insert into ModelYear([Year]) values(2004);
insert into ModelYear([Year]) values(2005);
insert into ModelYear([Year]) values(2006);
insert into ModelYear([Year]) values(2007);
insert into ModelYear([Year]) values(2008);
insert into ModelYear([Year]) values(2009);
insert into ModelYear([Year]) values(2010);
insert into ModelYear([Year]) values(2011);
insert into ModelYear([Year]) values(2012);
insert into ModelYear([Year]) values(2013);
insert into ModelYear([Year]) values(2014);
insert into ModelYear([Year]) values(2015);
insert into ModelYear([Year]) values(2016);

GO

insert into TypeOfCost([Type]) values('Däck');
insert into TypeOfCost([Type]) values('Förbrukningsvara');
insert into TypeOfCost([Type]) values('Reparation');
insert into TypeOfCost([Type]) values('Försäkring');
insert into TypeOfCost([Type]) values('Fordonsskatt');
insert into TypeOfCost([Type]) values('Besiktning');

GO

insert into Color values ('Röd');
insert into Color values ('Svart');
insert into Color values ('Brandgul');
insert into Color values ('Grön');
insert into Color values ('Gul');
insert into Color values ('Ljusbrun');
insert into Color values ('Silver');
insert into Color values ('Blå');
insert into Color values ('Ljusgul');
insert into Color values ('Azurblå');
insert into Color values ('Mörkgrön');
insert into Color values ('Ragusa');
insert into Color values ('Ljusröd');
insert into Color values ('Flerfärgad');
insert into Color values ('Vinröd');
insert into Color values ('Vit');