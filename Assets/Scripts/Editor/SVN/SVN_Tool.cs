#if UNITY_EDITOR_WIN
//@TODO WINDOW 외 다른 OS 테스트
//      SVN 기능 -> SVN class
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEditor;

// Update
// Commit
// -------
// Add
// Delete
// Revert
// Rename
// -------
// Lock
// Unlock
// -------
// Show log

[InitializeOnLoad]
public static class SVN_Tools
{
    // svn editor version
    enum eSVN_Ed_Ver
    {
        NONE = 0,
        AddWorkSpace,
        Current,
    }

    // menu
    enum eSVNMenu
    {
        Line_00 = 0,
        Update,
        Commit,
        Line_01 = 50,
        Add,
        Delete,
        Revert,
        Rename,
        Line_02 = 100,
        Lock,
        Unlock,
        Line_03 = 150,
        Showlog,
        Repo_Browser,
        Test,
    }

    public class SVNRevInfo
    {
        public string m_strFileName = string.Empty;
        public Int64 curRev = 0;
        public Int64 finRev = 0;
        public bool bDiff = false;

        public SVNRevInfo(string fileName)
        {
            m_strFileName = fileName;
        }

        public void ReadData(string line)
        {
            if (line.Contains("revision"))
            {

            }
            //WCREV = int.Parse(streamrader.ReadLine());
            //WCDATE = int.Parse(streamrader.ReadLine());
            //WCRANGE = int.Parse(streamrader.ReadLine());
            //WCURL = int.Parse(streamrader.ReadLine());
            //WCNOW = int.Parse(streamrader.ReadLine());
        }
    }

    #region properties
    // Editor Prefab Key
    const string Key_Enable = "SVN_Editor_Enable";
    const string Key_Ed_Version = "SVN_Editor_Ver";
    const string Key_TortoisePoc = "SVN_TortoiseProc";
    const string Key_REPOSITORY = "SVN_Repository";
    const string Key_WORKSPACE = "SVN_WORKSPACE";

    //version
    private const eSVN_Ed_Ver Editor_Ver = eSVN_Ed_Ver.Current;

    // log tag
    private const string CONST_SVN_LOG_TAG = "[SVN]";
    private const string CONST_SVN_REV_INFO = "/../SVNRevInfo.local";

    // use svn ?
    static bool Enable_SVN = false;
    // svn proc path
    static string m_strProcPath = string.Empty;
    // repository path
    static string m_strRepoPath = string.Empty;
    // workspace path
    static string m_strWorkPath = string.Empty;

    static Dictionary<string, SVNRevInfo> m_listRevInfo = new Dictionary<string, SVNRevInfo>();
    #endregion // properties

    #region Editor Prefabs
    static bool EP_Enable_SVN
    {
        get
        {
            return (EditorPrefs.GetInt(Key_Enable, 0) == 1);
        }

        set
        {
            EditorPrefs.SetInt(Key_Enable, value ? 1 : 0);
        }
    }

    static int EP_Ed_VER
    {
        get
        {
            return EditorPrefs.GetInt(Key_Ed_Version, (int)eSVN_Ed_Ver.NONE);
        }
        set
        {
            EditorPrefs.SetInt(Key_Ed_Version, value);
        }
    }

    static string EP_TortoiseProc
    {
        get
        {
            return EditorPrefs.GetString(Key_TortoisePoc, string.Empty);
        }
        set
        {
            EditorPrefs.SetString(Key_TortoisePoc, value);
        }
    }

    static string EP_Repository
    {
        get
        {
            return EditorPrefs.GetString(Key_REPOSITORY, string.Empty);
        }
        set
        {
            EditorPrefs.SetString(Key_REPOSITORY, value);
        }
    }

    static string EP_Workspace
    {
        get
        {
            return EditorPrefs.GetString(Key_WORKSPACE, string.Empty);
        }
        set
        {
            EditorPrefs.SetString(Key_WORKSPACE, value);
        }
    }
    #endregion // Editor Prefabs

    // running on launch
    static SVN_Tools()
    {
        Initialize();
    }

