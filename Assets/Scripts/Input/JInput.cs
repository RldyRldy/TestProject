using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    public class JInputManager
    {
        protected static JInput _instance;
        public static JInput instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance();
                }

                return _instance;
            }
        }

        static JInput CreateInstance()
        {
#if UNITY_EDITOR
            return new PCInput();
#elif UNITY_ANDROID || UNITY_IOS
            return new MobileInput();
#else
            return new PCInput();
#endif
        }

        public static bool ButtonDown(int btIndex)
        {
            return instance.ButtonDown(btIndex);
        }

        public static bool ButtonUp(int btIndex)
        {
            return instance.ButtonUp(btIndex);
        }

        public static Vector3 GetScreenPosition(int btIndex)
        {
            return instance.GetScreenPosition(btIndex);
        }

        public static bool IsResizing()
        {
            return instance.IsResizing();
        }

        public static float GetResizingDelta()
        {
            return instance.GetResizingDelta() * JInput.resizeFactor;
        }

        public static bool IsButtonDragging(int btIndex)
        {
            return instance.IsButtonDragging(btIndex);
        }

        public static Vector2 GetButtonDelta( int btIndex)
        {
            return instance.GetButtonDelta(btIndex) * JInput.buttonDeltaFactor;
        }
    }

    abstract public class JInput
    {
        public static float resizeFactor;
        public static float buttonDeltaFactor;

        public abstract bool IsResizing();
        public abstract bool ButtonDown(int btIndex);
        public abstract bool ButtonUp(int btIndex);
        public abstract Vector3 GetScreenPosition(int btIndex);
        public abstract float GetResizingDelta();
        public abstract bool IsButtonDragging(int btIndex);
        public abstract Vector2 GetButtonDelta(int btIndex);
    }

    public class PCInput : JInput
    {
        public PCInput()
        {
            resizeFactor = 2.0f;
            buttonDeltaFactor = 1.0f;
        }

        public override bool IsResizing()
        {
            return (Input.GetAxis("Mouse ScrollWheel") != 0f);
        }

        public override bool ButtonDown(int btIndex)
        {
            return Input.GetMouseButtonDown(btIndex);
        }

        public override bool ButtonUp(int btIndex)
        {
            return Input.GetMouseButtonUp(btIndex);
        }

        public override Vector3 GetScreenPosition(int btIndex)
        {
            return Input.mousePosition;
        }

        public override float GetResizingDelta()
        {
            return -Input.GetAxis("Mouse ScrollWheel");
        }

        public override bool IsButtonDragging(int btIndex)
        {
            return (Input.GetMouseButton(btIndex) && (GetButtonDelta(btIndex).magnitude > 0));
        }

        public override Vector2 GetButtonDelta(int btIndex)
        {
            Vector2 delta = new Vector2();

            delta.x = Input.GetAxis("Mouse X");
            delta.y = Input.GetAxis("Mouse Y");

            return delta;
        }
    }

    public class MobileInput : JInput
    {
        public MobileInput()
        {
            resizeFactor = 0.01f;
            buttonDeltaFactor = 0.01f;
        }

        public override bool IsResizing()
        {
            return (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved);
        }

        public override bool ButtonDown(int btIndex)
        {
            return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        }

        public override bool ButtonUp(int btIndex)
        {
            return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
        }

        public override Vector3 GetScreenPosition(int btIndex)
        {
            Vector3 retval = new Vector3();
            if( Input.touchCount > btIndex )
            {
                retval.x = Input.GetTouch(btIndex).position.x;
                retval.y = Input.GetTouch(btIndex).position.y;
            }

            return retval;
        }

        public override float GetResizingDelta()
        {
            var curDist = Input.GetTouch(0).position - Input.GetTouch(1).position; //current distance between finger touches
            var prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition)); //difference in previous locations using delta positions
            return prevDist.magnitude - curDist.magnitude;
        }

        public override bool IsButtonDragging( int btIndex )
        {
            return (Input.touchCount > btIndex && Input.GetTouch(btIndex).phase == TouchPhase.Moved);
        }

        public override Vector2 GetButtonDelta( int btIndex )
        {
            return Input.GetTouch(btIndex).deltaPosition;
        }
    }
}