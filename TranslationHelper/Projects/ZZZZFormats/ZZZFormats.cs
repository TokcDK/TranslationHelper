using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats;

namespace TranslationHelper.Projects.ZZZZFormats
{
    class ZZZZFormats : ProjectBase
    {
        public ZZZZFormats() : base()
        {
        }

        private void GetValidOpenable()
        {
            formats = GetListOfSubClasses();
        }

        List<FormatBase> formats;
        FormatBase SelectedFormat;

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// non linq a variant of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclasses</typeparam>
        /// <param name="parameter">parameter required for subclass(remove if not need)</param>
        /// <returns>List of subclasses of abstract class</returns>
        private static List<FormatBase> GetListOfSubClasses(params object[] args)
        {
            var ListOfSubClasses = new List<FormatBase>();
            var type = typeof(FormatBase);
            bool noargs = args == null || args.Length == 0;
            foreach (var ClassType in type.Assembly.GetTypes())
            {
                if (!ClassType.FullName.StartsWith("TranslationHelper.Formats"))
                {
                    continue;
                }

                var c1 = ClassType.GetMethod("Ext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var c2 = ClassType.GetMethod("ExtIdentifier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var c3 = ClassType.GetMethod("Name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (!ClassType.IsClass 
                    || ClassType.IsAbstract 
                    || ClassType.IsInterface 
                    || !ClassType.IsSubclassOf(type) 
                    || c1 == null 
                    || c2 == null 
                    || c3 == null)
                {
                    continue;
                }

                FormatBase instance;

                if (noargs)
                {
                    instance = (FormatBase)Activator.CreateInstance(ClassType);
                }
                else
                {
                    instance = (FormatBase)Activator.CreateInstance(ClassType, args);
                }

                ListOfSubClasses.Add(instance);
            }

            return ListOfSubClasses;
        }

        internal override bool Check()
        {
            GetValidOpenable();

            foreach (var format in formats)
            {
                if (format.Ext() == Path.GetExtension(ProjectData.SelectedFilePath) && format.ExtIdentifier())
                {
                    SelectedFormat = format;
                    return true;
                }
            }

            return false;
        }

        internal override string Name()
        {
            return SelectedFormat.Name();
        }

        internal override bool Open()
        {
            return SelectedFormat.Open();
        }

        internal override bool Save()
        {
            return SelectedFormat.Save();
        }
    }
}
