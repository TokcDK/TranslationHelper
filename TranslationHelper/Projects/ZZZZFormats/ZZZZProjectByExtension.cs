using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats;

namespace TranslationHelper.Projects.ZZZZFormats
{
    class ZZZZProjectByExtension : ProjectBase
    {
        /// <summary>
        /// Project will be set be specified extension of found format
        /// </summary>
        public ZZZZProjectByExtension()
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

            var fileExt = Path.GetExtension(ProjectData.SelectedFilePath);
            foreach (var formatType in formatsTypes)
            {
                var format = (FormatBase)Activator.CreateInstance(formatType);
                if (format.Ext() == fileExt /*&& format.ExtIdentifier()*/)
                {
                    CurrentFormat = format;
                    return true;
                }
            }

            return false;
        }

        internal override string Name()
        {
            return string.IsNullOrWhiteSpace(CurrentFormat.Name()) ? CurrentFormat.Ext() : CurrentFormat.Name();
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
