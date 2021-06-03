using System;
using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    internal static class SettingsBaseTools
    {
        internal static Dictionary<string, SettingsBase> GetSettingsList(THDataWork thDataWork)
        {
            var listOfSubclasses = GetListOfSubClasses.Inherited.GetListOfinheritedSubClasses<SettingsBase>(thDataWork);
            return GetSettingsList(listOfSubclasses);
        }

        /// <summary>
        /// Get list of settings classes with 'settingname, class' pair  
        /// </summary>
        /// <param name="listOfSubclasses"></param>
        /// <returns></returns>
        internal static Dictionary<string, SettingsBase> GetSettingsList(List<SettingsBase> listOfSubclasses)
        {
            var SettingsList = new Dictionary<string, SettingsBase>();
            foreach (var Subclass in listOfSubclasses)
            {
                SettingsList.Add(Subclass.ID(), Subclass);
            }
            return SettingsList;
        }
    }

    abstract class SettingsBase/* : ApplicationSettingsBase*/
    {
        //Dictionary<string, SettingsBase> SettingsList;
        protected THDataWork thDataWork;
        protected SettingsBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal virtual string Section { get => string.Empty; }

        internal abstract string Key { get; }

        internal abstract string Default { get; }
        internal virtual bool DefaultBool { get => false; }
        internal virtual int DefaultInt { get => 0; }

        internal abstract void Set(bool SetObject = false);
        internal abstract string Get();

        internal abstract string ID();

        //protected INIFile THConfigINI = new INIFile(Application.ProductName+".ini");
    }
}
