namespace MesScriptDissAssTest
{
    internal class SilkyMesScriptDefault : IVersionData
    {
        public string Name => "SilkyMesScriptDefault";

        public (byte Opcode, string Struct, string Name)[] CommandLibrary
            => new (byte Opcode, string Struct, string Name)[]
            {
                (0x00, "", "NULL"),
                (0x01, "I", ""),  // Found only in LIBLARY.LIB
                (0x02, "", ""),
                (0x03, "", ""),  // Found only in LIBLARY.LIB
                (0x04, "", ""),
                (0x05, "", ""),
                (0x06, "", ""),  // Found only in LIBLARY.LIB
                (0x0A, "S", "STR_CRYPT"),
                (0x0B, "S", "STR_UNCRYPT"),
                (0x0C, "", ""),
                (0x0D, "", ""),
                (0x0E, "", ""),
                (0x0F, "", ""),
                (0x10, "B", ""),
                (0x11, "", ""),
                (0x14, ">I", "JUMP"),
                (0x15, ">I", "MSG_OFSETTER"),
                (0x16, ">I", "SPEC_OFSETTER"),  // Found only in LIBLARY.LIB
                (0x17, "", ""),
                (0x18, "", ""),
                (0x19, ">I", "MESSAGE"),
                (0x1A, ">I", ""),
                (0x1B, ">I", ""),
                (0x1C, "B", "TO_NEW_STRING"),
                (0x32, ">hh", ""),
                (0x33, "S", "STR_RAW"),
                (0x34, "", ""),
                (0x35, "", ""),
                (0x36, "B", "JUMP_2"),
                (0x37, "", ""),
                (0x3A, "", ""),
                (0x3B, "", ""),
                (0x3C, "", ""),
                (0x3D, "", ""),
                (0x3E, "", ""),
                (0x42, "", ""),
                (0x43, "", ""),
                (0xFA, "", ""),
                (0xFB, "", ""),
                (0xFC, "", ""),
                (0xFD, "", ""),
                (0xFE, "", ""),
                (0xFF, "", "")
            };
    }

}