﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tablet.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("net.tcp://localhost:8010")]
        public string ServerAddress {
            get {
                return ((string)(this["ServerAddress"]));
            }
            set {
                this["ServerAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int UserId {
            get {
                return ((int)(this["UserId"]));
            }
            set {
                this["UserId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Video data processing ({0}%)")]
        public string Panopticon_Processing_Label {
            get {
                return ((string)(this["Panopticon_Processing_Label"]));
            }
            set {
                this["Panopticon_Processing_Label"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Video downloading ({0}%)")]
        public string Panopticon_Downloading_Label {
            get {
                return ((string)(this["Panopticon_Downloading_Label"]));
            }
            set {
                this["Panopticon_Downloading_Label"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("net.tcp://localhost:8020")]
        public string FileServerAddress {
            get {
                return ((string)(this["FileServerAddress"]));
            }
            set {
                this["FileServerAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Click here to get started")]
        public string Register_Instructions {
            get {
                return ((string)(this["Register_Instructions"]));
            }
            set {
                this["Register_Instructions"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Something interesting happened")]
        public string MarkButtonLabel {
            get {
                return ((string)(this["MarkButtonLabel"]));
            }
            set {
                this["MarkButtonLabel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Start round")]
        public string RoundTimeStartLabel {
            get {
                return ((string)(this["RoundTimeStartLabel"]));
            }
            set {
                this["RoundTimeStartLabel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Next round")]
        public string RoundTimerNextLabel {
            get {
                return ((string)(this["RoundTimerNextLabel"]));
            }
            set {
                this["RoundTimerNextLabel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Comment")]
        public string Comment {
            get {
                return ((string)(this["Comment"]));
            }
            set {
                this["Comment"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Cancel")]
        public string Cancel {
            get {
                return ((string)(this["Cancel"]));
            }
            set {
                this["Cancel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Finish game")]
        public string Finish {
            get {
                return ((string)(this["Finish"]));
            }
            set {
                this["Finish"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Please enter your group\'s research question on one of the tablets")]
        public string ResearchQuestionLabel {
            get {
                return ((string)(this["ResearchQuestionLabel"]));
            }
            set {
                this["ResearchQuestionLabel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Submit")]
        public string ResearchQuestionSubmitLabel {
            get {
                return ((string)(this["ResearchQuestionSubmitLabel"]));
            }
            set {
                this["ResearchQuestionSubmitLabel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Are you sure your would like to go on to the next round?")]
        public string AreYouSure {
            get {
                return ((string)(this["AreYouSure"]));
            }
            set {
                this["AreYouSure"] = value;
            }
        }
    }
}