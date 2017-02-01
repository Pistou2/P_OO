using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace P_OO_merkya_SmartThesaurus
{
    public static class DirectoryIndexation
    {
        public static void directory(string _path, DB _database)
        {
            //check if the path is reachable
            if (Directory.Exists(_path))
            {
                //make sure all the folders are set up to this point
                createRouteToPath(_path, _database);
                exploration(_path, "", _database);
            }
            else
            {
                throw new DriveNotFoundException();
            }
        }

        public static void createRouteToPath(string _finalPath, DB _database, string _previousFolder = "")
        {
            //Add the folder to the DB
            string[] splitedPath = _finalPath.Split(new char[] { '/', '\\' }, 2);


            _database.addDirectoryFolder(splitedPath[0], _previousFolder);

            if (splitedPath.Length > 1)
            {
                createRouteToPath(splitedPath[1], _database, _previousFolder + splitedPath[0] + "/");
            }
        }

        /// <summary>
        /// Used when entering a new directory
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_database"></param>
        public static void exploration(string _path, string _parentPath, DB _database)
        {
            //Add the folder to the DB
            _database.addDirectoryFolder(_path, _parentPath);

            //Recursively call all the child directories
            foreach (string childPath in Directory.GetDirectories(_path))
            {
                exploration(childPath, _path, _database);
            }

            //Call all the files to be added
            foreach (string filePath in Directory.GetFiles(_path))
            {
                ///\todo Scan the file content?
                _database.addDirectoryFile(filePath, _path);
            }

        }
    }
}