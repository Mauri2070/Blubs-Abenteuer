using UnityEngine;

// contains information for the progress bar controller
[CreateAssetMenu(fileName = "new Progress Bar Info", menuName = "Progress/ProgressBarInfo")]
public class ProgressBarInformation : ScriptableObject
{
    public Room room;
    public ProgressStepInformation[] informations;

    [System.Serializable]
    public class ProgressStepInformation
    {
        public int idx;
        public int games;
        public bool hasPrecondition;
        public Room preconditionRoom;
        public int preconditionIdx;
        public bool hasSecondPrecondition;
        public Room secondPreconditionRoom;
        public int secondPreconditionIdx;
    }

    public int GetTotalNumberOfGames()
    {
        int sum = 0;

        foreach (ProgressStepInformation i in informations)
        {
            sum += i.games;
        }

        return sum;
    }

    public int GetCurrentIdx(int roomIdx, int gameIdx)
    {
        int ret = 0;
        for (int i = 0; i < informations.Length; i++)
        {
            if (roomIdx < informations[i].idx)
            {
                return ret;
            }
            if (roomIdx == informations[i].idx)
            {
                return ret + gameIdx;
            }
            ret += informations[i].games;
        }
        return ret;
    }

    public int GetNextLockIdx(int currentIdx, ProgressController controller, out Room conditionRoom)
    {
        int idxCount = 0;
        for (int i = 0; i < informations.Length; i++)
        {
            if (idxCount >= currentIdx)
            {
                if (informations[i].hasPrecondition && !controller.IsStepCompleted(informations[i].preconditionRoom, informations[i].preconditionIdx))
                {
                    conditionRoom = informations[i].preconditionRoom;
                    return idxCount;
                }
                if (informations[i].hasSecondPrecondition && !controller.IsStepCompleted(informations[i].secondPreconditionRoom, informations[i].secondPreconditionIdx))
                {
                    conditionRoom = informations[i].secondPreconditionRoom;
                    return idxCount;
                }
            }
            idxCount += informations[i].games;
        }
        conditionRoom = Room.HUB;
        return -1;
    }
}
