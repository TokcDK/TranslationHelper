using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THConfig.Interfaces.SettingsGroups
{
    public interface IToolsSettings
    {
        [DefaultValue(true)]
        bool UseOnlineTranslationCache { get; set; }
        [DefaultValue("https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}")]
        string WebTranslationLink { get; set; }
        [DefaultValue("G|https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}||D|https://www.deepl.com/ru/translator#{from}/{to}/{text}||Y|https://translate.yandex.com/?lang={from}-{to}&text={text}")]
        string WebTranslationLinkServices { get; set; }
        [DefaultValue(true)]
        bool SearchRowIssueOptionsCheckProjectSpecific { get; set; }
        [DefaultValue(true)]
        bool SearchRowIssueOptionsCheckAnyLineTranslatable { get; set; }
        [DefaultValue(true)]
        bool SearchRowIssueOptionsCheckNonRomaji { get; set; }
        [DefaultValue(true)]
        bool SearchRowIssueOptionsCheckActors { get; set; }
    }
}
