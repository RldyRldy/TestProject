using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    abstract public class UIObject : MonoBehaviour
    {
        private void Awake()
        {
            InitFromAwake();
        }

        // Use this for initialization
        void Start()
        {
            InitFromStart();
            if(UIManager.instance != null )
                UIManager.instance.AddUIObject(this);
        }

        protected virtual void InitFromAwake()
        {
            
        }

        protected virtual void InitFromStart()
        {
            Resizing();
        }

        // instantiated
        public virtual void OnPoolInstiate()
        {
        }

        // Re Use
        public virtual void OnPoolReuse()
        {
            if (UIManager.instance != null)
                UIManager.instance.AddUIObject(this);
        }

        // Release
        public virtual void OnPoolReleased()
        {
            if (UIManager.instance != null)
                UIManager.instance.RemoveUIObject(this);
        }

        public abstract void Resizing();
    }

}