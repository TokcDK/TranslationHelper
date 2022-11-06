using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THConfig.Groups
{
    public interface IToolsSettings
    {
        [DefaultValue("True")]
        bool UseOnlineTranslationCache { get; set; }
        [DefaultValue("https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}")]
        string WebTranslationLink { get; set; }
        [DefaultValue("G|https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}||D|https://www.deepl.com/ru/translator#{from}/{to}/{text}||Y|https://translate.yandex.com/?lang={from}-{to}&text={text}")]
        string WebTranslationLinkServices { get; set; }
        [DefaultValue("True")]
        bool SearchRowIssueOptionsCheckProjectSpecific { get; set; }
        [DefaultValue("True")]
        bool SearchRowIssueOptionsCheckAnyLineTranslatable { get; set; }
        [DefaultValue("True")]
        bool SearchRowIssueOptionsCheckNonRomaji { get; set; }
        [DefaultValue("True")]
        bool SearchRowIssueOptionsCheckActors { get; set; }
    }
}
