/** ETML
 * \file      WebIndexWindow.xaml.cs   
 * \author    Merk Yann
 * \brief     Window used to start a file indexation
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
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class WebIndexWindow : Window
    {
        WebIndexation webIndexer;
        public WebIndexWindow()
        {            
            InitializeComponent();
        }
    }
}
