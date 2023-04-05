using UnityEngine;
using UnityEditor;

// custom inspector for debuggin connectVs mini game
[CustomEditor(typeof(ConnectVsMiniGame))]
public class ConnectVsMiniGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        ConnectVsMiniGame connectVsMiniGame = (ConnectVsMiniGame)target;

        if (GUILayout.Button("Create Mixed"))
        {
            connectVsMiniGame.GenerateObjects(true);
        }
        if (GUILayout.Button("Create Single"))
        {
            connectVsMiniGame.GenerateObjects(false);
        }
        if (GUILayout.Button("Clear"))
        {
            connectVsMiniGame.ClearField();
        }

        base.OnInspectorGUI();
    }
}
