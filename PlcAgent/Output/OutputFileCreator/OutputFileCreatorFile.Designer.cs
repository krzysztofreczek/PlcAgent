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
    internal sealed partial class OutputFileCreatorFile : global::System.Configuration.ApplicationSettingsBase {
        
        private static OutputFileCreatorFile defaultInstance = ((OutputFileCreatorFile)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new OutputFileCreatorFile())));
        
        public static OutputFileCreatorFile Default {
            get {
                return defaultInstance;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<ArrayOfInt>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                        <int>0</int>
                                                                    </ArrayOfInt>")]
        public int[] SelectedIndex
        {
            get { return ((int[])(this["SelectedIndex"])); }
            set { this["SelectedIndex"] = value; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<ArrayOfString>
                                                                        <string>noName</string>
                                                                        <string>noName</string>
                                                                        <string>noName</string>
                                                                        <string>noName</string>
                                                                        <string>noName</string>
                                                                        <string>noName</string>
                                                                        <string>noName</string>
                                                                        <string>noName</string>
                                                                        <string>noName</string>
                                                                    </ArrayOfString>")]
        public string[] FileNameSuffixes
        {
            get { return ((string[])(this["FileNameSuffixes"])); }
            set { this["FileNameSuffixes"] = value; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<ArrayOfString>
                                                                        <string>Output</string>
                                                                        <string>Output</string>
                                                                        <string>Output</string>
                                                                        <string>Output</string>
                                                                        <string>Output</string>
                                                                        <string>Output</string>
                                                                        <string>Output</string>
                                                                        <string>Output</string>
                                                                        <string>Output</string>
                                                                    </ArrayOfString>")]
        public string[] DirectoryPaths
        {
            get { return ((string[])(this["DirectoryPaths"])); }
            set { this["DirectoryPaths"] = value; }
        }
    }
}
