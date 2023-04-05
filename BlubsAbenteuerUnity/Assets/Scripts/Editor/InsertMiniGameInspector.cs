using UnityEngine;
using UnityEditor;

// custom inspector for debuggin insert mini game
[CustomEditor(typeof(InsertMiniGame))]
public class InsertMiniGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        InsertMiniGame insertMiniGame = (InsertMiniGame)target;

        if (GUILayout.Button("Fill Grid single"))
        {
            insertMiniGame.FillGrid(false);
        }
        if (GUILayout.Button("Fill Grid multiple"))
        {
            insertMiniGame.FillGrid(true);
        }
        if (GUILayout.Button("Clear Parents"))
        {
            insertMiniGame.ClearParents();
        }

        base.OnInspectorGUI();
    }
}