    static void Initialize()
    {
        Enable_SVN = EP_Enable_SVN;

        // version
        CheckAndUpdate();

        // get info
        m_strProcPath = EP_TortoiseProc;
        m_strRepoPath = EP_Repository;
        m_strWorkPath = EP_Workspace;

        // check is alive
        if (Enable_SVN)
        {
            Enable_SVN = AliveSVN();

            if (!Enable_SVN)
            {
                EP_Enable_SVN = Enable_SVN;
            }
        }
    }

    static bool AliveSVN()
    {
        if (!Directory.Exists(m_strProcPath))
        {
            ShowMessage("Error", "SVN 비활성화. Tortoise 설치폴더를 지정해주세요.");
            return false;
        }

        return true;
    }

    // Add PreferenceItem
    [PreferenceItem("SVN")]
    static void PreferencesGUI()
    {
        Enable_SVN = EditorGUILayout.Toggle("Enable SVN", Enable_SVN);

        EditorGUILayout.Space();

        // Paths        
        GUILayout.Label("Path", EditorStyles.boldLabel);

        //EditorGUI.BeginDisabledGroup(!Enable_SVN);
        {
            float pathLabelWidth = 70.0f;

            // SVN Proc
            DrawPath("Tortoise", ref m_strProcPath, GUILayout.Width(pathLabelWidth));

            // Repository
            DrawPath("Repository", ref m_strRepoPath, GUILayout.Width(pathLabelWidth));

            // Workspace
            DrawPath("Workspace", ref m_strWorkPath, GUILayout.Width(pathLabelWidth));
        }
        //EditorGUI.EndDisabledGroup();

        // Save the preferences
        if (GUI.changed)
        {
            var oldEnable = EP_Enable_SVN;

            EP_Enable_SVN = Enable_SVN;
            if (Enable_SVN)
            {
                EP_TortoiseProc = m_strProcPath;
                EP_Repository = m_strRepoPath;
                EP_Workspace = m_strWorkPath;

                if (oldEnable != Enable_SVN)
                {
                    Initialize();
                }
            }
        }
    }

