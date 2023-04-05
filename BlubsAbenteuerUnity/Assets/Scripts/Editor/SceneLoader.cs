using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneLoader
{
    [MenuItem("Scenes/Main Menu")]
    private static void OpenMainMenu()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
    }

    [MenuItem("Scenes/Story Scene")]
    private static void OpenStoryScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/StoryScene.unity");
        }
    }

    [MenuItem("Scenes/Free Play Scene")]
    private static void OpenFreePlayScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/FreePlay.unity");
        }
    }
}
