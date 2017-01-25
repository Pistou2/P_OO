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

///<summary>Database class, handling the connection and the querries</summary>
///\date 25.01.2017
public class DB
{
    SQLiteConnection sqlConnection;

    /// <summary>
    /// Constructor
    /// Initialize the Database, and connect it to the given file
    /// </summary>
    /// <param name="_filePath">.db file path</param>
    public DB(string _filePath)
    {
        try
        {
            sqlConnection = new SQLiteConnection("Data Source = " + _filePath);
        }

        catch
        {
            throw new Exception("Could not connect to the DB");
        }
    }
    /// <summary>
    /// Send a Query to the Database, and return the result
    /// 
    /// Code HUGELY inspired from a stackoverflow Anser
    /// </summary>
    /// <param name="_request">SQL query</param>
    /// <returns>DataTable, containing the result</returns>
    /// \see <http://stackoverflow.com/a/1253002>
    public DataTable query(string _request)
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

        }
        sqlConnection.Close();
        return table;
    }

}

