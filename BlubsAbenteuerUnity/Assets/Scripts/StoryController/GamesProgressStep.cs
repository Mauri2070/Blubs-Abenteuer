using UnityEngine;
using UnityEngine.UI;

// progress step for displaying mini game blocks in story
[CreateAssetMenu(fileName = "new Games Progress Step", menuName = "Progress/GamesProgressStep")]
public class GamesProgressStep : ProgressStep
{
    [Header("MiniGames")]
    [SerializeField] private MiniGameOptions[] miniGames;
    [SerializeField] private MiniGameOptions[] exerciseGames;
    [SerializeField] private Vector2[] miniGameAnchorPoints;

    [Header("Other")]
    [SerializeField] private GameObject selectorPrefab;
    [SerializeField] private Room room;

    private int idx;
    public int Idx
    {
        get
        {
            //Debug.Log("(GET) Games index of " + name + " is " + idx);
            return idx;
        }

        set
        {
            idx = value;
        }
    }

    public void GenerateMiniGameButtons(Transform parent, StorySceneCanvasController controller)
    {
        //Debug.Log("(GENERATE) Games index of " + name + " is " + idx);
        for (int i = idx; i < miniGames.Length; i++)
        {
            GameObject instantiated = Instantiate(selectorPrefab, parent);
            instantiated.GetComponent<Button>().onClick.AddListener(() =>
            {
                controller.StartMiniGame(miniGames[idx], instantiated, exerciseGames[idx]);
            });
            instantiated.GetComponent<RectTransform>().anchoredPosition = miniGameAnchorPoints[i];
            instantiated.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-90f, 90f));
            instantiated.GetComponentsInChildren<Image>()[1].sprite = GameStartSpriteDistributer.instance.GetSprite(room);
        }
    }

    public void GameCompleted()
    {
        idx++;
        if (idx >= miniGames.Length)
        {
            Completed = true;
        }
    }
}
