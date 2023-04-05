using UnityEngine;
using UnityEditor;

// custom inspector for debuggin pairs mini game
[CustomEditor(typeof(PairsMiniGame))]
public class PairsMiniGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        PairsMiniGame pairsMiniGame = (PairsMiniGame)target;

        if (GUILayout.Button("Create Pairs"))
        {
            pairsMiniGame.GenerateObjects();
        }
        if (GUILayout.Button("Clear"))
        {
            pairsMiniGame.ClearField();
        }

        base.OnInspectorGUI();
    }
}
