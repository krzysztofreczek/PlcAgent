﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _PlcAgent.Output.OutputFileCreator {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    public sealed partial class OutputFileCreatorInterfaceAssignmentFile : global::System.Configuration.ApplicationSettingsBase {
        
        private static OutputFileCreatorInterfaceAssignmentFile defaultInstance = ((OutputFileCreatorInterfaceAssignmentFile)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new OutputFileCreatorInterfaceAssignmentFile())));
        
        public static OutputFileCreatorInterfaceAssignmentFile Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string[][] Assignment {
            get {
                return ((string[][])(this["Assignment"]));
            }
            set {
                this["Assignment"] = value;
            }
        }
    }
}
