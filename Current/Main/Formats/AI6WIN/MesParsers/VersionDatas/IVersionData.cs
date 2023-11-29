namespace MesScriptDissAssTest
{
    internal interface IVersionData
    {
        string Name { get; }
        (byte Opcode, string Struct, string Name)[] CommandLibrary { get; }
    }

}