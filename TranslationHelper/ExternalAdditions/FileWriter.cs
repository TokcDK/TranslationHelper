using System.IO;
using System.Threading;

namespace TranslationHelper
{
    public static class FileWriter //example: https://stackoverflow.com/questions/47608949/c-sharp-multiple-threads-writing-to-the-same-file
    {
        private static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public static void WriteData(string filePath, string data, bool b = false)
        {
            if (filePath.Contains("TranslationHelper.log") && !b)
            {
                return;
            }
            locker.EnterWriteLock();
            try
            {
                File.AppendAllText(filePath, data);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }
}
