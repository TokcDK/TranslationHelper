using System;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Formats.Abstractions;
using TranslationHelper.Formats.WolfRPG;

namespace TranslationHelper.Formats.RPGMTrans
{
    abstract class RPGMTransPatchBase : RPGMWolfTransPatchBase
    {
        protected RPGMTransPatchBase(IFormatHost host) : base(host)
        {
        }
    }
}
