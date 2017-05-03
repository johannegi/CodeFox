/*INSERT INTO Files(Name, Type, Location, DateCreated, DateModified, FolderStructure_ID)
VALUES ('Readme', 'txt', '', '20120618 10:34:09 AM', '20120618 10:34:09 AM', NULL)*/

/*INSERT INTO Projects(Name, Type, DateCreated, DateModified, Owner_ID, ReadMe_ID)
VALUES ('test project', 'txt', '20120618 10:34:09 AM', '20120618 10:34:09 AM', 2, 1)*/

INSERT INTO ProjectShares(ShareUser_ID, ShareProject_ID)
VALUES (6,1)

INSERT INTO ProjectOwners(Owner_ID, OwnerProject_ID)
VALUES (6,1)