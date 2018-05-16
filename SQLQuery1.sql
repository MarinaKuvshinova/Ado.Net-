create database LandscapeDesign
use LandscapeDesign

create table PlantSpecies
(
	IDPlantSpecies int not null primary key identity,
	PlantSpecies nvarchar(20) not null
)
insert into PlantSpecies
values 
	('������� ������'),
	('�������� ������'),
	('������� ���������')
create table Plants
(
	IDPlant int not null primary key identity,
	PlantName nvarchar(50) not null,
	IDPlantSpecies int not null foreign key references PlantSpecies(IDPlantSpecies) ON DELETE CASCADE,
	PlantInfo nvarchar(100) not null,
	WidthPlant float,
	HeightPlant float,
	PicturePlant nvarchar(50) not null,
	PicturePlantTop nvarchar(50) not null
)
insert into Plants
values 
	('������',2,'������������, ������� � ������ (2����).', 30, 30, '������Front.jpg','������Top.png'),
	('���',1,'������� � ������ (2����).', 40, 40, '���Front.jpg','���Top.png'),
	('���',3,'������������� ����', 15, 15, '���Front.jpg','���Top.png')

create table Structures
(
	IDStructure int not null primary key identity,
	StructureName nvarchar(20) not null,
	StructurePicture nvarchar(50) not null,
	StructureTop nvarchar(50) not null
)
insert into Structures
values 
	('���', '���Front.jpg','���Top.jpg'),
	('�����', '�����Front.jpg','�����Top.jpg'),
	('�������', '�������Front.jpg','�������Top.jpg'),
	('�������', '�������Front.jpg','�������Top.png')

create table Projects
(
	IDProject int not null primary key identity,
	Project nvarchar(20) not null,
	Width float not null,
	Height float not null,
	Scale float not null
)

create table ProjectStructure
(
	IDProjectStructure int not null  primary key identity,
	IDProject int not null foreign key references Projects(IDProject) ON DELETE CASCADE,
	IDStructure int not null foreign key references Structures(IDStructure) ON DELETE CASCADE,
	WidthStructure float not null,
	HeightStructure float not null,
	PointX float not null,
	PointY float not null
)

create table ProjectPlants
(
	IDProjectPlants int not null  primary key identity,
	IDProject int not null foreign key references Projects(IDProject) ON DELETE CASCADE,
	IDPlant int not null foreign key references Plants(IDPlant) ON DELETE CASCADE,
	PointX float not null,
	PointY float not null
)

drop table Structures









