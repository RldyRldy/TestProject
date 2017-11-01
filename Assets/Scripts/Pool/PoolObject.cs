using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame.Pool
{
    public class PoolObject : MonoBehaviour
    {
        // key from objectPoolManager. ( prefab )
        public GameObject keyObj = null;

        // instantiated
        public virtual void OnInstantiate()
        {
            gameObject.SetActive(false);
        }

        // Re Use
        public virtual void OnReuse()
        {
            gameObject.SetActive(true);
        }

        // Release
        public virtual void OnRelease()
        {
            gameObject.SetActive(false);
        }
    }
}