    #region GUI Func
    static void DrawPath(string text_Label, ref string path, params GUILayoutOption[] LabelOptions)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(text_Label, LabelOptions);
            path = EditorGUILayout.TextField(path);
            if (GUILayout.Button("Browser"))
            {
                var _newPath = EditorUtility.OpenFolderPanel(text_Label, path, "");

                if (_newPath != string.Empty)
                {
                    path = _newPath;
                }
            }
        }
        GUILayout.EndHorizontal();
    }
    #endregion // GUI Func

    #region SVN_Menu
    // update
    [MenuItem("Assets/SVN/Update", true)]
    static bool Menu_Checker_Update()
    {
        if (Enable_SVN == false) return false;
        return true;
    }
    [MenuItem("Assets/SVN/Update", false, (int)eSVNMenu.Update)]
    static void Menu_Update()
    {
        string Cmd = MakeCommand("update", GetPaths(GetSelectPaths()));
        ExecuteSVNCommand(Cmd);
    }
    // Commit
    [MenuItem("Assets/SVN/Commit", true)]
    static bool Menu_Checker_Commit()
    {
        if (Enable_SVN == false) return false;
        return true;
    }
    [MenuItem("Assets/SVN/Commit", false, (int)eSVNMenu.Commit)]
    static void Menu_Commit()
    {
        AssetDatabase.SaveAssets();
        string Cmd = MakeCommand("commit", GetPaths(GetSelectPaths(true)));
        ExecuteSVNCommand(Cmd);
    }

    // Add
    [MenuItem("Assets/SVN/Add", true)]
    static bool Menu_Checker_Add()
    {
        if (Enable_SVN == false) return false;
        return true;
    }

    [MenuItem("Assets/SVN/Add", false, (int)eSVNMenu.Add)]
    static void Menu_Add()
    {
        string Cmd = MakeCommand("add", GetPaths(GetSelectPaths(true)));
        ExecuteSVNCommand(Cmd);
    }
    // Delete
    [MenuItem("Assets/SVN/Delete", true)]
    static bool Menu_Checker_Delete()
    {
        if (Enable_SVN == false) return false;
        return true;
    }
    [MenuItem("Assets/SVN/Delete", false, (int)eSVNMenu.Delete)]
    static void Menu_Delete()
    {
        string Cmd = MakeCommand("remove", GetPaths(GetSelectPaths(true)));
        ExecuteSVNCommand(Cmd);
    }
    // Revert
    [MenuItem("Assets/SVN/Revert", true)]
    static bool Menu_Checker_Revert()
    {
        if (Enable_SVN == false) return false;
        return true;
    }
    [MenuItem("Assets/SVN/Revert", false, (int)eSVNMenu.Revert)]
    static void Menu_Revert()
    {
        string Cmd = MakeCommand("revert", GetPaths(GetSelectPaths(true)));
        ExecuteSVNCommand(Cmd);
    }
    // Rename
    [MenuItem("Assets/SVN/Rename", true)]
    static bool Menu_Checker_Rename()
    {
        if (Enable_SVN == false) return false;
        return true;
    }

    [MenuItem("Assets/SVN/Rename", false, (int)eSVNMenu.Rename)]
    static void Menu_Rename()
    {
        string Cmd = MakeCommand("rename", GetPaths(GetSelectPaths())); // meta도 같이 물어보므로 패스
        ExecuteSVNCommand(Cmd);
    }
    // Lock
    [MenuItem("Assets/SVN/Lock", true)]
    static bool Menu_Checker_Lock()
    {
        if (Enable_SVN == false) return false;
        return true;
    }

    [MenuItem("Assets/SVN/Lock", false, (int)eSVNMenu.Lock)]
    static void Menu_Lock()
    {
        string Cmd = MakeCommand("lock", GetPaths(GetSelectPaths()));
        ExecuteSVNCommand(Cmd);
    }
    // Unlock
    [MenuItem("Assets/SVN/Unlock", true)]
    static bool Menu_Checker_Unlock()
    {
        if (Enable_SVN == false) return false;
        return true;
    }

    [MenuItem("Assets/SVN/Unlock", false, (int)eSVNMenu.Unlock)]
    static void Menu_Unlock()
    {
        string Cmd = MakeCommand("unlock", GetPaths(GetSelectPaths()));
        ExecuteSVNCommand(Cmd);
    }
    // -------
    // Show log
    [MenuItem("Assets/SVN/Show log", true)]
    static bool Menu_Checker_Showlog()
    {
        if (Enable_SVN == false) return false;
        return true;
    }

    [MenuItem("Assets/SVN/Show log", false, (int)eSVNMenu.Showlog)]
    static void Menu_Showlog()
    {
        string Cmd = MakeCommand("log", GetPaths(GetSelectPaths()));
        ExecuteSVNCommand(Cmd);
    }
    // Repo-Browser
    [MenuItem("Assets/SVN/Repo Browser", true)]
    static bool Menu_Checker_RepBrowser()
    {
        if (Enable_SVN == false) return false;
        return true;
    }
    [MenuItem("Assets/SVN/Repo Browser", false, (int)eSVNMenu.Repo_Browser)]
    static void Menu_RepBrowser()
    {
        //string Cmd = MakeCommand("log", GetPaths(GetSelectPaths()));
        //ExecuteSVNCommand(Cmd);
    }
    // Test
    [MenuItem("Assets/SVN/Test", false, (int)eSVNMenu.Test)]
    static void Menu_Test()
    {
        string[] _Paths = GetSelectPaths();
        foreach (var _path in _Paths)
        {
            UpdateRevisionInfo(_path);
        }
    }
    #endregion

    #region SVN_Cmd
    static string[] GetSelectPaths(bool bWithMeta = false)
    {
        var _paths = new ArrayList();

        string _path;

        foreach (var _obj in Selection.objects)
        {
            if (_obj != null)
            {
                _path = AssetDatabase.GetAssetPath(_obj.GetInstanceID());
                _path = Path.Combine(Directory.GetCurrentDirectory(), _path);
                _path = _path.Replace("/", "\\");

                _paths.Add(_path);

                if (bWithMeta)
                {
                    _paths.Add(_path + ".meta");
                }
            }
        }

        return (string[])_paths.ToArray(typeof(string));
    }

    static string GetPaths(string[] _paths)
    {
        string Cmd = "/path:\"";

        foreach (var _path in _paths)
        {
            Cmd += _path + "*";
        }

        Cmd = Cmd.Remove(Cmd.Length - 1);// delete *
        Cmd += "\""; // add "

        return Cmd;
    }

    static string MakeCommand(string Command, params string[] _params)
    {
        string Cmd = @"/c TortoiseProc.exe /command:" + Command;

        foreach (var _param in _params)
        {
            Cmd += " " + _param;
        }

        Cmd += " /closeonend:0";

        return Cmd;
    }

    static void ExecuteSVNCommand(string SVNCommand)
    {
        if (!Enable_SVN)
        {
            ShowMessage("Error", "SVN 비활성화 \n [Edit]->[Preferences]->[SVN]");
            return;
        }

        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = m_strProcPath;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.UseShellExecute = false;
            processStartInfo.Arguments = SVNCommand;
            Process process = Process.Start(processStartInfo);
            process.WaitForExit();

            int eCode = process.ExitCode;
            if (eCode <= 0) // -1 = cancel
            {
                //Success
            }
            else
            {
                //Failed
                LogError(string.Format("Faield Cmd : {0} \n ErrorCode : {1}", SVNCommand, eCode));
            }
        }
        catch (Exception ex)
        {
            //Catch Exception
            LogError(ex.Message);
        }
    }

    private static void UpdateRevisionInfo(string targetFile)
    {
        // create file for revision info
        var localRevisionInfo = new FileInfo(Application.dataPath + CONST_SVN_REV_INFO);
        if (localRevisionInfo.Exists)
        {
            localRevisionInfo.Delete();
        }

        // write revisioninfo format
        using (StreamWriter filewriter = new StreamWriter(localRevisionInfo.FullName, false))
        {
            filewriter.WriteLine("$WCREV$");
            filewriter.WriteLine("$WCDATE$");
            filewriter.WriteLine("$WCRANGE$");
            filewriter.WriteLine("$WCURL$");
            filewriter.WriteLine("$WCNOW$");
        }

        // SubWCRev cmd
        string arg = string.Format(" {0} {1} {1}", targetFile, localRevisionInfo.FullName);

        // dododododo
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.FileName = "SubWCRev.exe";
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.Arguments = arg;
        p.Start();

        // read revision info 
        SVNRevInfo revinfo = new SVNRevInfo(targetFile);

        // read
        while (!p.StandardOutput.EndOfStream)
        {
            string line = p.StandardOutput.ReadLine();
            revinfo.ReadData(line);
            UnityEngine.Debug.Log("line : " + line);
        }

        // add to caching list
        if (!m_listRevInfo.ContainsKey(targetFile))
        {
            m_listRevInfo.Add(targetFile, null);
        }

        m_listRevInfo[targetFile] = revinfo;
    }
    #endregion //SVN_Cmd

    #region Message
    static private void ShowMessage(string title, string msg)
    {
        EditorUtility.DisplayDialog(title, msg, "OK");
    }
    #endregion // Message

    #region Log
    static void Log(string msg)
    {
        UnityEngine.Debug.Log(CONST_SVN_LOG_TAG + msg);
    }

    static void LogWarning(string msg)
    {
        UnityEngine.Debug.LogWarning(CONST_SVN_LOG_TAG + msg);
    }

    static void LogError(string msg)
    {
        UnityEngine.Debug.LogError(CONST_SVN_LOG_TAG + msg);
    }
    #endregion // log

    static void CheckAndUpdate()
    {
        eSVN_Ed_Ver _version = (eSVN_Ed_Ver)EP_Ed_VER;
        if (_version < Editor_Ver)
        {
            UpdateVersion(_version);
        }
    }

    static void UpdateVersion(eSVN_Ed_Ver _ver)
    {
        if (_ver <= eSVN_Ed_Ver.NONE)
        {
            var TestPath = @"C:\Program Files\TortoiseSVN\bin";
            if (Directory.Exists(TestPath))
            {
                EP_TortoiseProc = TestPath;
            }
        }

        if (_ver <= eSVN_Ed_Ver.AddWorkSpace)
        {
            // set workspace
        }

        // update
        EP_Ed_VER = (int)eSVN_Ed_Ver.Current;
    }
}
#endif // UNITY_EDITOR_WIN