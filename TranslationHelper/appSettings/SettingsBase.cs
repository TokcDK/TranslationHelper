using System;
using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    internal static class SettingsBaseTools
    {
        internal static Dictionary<string, SettingsBase> GetSettingsList()
        {
            var listOfSubclasses = GetListOfSubClasses.Inherited.GetListOfinheritedSubClasses<SettingsBase>();
            return GetSettingsList(listOfSubclasses);
        }

        /// <summary>
        /// Get list of settings classes with 'settingname, class' pair  
        /// </summary>
        /// <param name="listOfSubclasses"></param>
        /// <returns></returns>
        internal static Dictionary<string, SettingsBase> GetSettingsList(List<SettingsBase> listOfSubclasses)
        {
            var settingsList = new Dictionary<string, SettingsBase>();
            foreach (var subclass in listOfSubclasses)
            {
                settingsList.Add(subclass.Id(), subclass);
            }
            return settingsList;
        }
    }

    abstract class SettingsBase/* : ApplicationSettingsBase*/
    {
        //Dictionary<string, SettingsBase> SettingsList;
        
        protected SettingsBase()
        {
            
        }

        internal virtual string Section { get => string.Empty; }

        internal abstract string Key { get; }

        internal abstract string Default { get; }
        internal virtual bool DefaultBool { get => false; }
        internal virtual int DefaultInt { get => 0; }

        internal abstract void Set(bool setObject = false);
        internal abstract string Get();

        internal abstract string Id();

        //protected INIFile THConfigINI = new INIFile(Application.ProductName+".ini");
    }
}
