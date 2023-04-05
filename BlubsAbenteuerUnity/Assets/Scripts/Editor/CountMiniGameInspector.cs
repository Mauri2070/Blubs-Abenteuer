using UnityEngine;
using UnityEditor;

// custom inspector for debuggin count mini game
[CustomEditor(typeof(CountMiniGame))]
public class CountMiniGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CountMiniGame countMiniGame = (CountMiniGame)target;

        if (GUILayout.Button("Fill Grid single"))
        {
            countMiniGame.FillGrid(false);
        }
        if (GUILayout.Button("Fill Grid multiple"))
        {
            countMiniGame.FillGrid(true);
        }
        if (GUILayout.Button("Clear Grid"))
        {
            countMiniGame.ClearGrid();
        }

        base.OnInspectorGUI();
    }
}
