using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor;

using GD;
using SGD;
using UD;
using SUD;

namespace GD
{
    public class GD_Test2 : GD_Test
    {
        public int i = 0;
    }
}

namespace DataGenerator
{
    // file ( .cpp, .h, .cs, .ndt ) version
    public enum File_Ver
    {
        none = 0,
        current,
    }

    public class DataGenerator_Data
    {
        // namespace
        public string m_strNamespace = string.Empty;
        // class prefix
        public string m_strClassPrefix = string.Empty;
        // class name
        public string m_strClassName = string.Empty;
        // full name ( NAMESPACE.PREFIX_CLASSNAME )
        public string m_strFullName = string.Empty;

        protected Type m_pClassType = null;
        protected List<FieldInfo> m_arrFieldInfo = new List<FieldInfo>();
        protected List<PropertyInfo> m_arrPropertyInfo = new List<PropertyInfo>();
        protected List<MethodInfo> m_arrMethodInfo = new List<MethodInfo>();        

        public bool Exists { get; set; }

        public DataGenerator_Data(string _namesapce, string _prefix, string _className)
        {
            m_strNamespace = _namesapce;
            m_strClassPrefix = _prefix;
            m_strClassName = _className;

            m_strFullName = string.Format("{0}.{1}_{2}", m_strNamespace, m_strClassPrefix, m_strClassName);
        }

        public void Load()
        {
            Type _Class = null;

            var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                var assembly = System.Reflection.Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    _Class = assembly.GetType(m_strFullName);
                    if (_Class != null)
                        break;
                }
            }

            Exists = (_Class != null );

            if( Exists )
            {
                Load(_Class);
            }
        }

        private void Load( Type _class )
        {
            var flags = (BindingFlags)Int32.MaxValue;// (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );

            var fields = _class.GetFields(flags);
            var properties = _class.GetProperties(flags);
            var methods = _class.GetMethods(flags);

            foreach (var _field in fields)
            {
                m_arrFieldInfo.Add(_field);
            }

            foreach ( var _property in properties)
            {
                m_arrPropertyInfo.Add(_property);
            }

            foreach(var _method in methods )
            {
                m_arrMethodInfo.Add(_method);
            }
        }
        #region OnGUI
        public void OnGUI_Data()
        {
            GUILayout.Label(m_strFullName + (Exists?" (Exists)":""), EditorStyles.boldLabel);
            GUILayout.Space(1.0f);

            GUILayout.Label(m_arrPropertyInfo.Count.ToString());
            foreach( var _prop in m_arrPropertyInfo )
            {
                GUILayout.Label(_prop.Name);
            }

            GUILayout.Label(m_arrFieldInfo.Count.ToString());
            foreach (var _prop in m_arrFieldInfo)
            {
                GUILayout.Label(_prop.Name);
            }

            GUILayout.Label(m_arrMethodInfo.Count.ToString());
            foreach (var _prop in m_arrMethodInfo)
            {
                GUILayout.Label(_prop.Name);
            }            
        }
        #endregion // OnGUI
    }
}