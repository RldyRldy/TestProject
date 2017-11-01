using System.Collections.Generic;
using UnityEngine;

namespace JGame.Pool
{
    public class ObjectPoolManager : MonoBehaviour
    {
        // instance
        private static ObjectPoolManager _instance = null;
        public static ObjectPoolManager instance {
            get
            {
                if (_instance == null) CreateInstance();

                return _instance;
            }
        }

        private Dictionary<int, Queue<GameObject>> pool = new Dictionary<int, Queue<GameObject>>();
        
        // Create Instance
        protected static void CreateInstance()
        {
            if (_instance != null) return;

            GameObject obj = new GameObject();

            if (obj != null)
            {
                obj.name = "Object Pool";
                _instance = obj.AddComponent<ObjectPoolManager>();
            }
        }

        // get object from object pool
        public GameObject Pop(GameObject keyObject,Vector3 pos)
        {
            GameObject obj = Pop(keyObject);
            obj.transform.position = pos;
            return obj;
        }

        public GameObject Pop(GameObject keyObject, Vector3 pos, Quaternion rot)
        {
            GameObject obj = Pop(keyObject);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            return obj;
        }

        public GameObject Pop(GameObject keyObject)
        {
            int key = keyObject.GetInstanceID();
            GameObject obj = null;

            // has key??
            if (!pool.ContainsKey(key))
            {
                pool.Add(key, new Queue<GameObject>());
            }

            // if has any obj, get it!
            if (pool[key].Count > 0)
            {
                obj = pool[key].Dequeue();
            }

            // if don't have any pool object.
            // instantiate new object
            if (obj == null)
            {
                obj = Instantiate(keyObject);

                if (obj != null)
                {
                    // add pool object
                    PoolObject po = AddPoolObjectComponent(obj);
                    if (po != null)
                    {
                        po.keyObj = keyObject;
                        po.OnInstantiate();
                    }

                    obj.transform.SetParent(gameObject.transform);
                }
            }

            // call reuse
            if (obj != null)
            {
                PoolObject po = obj.GetComponent<PoolObject>();
                if (po != null)
                {
                    po.OnReuse();
                }
            }
            return obj;
        }

        // push object to pool
        public void Push(GameObject obj)
        {
            PoolObject po = obj.GetComponent<PoolObject>();

            // if is not pool object. destroy
            if (po == null || po.keyObj == null)
            {
                Destroy(obj);
                return;
            }

            int key = po.keyObj.GetInstanceID();

            // has key?
            if (!pool.ContainsKey(key))
            {
                pool.Add(key, new Queue<GameObject>());
            }

            // release
            if (po != null)
            {
                po.OnRelease();
            }

            // SetParent( this )
            obj.transform.SetParent(gameObject.transform);

            // push
            pool[key].Enqueue(obj);
        }

        // add pool object component
        protected PoolObject AddPoolObjectComponent(GameObject obj)
        {
            // check actor
            var actorComp = obj.GetComponent<Actor>();
            if( actorComp != null )
            {
                return obj.AddComponent<PoolObject_Actor>();
            }

            // check ui
            var uiComp = obj.GetComponent<UIObject>();
            if( uiComp != null )
            {
                return obj.AddComponent<PoolObject_UI>();
            }

            return obj.AddComponent<PoolObject>();
        }
    }
}