using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    [System.Serializable]
    public enum UIPanel_AnimType
    {
        None,
        FromRight,
        FromLeft,
        ToRight,
        ToLeft,
        FromUpper,
        FromBottom,
        ToUpper,
        ToBottom,
        ScaleUp,
        ScaleDown,
    }

    [System.Serializable]
    public enum EUIResizeType
    {
        None,
        EachByResolution,
        AllByBigResolutionRatio,
        AllBySmallResolutionRatio,
    }

    public class UIPanel : UIObject
    {
        #region active/deactive anim
        public UIPanel_AnimType activateAnimType = UIPanel_AnimType.None;
        public float activateAnimTime = 0.1f;
        public UIPanel_AnimType deactivateAnimType = UIPanel_AnimType.None;
        public float deactivateAnimTime = 0.1f;

        protected IEnumerator activeCoroutine = null;
        protected bool _isActive;
        #endregion

        // {{ resize properties
        public EUIResizeType resizeType = EUIResizeType.None;
        protected RectTransform rectTransform;
        protected Vector2 cachedRectSize;
        protected Vector2 cachedRectPosition;
        // }} resize properties

        protected override void InitFromAwake()
        {
            rectTransform = transform as RectTransform;
            cachedRectSize = rectTransform.sizeDelta;
            cachedRectPosition = rectTransform.anchoredPosition;

            _isActive = gameObject.activeInHierarchy;

            base.InitFromAwake();
        }

        protected override void InitFromStart()
        {
            base.InitFromStart();
        }

        public Vector2 GetResizingSize(ref Vector2 size, EUIResizeType type)
        {
            if (type == EUIResizeType.None) return size;

            Vector2 retval = size;

            switch (type)
            {
                case EUIResizeType.EachByResolution:
                    retval.x *= JUtil.GetResRatioX();
                    retval.y *= JUtil.GetResRatioY();
                    break;
                case EUIResizeType.AllByBigResolutionRatio:
                    retval *= JUtil.GetMaxResRatio();
                    break;
                case EUIResizeType.AllBySmallResolutionRatio:
                    retval *= JUtil.GetMinResRatio();
                    break;
                default:
                    break;
            }

            return retval;
        }

        public override void Resizing()
        {
            if( resizeType != EUIResizeType.None )
            {
                var newSize = cachedRectSize;

                newSize = GetResizingSize(ref newSize, resizeType);

                rectTransform.sizeDelta = newSize;
            }
        }

        public void SetActive(bool bShow, bool bSkipAnim = false )
        {
            if ( bShow == _isActive )
            {
                return;
            }

            _isActive = bShow;

            if (bSkipAnim)
            {
                gameObject.SetActive(bShow);
            }
            else
            {
                // set active
                gameObject.SetActive(true);

                // if doing. Stop
                if(activeCoroutine != null )
                {
                    StopCoroutine(activeCoroutine);
                    activeCoroutine = null;
                }

                activeCoroutine = DoAnimAndActive(bShow);
                StartCoroutine(activeCoroutine);
            }
        }

        // play animation and setactive
        protected IEnumerator DoAnimAndActive( bool bShow )
        {
            IEnumerator e = null;

            // do animation
            if (bShow)
            {
                e = DoAnim(activateAnimType, activateAnimTime);
            }
            else
            {
                e = DoAnim(deactivateAnimType, deactivateAnimTime);
            }

            while( e.MoveNext() )
            {
                yield return e.Current;
            }

            // set disable
            if( !bShow )
            {
                gameObject.SetActive(false);
            }
            
            activeCoroutine = null;
        }
        
        protected IEnumerator DoAnim(UIPanel_AnimType animType,float animTime)
        {
            if( animType == UIPanel_AnimType.None || animTime <= 0.0f )
            {
                yield break;
            }

            yield return null;
            
            IEnumerator e = null;

            switch(animType)
            {
                case UIPanel_AnimType.FromRight:
                        e = MoveAnim(animTime,
                            rectTransform.sizeDelta.x , 0,
                            0,0
                            );
                    break;
                case UIPanel_AnimType.FromLeft:
                    e = MoveAnim(animTime,
                            -rectTransform.sizeDelta.x, 0,
                            0, 0
                            );
                    break;
                case UIPanel_AnimType.ToRight:
                    e = MoveAnim(animTime,
                            0,0,
                            rectTransform.sizeDelta.x, 0
                            );
                    break;
                case UIPanel_AnimType.ToLeft:
                    e = MoveAnim(animTime,
                            0, 0,
                            -rectTransform.sizeDelta.x, 0
                            );
                    break;
                case UIPanel_AnimType.FromUpper:
                    e = MoveAnim(animTime,
                            0, rectTransform.sizeDelta.y,
                            0, 0
                            );
                    break;
                case UIPanel_AnimType.FromBottom:
                    e = MoveAnim(animTime,
                            0, -rectTransform.sizeDelta.y,
                            0, 0
                            );
                    break;
                case UIPanel_AnimType.ToUpper:
                    e = MoveAnim(animTime,
                            0, 0,
                            0, rectTransform.sizeDelta.y
                            );
                    break;
                case UIPanel_AnimType.ToBottom:
                    e = MoveAnim(animTime,
                            0, 0,
                            0, -rectTransform.sizeDelta.y
                            );
                    break;
                case UIPanel_AnimType.ScaleDown:
                    e = ScaleAnim(animTime,
                        1, 1,
                        0, 0);
                    break;
                case UIPanel_AnimType.ScaleUp:
                    e = ScaleAnim(animTime,
                        0, 0,
                        1, 1);
                    break;
            }

            if( e != null )
            {
                while(e.MoveNext() )
                {
                    yield return e.Current;
                }
            }
        }

        protected IEnumerator MoveAnim(float time, float startX, float startY, float endX, float endY)
        {
            var newPosition = cachedRectPosition + new Vector2(startX, startY);
            var lerpSpeed = new Vector2((endX - startX) / time, (endY - startY) / time);

            rectTransform.anchoredPosition = newPosition;

            yield return null;

            while (time > 0.0f)
            {
                time -= Time.deltaTime;

                newPosition.x = Mathf.Clamp(newPosition.x + lerpSpeed.x * Time.deltaTime, Mathf.Min(startX, endX), Mathf.Max(startX, endX));
                newPosition.y = Mathf.Clamp(newPosition.y + lerpSpeed.y * Time.deltaTime, Mathf.Min(startY, endY), Mathf.Max(startY, endY));

                rectTransform.anchoredPosition = newPosition;
                yield return null;
            }
        }

        protected IEnumerator ScaleAnim(float time, float startX,float startY, float endX,float endY)
        {
            var newScale = new Vector3(startX, startY, 1.0f);
            var lerpSpeed = new Vector2((endX - startX) / time, (endY - startY) / time);
            transform.localScale = newScale;

            yield return null;

            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                newScale.x = Mathf.Clamp(newScale.x + lerpSpeed.x * Time.deltaTime, Mathf.Min(startX, endX), Mathf.Max(startX, endX));
                newScale.y = Mathf.Clamp(newScale.y + lerpSpeed.y * Time.deltaTime, Mathf.Min(startY, endY), Mathf.Max(startY, endY));
                transform.localScale = newScale;
                yield return null;
            }            
        }

        private void OnDisable()
        {
            _isActive = false;
        }
    }
}