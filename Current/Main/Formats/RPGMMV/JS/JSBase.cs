using GetListOfSubClasses;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMMV.JsonParser;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JSBase : RPGMMVBase, IUseJSLocationInfo, IUseJsonParser
    {
        protected JSBase(ProjectBase parentProject) : base(parentProject)
        {
            JsonParser = new JSJsonParser(this);
        }

        internal override bool UseTableNameWithoutExtension => false;

        public override string Extension => ".js";

        public override string Description => "RPGMakerMV plugin js file";

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<System.Type> GetListOfJSTypes()
        {
            return Inherited.GetListOfInheritedTypes(typeof(JSBase));
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<JSBase> GetListOfJS()
        {
            return Inherited.GetListOfinheritedSubClasses<JSBase>();
        }

        public abstract string JSName { get; }

        public virtual string JSSubfolder => "plugins";

        JsonParserBase JParser;

        public JsonParserBase JsonParser { get => JParser; set => JParser = value; }

        protected bool IsValidToken(JValue value)
        {
            return value.Type == JTokenType.String
                && JSTokenValid(value)
                //&& (!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
                && !string.IsNullOrWhiteSpace(value + "")
                && !(THSettings.SourceLanguageIsJapanese && value.ToString().HaveMostOfRomajiOtherChars());
        }

        protected virtual bool JSTokenValid(JValue value)
        {
            return true;
        }
    }
}
