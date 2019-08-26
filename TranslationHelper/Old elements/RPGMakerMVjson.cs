//namespace TranslationHelper
//{
//    class RPGMakerMVjson
//    {
//        //name - string
//        //description - string
//        //displayName - string
//        //note - string
//        //parameters - array?

//        public string Name { get; set; }
//        public string Nickname { get; set; }
//        public string DisplayName { get; set; }
//        public string Description { get; set; }
//        public string Note { get; set; }
//        public string Profile { get; set; }
//        public string Message1 { get; set; }
//        public string Message2 { get; set; }
//        public string Message3 { get; set; }
//        public string Message4 { get; set; }

//    }
//    class RPGMakerMVjsonItemsArmorsWeapons
//    {
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public string Note { get; set; }
//    }
//    class RPGMakerMVjsonSkills
//    {
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public string Note { get; set; }
//        public string Message1 { get; set; }
//        public string Message2 { get; set; }
//    }
//    class RPGMakerMVjsonStates
//    {
//        public string Name { get; set; }
//        public string Note { get; set; }
//        public string Message1 { get; set; }
//        public string Message2 { get; set; }
//        public string Message3 { get; set; }
//        public string Message4 { get; set; }
//    }
//    class RPGMakerMVjsonClassesEnemiesTilesets
//    {
//        public string Name { get; set; }
//        public string Note { get; set; }
//    }
//    class RPGMakerMVjsonAnimationsMapInfosTroops
//    {
//        public string Name { get; set; }
//    }
//    /*
//    class RPGMakerMVjsonActors
//    {
//        public string Name { get; set; }
//        public string Nickname { get; set; }
//        public string Profile { get; set; }
//        public string Note { get; set; }
//    }
//    */

//    class RPGMakerMVjsonName
//    {
//        public string Name { get; set; }
//    }

//    class RPGMakerMVjsonDescription
//    {
//        public string Description { get; set; }
//    }

//    class RPGMakerMVjsonDisplayName
//    {
//        public string DisplayName { get; set; }
//    }

//    class RPGMakerMVjsonNote
//    {
//        public string Note { get; set; }
//    }

//    class RPGMakerMVjsonMessage1
//    {
//        public string Message1 { get; set; }
//    }

//    class RPGMakerMVjsonMessage2
//    {
//        public string Message2 { get; set; }
//    }

//    class RPGMakerMVjsonMessage3
//    {
//        public string Message3 { get; set; }
//    }

//    class RPGMakerMVjsonMessage4
//    {
//        public string Message4 { get; set; }
//    }

//    class RPGMakerMVjsonNickname
//    {
//        public string Nickname { get; set; }
//    }

//    class RPGMakerMVjsonProfile
//    {
//        public string Profile { get; set; }
//    }


//    /////////////////////////
//    /*
//    public class RPGMakerMVjsonMap
//    {
//        public string DisplayName { get; set; }
//        public string Note { get; set; }
//        public Event[] Events { get; set; }
//    }

//    public class Event
//    {
//        public string Name { get; set; }
//        public string Note { get; set; }
//    }
//    */

//    //RPGMakerMVjsonMap
//    public class RPGMakerMVjsonMap
//    {
//        public string DisplayName { get; set; }
//        public string Note { get; set; }
//        public RPGMakerMVjsonMapEvent[] Events { get; set; }
//    }

//    public class RPGMakerMVjsonMapEvent
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public string Note { get; set; }
//        public RPGMakerMVjsonMapPage[] Pages { get; set; }
//    }

//    public class RPGMakerMVjsonMapPage
//    {
//        public RPGMakerMVjsonMapPageList[] List { get; set; }
//        public RPGMakerMVjsonMapMoveroute MoveRoute { get; set; }
//    }

//    public class RPGMakerMVjsonMapMoveroute
//    {
//        public RPGMakerMVjsonMapList[] List { get; set; }
//    }

//    public class RPGMakerMVjsonMapList
//    {
//        public int Code { get; set; }
//        public object[] Parameters { get; set; }
//    }

//    public class RPGMakerMVjsonMapPageList
//    {
//        public int Code { get; set; }
//        public int Indent { get; set; }
//        public object[] Parameters { get; set; }
//    }
//    //RPGMakerMVjsonMap

