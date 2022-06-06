CREATE TABLE dbo.WorkItemRequestLog (
    Id int identity(1,1) primary key,
    CreatorId varchar(255) not null,
    WorkItemId int not null,
    WorkItemTitle varchar(255) not null,
    WorkItemUrl varchar(255) not null,
    RequestTimeStamp datetime2 not null
)