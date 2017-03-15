/** ETML
 * \file      DB.cs   
 * \author    Merk Yann
 * \brief     Contains the class used to connect and query the SQLite Database
 */
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace P_OO_merkya_SmartThesaurus
{
    /// <summary>
    /// Database class, handling the connection and the querries
    /// Singleton
    /// </summary>
    public class DB
    {

        static SQLiteConnection sqlConnection;

        // used for the singleton       
        private static DB dbInstance;

        /// <summary>
        /// Get the single instance
        /// </summary>
        /// <param name="_dbPath">The path were the .db file must be registered</param>
        /// <returns></returns>
        public static DB getInstance(string _dbPath = null)
        {
            if (dbInstance == null && _dbPath != null)
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
        public string addFolder(string _folderName, string _idParent, FileOrigin _origin)
        {
            string folName = _folderName;
            string folType = Convert.ToString((int)_origin);

            //get the id of the folder inputed, and return it
            return Convert.ToString(query(String.Format("INSERT INTO t_folder (folName,folType,idParent) VALUES (\"{0}\", {1}, {2});\nSELECT last_insert_rowid();", folName, folType, _idParent)).Rows[0].ItemArray[0]);

        }

        /// <summary>
        /// Add a file to the database's file table
        /// </summary>
        /// <param name="_filName">The name of the file</param>
        /// <param name="_filExtension">his extension (without the .)</param>
        /// <param name="_filCreationDate">His creation date, in SQL format</param>
        /// <param name="_filModificationDate">His last modification date, in SQL format</param>
        /// <param name="_filAuthor">the file's author. With "" around the text, or just NULL</param>
        /// <param name="_filSize">Size of the file, in octet</param>
        /// <param name="_filTextContent">the text content. With "" around the text, or just NULL</param>
        /// <param name="_idFolder">the id of the parent folder</param>
        public void addFile(string _filName, string _filExtension, string _filCreationDate, string _filModificationDate, string _filAuthor, string _filSize, string _filTextContent, string _idFolder)
        {
            query(String.Format("INSERT INTO t_files (filName, filExtension, filCreationDate, filModificationDate, filAuthor, filSize, filTextContent, idFolder) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", {4}, {5}, {6}, {7})", _filName, _filExtension, _filCreationDate, _filModificationDate, _filAuthor, _filSize, _filTextContent, _idFolder));
        }

        /// <summary>
        /// Get a datatable containing all infos from all the files contained in a folder
        /// </summary>
        /// <param name="_folderID">The ID of the folder</param>
        /// <returns>A DataTable oredered by file then file property</returns>
        public DataTable getFiles(string _folderID)
        {
            return query(String.Format("SELECT * FROM t_files WHERE idFolder = {0}", _folderID));
        }

        /// <summary>
        /// Get a datatable containing all infos from all the folders contained in a folder
        /// </summary>
        /// <param name="_parentFolderID">The ID of the parent folder, by default get the folders at root</param>
        /// <returns>A DataTable oredered by file then file property</returns>
        public DataTable getFolders(string _parentFolderID = "0")
        {
            return query(String.Format("SELECT * FROM t_folder WHERE idParent = {0}", _parentFolderID));
        }

        /// <summary>
        /// Get the name of a folder
        /// </summary>
        /// <param name="_idFolder">Folder's ID</param>
        /// <returns>The name of the folder</returns>
        public string getFolderName(string _idFolder)
        {
            return Convert.ToString(query(String.Format("SELECT folName FROM t_folder WHERE idFolder = {0}", _idFolder)).Rows[0].ItemArray[0]);
        }

        /// <summary>
        /// 
        /// </summary>
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