using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JGame.Localization;

namespace JGame
{
    [RequireComponent(typeof(Text))]
    public class UIText : UIObject
    {
        public bool localizeText = true;

        protected string textStr;
        protected Text text; // default text. for localizing
        protected float fontSize; // default size. for resize
        
        protected override void InitFromAwake()
        {
            text = gameObject.GetComponent<Text>();

            // caching for localizing
            if (localizeText)
            {
                textStr = text.text;
            }

            fontSize = text.fontSize;

            base.InitFromAwake();
        }

        protected override void InitFromStart()
        {
            base.InitFromStart();

            Localizing();
        }

        // localizing. key = default text
        public void Localizing()
        {
            if (localizeText)
            {
                text.text = LocalizationManager.instance.GetLocalizedValue(textStr);
            }
        }

        public override void Resizing()
        {
            float newSize = JUtil.GetMinResRatio();
            text.fontSize = (int)(newSize * fontSize);
        }
    }
}
