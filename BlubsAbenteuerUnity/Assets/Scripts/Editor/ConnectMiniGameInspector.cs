using UnityEngine;
using UnityEditor;

// custom inspector for debuggin connect mini game
[CustomEditor(typeof(ConnectMiniGame))]
public class ConnectMiniGameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ConnectMiniGame connectMiniGame = (ConnectMiniGame)target;

        if (GUILayout.Button("Create Pairs"))
        {
            connectMiniGame.GenerateObjects();
        }
        if (GUILayout.Button("Clear"))
        {
            connectMiniGame.ClearField();
        }

        base.OnInspectorGUI();
    }
}
