using System.Threading;

namespace TranslationHelper.Main.Functions
{
    public static class FunctionsThreading
    {
        public static void WaitThreaded(int time)
        {
            Thread.Sleep(time);
        }
    }
}
