using RPGMVJsonParser;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonActors : JsonTypeBase
    {
        public JsonActors(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override object ParseJson(string path)
        {
            var data = Helper.LoadActors(path);
            int count = data.Count;

            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null)
                {
                    continue;
                }

                // Process Actor Name
                string s = item.Name;
                string nameInfo = SaveFileMode
                    ? ""
                    :
                    $"\r\ntype:Name" +
                    $"\r\nID: {item.Id}" +
                    $"\r\nbattlerName: \"{item.BattlerName}\"" +
                    $"\r\nCharacterName: \"{item.CharacterName}\"" +
                    $"\r\nNote: \"{item.Note}\"";
                if (AddRowData(ref s, nameInfo) && SaveFileMode)
                {
                    item.Name = s;
                }

                // Process Nickname
                s = item.Nickname;
                string nickNameInfo = SaveFileMode
                    ? ""
                    :
                    $"\r\ntype:NickName" +
                    $"\r\nID: {item.Id}" +
                    $"\r\nName: \"{item.Name}\"" +
                    $"\r\nbattlerName: \"{item.BattlerName}\"";
                if (AddRowData(ref s, nickNameInfo) && SaveFileMode)
                {
                    item.Nickname = s;
                }

                // Process CharacterName
                s = item.CharacterName;
                string characterNameInfo = SaveFileMode
                    ? ""
                    :
                    $"\r\ntype:CharacterName" +
                    $"\r\nID: {item.Id}" +
                    $"\r\nName: \"{item.Name}\"" +
                    $"\r\nbattlerName: \"{item.BattlerName}\"";
                if (AddRowData(ref s, characterNameInfo) && SaveFileMode)
                {
                    item.CharacterName = s;
                }

                // Process Profile
                s = item.Profile;
                string profileInfo = SaveFileMode
                    ? ""
                    :
                    $"\r\ntype:Profile" +
                    $"\r\nID: {item.Id}" +
                    $"\r\nName: \"{item.Name}\"" +
                    $"\r\nCharacterName: \"{item.CharacterName}\"" +
                    $"\r\nbattlerName: \"{item.BattlerName}\"";
                if (AddRowData(ref s, profileInfo) && SaveFileMode)
                {
                    item.Profile = s;
                }

                // Process Note
                s = item.Note;
                string noteInfo = SaveFileMode
                    ? ""
                    :
                    $"\r\ntype:Note" +
                    $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"" +
                    $"\r\nCharacterName: \"{item.CharacterName}\"" +
                    $"\r\nbattlerName: \"{item.BattlerName}\"";
                if (AddRowData(ref s, noteInfo) && SaveFileMode)
                {
                    item.Note = s;
                }
            }

            return data;
        }
    }
}
