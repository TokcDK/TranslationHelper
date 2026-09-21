using System.IO;

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

        static readonly object _soundLocker = new object();

        /// <summary>
        /// Player of the "complete" sound. Kept for the lifetime of the app so the wav file is
        /// read from disk once instead of on every call, and so no <see cref="System.Media.SoundPlayer"/>
        /// instance is leaked (it is <see cref="IDisposable"/>).
        /// </summary>
        static System.Media.SoundPlayer _completeSoundPlayer;

        internal static void PlayBeep()
        {
            lock (_soundLocker)
            {
                string soundFilePath = Path.GetFullPath(Data.THSettings.ResDirPath + @"\sounds\complete.wav");
				if (!File.Exists(soundFilePath)) return;

                if (_completeSoundPlayer == null)
                {
                    _completeSoundPlayer = new System.Media.SoundPlayer(soundFilePath);
                }

                _completeSoundPlayer.Play();
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
