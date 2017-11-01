using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame.Pool
{
    public class PoolObject_Actor : PoolObject
    {
        // instantiated
        public override void OnInstantiate()
        {
            base.OnInstantiate();

            var actor = gameObject.GetComponent<Actor>();

            if (actor != null)
            {
                actor.OnPoolInstiate();
            }
        }

        // Re Use
        public override void OnReuse()
        {
            base.OnReuse();

            var actor = gameObject.GetComponent<Actor>();

            if (actor != null)
            {
                actor.OnPoolReuse();
            }
        }

        // Release
        public override void OnRelease()
        {
            base.OnRelease();

            var actor = gameObject.GetComponent<Actor>();

            if (actor != null)
            {
                actor.OnPoolReleased();
            }
        }
    }
}
