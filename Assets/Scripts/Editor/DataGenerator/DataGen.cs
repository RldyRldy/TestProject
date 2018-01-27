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

        protected Dictionary<MemberTypes, List<MemberInfo>> m_listMebmerInfo = new Dictionary<MemberTypes, List<MemberInfo>>();

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
            
            var members = _class.GetMembers(flags);

            var _memberTypes = (MemberTypes [])Enum.GetValues(typeof(MemberTypes));

            m_listMebmerInfo.Clear();

            foreach( var _type in _memberTypes )
            {
                m_listMebmerInfo.Add(_type, new List<MemberInfo>());
            }

            foreach (var _member in members)
            {
                m_listMebmerInfo[MemberTypes.All].Add(_member);

                foreach (var _type in _memberTypes)
                {
                    if(_type != MemberTypes.All && (_member.MemberType & _type) != 0 )
                    {
                        m_listMebmerInfo[_type].Add(_member);
                    }
                }
            }
        }

        #region OnGUI
        public void OnGUI_Data()
        {
            GUILayout.Label(m_strFullName + (Exists?" (Exists)":""), EditorStyles.boldLabel);
            GUILayout.Space(1.0f);

            GUILayout.Label(m_listMebmerInfo[MemberTypes.All].Count.ToString());
            foreach (var _prop in m_listMebmerInfo[MemberTypes.All])
            {
                GUILayout.Label(_prop.ToString());
            }           
        }
        #endregion // OnGUI
    }
}