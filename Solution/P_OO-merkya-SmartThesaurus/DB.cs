/**
 * \file      DB.cs   
 * \author    Merk Yann
 * \version   1.0
 * \date      25.01.2017
 * \brief     Class used to connect and query the SQLite Database
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace P_OO_merkya_SmartThesaurus
{
    ///<summary>Database class, handling the connection and the querries</summary>
    ///\date 25.01.2017
    public class DB
    {
        private readonly Regex UNAUTHORISED_CHAR_IN_FILES_REGEX = new Regex(@"[^\p{L}\p{N}\p{P}\p{Sc}\p{Z}]+");
        

        static SQLiteConnection sqlConnection;

        // used for the singleton       
        private static DB dbInstance;

        /// <summary>
        /// Get the single instances
        /// </summary>
        /// <param name="_dbPath">The path were the .db file must be registered</param>
        /// <returns></returns>
        public static DB getInstance(string _dbPath)
        {
            if (dbInstance == null)
            {
                dbInstance = new DB(_dbPath);
            }

            return dbInstance;
        }

        /// <summary>
        /// Constructor
        /// Initialize the Database, and connect it to the given file
        /// </summary>
        /// <param name="_filePath">.db file path</param>
        private DB(string _filePath)
        {
            try
            {
                sqlConnection = new SQLiteConnection("Data Source = " + _filePath);
                //initialise the db
                query(System.IO.File.ReadAllText("Resources/BD.sql"));
            }

            catch
            {
                throw new Exception("Could not connect to the DB");
            }
        }
        /// <summary>
        /// Send a Query to the Database, and return the result
        /// 
        /// Code HUGELY inspired from a stackoverflow Answer
        /// </summary>
        /// <param name="_request">SQL query</param>
        /// <returns>DataTable, containing the result</returns>
        /// \see <http://stackoverflow.com/a/1253002>
        private DataTable query(string _request)
        {
            SQLiteDataAdapter ad;
            DataTable table = new DataTable();

            try
            {
                SQLiteCommand cmd;
                sqlConnection.Open();  //Initiate connection to the db
                cmd = sqlConnection.CreateCommand();
                cmd.CommandText = _request;  //set the passed query
                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(table); //fill the datasource
            }

            catch (SQLiteException e)
            {
                Debug.Print("SQL Error");
            }

            sqlConnection.Close();
            return table;
        }

        /// <summary>
        /// Add a folder in the folder table
        /// </summary>
        /// \todo comments
        /// <param name="_folderName"></param>
        /// <returns>The id of the folder</returns>
        public string addDirectoryFolder(string _folderName, string _idParent)
        {
            string folName = _folderName;
            ///\todo
            string folStillExist = "1";
            string folType = Convert.ToString((int)FolderType.Directory);

            //get the id of the folder inputed, and return it
            return Convert.ToString(query(String.Format("INSERT INTO t_folder (folName,folStillExist,folType,idParent) VALUES (\"{0}\", {1}, {2}, {3});\nSELECT last_insert_rowid();", folName, folStillExist, folType, _idParent)).Rows[0].ItemArray[0]);

        }

        /// <summary>
        /// Add a file in the file table
        /// </summary>
        /// <param name="_filePath"></param>
        public void addDirectoryFile(string _filePath, string _folderPath, string _idFolder)
        {

            string filName = "", filExtension = "";
            string filCreationDate, filModificationDate;
            string filAuthor = "", filTextContent;
            ///\todo
            string filStillExist = "1";

            //get the name and the extension of the file
            string[] fileSplit = _filePath.Substring(_folderPath.Length + 1 /*+1 for the / */).Split(new char[] { '.' });

            //get the file name and extension
            #region check if there's a point to mark an extension
            if (fileSplit.Length >= 1)
            {
                #region check if there's really an extension after the point
                if (fileSplit.Last() != "")
                {
                    //recombine the entire file name
                    for (int i = 0; i < fileSplit.Length - 1; i++)
                    {
                        //put an dot after all except just before the extension
                        if (i < fileSplit.Length - 2)
                        {
                            filName += fileSplit[i] + ".";
                        }
                        else
                        {
                            filName += fileSplit[i];
                        }
                    }
                    filExtension = fileSplit.Last();
                }
                else
                {
                    //combine the entire file name
                    for (int i = 0; i < fileSplit.Length; i++)
                    {
                        filName += fileSplit[i] + ".";
                    }
                    //set the extension to empty
                    filExtension = "";
                }
                #endregion
            }
            else
            {
                filName = fileSplit[0];
                filExtension = "";
            }
            #endregion

            //get the dates, convert to a SQL compatible format
            filCreationDate = File.GetCreationTime(_filePath).ToString("yyyy-MM-dd");
            filModificationDate = File.GetLastWriteTime(_filePath).ToString("yyyy-MM-dd");

            //get the author
            //http://stackoverflow.com/a/7445261
            try
            {
                filAuthor = File.GetAccessControl(_filePath).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            catch
            {
                Debug.Print("Error at finding file's author : " + _filePath);
            }

            //get the text content
            try
            {
                filTextContent = File.ReadAllText(_filePath);

                //Replace all the Special chars of the file by Spaces
                filTextContent = UNAUTHORISED_CHAR_IN_FILES_REGEX.Replace(filTextContent, "");

                //escape the chars that needs it
                //filTextContent = (new Regex(@"[\\]")).Replace(filTextContent, "\\\\");
                filTextContent = (new Regex("[\"]")).Replace(filTextContent, "\"\"");
            }
            catch (Exception e)
            {
                filTextContent = null;
                Debug.Print("Error at reading file's content : " + _filePath);
            }


            //send all those datas in the t_file table
            query(String.Format("INSERT INTO t_files (filName, filExtension, filCreationDate, filModificationDate, filAuthor, filTextContent, filStillExist, idFolder) VALUES (\"{0}\", \"{1}\", {2}, {3}, \"{4}\", \"{5}\", {6}, {7})", filName, filExtension, filCreationDate, filModificationDate, filAuthor, filTextContent, filStillExist, _idFolder));
        }

        /// <summary>
        /// 
        /// </summary>
        /// \todo check if does really work
        /// <param name="_path"></param>
        /// <param name="_idParrentFolder"></param>
        private string findParentFolder(string _path, string _idParrentFolder = "0")
        {
            //if the path is empty, that means the directory is at root
            if (_path == "")
            {
                return "0";
            }
            else
            {
                string[] splitedPath = _path.Split(new char[] { '/', '\\' }, 2);
                string folderName = splitedPath[0];

                //Get the searched folder id

                string idCurrentFolder = Convert.ToString(query(String.Format("SELECT idFolder FROM t_folder WHERE idParent = {0} AND folName = \"{1}\"", _idParrentFolder, folderName)).Rows[0].ItemArray[0]);

                ///\todo 
                //if there's still folder under it, continue recursively
                if (splitedPath.Length > 1 && splitedPath[1] != "")
                {
                    return findParentFolder(splitedPath[1], idCurrentFolder);
                }
                //else simply return the current id
                else
                {
                    return idCurrentFolder;
                }
            }
        }
    }
}