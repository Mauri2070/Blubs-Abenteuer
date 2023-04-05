using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ScriptableObject representing a (group) of mini games
[CreateAssetMenu(fileName = "new Mini Game Options", menuName = "MiniGameOptions")]
public class MiniGameOptions : ScriptableObject
{
    [Header("Standard MiniGame options")]
    [SerializeField] public MiniGameType gameType;
    [SerializeField] [Range(1, 19)] public int minValue;
    [SerializeField] [Range(2, 20)] public int maxValue;
    [SerializeField] [Tooltip("If set to true, only explicitValues content will be used to determine the used values in the MiniGame (insert, count, pairs, memory).")] public bool useExplicitValues;
    [SerializeField]
    [Tooltip("You can specify the values to use in the MiniGame (insert, count, pairs, memory). minValue and maxValue will be ignored (only if enought numbers are provided in case of memory).")]
    public int[] explicitValues;
    [SerializeField] [Tooltip("Only used if not useExplicitValues and at most MIN(maxValue-minValue+1, 10). Number of Pairs for pairs.")] [Range(2, 20)] public int numberOfValues;
    [SerializeField] public DisplayMode displayMode;
    [SerializeField] [Tooltip("The visual representation to use in Set and Mixed Mode.")] public NumberRepresentation numberRepresentation;
    [SerializeField] [Tooltip("Use alternative representation for 5 and 10 in Set and Mixed Mode.")] public bool alternativeRepresentation;
    [SerializeField] [Tooltip("Should the numbers increase or decrease? Does not apply to every MiniGame.")] public bool increasing;
    [SerializeField] [Tooltip("Number of Values not part of the Solution (insert, add).")] [Range(0, 10)] public int numberOfNonSolutionValues;
    [SerializeField] [Tooltip("Whether or not number audio should be activated during the game.")] public bool numberAudioActive = true;

    [Header("Insert GameType specific Options")]
    [SerializeField] [Tooltip("Should the specified missingValues be used?")] public bool useExplicitMissingValues;
    [SerializeField]
    [Tooltip("Specify the missing values that should be inserted. Values smaller then minValue (explicitValues.min), larger maxValue (explicitValues.max) or (if active) not contained in explicitValues won't be used!")]
    public List<int> missingValues;
    [SerializeField] [Tooltip("Only used, if useExplicitMissingValues = false. Specifies the number of gaps. At most numberOfValues - 1")] [Range(1, 9)] public int numberMissingValues;

    [Header("Pairs GameType specific Options")]
    [SerializeField] public DisplayMode rightSideDisplayMode;

    [Header("Add GameType specific Options")]
    [SerializeField] [Range(2, 20)] public int targetValue;

    [Header("Memory GameType specific Options")]
    [SerializeField] [Tooltip("Small: 4P (3x3), Medium: 10P (4x5), Large: 14P (4*7)")] public MemoryMiniGame.MemorySize memorySize;
    [SerializeField] [Tooltip("Should memory played Set<->Text (true) or Set<->Set (false)")] public bool matchSetText;
    [SerializeField] [Tooltip("Select second visual Number Representation (used if matchSetText=false or for other MiniGame types)")] public NumberRepresentation secondRepresentation;

    [Header("Connect GameType specific Options")]
    [SerializeField] public bool subtract;

    private System.Random rand = new System.Random();
    void OnEnable()
    {
        VerifyMinMax();

        if (useExplicitValues && explicitValues.Length == 0)
        {
            explicitValues = new int[2];
            explicitValues[0] = rand.Next(minValue, maxValue + 1);
            explicitValues[1] = rand.Next(minValue, maxValue + 1);
        }

        switch (gameType)
        {
            case MiniGameType.INSERT:
                VerifyInsert();
                break;
            case MiniGameType.ADD:
                VerifyAdd();
                break;
            case MiniGameType.COUNT_VS:
            case MiniGameType.COUNT:
                VerifyCount();
                break;
            case MiniGameType.PAIRS:
                VerifyPairs();
                break;
            case MiniGameType.MEMORY_VS:
            case MiniGameType.MEMORY:
                VerifyMemory();
                break;
            case MiniGameType.CONNECT_VS:
            case MiniGameType.CONNECT:
                VerifyConnect();
                break;
            default:
                break;
        }
    }

    public void VerifyMinMax()
    {
        // use only positive values in Range [1,20]
        minValue = minValue < 1 ? 1 : System.Math.Min(minValue, 19);
        maxValue = maxValue < 1 ? minValue + 1 : System.Math.Min(maxValue, 20);

        // make sure minValue < maxValue
        if (minValue > maxValue)
        {
            int tmp = minValue;
            minValue = maxValue;
            maxValue = tmp;
        }

        // make sure there are at least 2 values
        if (minValue == maxValue)
        {
            maxValue++;
        }

        // special adjustments for memory
        if (gameType == MiniGameType.MEMORY)
        {
            switch (memorySize)
            {
                case MemoryMiniGame.MemorySize.SMALL:
                    if (maxValue - minValue + 1 < 4)
                    {
                        maxValue = minValue + 4;
                    }
                    break;
                case MemoryMiniGame.MemorySize.MEDIUM:
                    if (maxValue - minValue + 1 < 10)
                    {
                        maxValue = minValue + 10;
                    }
                    break;
                case MemoryMiniGame.MemorySize.LARGE:
                    if (maxValue - minValue + 1 < 14)
                    {
                        maxValue = minValue + 14;
                    }
                    break;
            }
            while (maxValue > 20)
            {
                maxValue--;
                minValue--;
            }
        }
    }

