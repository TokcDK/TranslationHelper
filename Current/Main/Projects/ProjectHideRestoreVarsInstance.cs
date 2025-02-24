using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Projects
{
    class ProjectHideRestoreVarsInstance : IDisposable
    {
        /// <summary>
        /// list of variables for hide
        /// </summary>
        Dictionary<string, string> _hideVARSPatterns;
        /// <summary>
        /// list of found matches collections
        /// </summary>
        List<MatchCollection> _hideVARSMatchCollectionsList;

        int mcArrNum;

        public ProjectHideRestoreVarsInstance(Dictionary<string, string> hideVarsBase)
        {
            this._hideVARSPatterns = hideVarsBase;
        }

        internal string HideVARSBase(string str)
        {
            if (_hideVARSPatterns == null || _hideVARSPatterns.Count == 0) return str;

            var keyfound = false;
            foreach (var key in _hideVARSPatterns.Keys)
            {
                if (str.Contains(key)) { keyfound = true; break; }
            }
            if (!keyfound) return str;

            var mc = Regex.Matches(str, "(" + string.Join(")|(", _hideVARSPatterns.Values) + ")");
            if (mc.Count == 0) return str;

            if (_hideVARSMatchCollectionsList == null)//init list
                _hideVARSMatchCollectionsList = new List<MatchCollection>();

            if (mcArrNum != 0)//reset vars count
                mcArrNum = 0;

            _hideVARSMatchCollectionsList.Add(mc);

            for (int m = mc.Count - 1; m >= 0; m--)
            {
                try
                {
                    str = str.Remove(mc[m].Index, mc[m].Value.Length).Insert(mc[m].Index, "{VAR" + m.ToString("000") + "}");
                }
                catch { }
            }

            return str;
        }

        internal string RestoreVARS(string str)
        {
            if (_hideVARSMatchCollectionsList == null || _hideVARSMatchCollectionsList.Count == 0 || !Regex.IsMatch(str, @"\{ ?VAR ?([0-9]{3}) ?\}", RegexOptions.IgnoreCase) || _hideVARSMatchCollectionsList[mcArrNum].Count == 0)
            {
                return str;
            }

            //restore broken vars
            str = Regex.Replace(str, @"\{ ?VAR ?([0-9]{3}) ?\}", "{VAR$1}", RegexOptions.IgnoreCase);

            int mi = 0;
            foreach (Match m in _hideVARSMatchCollectionsList[mcArrNum])
            {
                str = str.Replace("{VAR" + mi.ToString("000") + "}", m.Value);
                mi++;
            }
            mcArrNum++;
            if (mcArrNum == _hideVARSMatchCollectionsList.Count)
            {
                _hideVARSMatchCollectionsList.Clear();
            }
            return str;
        }

        internal void Clear()
        {
            _hideVARSMatchCollectionsList.Clear();
            _hideVARSPatterns.Clear();
        }

        public void Dispose()
        {
            Clear();

            _hideVARSMatchCollectionsList = null;
            _hideVARSPatterns = null;
        }
    }
}
