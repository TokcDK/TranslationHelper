﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TranslationHelper.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DontLoadStringIfRomajiPercent {
            get {
                return ((bool)(this["DontLoadStringIfRomajiPercent"]));
            }
            set {
                this["DontLoadStringIfRomajiPercent"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("90")]
        public int DontLoadStringIfRomajiPercentNum {
            get {
                return ((int)(this["DontLoadStringIfRomajiPercentNum"]));
            }
            set {
                this["DontLoadStringIfRomajiPercentNum"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutotranslationForSimular {
            get {
                return ((bool)(this["AutotranslationForSimular"]));
            }
            set {
                this["AutotranslationForSimular"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsTranslationCacheEnabled {
            get {
                return ((bool)(this["IsTranslationCacheEnabled"]));
            }
            set {
                this["IsTranslationCacheEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsFullComprasionDBloadEnabled {
            get {
                return ((bool)(this["IsFullComprasionDBloadEnabled"]));
            }
            set {
                this["IsFullComprasionDBloadEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://translate.google.com/?ie=UTF-8&op=translate&sl=auto&tl=en&text={text}")]
        public string WebTranslationLink {
            get {
                return ((string)(this["WebTranslationLink"]));
            }
            set {
                this["WebTranslationLink"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Japanese")]
        public string OnlineTranslationSourceLanguage {
            get {
                return ((string)(this["OnlineTranslationSourceLanguage"]));
            }
            set {
                this["OnlineTranslationSourceLanguage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("English")]
        public string OnlineTranslationTargetLanguage {
            get {
                return ((string)(this["OnlineTranslationTargetLanguage"]));
            }
            set {
                this["OnlineTranslationTargetLanguage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool THdebug {
            get {
                return ((bool)(this["THdebug"]));
            }
            set {
                this["THdebug"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsTranslationHelperWasClosed {
            get {
                return ((bool)(this["IsTranslationHelperWasClosed"]));
            }
            set {
                this["IsTranslationHelperWasClosed"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int THOptionLineCharLimit {
            get {
                return ((int)(this["THOptionLineCharLimit"]));
            }
            set {
                this["THOptionLineCharLimit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"\"")]
        public string THSelectedGameDir {
            get {
                return ((string)(this["THSelectedGameDir"]));
            }
            set {
                this["THSelectedGameDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"\"")]
        public string THSelectedDir {
            get {
                return ((string)(this["THSelectedDir"]));
            }
            set {
                this["THSelectedDir"] = value;
            }
        }
    }
}
