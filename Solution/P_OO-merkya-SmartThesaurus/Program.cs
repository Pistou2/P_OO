/** ETML
 * \file      Program.cs   
 * \author    Merk Yann
 * \brief     Program entry point
 */
using System;

namespace P_OO_merkya_SmartThesaurus
{
    class Program
    {
        static void Main(string[] args)
        {
            ///\todo : Initialize properly the DB
            DB test = DB.getInstance("./test.db");

            /*
            DirectoryIndexation explo = new DirectoryIndexation();

            //explo.index(@"K:/INF/Eleves/Temp",FileExplorationType.Full);

            explo.index(@"C:/Users/merkya/Desktop/Temp",FileExplorationType.Full_And_File_Contents);

            //DirectoryIndexation.exploration("E:\\",FileExplorationType.Full);
            */

            WebIndexation webExplo = new WebIndexation();
            webExplo.index("http://etml.ch", FileExplorationType.Full_And_File_Contents);

            Viewer.getInstance().show();

            Console.ReadLine();
        }
    }
}
