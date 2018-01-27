using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace DataGenerator
{
    public class EdWnd_DataGnerator : EditorWindow
    {
        // editor window version
        public enum ED_Ver
        {
            none = 0,
            current,
        }

        public enum ED_Tab
        {
            GD,
            UD,
        }

        readonly string CONST_NATIVE_NULL = "nullptr";
        readonly string CONST_NULL = "null";

        #region Path
        // .cs
        string path_Script = string.Empty;
        string ED_PATH_SCRIPT
        {
            get
            {
                return EditorPrefs.GetString("ED_DataGen_ScriptPath", "../");
            }

            set
            {
                EditorPrefs.SetString("ED_DataGen_ScriptPath", value);
            }
        }

        // .cpp, .h
        string path_Native = string.Empty;
        string ED_PATH_NATIVE
        {
            get
            {
                return EditorPrefs.GetString("ED_DataGen_NativePath", "../");
            }

            set
            {
                EditorPrefs.SetString("ED_DataGen_NativePath", value);
            }
        }

        // .ndt
        string path_FileDB = string.Empty;
        string ED_PATH_FILEDB
        {
            get
            {
                return EditorPrefs.GetString("ED_DataGen_FileDBPath", "../");
            }

            set
            {
                EditorPrefs.SetString("ED_DataGen_FileDBPath", value);
            }
        }
        #endregion // Path

        // class name
        string m_strClassName = "ClassName";

        // is system data?
        bool bIsSystemData = false;

        // window scroll position
        Vector2 m_sclMain = new Vector2();

        // current tab
        int m_nTab = (int)ED_Tab.GD;

        // data box scroll position
        Vector2 m_sclData = new Vector2();

        DataGenerator_Data m_gdInfo = null;
        DataGenerator_Data m_udInfo = null;

        [MenuItem("Window/Data_Gnerator")]
        static void Init()
        {
            EdWnd_DataGnerator wnd = (EdWnd_DataGnerator)GetWindow(typeof(EdWnd_DataGnerator), false, "DataGen");
            wnd.Initialize();
            wnd.Show();
        }

        public void Initialize()
        {
            path_Script = ED_PATH_SCRIPT;
            path_Native = ED_PATH_NATIVE;
            path_FileDB = ED_PATH_FILEDB;
        }

        private void OnGUI()
        {
            m_sclMain = EditorGUILayout.BeginScrollView(m_sclMain);
            GUILayout.Space(3.0f);

            OnGUI_Paths();

            GUILayout.Space(2.0f);

            OnGUI_Class();

            GUILayout.Space(2.0f);

            OnGUI_Data();

            GUILayout.Space(2.0f);

            OnGUI_Execute();

            EditorGUILayout.EndScrollView();
        }

        #region OnGUI_func
        private void OnGUI_Paths()
        {
            GUILayout.Label("Path", EditorStyles.boldLabel);
            GUILayout.Space(1.0f);

            // cs path
            OnGUI_LayoutPath("Script Path", path_Script, (string newPath) =>
            {
                if (newPath.Length > 0)
                {
                    ED_PATH_SCRIPT = path_Script = ToRelativePath(newPath);
                }
            });
            GUILayout.Space(1.0f);

            // cpp , h path
            OnGUI_LayoutPath("Native Path", path_Native, (string newPath) =>
            {
                if (newPath.Length > 0)
                {
                    ED_PATH_NATIVE = path_Native = ToRelativePath(newPath);
                }
            });
            GUILayout.Space(1.0f);

            // ndt path
            OnGUI_LayoutPath("FileDB Path", path_FileDB, (string newPath) =>
            {
                if (newPath.Length > 0)
                {
                    ED_PATH_FILEDB = path_FileDB = ToRelativePath(newPath);
                }
            });
        }

        private void OnGUI_Class()
        {
            GUILayout.Label("Class", EditorStyles.boldLabel);
            GUILayout.Space(1.0f);

            // ClassName
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("ClassName", GUILayout.Width(80.0f));
                m_strClassName = GUILayout.TextField(m_strClassName);
            }
            EditorGUILayout.EndHorizontal();

            // is System Data ?
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("System Data", GUILayout.Width(80.0f));
                bIsSystemData = EditorGUILayout.Toggle(bIsSystemData);
            }
            EditorGUILayout.EndHorizontal();

            // Load 
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Load", GUILayout.Width(80.0f)))
                {
                    m_gdInfo = new DataGenerator_Data( (bIsSystemData? "SGD" : "GD" ), "GD",m_strClassName);
                    m_udInfo = new DataGenerator_Data( (bIsSystemData? "SUD" : "UD" ), "UD", m_strClassName);
                    m_gdInfo.Load();
                    m_udInfo.Load();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnGUI_Data()
        {
            var arrTab = new ArrayList();

            foreach (var _tab in Enum.GetValues(typeof(ED_Tab)))
            {
                arrTab.Add(_tab.ToString());
            }

            var tab = GUILayout.Toolbar(m_nTab, (string[])arrTab.ToArray(typeof(string)));

            if (m_nTab != tab)
            {
                m_nTab = tab;
            }

            EditorGUILayout.BeginHorizontal(GUILayout.Height(450.0f));
            {
                m_sclData = EditorGUILayout.BeginScrollView(m_sclData);
                {
                    switch ((ED_Tab)m_nTab)
                    {
                        case ED_Tab.GD:
                            if(m_gdInfo != null )
                            {
                                m_gdInfo.OnGUI_Data();
                            }
                            break;
                        case ED_Tab.UD:
                            if( m_udInfo != null )
                            {
                                m_udInfo.OnGUI_Data();
                            }
                            break;
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnGUI_Execute()
        {
            if (GUILayout.Button("Create Files", GUILayout.Height(50.0f)))
            {
                OpenFolder(path_Script);
                OpenFolder(path_Native);
                OpenFolder(path_FileDB);
            }
        }

        private void OnGUI_LayoutPath(string _title, string _path, Action<string> _cbChangePath)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // title
                GUILayout.Label(_title, GUILayout.Width(80.0f));

                // path
                GUILayout.TextArea(_path);

                // find path
                if (GUILayout.Button("...", GUILayout.Width(80.0f)))
                {
                    var _newPath = EditorUtility.OpenFolderPanel(_title, _path, "");

                    if (_cbChangePath != null)
                    {
                        _cbChangePath(_newPath);
                    }
                }

                // open path
                if (GUILayout.Button("Open", GUILayout.Width(80.0f)))
                {
                    OpenFolder(_path);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion // OnGUI_func

        #region utility
        object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        // get default value to string
        string GetDefaultString(Type type, bool bNative)
        {
            var value = GetDefault(type);

            if (value != null)
            {
                return value.ToString();
            }

            return (bNative ? CONST_NATIVE_NULL : CONST_NULL);
        }

        string ToRelativePath(string absolutePath)
        {
            var fileUri = new Uri(absolutePath);
            var referenceUri = new Uri(Application.dataPath);
            return referenceUri.MakeRelativeUri(fileUri).ToString();
        }

        void OpenFolder(string Path)
        {
            Path = Path.Replace("/", @"\");
            System.Diagnostics.Process.Start("explorer.exe", Path);
        }
        #endregion //utility
    }
}