//    //RPGMakerMVjsonSystem
//    /*
//    public class RPGMakerMVjsonSystem
//    {
//        public Airship Airship { get; set; }
//        public string[] ArmorTypes { get; set; }
//        public Attackmotion[] AttackMotions { get; set; }
//        public Battlebgm battleBgm { get; set; }
//        public string battleback1Name { get; set; }
//        public string battleback2Name { get; set; }
//        public int battlerHue { get; set; }
//        public string battlerName { get; set; }
//        public Boat boat { get; set; }
//        public string currencyUnit { get; set; }
//        public Defeatme defeatMe { get; set; }
//        public int editMapId { get; set; }
//        public string[] Elements { get; set; }
//        public string[] EquipTypes { get; set; }
//        public string GameTitle { get; set; }
//        public Gameoverme gameoverMe { get; set; }
//        public string locale { get; set; }
//        public int[] MagicSkills { get; set; }
//        public bool[] menuCommands { get; set; }
//        public bool optDisplayTp { get; set; }
//        public bool optDrawTitle { get; set; }
//        public bool optExtraExp { get; set; }
//        public bool optFloorDeath { get; set; }
//        public bool optFollowers { get; set; }
//        public bool optSideView { get; set; }
//        public bool optSlipDeath { get; set; }
//        public bool optTransparent { get; set; }
//        public int[] partyMembers { get; set; }
//        public Ship ship { get; set; }
//        public string[] skillTypes { get; set; }
//        public Sound[] sounds { get; set; }
//        public int startMapId { get; set; }
//        public int startX { get; set; }
//        public int startY { get; set; }
//        public string[] switches { get; set; }
//        public RPGMakerMVjsonSystemTerms terms { get; set; }
//        public Testbattler[] testBattlers { get; set; }
//        public int testTroopId { get; set; }
//        public string title1Name { get; set; }
//        public string title2Name { get; set; }
//        public Titlebgm titleBgm { get; set; }
//        public string[] variables { get; set; }
//        public int versionId { get; set; }
//        public Victoryme victoryMe { get; set; }
//        public string[] WeaponTypes { get; set; }
//        public int[] windowTone { get; set; }
//    }

//    public class Airship
//    {
//        public Bgm bgm { get; set; }
//        public int characterIndex { get; set; }
//        public string characterName { get; set; }
//        public int startMapId { get; set; }
//        public int startX { get; set; }
//        public int startY { get; set; }
//    }

//    public class Bgm
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class Battlebgm
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class Boat
//    {
//        public Bgm1 bgm { get; set; }
//        public int characterIndex { get; set; }
//        public string characterName { get; set; }
//        public int startMapId { get; set; }
//        public int startX { get; set; }
//        public int startY { get; set; }
//    }

//    public class Bgm1
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class Defeatme
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class Gameoverme
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class Ship
//    {
//        public Bgm2 bgm { get; set; }
//        public int characterIndex { get; set; }
//        public string characterName { get; set; }
//        public int startMapId { get; set; }
//        public int startX { get; set; }
//        public int startY { get; set; }
//    }

//    public class Bgm2
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class RPGMakerMVjsonSystemTerms
//    {
//        public string[] basic { get; set; }
//        public string[] commands { get; set; }
//        public string[] Params { get; set; }
//        public RPGMakerMVjsonSystemMessages Messages { get; set; }
//    }

//    public class RPGMakerMVjsonSystemMessages
//    {
//        public string actionFailure { get; set; }
//        public string actorDamage { get; set; }
//        public string actorDrain { get; set; }
//        public string actorGain { get; set; }
//        public string actorLoss { get; set; }
//        public string actorNoDamage { get; set; }
//        public string actorNoHit { get; set; }
//        public string actorRecovery { get; set; }
//        public string alwaysDash { get; set; }
//        public string bgmVolume { get; set; }
//        public string bgsVolume { get; set; }
//        public string buffAdd { get; set; }
//        public string buffRemove { get; set; }
//        public string commandRemember { get; set; }
//        public string counterAttack { get; set; }
//        public string criticalToActor { get; set; }
//        public string criticalToEnemy { get; set; }
//        public string debuffAdd { get; set; }
//        public string defeat { get; set; }
//        public string emerge { get; set; }
//        public string enemyDamage { get; set; }
//        public string enemyDrain { get; set; }
//        public string enemyGain { get; set; }
//        public string enemyLoss { get; set; }
//        public string enemyNoDamage { get; set; }
//        public string enemyNoHit { get; set; }
//        public string enemyRecovery { get; set; }
//        public string escapeFailure { get; set; }
//        public string escapeStart { get; set; }
//        public string evasion { get; set; }
//        public string expNext { get; set; }
//        public string expTotal { get; set; }
//        public string file { get; set; }
//        public string levelUp { get; set; }
//        public string loadMessage { get; set; }
//        public string magicEvasion { get; set; }
//        public string magicReflection { get; set; }
//        public string meVolume { get; set; }
//        public string obtainExp { get; set; }
//        public string obtainGold { get; set; }
//        public string obtainItem { get; set; }
//        public string obtainSkill { get; set; }
//        public string partyName { get; set; }
//        public string possession { get; set; }
//        public string preemptive { get; set; }
//        public string saveMessage { get; set; }
//        public string seVolume { get; set; }
//        public string substitute { get; set; }
//        public string surprise { get; set; }
//        public string useItem { get; set; }
//        public string victory { get; set; }
//    }

//    public class Titlebgm
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class Victoryme
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class Attackmotion
//    {
//        public int type { get; set; }
//        public int weaponImageId { get; set; }
//    }

//    public class Sound
//    {
//        public string name { get; set; }
//        public int pan { get; set; }
//        public int pitch { get; set; }
//        public int volume { get; set; }
//    }

//    public class Testbattler
//    {
//        public int actorId { get; set; }
//        public int[] equips { get; set; }
//        public int level { get; set; }
//    }
//    */
//    //System

//}
