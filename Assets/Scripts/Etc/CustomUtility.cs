using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CustomUtility
{
    // Create dont destroy object
    static public T CreateDontDestroyObject<T>(string name = "") where T : MonoBehaviour
    {
        GameObject obj;
        T comp = null;

        obj = new GameObject();

        if (obj != null)
        {
            if (name.Length <= 0)
            {
                obj.name = typeof(T).Name;
            }
            else
            {
                obj.name = name;
            }
            comp = obj.AddComponent<T>();
            GameObject.DontDestroyOnLoad(obj);
        }

        return comp;
    }
}
