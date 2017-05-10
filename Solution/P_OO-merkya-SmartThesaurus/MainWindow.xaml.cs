/** ETML
 * \file      MainWindow.xaml.cs   
 * \author    Merk Yann
 * \brief     Program entry point / Window
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace P_OO_merkya_SmartThesaurus
{
    /// <summary>
    /// Logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FileIndexWindow fileWindow;
        WebIndexWindow webWindow;

        DB database;

        /// <summary>
        /// Program entry point
        /// </summary>
        /// \todo set a proper location for the db
        public MainWindow()
        {
            database = DB.getInstance("./tests.db");
            InitializeComponent();
        }
        
        /// \todo Check for Enters
        private void txtBSearch_KeyDown(object sender, KeyEventArgs e)
        {

        }
        ///\todo Placeholder
        private void txtBSearch_GotFocus(object sender, RoutedEventArgs e)
        {

        }
        ///\todo Placeholder
        private void txtBSearch_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Open the file indexation window
        /// </summary>
        private void btnLocalIndex_Click(object sender, RoutedEventArgs e)
        {
            fileWindow = new FileIndexWindow();

            fileWindow.ShowDialog();
        }
        /// <summary>
        /// Open the web indexation window
        /// </summary>
        private void btnWebIndex_Click(object sender, RoutedEventArgs e)
        {
            webWindow = new WebIndexWindow();

            webWindow.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// \todo comments and convert that to a form
        /// <param name="_idStartingFolder">Define from where we need to start searching Default : root (0)</param>
        /// <param name="_type"></param>
        public void Show(string _idStartingFolder = "0", ViewType _type = ViewType.Everything_Recursive, string _tempConsoleCurrentTabulation = "")
        {
            DB database = DB.getInstance();

            //List the current folder's name, if this is not 0
            if (_idStartingFolder != "0")
            {
                Console.WriteLine(_tempConsoleCurrentTabulation + "-" + database.getFolderName(_idStartingFolder));
            }

            //Go through all the files, if required
            if (_type == ViewType.Everything_Recursive || _type == ViewType.Files_Only || _type == ViewType.Folders_And_Files)
            {
                DataTable fileDataTable = database.getFiles(_idStartingFolder);
                foreach (DataRow file in fileDataTable.Rows)
                {
                    //output his name
                    Console.WriteLine(_tempConsoleCurrentTabulation + "|-" + file.ItemArray[1] + "." + file.ItemArray[2]);
                }
            }

            //Go through all the folders, if required
            if (_type == ViewType.Everything_Recursive || _type == ViewType.Folders_And_Files || _type == ViewType.Folders_Only_Recursive)
            {
                DataTable folderDataTable = database.getFolders(_idStartingFolder);


                foreach (DataRow folder in folderDataTable.Rows)
                {

                    //Recurse on it, if required
                    if (_type == ViewType.Everything_Recursive || _type == ViewType.Folders_Only_Recursive)
                    {
                        Show(Convert.ToString(folder.ItemArray[0]), _type, _tempConsoleCurrentTabulation + "|");
                    }
                }
            }
        }
    }
}
