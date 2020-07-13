namespace TranslationHelper.OnlineTranslators
{
    /// <summary>
    /// Class defining user agents for various browsers.
    ///
    /// Updated 2019-02-09.
    /// </summary>
    public static class UserAgents
    {
        /// <summary>
        /// Latest Chrome Win10 user-agent as of 2020-01-04.
        /// </summary>
        public static readonly string Chrome_Win10_Latest = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36";

        /// <summary>
        /// Latest Chrome Win7 user-agent as of 2020-01-04.
        /// </summary>
        public static readonly string Chrome_Win7_Latest = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36";

        /// <summary>
        /// Latest Firefox Win10 user-agent as of 2020-01-04.
        /// </summary>
        public static readonly string Firefox_Win10_Latest = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:71.0) Gecko/20100101 Firefox/71.0";

        /// <summary>
        /// Latest Edge Win10 user-agent as of 2020-01-04.
        /// </summary>
        public static readonly string Edge_Win10_Latest = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362";

        /// <summary>
        /// Was working early with G before G started block it
        /// </summary>
        public static readonly string OperaMini = "Opera/9.80 (J2ME/MIDP; Opera Mini/5.1.21214/28.2725; U; en) Presto/2.8.119 Version/11.10";
    }
}