    public void VerifyInsert()
    {
        if (numberOfValues < 2)
        {
            numberOfValues = 2;
        }
        else if (numberOfValues > (displayMode == DisplayMode.MIXED ? 7 : 12))
        {
            numberOfValues = (displayMode == DisplayMode.MIXED ? 7 : 12);
        }
        // at least 1 value has to be missing
        if (useExplicitMissingValues && missingValues.Count == 0)
        {
            missingValues.Add(rand.Next(minValue, maxValue + 1));
        }
        if (!useExplicitMissingValues)
        {
            if (numberMissingValues < 1)
            {
                numberMissingValues = 1;
            }
            else if (numberMissingValues > numberOfValues - 1)
            {
                numberMissingValues = numberOfValues - 1;
            }
        }
        // umberBaitObjects non-negative
        numberOfNonSolutionValues = numberOfNonSolutionValues < 0 ? 0 : numberOfNonSolutionValues;
        if (numberOfNonSolutionValues > (displayMode == DisplayMode.MIXED ? 28 : 48) - numberMissingValues)
        {
            numberOfNonSolutionValues = (displayMode == DisplayMode.MIXED ? 28 : 48) - numberMissingValues;
        }
    }

    public void VerifyAdd()
    {
        if (targetValue < 2)
        {
            targetValue = 2;
        }
        minValue = 1;
        if (maxValue < targetValue)
        {
            maxValue = targetValue;
        }
        if (numberOfValues < 1)
        {
            numberOfValues = 1;
        }
        else if (numberOfValues > targetValue / 2)
        {
            numberOfValues = targetValue / 2;
        }
        if (numberOfNonSolutionValues < 0)
        {
            numberOfNonSolutionValues = 0;
        }
        else if (maxValue - minValue + 1 - (targetValue / 2) < numberOfNonSolutionValues)
        {
            // otherwise there will be an endless loop
            numberOfNonSolutionValues = maxValue - minValue + 1 - (targetValue / 2);
        }
    }

    public void VerifyCount()
    {
        // Count and Pairs only use explicitValues or numberOfValues
        if (!useExplicitValues)
        {
            if (numberOfValues < 2)
            {
                numberOfValues = 2;
            }
            else if (numberOfValues > maxValue - minValue + 1)
            {
                numberOfValues = maxValue - minValue + 1;
            }
        }
    }

    public void VerifyPairs()
    {
        // Count and Pairs only use explicitValues or numberOfValues
        if (!useExplicitValues)
        {
            if (numberOfValues < 1)
            {
                numberOfValues = 1;
            }
            else if (numberOfValues > maxValue - minValue + 1)
            {
                numberOfValues = maxValue - minValue + 1;
            }
            if (numberOfValues > 6)
            {
                numberOfValues = 6;
            }
        }
        else if (explicitValues.Length > 6)
        {
            int[] newExplicitValues = new int[6];
            System.Array.Copy(explicitValues, newExplicitValues, 6);
            explicitValues = newExplicitValues;
        }

    }

    public void VerifyMemory()
    {
        switch (memorySize)
        {
            case MemoryMiniGame.MemorySize.SMALL:
                if (maxValue - minValue + 1 < 4)
                {
                    maxValue = minValue + 4;
                }
                break;
            case MemoryMiniGame.MemorySize.MEDIUM:
                if (maxValue - minValue + 1 < 10)
                {
                    maxValue = minValue + 10;
                }
                break;
            case MemoryMiniGame.MemorySize.LARGE:
                if (maxValue - minValue + 1 < 14)
                {
                    maxValue = minValue + 14;
                }
                break;
        }
        if (maxValue > 20)
        {
            int diff = maxValue - 20;
            maxValue -= diff;
            minValue -= diff;
        }
        if (useExplicitValues)
        {
            if (explicitValues.Length != (memorySize == MemoryMiniGame.MemorySize.SMALL ? 4 : memorySize == MemoryMiniGame.MemorySize.MEDIUM ? 10 : 14))
            {
                int oldSize = explicitValues.Length;
                int[] newExplicitValues = new int[1];
                switch (memorySize)
                {
                    case MemoryMiniGame.MemorySize.SMALL:
                        newExplicitValues = new int[4];
                        System.Array.Copy(explicitValues, newExplicitValues, oldSize < 4 ? oldSize : 4);
                        break;
                    case MemoryMiniGame.MemorySize.MEDIUM:
                        newExplicitValues = new int[10];
                        System.Array.Copy(explicitValues, newExplicitValues, oldSize < 10 ? oldSize : 10);
                        break;
                    case MemoryMiniGame.MemorySize.LARGE:
                        newExplicitValues = new int[14];
                        System.Array.Copy(explicitValues, newExplicitValues, oldSize < 14 ? oldSize : 14);
                        break;
                }
                if (oldSize < newExplicitValues.Length)
                {
                    for (int i = oldSize; i < newExplicitValues.Length; i++)
                    {
                        int value;
                        do
                        {
                            value = rand.Next(minValue, maxValue + 1);
                        } while (newExplicitValues.Contains(value));
                        newExplicitValues[i] = value;
                    }
                }
                explicitValues = newExplicitValues;
            }
        }
    }

    public void VerifyConnect()
    {
        minValue = System.Math.Min(minValue, maxValue / 2);
        int n = maxValue - 2 * minValue + 2;
        numberOfValues = System.Math.Min(numberOfValues, 6);
        numberOfValues = System.Math.Max(1, System.Math.Min(numberOfValues, (n * n) / 4));
    }

    public override string ToString()
    {
        return gameType + " GameOptions: [" + minValue + ", " + maxValue + "], " + numberOfValues + " values (" + numberMissingValues + " missing, " + numberOfNonSolutionValues + " additional)";
    }
}
