using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats;

namespace TranslationHelper.Projects.ZZZZFormats
{
    class ZZZZFormatByExtension : ProjectBase
    {
        /// <summary>
        /// Project will be set be specified extension of found format
        /// </summary>
        public ZZZZFormatByExtension()
        {
        }

        private void GetValidOpenable()
        {
            formatsTypes = GetListOfSubClasses.Inherited.GetListOfInheritedTypes(typeof(FormatBase));
        }

        List<Type> formatsTypes;

        internal override bool Check()
        {
            GetValidOpenable();

            foreach (var formatType in formatsTypes)
            {
                var format = (FormatBase)Activator.CreateInstance(formatType);
                if (format.Ext() == Path.GetExtension(ProjectData.SelectedFilePath) && format.ExtIdentifier())
                {
                    CurrentFormat = format;
                    return true;
                }
            }

            return false;
        }

        internal override string Name()
        {
            return CurrentFormat.Name();
        }

        internal override bool Open()
        {
            return CurrentFormat.Open();
        }

        internal override bool Save()
        {
            return CurrentFormat.Save();
        }
    }
}
