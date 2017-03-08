using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P_OO_merkya_SmartThesaurus
{
    public interface IIndexation
    {
        void index(string _path, FileExplorationType _type);
    }
}