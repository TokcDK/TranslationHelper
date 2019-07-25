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


    /////////////////////////
    /*
    public class RPGMakerMVjsonMap
    {
        public string DisplayName { get; set; }
        public string Note { get; set; }
        public Event[] Events { get; set; }
    }

    public class Event
    {
        public string Name { get; set; }
        public string Note { get; set; }
    }
    */

    //RPGMakerMVjsonMap
    public class RPGMakerMVjsonMap
    {
        public bool autoplayBgm { get; set; }
        public bool autoplayBgs { get; set; }
        public string battleback1Name { get; set; }
        public string battleback2Name { get; set; }
        public Bgm bgm { get; set; }
        public Bgs bgs { get; set; }
        public bool disableDashing { get; set; }
        public string DisplayName { get; set; }
        public Encounterlist[] encounterList { get; set; }
        public int encounterStep { get; set; }
        public int height { get; set; }
        public string Note { get; set; }
        public bool parallaxLoopX { get; set; }
        public bool parallaxLoopY { get; set; }
        public string parallaxName { get; set; }
        public bool parallaxShow { get; set; }
        public int parallaxSx { get; set; }
        public int parallaxSy { get; set; }
        public int scrollType { get; set; }
        public bool specifyBattleback { get; set; }
        public int tilesetId { get; set; }
        public int width { get; set; }
        public int[] data { get; set; }
        public Event[] Events { get; set; }
    }

    public class Bgm
    {
        public string name { get; set; }
        public int pan { get; set; }
        public int pitch { get; set; }
        public int volume { get; set; }
    }

    public class Bgs
    {
        public string name { get; set; }
        public int pan { get; set; }
        public int pitch { get; set; }
        public int volume { get; set; }
    }

    public class Encounterlist
    {
        public object[] regionSet { get; set; }
        public int troopId { get; set; }
        public int weight { get; set; }
    }

    public class Event
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public Page[] pages { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Page
    {
        public Conditions conditions { get; set; }
        public bool directionFix { get; set; }
        public Image image { get; set; }
        public PageList[] list { get; set; }
        public int moveFrequency { get; set; }
        public Moveroute moveRoute { get; set; }
        public int moveSpeed { get; set; }
        public int moveType { get; set; }
        public int priorityType { get; set; }
        public bool stepAnime { get; set; }
        public bool through { get; set; }
        public int trigger { get; set; }
        public bool walkAnime { get; set; }
    }

    public class Conditions
    {
        public int actorId { get; set; }
        public bool actorValid { get; set; }
        public int itemId { get; set; }
        public bool itemValid { get; set; }
        public string selfSwitchCh { get; set; }
        public bool selfSwitchValid { get; set; }
        public int switch1Id { get; set; }
        public bool switch1Valid { get; set; }
        public int switch2Id { get; set; }
        public bool switch2Valid { get; set; }
        public int variableId { get; set; }
        public bool variableValid { get; set; }
        public int variableValue { get; set; }
    }

    public class Image
    {
        public int characterIndex { get; set; }
        public string characterName { get; set; }
        public int direction { get; set; }
        public int pattern { get; set; }
        public int tileId { get; set; }
    }

    public class Moveroute
    {
        public List[] list { get; set; }
        public bool repeat { get; set; }
        public bool skippable { get; set; }
        public bool wait { get; set; }
    }

    public class List
    {
        public int code { get; set; }
        public object[] parameters { get; set; }
    }

    public class PageList
    {
        public int code { get; set; }
        public int indent { get; set; }
        public object[] parameters { get; set; }
    }
    //RPGMakerMVjsonMap

    //RPGMakerMVjsonCommonEvents
    public class RPGMakerMVjsonCommonEvents
    {
        public int Id { get; set; }
        public CEList[] list { get; set; }
        public string Name { get; set; }
        public int SwitchId { get; set; }
        public int Trigger { get; set; }
    }

    public class CEList
    {
        public int Code { get; set; }
        public int Indent { get; set; }
        public object[] parameters { get; set; }
    }
    //RPGMakerMVjsonCommonEvents
}
