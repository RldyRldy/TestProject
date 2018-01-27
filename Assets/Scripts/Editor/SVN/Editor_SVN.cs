using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

using UnityEngine;
using UnityEditor;

namespace EditorSVN
{
    public static class SVN
    {
        // log tag
        private const string CONST_SVN_LOG_TAG = "[SVN]";

        public static string m_strProcPath = string.Empty;

        public static string MakeCommand(string Command, params string[] _params)
        {
            string Cmd = @"/c TortoiseProc.exe /command:" + Command;

            foreach (var _param in _params)
            {
                Cmd += " " + _param;
            }

            Cmd += " /closeonend:0";

            return Cmd;
        }

        public static void ExecuteSVNCommand(string SVNCommand)
        {
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
    }
}