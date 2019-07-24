using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper
{
    class RPGMakerMVjson
    {
        //name - string
        //description - string
        //displayName - string
        //note - string
        //parameters - array?

        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string Note { get; set; }
        public string Parameters { get; set; }

    }

    class RPGMakerMVjsonName
    {
        public string Name { get; set; }
    }

    class RPGMakerMVjsonDescription
    {
        public string Description { get; set; }
    }

    class RPGMakerMVjsonDisplayName
    {
        public string DisplayName { get; set; }
    }

    class RPGMakerMVjsonNote
    {
        public string Note { get; set; }
    }

    class RPGMakerMVjsonMessage1
    {
        public string Message1 { get; set; }
    }

    class RPGMakerMVjsonMessage2
    {
        public string Message2 { get; set; }
    }

    class RPGMakerMVjsonMessage3
    {
        public string Message3 { get; set; }
    }

    class RPGMakerMVjsonMessage4
    {
        public string Message4 { get; set; }
    }

    class RPGMakerMVjsonNickname
    {
        public string Nickname { get; set; }
    }

    class RPGMakerMVjsonProfile
    {
        public string Profile { get; set; }
    }
}
