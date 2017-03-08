DROP TABLE IF EXISTS t_files;
DROP TABLE IF EXISTS t_folder;
CREATE TABLE t_files(
		idFile				 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
		filName				 Varchar (100) NOT NULL ,
		filExtension		 Varchar (30) NOT NULL,
		filCreationDate		 Date NOT NULL ,
		filModificationDate	 Date,
		filAuthor			 Varchar (40) ,
		filSize				 INTEGER,
		filTextContent		 Varchar (100000) ,
		idFolder			 Int NOT NULL ,
		FOREIGN KEY (idFolder) REFERENCES t_folder(idFolder)
);


CREATE TABLE t_folder(
		idFolder	 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
		folName		 Varchar (50) NOT NULL ,
		folType		 int(11) NOT NULL,
		idParent	 Int 
);
