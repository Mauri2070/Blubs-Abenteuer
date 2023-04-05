using UnityEngine;
using UnityEditor;

// custom inspector for debuggin add mini game
[CustomEditor(typeof(AddMiniGame))]
public class AddMiniGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        AddMiniGame addMiniGame = (AddMiniGame)target;

        if (GUILayout.Button("Fill Grid mixed"))
        {
            addMiniGame.FillGrid(true);
        }
        if (GUILayout.Button("Fill Grid single"))
        {
            addMiniGame.FillGrid(false);
        }
        if (GUILayout.Button("Clear"))
        {
            addMiniGame.ClearGrid();
        }

        base.OnInspectorGUI();
    }
}
