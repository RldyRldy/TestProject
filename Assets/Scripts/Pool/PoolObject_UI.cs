using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame.Pool
{
    public class PoolObject_UI : PoolObject
    {
        // instantiated
        public override void OnInstantiate()
        {
            base.OnInstantiate();

            var ui = gameObject.GetComponent<UIObject>();

            if (ui != null)
            {
                ui.OnPoolInstiate();
            }
        }

        // Re Use
        public override void OnReuse()
        {
            base.OnReuse();

            var ui = gameObject.GetComponent<UIObject>();

            if (ui != null)
            {
                ui.OnPoolReuse();
            }
        }

        // Release
        public override void OnRelease()
        {
            base.OnRelease();

            var ui = gameObject.GetComponent<UIObject>();

            if (ui != null)
            {
                ui.OnPoolReleased();
            }
        }
    }
}
