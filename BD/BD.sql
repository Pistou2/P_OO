#------------------------------------------------------------#
#ETML
#Author  : Merk Yann
#Date    : 18.01.2017
#Summary : P_OO, database creation script
#------------------------------------------------------------#

DROP DATABASE IF EXISTS db_oo;
CREATE DATABASE db_oo;

USE db_oo;


#------------------------------------------------------------
# Table: t_files
#------------------------------------------------------------

CREATE TABLE t_files(
        idFile              int (11) Auto_increment  NOT NULL ,
        filName             Varchar (100) NOT NULL ,
        filCreationDate     Date NOT NULL ,
        filModificationDate Date ,
        filAuthor           Varchar (40) ,
        filTextContent      Varchar (10000) ,
        filStillExist       Boolean NOT NULL ,
        idFolder            Int NOT NULL ,
        PRIMARY KEY (idFile )
)ENGINE=InnoDB;


#------------------------------------------------------------
# Table: t_folder
#------------------------------------------------------------

CREATE TABLE t_folder(
        idFolder      int (11) Auto_increment  NOT NULL ,
        folName       Varchar (50) NOT NULL ,
        folStillExist Boolean NOT NULL ,
        idParent    Int ,
        PRIMARY KEY (idFolder )
)ENGINE=InnoDB;

ALTER TABLE t_files ADD CONSTRAINT FK_t_files_idFolder FOREIGN KEY (idFolder) REFERENCES t_folder(idFolder);
ALTER TABLE t_folder ADD CONSTRAINT FK_t_folder_idParent FOREIGN KEY (idParent) REFERENCES t_folder(idFolder);
