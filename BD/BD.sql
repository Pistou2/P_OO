DROP TABLE IF EXISTS t_files;
DROP TABLE IF EXISTS t_folder;
CREATE TABLE t_files(
        idFile              int (11) NOT NULL PRIMARY KEY ,
        filName             Varchar (100) NOT NULL ,
		filExtension		Varchar (30) NOT NULL,
        filCreationDate     Date NOT NULL ,
        filModificationDate Date ,
        filAuthor           Varchar (40) ,
        filTextContent      Varchar (10000) ,
        filStillExist       Boolean NOT NULL ,
        idFolder            Int NOT NULL ,
		FOREIGN KEY (idFolder) REFERENCES t_folder(idFolder)
);


CREATE TABLE t_folder(
        idFolder      int (11) NOT NULL PRIMARY KEY ,
        folName       Varchar (50) NOT NULL ,
        folStillExist Boolean NOT NULL ,
		folType		int(11) NOT NULL,
        idParent    Int ,
		FOREIGN KEY (idParent) REFERENCES t_folder(idFolder)
);
