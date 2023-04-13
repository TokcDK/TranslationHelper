namespace TranslationHelper.Functions
{
    class FunctionsSounds
    {
        /// <summary>
        /// play the sound when DB completely saved
        /// </summary>
        internal static void SaveDBComplete()
        {

            PlayBeep();
        }

        /// <summary>
        /// play the sound when DB completely loaded
        /// </summary>
        internal static void LoadDBCompleted()
        {
            PlayBeep();
        }

        /// <summary>
        /// play the sound when project succefully opened
        /// </summary>
        internal static void OpenProjectComplete()
        {
            PlayAsterisk();
        }

        /// <summary>
        /// pla sound after global function finished work
        /// </summary>
        internal static void GlobalFunctionFinishedWork()
        {
            PlayBeep();
        }

        /// <summary>
        /// play the sound when project open failed
        /// </summary>
        internal static void OpenProjectFailed()
        {
            PlayExclamation();
        }

        static object _soundLocker = new object();
        internal static void PlayBeep()
        {
            lock (_soundLocker)
            {
                new System.Media.SoundPlayer(Data.AppSettings.ApplicationStartupPath + @"\RES\sounds\complete.wav").Play();
            }
        }

        internal static void PlayExclamation()
        {
            //PlayBeep();
        }

        internal static void PlayAsterisk()
        {
            //PlayBeep();
        }
    }
}
