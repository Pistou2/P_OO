using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace P_OO_merkya_SmartThesaurus
{
    /// \todo Include that into a factory
    public static class DirectoryIndexation
    {

        /// <summary>
        /// Create all the folders to get from the origin to the final path
        /// </summary>
        /// <param name="_finalPath">The target path</param>
        /// <param name="_database">The database, used to input the folders</param>
        /// <param name="_previousFolderID">the final path of the previous recursive iteration, leave it blank</param>
        /// <returns>The id of the last folder</returns>
        public static string createRouteToPath(string _finalPath, string _previousFolderID = "0")
        {
            DB database = DB.getInstance();

            //Split the first ellement of the path, and the rest
            string[] splitedPath = _finalPath.Split(new char[] { '/', '\\' }, 2);

            //Add the folder to the database
            string idFolder = database.addDirectoryFolder(splitedPath[0], _previousFolderID);

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
        /// Used when entering a new directory
        /// </summary>
        /// <param name="_realPath"></param>
        /// <param name="_database"></param>
        /// \todo Protect the non-first iterations
        public static void exploration(string _realPath, FileExplorationType _type = FileExplorationType.Full_And_File_Contents, string _parentID = "0", bool _firstIteration = true)
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

            //Add the folder to the DB

            string folderID;
            if (!_firstIteration)
            {
                //don't add it to the first iteration
                folderID = database.addDirectoryFolder(folderName, _parentID);
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
                    exploration(childPath, _type, folderID, false);
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
                    database.addDirectoryFile(filePath, _realPath, folderID, true);
                }
                else
                {
                    database.addDirectoryFile(filePath, _realPath, folderID, false);
                }
            }

        }
    }
}