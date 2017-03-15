/** ETML
 * \file      IIndexation.cs   
 * \author    Merk Yann
 * \brief     Contains the Interface for all the different indexation types
 */
namespace P_OO_merkya_SmartThesaurus
{
    public interface IIndexation
    {
        void index(string _path, FileExplorationType _type);
    }
}