using System.Collections.Generic;

namespace MesScriptDissAssTest
{
    internal interface IArgsParser
    {
        List<char> Instances { get; }
        object Get(object[] args);
        byte[] Set(object[] args);
    }

}