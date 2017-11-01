using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    abstract public class Actor : MonoBehaviour
    {
        [HideInInspector]
        protected Team _team = null;
        public Team team { get { return _team; } }

        public SpriteRenderer spriteRenderer;

        public bool isSelecteable = true;
        static protected Color selectableColor = new Color(1, 1, 1, 1);
        static protected Color notSelectableColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        // SetTeam
        public virtual void SetTeam( Team newTeam )
        {
            if(_team == newTeam )
            {
                return;
            }

            _team = newTeam;
        }

        public virtual void SetSelectable(bool selectable)
        {
            if (isSelecteable == selectable) return;

            isSelecteable = selectable;

            if(spriteRenderer != null )
            {
                if (isSelecteable)
                {
                    spriteRenderer.color = selectableColor;
                }
                else
                {
                    spriteRenderer.color = notSelectableColor;
                }
            }
        }

        // instantiated
        public virtual void OnPoolInstiate()
        {
        }

        // Re Use
        public virtual void OnPoolReuse()
        {
            SetSelectable(true);
        }

        // Release
        public virtual void OnPoolReleased()
        {
            SetSelectable(false);
        }
    }
}
