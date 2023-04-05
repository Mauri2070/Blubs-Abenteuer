using UnityEngine;
using UnityEditor;

// custom inspector for debuggin countVS mini game
[CustomEditor(typeof(CountVsMiniGame))]
public class CountVsMiniGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CountVsMiniGame countMiniGame = (CountVsMiniGame)target;

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
