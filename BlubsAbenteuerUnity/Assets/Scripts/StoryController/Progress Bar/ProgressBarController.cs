using UnityEngine;

// controls the progress bar
public class ProgressBarController : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] GameObject spriteMask;

    [Header("Lock Objects")]
    [SerializeField] GameObject navLockPrefab;
    [SerializeField] GameObject labLockPrefab;
    [SerializeField] GameObject engineLockPrefab;
    [SerializeField] float minLockX = -642;
    [SerializeField] float maxLockX = 642;

    private ProgressBarInformation currentInfo;
    private int maxValue;
    private int currentValue;

    private ProgressController progressController;

    private GameObject lockObject;

    private void Awake()
    {
        progressController = FindObjectOfType<ProgressController>();
    }

    public void ChangeProgressBar(ProgressBarInformation newInfo, int roomIdx = 0, int gameIdx = 0)
    {
        if (lockObject != null)
        {
            Destroy(lockObject);
        }

        if (newInfo == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        currentInfo = newInfo;

        maxValue = currentInfo.GetTotalNumberOfGames();
        currentValue = currentInfo.GetCurrentIdx(roomIdx, gameIdx) - 1;
        GameProgressed();
        //Debug.Log("Progress Bar change: max-" + maxValue + ", current-" + currentValue + ", locked-" + (lockObject != null) + "; gameIdx=" + gameIdx + ", roomIdx=" + roomIdx);
    }

    public void GameProgressed()
    {
        currentValue++;

        if (lockObject != null)
        {
            Destroy(lockObject);
        }

        int lockIdx;
        if ((lockIdx = currentInfo.GetNextLockIdx(currentValue, progressController, out Room conditionRoom)) != -1)
        {
            // calculate position, spawn object with room
            if (conditionRoom == Room.NAV)
            {
                lockObject = Instantiate(navLockPrefab, transform);
            }
            else if (conditionRoom == Room.LAB)
            {
                lockObject = Instantiate(labLockPrefab, transform);
            }
            else
            {
                lockObject = Instantiate(engineLockPrefab, transform);
            }
            lockObject.transform.localPosition = new Vector3(minLockX + (lockIdx * (Mathf.Abs(minLockX) + Mathf.Abs(maxLockX)) / maxValue), 0);
        }

        ScaleSpriteMask();
    }

    private void ScaleSpriteMask()
    {
        spriteMask.transform.localScale = new Vector3(2 * (1 - (float)currentValue / maxValue), 1.1f);
    }
}
