use db0
go

create table pictures
(
	id int identity primary key,
	[name] nvarchar(150) not null,
	picture image not null
);

select * from pictures