﻿/**
 * \file      Program.cs   
 * \author    Merk Yann
 * \version   1.0
 * \date      25.01.2017
 * \brief     Program entry point
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace P_OO_merkya_SmartThesaurus
{
    class Program
    {
        static void Main(string[] args)
        {
            ///\todo : Initialize properly the DB
            DB test = new DB("./test.db");

            DirectoryIndexation.directory("K:\\INF\\Eleves\\Temp", test);
        }
    }
}
