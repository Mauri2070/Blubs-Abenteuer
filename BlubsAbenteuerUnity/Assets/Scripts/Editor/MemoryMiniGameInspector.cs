using UnityEngine;
using UnityEditor;

// custom inspector for debuggin memory mini game
[CustomEditor(typeof(MemoryMiniGame))]
public class MemoryMiniGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        MemoryMiniGame memoryMiniGame = (MemoryMiniGame)target;

        if (GUILayout.Button("Fill Grid Small"))
        {
            memoryMiniGame.FillGrid(MemoryMiniGame.MemorySize.SMALL);
        }
        if (GUILayout.Button("Fill Grid Medium"))
        {
            memoryMiniGame.FillGrid(MemoryMiniGame.MemorySize.MEDIUM);
        }
        if (GUILayout.Button("Fill Grid Large"))
        {
            memoryMiniGame.FillGrid(MemoryMiniGame.MemorySize.LARGE);
        }
        if (GUILayout.Button("Clear Grid"))
        {
            memoryMiniGame.ClearGrid();
        }

        base.OnInspectorGUI();
    }
}
