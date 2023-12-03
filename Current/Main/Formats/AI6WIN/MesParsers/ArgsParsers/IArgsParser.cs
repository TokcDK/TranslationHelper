using System.Collections.Generic;

namespace MesScriptDissAss
{
    internal interface IArgsParser
    {
        List<char> Instances { get; }
        object Get(object[] args);
        byte[] Set(object[] args);
    }

}