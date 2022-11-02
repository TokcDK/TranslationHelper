//using System.ComponentModel;

//namespace TranslationHelper
//{
//    internal class THRPGMTransPatchFile
//    {
//        public string _procent;

//        public BindingList<Block> blocks;  //Блоки

//        public string Path { get; }

//        public string Name { get; }

//        public string Procent
//        {
//            get => _procent;
//            set => _procent = value;
//        }

//        public THRPGMTransPatchFile(string Nm, string Pt, string Pc)
//        {
//            Name = Nm;
//            Path = Pt;
//            _procent = Pc;
//            blocks = new BindingList<Block>();
//        }
//    }

//    internal class Block
//    {
//        public string Context { get; }

//        public string Advice { get; }

//        public string Original { get; set; }

//        public string Translation { get; set; }

//        public int Status { get; set; }

//        public Block(string Cont,
//                     string Adv,
//                     string Untr,
//                     string Tr,
//                     int St
//                    )
//        {
//            Context = Cont;
//            Advice = Adv;
//            Original = Untr;
//            Translation = Tr;
//            Status = St;
//        }
//    }
//}