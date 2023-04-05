using UnityEngine;

// Object to store game options set in main menu to use them in free play/ quick play mode
public class FreePlayOptionsSingleton : MonoBehaviour
{
    // Singleton base code from: http://www.unitygeek.com/unity_c_singleton/
    private static FreePlayOptionsSingleton instance = null;
    public static FreePlayOptionsSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FreePlayOptionsSingleton>();
                if (instance == null)
                {
                    GameObject go = new GameObject("Free Play Options");
                    instance = go.AddComponent<FreePlayOptionsSingleton>();
                    instance.gameOptions = ScriptableObject.CreateInstance<MiniGameOptions>();
                    instance.SetBaseSettings();
                    instance.gameOptions.gameType = MiniGameType.INSERT;
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private MiniGameOptions gameOptions;
    public MiniGameOptions GameOptions
    {
        get
        {
            return gameOptions;
        }
    }

    private bool quickPlay;
    public bool QuickPlay
    {
        get
        {
            return quickPlay;
        }

        set
        {
            quickPlay = value;
        }
    }

    private bool parentMode;
    public bool ParentMode
    {
        get { return parentMode; }
        set { parentMode = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (gameOptions == null)
            {
                //gameOptions = ScriptableObject.CreateInstance<MiniGameOptions>();
                //SetBaseSettings();
                //instance.gameOptions.gameType = MiniGameType.INSERT;
                gameOptions = debugOptions;
            }
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetBaseSettings()
    {
        // Common
        //gameOptions.gameType = MiniGameType.INSERT;
        gameOptions.minValue = 1;
        gameOptions.maxValue = 20;
        gameOptions.useExplicitValues = false;
        gameOptions.numberOfValues = 2;
        gameOptions.displayMode = DisplayMode.MIXED;
        gameOptions.numberRepresentation = NumberRepresentation.MIXED;
        gameOptions.alternativeRepresentation = false;
        gameOptions.increasing = true;
        gameOptions.numberOfNonSolutionValues = 0;
        gameOptions.numberAudioActive = true;
        // Insert
        gameOptions.useExplicitMissingValues = false;
        gameOptions.numberMissingValues = 1;
        // Pairs
        gameOptions.rightSideDisplayMode = DisplayMode.MIXED;
        // Add
        gameOptions.targetValue = 2;
        // Memory
        gameOptions.memorySize = MemoryMiniGame.MemorySize.SMALL;
        gameOptions.matchSetText = true;
        // Connect
        gameOptions.subtract = false;
    }

    // generate randomized options for quick play (single player only)
    public static void GenerateRandomizedOptions(bool keepGameType = false)
    {
        Instance.GenerateRandomizedOptionsI(keepGameType);
    }

    private System.Random rand = new System.Random();
    public void GenerateRandomizedOptionsI(bool keepGameType)
    {
        DisplayMode currentDisplayMode = gameOptions.displayMode;
        // decide for MiniGameType (single player only) or keep multiplayer type
        int rn = rand.Next(0, 6);
        if (keepGameType)
        {
            switch (gameOptions.gameType)
            {
                case MiniGameType.INSERT:
                    rn = 0;
                    break;
                case MiniGameType.COUNT:
                    rn = 1;
                    break;
                case MiniGameType.PAIRS:
                    rn = 2;
                    break;
                case MiniGameType.ADD:
                    rn = 3;
                    break;
                case MiniGameType.MEMORY:
                    rn = 4;
                    break;
                case MiniGameType.CONNECT:
                    rn = 5;
                    break;
                case MiniGameType.MEMORY_VS:
                    rn = 6;
                    break;
                case MiniGameType.CONNECT_VS:
                    rn = 7;
                    break;
                case MiniGameType.COUNT_VS:
                    rn = 8;
                    break;
            }
        }

        // skip other options if staying in multiplayer
        if(rn > 5)
        {
            return;
        }

        SetBaseSettings();
        gameOptions.displayMode = currentDisplayMode;
        int diff = PlayerPrefsController.GetQuickPlayDifficulty();

        gameOptions.minValue = 1;
        gameOptions.maxValue = diff == 0 ? 5 : diff == 1 ? 10 : 20;

        gameOptions.numberAudioActive = diff < 1;

        gameOptions.numberRepresentation = diff == 0 ? NumberRepresentation.RANDOM : NumberRepresentation.MIXED;
        gameOptions.secondRepresentation = gameOptions.numberRepresentation;

        /*
        // display Mode settings
        int rn = rand.Next(0, 3);
        gameOptions.displayMode = rn == 0 ? DisplayMode.SET : rn == 1 ? DisplayMode.MIXED : DisplayMode.TEXT;
        */
        switch (rn)
        {
            case 0:
                gameOptions.gameType = MiniGameType.INSERT;
                // numberOfValues, numberOfMissingValues, numberOfNonSolutionValues, increasing
                gameOptions.numberOfValues = diff == 0 ? 4 : 7;
                gameOptions.numberMissingValues = diff == 0 ? 2 : diff == 1 ? 3 : 5;
                gameOptions.numberOfNonSolutionValues = diff == 0 ? 0 : diff == 1 ? 2 : 6;
                gameOptions.increasing = rand.Next(0, 2) == 0;
                break;
            case 1:
                gameOptions.gameType = MiniGameType.COUNT;
                // increasing, numberOfValues
                gameOptions.numberOfValues = diff == 0 ? 4 : diff == 1 ? 8 : 15;
                gameOptions.increasing = diff == 0 || rand.Next(0, 2) == 0;
                break;
            case 2:
                gameOptions.gameType = MiniGameType.PAIRS;
                // numberOfValues, rightSideDisplayMode
                gameOptions.numberOfValues = diff == 0 ? 2 : 4;
                gameOptions.rightSideDisplayMode = gameOptions.displayMode;
                break;
            case 3:
                gameOptions.gameType = MiniGameType.ADD;
                // targetValue, numberOfValues, numberOfNonSolutionValues
                gameOptions.numberOfValues = diff == 0 ? 2 : diff == 1 ? 4 : 7;
                gameOptions.targetValue = rand.Next(gameOptions.numberOfValues + 1, gameOptions.maxValue);
                gameOptions.numberOfNonSolutionValues = diff == 0 ? 0 : diff == 1 ? 3 : 7;
                break;
            case 4:
                gameOptions.gameType = MiniGameType.MEMORY;
                // memorySize, matchSetText
                gameOptions.memorySize = diff == 0 ? MemoryMiniGame.MemorySize.SMALL : diff == 1 ? MemoryMiniGame.MemorySize.MEDIUM : MemoryMiniGame.MemorySize.LARGE;
                gameOptions.matchSetText = rand.Next(0, 2) == 0;
                break;
            case 5:
                gameOptions.gameType = MiniGameType.CONNECT;
                // numberOfValues, subtract, rightSideDisplayMode
                gameOptions.numberOfValues = diff == 0 ? 2 : 4;
                gameOptions.rightSideDisplayMode = gameOptions.displayMode;
                gameOptions.subtract = diff != 0 && rand.Next(0, 2) == 0;
                break;
        }
    }

    public MiniGameOptions debugOptions;
}
