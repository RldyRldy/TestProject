using UnityEngine;
using UnityEditor;

namespace JGame.Sound
{
    public class SoundEditor : EditorWindow
    {
        [MenuItem("Assets/Create/SoundItem")]
        public static void CreateLevelData()
        {
            AudioClip clip = Selection.activeObject as AudioClip;
            SoundItem item = ScriptableObject.CreateInstance<SoundItem>();

            if (item != null && clip != null)
            {
                item.clip = clip;
            }

            CustomEditorUtility.CreateAsset<SoundItem>(item, (clip != null ? clip.name : ""));

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = item;
        }
    }
}