/** ETML
 * \file      WebIndexWindow.xaml.cs   
 * \author    Merk Yann
 * \brief     Window used to start a web indexation
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
using System.Windows.Shapes;

namespace P_OO_merkya_SmartThesaurus
{
    /// <summary>
    /// Logic Window1.xaml
    /// </summary>
    public partial class FileIndexWindow : Window
    {
        FileIndexation fileIndexer;

        public FileIndexWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Start the indexation
        /// </summary>
        /// \todo Check the path
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            fileIndexer = new FileIndexation();

            //check what type is selected
            FileExplorationType explorationType;

            if (((string)((ListBoxItem)listBox.SelectedItem).Content) == "Complet")
            {
                explorationType = FileExplorationType.Full_And_File_Contents;
            }
            else
            {
                explorationType = FileExplorationType.Full;
            }

            fileIndexer.index(txtBLocation.Text, explorationType);
        }
    }
}
