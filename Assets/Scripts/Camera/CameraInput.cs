using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    public class CameraInput : MonoBehaviour
    {
        protected bool bLockInput = false;

        // camera size min max. only Orthographic
        public Vector2 sizeRange = new Vector2(2.0f, 7.0f);
        // cam move area. top/bottom position ( world position )
        public Vector3 minPos = new Vector3(0.0f,-8.0f,0.0f);
        public Vector3 maxPos = new Vector3(0.0f, 10.0f, 0.0f);
        
        // cached camera
        protected Camera cam = null;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            // roate min,max position
            // diamond area -> square area
            minPos = Quaternion.Euler(0, 0, -45) * minPos;
            maxPos = Quaternion.Euler(0, 0, -45) * maxPos;
        }
        
        // process move
        public bool ProcessInput()
        {
            if (bLockInput)
                return false;

            // check scale input
            if ( IsScaleInput() )
            {
                var size = cam.orthographicSize;

                // resize
                size += GetScaleDelta();                
                cam.orthographicSize = ClampScale(size);

                return true;
            }
            // check move input
            else if( IsMoveInput() )
            {
                var curPos = transform.position;

                // move
                curPos.x -= GetDraggingDelta(0).x;
                curPos.y -= GetDraggingDelta(0).y;

                SetPosition(curPos);

                return true;
            }

            return false;
        }
        
        public void SetPosition( Vector3 pos )
        {
            SetPosition(pos.x, pos.y);
        }

        public void SetPosition( Vector2 pos )
        {
            SetPosition(pos.x, pos.y);
        }

        public void SetPosition( float x, float y )
        {
            var curPos = transform.position;

            curPos.x = x;
            curPos.y = y;

            transform.position = ClampPos(curPos);
        }

        protected bool IsScaleInput()
        {
            return JInputManager.IsResizing();
        }

        protected bool IsMoveInput()
        {
            return JInputManager.IsButtonDragging(0);
        }

        // get delta touch move.
        protected Vector2 GetDraggingDelta(int index)
        {
            return JInputManager.GetButtonDelta(index);
        }

        // get delta touch scaled
        protected float GetScaleDelta( )
        {
            return JInputManager.GetResizingDelta();
        }

        public void SetLockCameraInput(bool bLock)
        {
            bLockInput = bLock;
        }

        #region Clamp
        // Clamp
        protected Vector3 ClampPos(Vector3 pos)
        {
            pos = Quaternion.Euler(0, 0, -45) * pos;
            pos.x = Mathf.Clamp(pos.x, minPos.x, maxPos.x);
            pos.y = Mathf.Clamp(pos.y, minPos.y, maxPos.y);
            pos = Quaternion.Euler(0, 0, 45) * pos;

            return pos;
        }

        protected float ClampScale(float size)
        {
            return Mathf.Clamp(size, sizeRange.x, sizeRange.y);
        }
#endregion
    }
}
