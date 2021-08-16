select * from AEMENERSOL..Platforms
select * from AEMENERSOL..Well

SET IDENTITY_INSERT AEMENERSOL..Platforms ON
SET IDENTITY_INSERT AEMENERSOL..Well ON

truncate table AEMENERSOL..Platforms
truncate table AEMENERSOL..Well

ALTER TABLE AEMENERSOL..Well
ADD FOREIGN KEY (platformId) REFERENCES Platforms(id);

DROP TABLE Well
DROP TABLE Platforms

CREATE TABLE Platforms
(
id INT primary key ,
uniqueName VARCHAR(200),
latitude float,
longitude float,
createdAt datetime,
updatedAt datetime
)

CREATE TABLE Well
(
id INT primary key,
platformId int,
uniqueName VARCHAR(200),
latitude float,
longitude float,
createdAt datetime,
updatedAt datetime
)

select Platforms.uniqueName as PlatformName,Well.id,platformId,Well.uniqueName,well.latitude,well.longitude,well.createdAt,well.updatedAt 
from Well INNER JOIN Platforms on Well.platformId=Platforms.id WHERE Well.id IN (1,5,8,10,14)

