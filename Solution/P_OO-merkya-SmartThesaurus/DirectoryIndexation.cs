/**
 * \file      DirectoryIndexation.cs   
 * \author    Merk Yann
 * \version   1.0
 * \date      08.03.2017
 * \brief     contains the Class used to index local files
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace P_OO_merkya_SmartThesaurus
{
    /// \todo Include that into a factory
    public class DirectoryIndexation : IIndexation
    {
        /// <summary>
        /// All carracters that are Not Letters, Numbers, Punction, Currency Symbol, Separators and tabs/spaces/cartridge return < >
        /// </summary>
        private readonly Regex UNAUTHORISED_CHAR_IN_FILES_REGEX = new Regex(@"[^\p{L}\p{N}\p{P}\p{Sc}\p{Z}\s<>]+", RegexOptions.IgnoreCase);

        /// <summary>
        /// To run with the file regex,  contains an ALT+160 char that can causes problem
        /// </summary>
        private readonly Regex UNAUTHORISED_CHAR_IN_FOLDER_REGEX = new Regex(@"[ ]+");

        /// <summary>
        /// List the extension of the files that need to not be readed
        /// This list was obtained by simply executing the program on my SSD, and check every file that take more than 0.025 seconds to read
        /// </summary>
        /// \todo
        private readonly Regex DO_NOT_READ_TEXT_EXTENSIONS = new Regex(@"^(png|exe|dll|jpg|bmp|cache|jar|myd|myi|frm|so|ico|gif|md3|ogg|java|pyd|pyc|pyo|gbr|pgm|pat|mo|pdf|tfm|pfb|ps|bz2|tpm|xdy|wav|xlsx)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// in Octets
        /// </summary>
        private long maxReadableFileSize = 10240;


        /// <summary>
        /// Create all the folders to get from the origin to the final path
        /// </summary>
        /// <param name="_finalPath">The target path</param>
        /// <param name="_database">The database, used to input the folders</param>
        /// <param name="_previousFolderID">the final path of the previous recursive iteration, leave it blank</param>
        /// <returns>The id of the last folder</returns>
        private string createRouteToPath(string _finalPath, string _previousFolderID = "0")
        {
            DB database = DB.getInstance();

            //Split the first ellement of the path, and the rest
            string[] splitedPath = _finalPath.Split(new char[] { '/', '\\' }, 2);

            //Add the folder to the database
            string idFolder = database.addFolder(splitedPath[0], _previousFolderID, FileOrigin.Directory);

            if (splitedPath.Length > 1)
            {
                return createRouteToPath(splitedPath[1], idFolder);
            }
            else
            {
                return idFolder;
            }
        }


        /// <summary>
        /// Start the indexation        
        /// </summary>
        /// <param name="_path">Path to index</param>
        /// <param name="_type">Type of exploration, by default explore all the files and their content</param>
        public void index(string _path, FileExplorationType _type = FileExplorationType.Full_And_File_Contents)
        {
            recursiveExploration(_path, _type);
        }

        /// <summary>
        /// Used when entering a new directory
        /// </summary>
        /// \todo Protect the non-first iterations + comments
        private void recursiveExploration(string _realPath, FileExplorationType _type = FileExplorationType.Full_And_File_Contents, string _parentID = "0", bool _firstIteration = true)
        {
            DB database = DB.getInstance();
            if (_firstIteration)
            {
                //check if the path is reachable
                if (Directory.Exists(_realPath))
                {
                    //make sure all the folders are set up to this point
                    _parentID = createRouteToPath(_realPath);
                }
                else
                {
                    throw new DriveNotFoundException();
                }
            }

            string[] folderPaths = _realPath.Split(new char[] { '/', '\\' });

            string folderName = folderPaths[folderPaths.Length - 1];

            //check that the folder name is correct. if not, do not do further operations in it to avoid weird things caused by the Directory.GetDirectories()
            if (!UNAUTHORISED_CHAR_IN_FILES_REGEX.IsMatch(folderName) && !UNAUTHORISED_CHAR_IN_FOLDER_REGEX.IsMatch(folderName))
            {
                //Add the folder to the DB

                string folderID;

                if (!_firstIteration)
                {
                    //don't add it to the first iteration
                    folderID = database.addFolder(folderName, _parentID, FileOrigin.Directory);
                }
                else
                {
                    //else take the id of the parent
                    folderID = _parentID;
                }

                //Recursively call all the child directories
                try
                {
                    foreach (string childPath in Directory.GetDirectories(_realPath))
                    {
                        recursiveExploration(childPath, _type, folderID, false);
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    Debug.Print("Folder Error");
                }

                //Call all the files to be added
                foreach (string filePath in Directory.GetFiles(_realPath))
                {
                    //Check if we need to read the file content or no
                    if (_type == FileExplorationType.Full_And_File_Contents)
                    {
                        addFile(filePath, _realPath, folderID, true);
                    }
                    else
                    {
                        addFile(filePath, _realPath, folderID, false);
                    }
                }
            }
            else
            {
                Debug.WriteLine("Error : Folder with name not supported : " + _realPath);
            }

        }

        /// <summary>
        /// Add a file in the Database file table by calling the addFile
        /// </summary>
        /// <param name="_filePath"></param>
        private void addFile(string _filePath, string _folderPath, string _idFolder, bool _readFileContent = false)
        {

            string filName, filExtension;
            string filCreationDate, filModificationDate;
            string filAuthor, filTextContent;
            string filSize;


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


            //if the file has a valid name to use the FileInfo commands
            if (File.Exists(_filePath))
            {
                //get the author of the file
                try
                {
                    //http://stackoverflow.com/a/7445261
                    filAuthor = '"' + File.GetAccessControl(_filePath).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString() + '"';
                }
                catch
                {
                    filAuthor = "NULL";
                    Debug.Print("Error at finding file's author : " + _filePath);
                }

                //get the file size
                filSize = Convert.ToString(new FileInfo(_filePath).Length);

                //get the text content only if required, and the extension isn't in the blacklist, AND the file size
                if (_readFileContent && !DO_NOT_READ_TEXT_EXTENSIONS.IsMatch(filExtension) && new FileInfo(_filePath).Length <= maxReadableFileSize)
                {
                    try
                    {
                        filTextContent = File.ReadAllText(_filePath);

                        //Remove all the Special chars of the file
                        filTextContent = UNAUTHORISED_CHAR_IN_FILES_REGEX.Replace(filTextContent, "");

                        //escape the chars that needs it
                        //filTextContent = (new Regex(@"[\\]")).Replace(filTextContent, "\\\\");
                        filTextContent = (new Regex("[\"]")).Replace(filTextContent, "\"\"");
                        filTextContent = '"' + filTextContent + '"';
                    }
                    catch (Exception e)
                    {
                        filTextContent = "NULL";
                        Debug.Print("Error at reading file's content : " + _filePath);
                    }
                }
                else
                {
                    filTextContent = "NULL";
                }
            }
            else
            {
                filAuthor = "NULL";
                filSize = "NULL";
                filTextContent = "NULL";
            }


            //send all those datas in the database
            DB.getInstance().addFile(filName, filExtension, filCreationDate, filModificationDate, filAuthor, filSize, filTextContent, _idFolder);

        }
    }
}