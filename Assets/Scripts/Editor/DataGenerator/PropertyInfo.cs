using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataGenerator
{
    struct stProp
    {
        Type _type;
        string _native_Name;
        string _value;

        public stProp(Type t)
        {
            _type = t;
            _native_Name = t.ToString();
            _value = "0";
        }

        public stProp(Type t,string defaultVal)
        {
            _type = t;
            _native_Name = t.ToString();
            _value = defaultVal;
        }

        public stProp(Type t, string nativeName, string defaultVal )
        {
            _type = t;
            _native_Name = nativeName;
            _value = defaultVal;
        }

        string GetName( bool bNative )
        {
            if( bNative == false || _native_Name.Length == 0 )
            {
                _type.ToString();
            }

            return _native_Name;
        }
    }

    struct stClass
    {
        Dictionary<string,stProp> _properties;
    }
}