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
        /// <summary>
        /// All carracters that are Not Letters, Numbers, Punction, Currency Symbol, Separators and tabs/spaces/cartridge return
        /// </summary>
        private readonly Regex UNAUTHORISED_CHAR_IN_FILES_REGEX = new Regex(@"[^\p{L}\p{N}\p{P}\p{Sc}\p{Z}\s<>]+", RegexOptions.IgnoreCase);

        /// <summary>
        /// List the extension of the files that need to not be readed
        /// This list was obtained by simply executing the program on my SSD, and check every file that take more than 0.025 seconds to read
        /// </summary>
        /// \todo
        private readonly Regex DO_NOT_READ_TEXT_EXTENSIONS = new Regex(@"^png|exe|dll|jpg|bmp|cache|jar|myd|myi|frm|so|gif|java|pyd|pyc|pyo|gbr|pgm|pat|mo|pdf|tfm|pfb|ps|bz2|tpm|xdy|wav|xlsx", RegexOptions.IgnoreCase);

        /// <summary>
        /// in Octets
        /// </summary>
        private long maxReadableFileSize = 10240;

        ///\todo only debug thing
        private const double TEMP_MAX_ALLOWED_TIME = 0.0025;

        static SQLiteConnection sqlConnection;

        // used for the singleton       
        private static DB dbInstance;

        /// <summary>
        /// Get the single instances
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
        public string addDirectoryFolder(string _folderName, string _idParent)
        {
            string folName = _folderName;
            string folType = Convert.ToString((int)FolderType.Directory);

            //get the id of the folder inputed, and return it
            return Convert.ToString(query(String.Format("INSERT INTO t_folder (folName,folType,idParent) VALUES (\"{0}\", {1}, {2});\nSELECT last_insert_rowid();", folName, folType, _idParent)).Rows[0].ItemArray[0]);

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
        /// Add a file in the file table
        /// </summary>
        /// <param name="_filePath"></param>
        public void addDirectoryFile(string _filePath, string _folderPath, string _idFolder, bool _readFileContent = false)
        {

            string filName = "", filExtension = "";
            string filCreationDate, filModificationDate;
            string filAuthor = "", filTextContent;


            //get the file name and extension
            filName = new FileInfo(_filePath).Name;
            filExtension = new FileInfo(_filePath).Extension;

            //if the extension isn't empty
            if (filExtension != "")
            {
                //remove the extension in the name
                filName = filName.Substring(0, filName.Length - filExtension.Length);

                //remove the . in the extension            
                filExtension = filExtension.Substring(1, filExtension.Length - 1);
            }

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

            //get the text content only if required, and the extension isn't in the blacklist, AND the file size
            if (_readFileContent && !DO_NOT_READ_TEXT_EXTENSIONS.IsMatch(filExtension) && new FileInfo(_filePath).Length <= maxReadableFileSize)
            {

                //Todo remove that debug line
                //register the time before starting reading
                DateTime temp = DateTime.Now;

                try
                {


                    filTextContent = File.ReadAllText(_filePath);

                    //Remove all the Special chars of the file
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

                //TODO
                //output it if this is bigger than a certain amount
                TimeSpan timespan = DateTime.Now - temp;


                if (timespan.TotalSeconds > TEMP_MAX_ALLOWED_TIME)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(String.Format(".{0} : {1} secs", filExtension, timespan.TotalSeconds));
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ReadLine();

                }
            }
            else
            {
                filTextContent = "";
            }


            //send all those datas in the t_file table
            query(String.Format("INSERT INTO t_files (filName, filExtension, filCreationDate, filModificationDate, filAuthor, filTextContent, idFolder) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", {6})", filName, filExtension, filCreationDate, filModificationDate, filAuthor, filTextContent, _idFolder));
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