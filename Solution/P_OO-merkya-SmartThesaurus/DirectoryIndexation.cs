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
        public static void directory(string _path, DB _database)
        {
            //check if the path is reachable
            if (Directory.Exists(_path))
            {
                //make sure all the folders are set up to this point
                string idLastFolder = createRouteToPath(_path, _database);

               exploration(_path, _database, idLastFolder);
            }
            else
            {
                throw new DriveNotFoundException();
            }
        }

        /// <summary>
        /// Create all the folders to get from the origin to the final path
        /// </summary>
        /// <param name="_finalPath">The target path</param>
        /// <param name="_database">The database, used to input the folders</param>
        /// <param name="_previousFolderID">the final path of the previous recursive iteration, leave it blank</param>
        /// <returns>The id of the last folder</returns>
        public static string createRouteToPath(string _finalPath, DB _database, string _previousFolderID = "0")
        {
            //Split the first ellement of the path, and the rest
            string[] splitedPath = _finalPath.Split(new char[] { '/', '\\' }, 2);

            //Add the folder to the database
            string idFolder = _database.addDirectoryFolder(splitedPath[0], _previousFolderID);

            if (splitedPath.Length > 1)
            {
                return createRouteToPath(splitedPath[1], _database, idFolder);
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
        public static void exploration(string _realPath, DB _database, string _parentID = "0")
        {
            string[] folderPaths = _realPath.Split(new char[] { '/', '\\' });

            string folderName = folderPaths[folderPaths.Length - 1];

            //Add the folder to the DB
            string folderID = _database.addDirectoryFolder(folderName, _parentID);

            //Recursively call all the child directories
            try
            {
                foreach (string childPath in Directory.GetDirectories(_realPath))
                {
                    exploration(childPath, _database, folderID);
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.Print("Folder Error");
            }

            //Call all the files to be added
            foreach (string filePath in Directory.GetFiles(_realPath))
            {
                ///\todo Scan the file content?
                _database.addDirectoryFile(filePath, _realPath, folderID);
            }

        }
    }
}