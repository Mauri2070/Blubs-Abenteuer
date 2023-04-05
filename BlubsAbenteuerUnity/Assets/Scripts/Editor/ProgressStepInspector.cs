using UnityEngine;
using UnityEditor;

// custom inspector to manually reset progress used in early development
[CustomEditor(typeof(LegacyProgressStep))]
public class ProgressStepInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Reset Progress"))
        {
            PlayerPrefsController.ResetProgress();
        }

        base.OnInspectorGUI();
    }
}
