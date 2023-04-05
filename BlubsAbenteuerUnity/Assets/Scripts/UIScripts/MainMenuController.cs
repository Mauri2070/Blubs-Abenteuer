using UnityEngine;
using UnityEngine.UI;
using TMPro;

// UI controller script for the main menu
public class MainMenuController : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject startMenuParent;
    [SerializeField] private GameObject quickPlayMenu;
    [SerializeField] private GameObject freePlayMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject parentalInformationMenu;
    [SerializeField] private GameObject quitMenu;
    [SerializeField] public GameObject startMenuChild;
    [SerializeField] private GameObject secretCredits;
    [SerializeField] private GameObject freePlayMenuChild;

    [Header("Start Menu Parent")]
    [SerializeField] private TextMeshProUGUI storyButtonText;
    [SerializeField] public Button freePlayButton;
    [SerializeField] public Button quickPlayButton;

    [Header("Start Menu Child")]
    [SerializeField] private Button quickPlayButtonChildren;
    [SerializeField] public Button freePlayButtonChildren;

    [Header("Parental Information Menu")]
    [SerializeField] private Toggle informationToggle;
    [SerializeField] private GameObject informationObjects;
    [SerializeField] private GameObject impressumObjects;

    [Header("Options Menu")]
    [SerializeField] private GameObject optionsButtons;
    [SerializeField] private GameObject optionsInformationElements;
    [SerializeField] private TextMeshProUGUI informationTextBox;
    //[SerializeField] private Toggle playAudioToggle;
    [SerializeField] private Slider overrideAudioSlider;
    [SerializeField] private GameObject inputScreen;
    [SerializeField] private TextMeshProUGUI inputInstructionText;
    [SerializeField] private TextMeshProUGUI inputFeedbackText;
    [SerializeField] private TextMeshProUGUI taskInstructionText;
    [SerializeField] private TMP_InputField taskInputField;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Slider difficultySlider;

    private void Start()
    {
        // activate menus
        startMenuChild.SetActive(true);
        startMenuParent.SetActive(false);
        parentalInformationMenu.SetActive(false);
        optionsMenu.SetActive(false);
        freePlayMenu.SetActive(false);
        quickPlayMenu.SetActive(false);
        quitMenu.SetActive(false);
        secretCredits.SetActive(false);
        freePlayMenuChild.SetActive(false);

        // actiavate sub-menus
        informationObjects.SetActive(true);
        impressumObjects.SetActive(false);
        optionsButtons.SetActive(true);
        optionsInformationElements.SetActive(false);
        inputScreen.SetActive(false);

        // lock/unlock FreePlay, QuickPlay
        if (!PlayerPrefsController.IsFreePlayUnlocked())
        {
            freePlayButton.interactable = false;
            quickPlayButton.interactable = false;
            quickPlayButtonChildren.interactable = false;
            freePlayButtonChildren.interactable = false;
        }
        else
        {
            freePlayButton.interactable = true;
            quickPlayButton.interactable = true;
            quickPlayButtonChildren.interactable = true;
            freePlayButtonChildren.interactable = true;
        }
    }

    private void Awake()
    {
        informationToggle.isOn = PlayerPrefsController.ShowInformation();
        // playAudioToggle.isOn = PlayerPrefsController.OverridePlayNumberAudio();

        storyButtonText.text = PlayerPrefsController.LoadRoomIdx(Room.HUB) == 0 ? "Geschichte starten" : "Geschichte fortsetzen";

        overrideAudioSlider.onValueChanged.RemoveAllListeners();
        overrideAudioSlider.value = PlayerPrefsController.GetOverridePlayNumberAudio();
        overrideAudioSlider.onValueChanged.AddListener(delegate { ChangeAudioOverrideSetting(); });

        difficultySlider.onValueChanged.RemoveAllListeners();
        difficultySlider.value = PlayerPrefsController.GetHelpDifficulty();
        difficultySlider.onValueChanged.AddListener(delegate { ChangeDifficultySetting(); });
    }

    // Toggles
    public void OnParentalToggleSwitch()
    {
        PlayerPrefsController.SafeShowInformation(informationToggle.isOn);
    }

    /*
    public void OnPlayAudioToggleSwitch()
    {
        // TODO: switch to slider: always on/ default/ always off
        PlayerPrefsController.SafeNumberAudioOverride(playAudioToggle.isOn);
    }
    */

    // Options information
    private void DisplayOptionInformation()
    {
        optionsButtons.SetActive(false);
        optionsInformationElements.SetActive(true);
    }

    // Legacy
    public void DisplayResetInformation()
    {
        DisplayOptionInformation();
        // FB
        informationTextBox.text = "Fortschritt der Geschichte zurücksetzen\n\n" +
            "Diese Option setzt den Fortschritt der Geschichte zurück. " +
            "Als Bestätigung muss eine Rechenaufgabe korrekt beantwortet werden.\n" +
            "Warnung: Das Zurücksetzen kann nicht direkt rückgängig gemacht werden!";
    }

    // Legacy
    public void DisplayRestoreInformation()
    {
        DisplayOptionInformation();
        // FB
        informationTextBox.text = "Freischaltcode eingeben\n\n" +
            "Diese Option ermöglicht es, den Fortschritt der Geschichte auf bestimmte Punkte zu setzen. " +
            "Dadurch kann ein versehentlich zurückgesetzter Fortschritt wiederhergestellt werden.\n" +
            "Warnung: Wird der Fortschritt auf einen noch nicht erreichten Punkt in der Geschichte gesetzt," +
            " kann dies negativen Einfluss auf Spielspaß und Lernfortschritt haben!\n" +
            "Außerdem kann Schnelles/Freies Spiel, der \"Nächstes Video\"-Button ein oder ausgeschaltet werden.\n" +
            "Unterstützte Codes:\n" +
            "101\t-\tEinführungsvideo überspringen\n" +
            "102\t-\tRaumschiff Tour überspringen\n" +
            "103\t-\tErste Lernphase (Zahlen 1-6) überpringen\n" +
            "104\t-\tZweite Lernphase (Zahlen 1-10) überspringen\n" +
            "105\t-\tDritte Lernphase (Zahlen 1-15) überspringen\n" +
            "106\t-\tFinales Video abspielen (Geschichte vollständig überspringen)\n" +
            "201\t-\t\"Nächstes Video\"-Button immer aktiviert ein/aus\n" +
            "202\t-\tFreies Spiel/ Schnelles Spiel freischalten/sperren\n";
    }

    public void DisplayCodeInputInformation()
    {
        DisplayOptionInformation();
        informationTextBox.text = "Codes eingeben\n\n" +
            "Diese Option ermöglicht es, den Fortschritt der Geschichte auf bestimmte Punkte zu setzen oder vollständig"+
            " zurück zu setzten.\n" +
            "Warnung: Wird der Fortschritt auf einen noch nicht erreichten Punkt in der Geschichte gesetzt," +
            " kann dies negativen Einfluss auf Spielspaß und Lernfortschritt haben!\n" +
            "Außerdem kann Schnelles/Freies Spiel und der \"Nächstes Video\"-Button ein oder ausgeschaltet werden.\n" +
            "Unterstützte Codes:\n" +
            "100\t-\tGeschichte zurücksetzen\n" +
            "101\t-\tEinführungsvideo überspringen\n" +
            "102\t-\tRaumschiff Tour überspringen\n" +
            "103\t-\tErste Lernphase (Zahlen 1-6) überpringen\n" +
            "104\t-\tZweite Lernphase (Zahlen 1-10) überspringen\n" +
            "105\t-\tDritte Lernphase (Zahlen 1-15) überspringen\n" +
            "106\t-\tFinales Video abspielen (Geschichte vollständig überspringen)\n" +
            "201\t-\t\"Nächstes Video\"-Button immer aktiviert ein/aus\n" +
            "202\t-\tFreies Spiel/ Schnelles Spiel freischalten/sperren\n";
    }

    public void DisplayNumberAudioInformation()
    {
        DisplayOptionInformation();
        informationTextBox.text = "Zahlen bei Interaktion vorlesen\n\n" +
            "Diese Option gibt das Verhalten von Zahlen bei Interaktionen mit ihnen an.\n" +
            "Immer an: Audio wird immer ausgegeben.\n" +
            "Standard: Die Ausgabe von Audio hängt vom Storyfortschritt und der Hilfeeinstellung ab.\n" +
            "Immer aus: Audio wird nie ausgegeben.\n" +
            "Beispiel für Interaktion: ein Spielelement mit der Zahl \"1\" wird angetippt.\n" +
            "Es wird \"eins\" über Audio ausgegeben, falls die Option bzw. das Spiel entsprechend eingestellt ist.\n" +
            "In Memory-Spielen hat diese Option keine Auswirkung.";
    }

    public void DisplayHelpInformation()
    {
        DisplayOptionInformation();
        informationTextBox.text = "Sollte das spielende Kind eine gewisse Anzahl an falschen Eingaben tätigen, so wird vom Spiel eine Hilfestellung angezeigt." +
            "Wie viele Eingaben für diese Hilfe benötigt werden, hängt vom aktuellen Spiel und dieser Option ab. " +
            "Das Hilfe-System ist adaptiv. Sollten also viele Probleme während der Minispiele auftreten, so wird automatisch mehr Hilfe angeboten. " +
            "Läuft alles sehr gut, dann kommt seltener Hilfe.";
    }

    // Options input
    //private bool reset;
    private System.Random random = new System.Random();
    private int numb1;
    private int numb2;

    // Legacy
    //public void DisplayStoryReset()
    //{
    //    reset = true;

    //    inputScreen.SetActive(true);
    //    optionsButtons.SetActive(false);

    //    numb1 = random.Next(20, 250);
    //    numb2 = random.Next(20, 250);

    //    inputInstructionText.text = "Lösen Sie zum Zurücksetzten des Geschichtsfortschritts folgende Aufgabe:\n" + numb1 + " + " + numb2;
    //    taskInputField.text = "";
    //    inputFeedbackText.text = "Erwarte Eingabe";
    //}

    // Legacy
    //public void DisplayStoryRestore()
    //{
    //    reset = false;

    //    inputScreen.SetActive(true);
    //    optionsButtons.SetActive(false);

    //    inputInstructionText.text = "Geben Sie den gewünschten Code ein.\n" +
    //        "101\t-\tEinführungsvideo überspringen\n" +
    //        "102\t-\tRaumschiff Tour überspringen\n" +
    //        "103\t-\tErste Lernphase (Zahlen 1-6) überpringen\n" +
    //        "104\t-\tZweite Lernphase (Zahlen 1-10) überspringen\n" +
    //        "105\t-\tDritte Lernphase (Zahlen 1-15) überspringen\n" +
    //        "106\t-\tFinales Video abspielen (Geschichte vollständig überspringen)\n" +
    //        "201\t-\t\"Nächstes Video\"-Button immer aktiviert ein/aus\n" +
    //        "202\t-\tFreies Spiel/ Schnelles Spiel freischalten/sperren\n";
    //    taskInputField.text = "";
    //    inputFeedbackText.text = "Erwarte Eingabe";
    //}

    public void DisplayCodeInput()
    {
        inputScreen.SetActive(true);
        optionsButtons.SetActive(false);

        numb1 = random.Next(20, 250);
        numb2 = random.Next(20, 250);

        inputInstructionText.text = "Geben Sie den gewünschten Code ein.\n" +
            "100\t-\tGeschichte zurücksetzen\n" +
            "101\t-\tEinführungsvideo überspringen\n" +
            "102\t-\tRaumschiff Tour überspringen\n" +
            "103\t-\tErste Lernphase (Zahlen 1-6) überpringen\n" +
            "104\t-\tZweite Lernphase (Zahlen 1-10) überspringen\n" +
            "105\t-\tDritte Lernphase (Zahlen 1-15) überspringen\n" +
            "106\t-\tFinales Video abspielen (Geschichte vollständig überspringen)\n" +
            "201\t-\t\"Nächstes Video\"-Button immer aktiviert ein/aus\n" +
            "202\t-\tFreies Spiel/ Schnelles Spiel freischalten/sperren\n";
        taskInputField.text = "";
        codeInputField.text = "";
        inputFeedbackText.text = "Erwarte Eingaben";
        taskInstructionText.text = numb1 + " + " + numb2 + " =";
    }

    public void ConfirmInput()
    {
        // Legacy
        //if (reset)
        //{
        //    if (!int.TryParse(taskInputField.text, out int res))
        //    {
        //        inputFeedbackText.text = "Die Eingabe ist keine gültige Zahl!";
        //        return;
        //    }
        //    if (res != numb1 + numb2)
        //    {
        //        inputFeedbackText.text = "Das eingegebene Ergebnis ist falsch!";
        //        return;
        //    }
        //    inputFeedbackText.text = "Der Geschichtsfortschritt wurde zurückgesetzt!";
        //    PlayerPrefsController.ResetProgress();
        //    storyButtonText.text = "Geschichte starten";
        //}
        //else
        //{

        // check task
        if (!int.TryParse(taskInputField.text, out int res))
        {
            inputFeedbackText.text = "Das eingegebene Ergebnis ist keine gültige Zahl!";
            return;
        }
        if (res != numb1 + numb2)
        {
            inputFeedbackText.text = "Das eingegebene Ergebnis ist falsch!";
            return;
        }

        // check code
        if (!int.TryParse(codeInputField.text, out res))
        {
            inputFeedbackText.text = "Der eingegebene Code ist keine gültige Zahl!";
            return;
        }

        switch (res)
        {
            case 100:
                PlayerPrefsController.ResetProgress();
                storyButtonText.text = "Geschichte starten";
                break;
            case 101: // skip story intro
                PlayerPrefsController.SafeRoomIdx(Room.HUB, 1);
                PlayerPrefsController.SafeRoomIdx(Room.LAB, 0);
                PlayerPrefsController.SafeRoomIdx(Room.NAV, 0);
                PlayerPrefsController.SafeRoomIdx(Room.ENGINE, 0);
                PlayerPrefsController.SafeGameIdx(Room.HUB, 0);
                PlayerPrefsController.SafeGameIdx(Room.LAB, 0);
                PlayerPrefsController.SafeGameIdx(Room.NAV, 0);
                PlayerPrefsController.SafeGameIdx(Room.ENGINE, 0);
                break;
            case 102: // skip room tour
                PlayerPrefsController.SafeRoomIdx(Room.HUB, 10);
                PlayerPrefsController.SafeRoomIdx(Room.LAB, 3);
                PlayerPrefsController.SafeRoomIdx(Room.NAV, 3);
                PlayerPrefsController.SafeRoomIdx(Room.ENGINE, 3);
                PlayerPrefsController.SafeGameIdx(Room.HUB, 0);
                PlayerPrefsController.SafeGameIdx(Room.LAB, 0);
                PlayerPrefsController.SafeGameIdx(Room.NAV, 0);
                PlayerPrefsController.SafeGameIdx(Room.ENGINE, 0);
                break;
            case 103: // skip lvl 1 (1-6)
                PlayerPrefsController.SafeRoomIdx(Room.HUB, 12);
                PlayerPrefsController.SafeRoomIdx(Room.LAB, 12);
                PlayerPrefsController.SafeRoomIdx(Room.NAV, 12);
                PlayerPrefsController.SafeRoomIdx(Room.ENGINE, 13);
                PlayerPrefsController.SafeGameIdx(Room.HUB, 0);
                PlayerPrefsController.SafeGameIdx(Room.LAB, 0);
                PlayerPrefsController.SafeGameIdx(Room.NAV, 0);
                PlayerPrefsController.SafeGameIdx(Room.ENGINE, 0);
                break;
            case 104: // skip lvl 2 (1-10)
                PlayerPrefsController.SafeRoomIdx(Room.HUB, 14);
                PlayerPrefsController.SafeRoomIdx(Room.LAB, 19);
                PlayerPrefsController.SafeRoomIdx(Room.NAV, 18);
                PlayerPrefsController.SafeRoomIdx(Room.ENGINE, 18);
                PlayerPrefsController.SafeGameIdx(Room.HUB, 0);
                PlayerPrefsController.SafeGameIdx(Room.LAB, 0);
                PlayerPrefsController.SafeGameIdx(Room.NAV, 0);
                PlayerPrefsController.SafeGameIdx(Room.ENGINE, 0);
                break;
            case 105: // skip lvl 3 (1-15)
                PlayerPrefsController.SafeRoomIdx(Room.HUB, 16);
                PlayerPrefsController.SafeRoomIdx(Room.LAB, 26);
                PlayerPrefsController.SafeRoomIdx(Room.NAV, 24);
                PlayerPrefsController.SafeRoomIdx(Room.ENGINE, 23);
                PlayerPrefsController.SafeGameIdx(Room.HUB, 0);
                PlayerPrefsController.SafeGameIdx(Room.LAB, 0);
                PlayerPrefsController.SafeGameIdx(Room.NAV, 0);
                PlayerPrefsController.SafeGameIdx(Room.ENGINE, 0);
                break;
            case 106: // skip lvl 4 (1-20) -> skip to final video
                PlayerPrefsController.SafeRoomIdx(Room.HUB, 16);
                PlayerPrefsController.SafeRoomIdx(Room.LAB, 33);
                PlayerPrefsController.SafeRoomIdx(Room.NAV, 30);
                PlayerPrefsController.SafeRoomIdx(Room.ENGINE, 28);
                PlayerPrefsController.SafeGameIdx(Room.HUB, 0);
                PlayerPrefsController.SafeGameIdx(Room.LAB, 0);
                PlayerPrefsController.SafeGameIdx(Room.NAV, 0);
                PlayerPrefsController.SafeGameIdx(Room.ENGINE, 0);
                break;
            case 201: // option to toggle video skip
                PlayerPrefsController.ToggleVideoSkip();
                break;
            case 202:   // option to toggle freePlay/quickPlay
                bool freePlayUnlocked = PlayerPrefsController.IsFreePlayUnlocked();
                PlayerPrefsController.SafeFreePlayUnlock(!freePlayUnlocked);
                freePlayButton.interactable = !freePlayUnlocked;
                quickPlayButton.interactable = !freePlayUnlocked;
                quickPlayButtonChildren.interactable = !freePlayUnlocked;
                freePlayButtonChildren.interactable = !freePlayUnlocked;
                break;
            case 42069: // debug: hard reset
                PlayerPrefs.DeleteAll();
                FindObjectOfType<SceneController>().LoadMainMenu();
                break;
            case 16101997:
                secretCredits.SetActive(true);
                break;
            case 666: // dynamic debug
                      //PlayerPrefsController.SafeRoomIdx(Room.HUB, 16);
                      //PlayerPrefsController.SafeRoomIdx(Room.LAB, 33);
                      //PlayerPrefsController.SafeRoomIdx(Room.NAV, 30);
                      //PlayerPrefsController.SafeRoomIdx(Room.ENGINE, 25);
                      //PlayerPrefsController.SafeGameIdx(Room.HUB, 0);
                      //PlayerPrefsController.SafeGameIdx(Room.LAB, 0);
                      //PlayerPrefsController.SafeGameIdx(Room.NAV, 0);
                      //PlayerPrefsController.SafeGameIdx(Room.ENGINE, 3);
                break;
            default:
                inputFeedbackText.text = "Der eingegebene Code ist nicht gültig!";
                return;
        }
        inputFeedbackText.text = "Die Eingabe war erfolgreich.\nNeue Eingabe möglich.";

        // Reset
        numb1 = random.Next(20, 250);
        numb2 = random.Next(20, 250);
        taskInputField.text = "";
        codeInputField.text = "";
        taskInstructionText.text = numb1 + " + " + numb2 + " =";
        //}
    }

    private void ChangeDifficultySetting()
    {
        PlayerPrefsController.SetHelpDifficulty((int)difficultySlider.value);
    }

    private void ChangeAudioOverrideSetting()
    {
        PlayerPrefsController.SafeNumberAudioOverride((int)overrideAudioSlider.value);
    }

    public void ActivateParentMode()
    {
        startMenuChild.SetActive(false);
        startMenuParent.SetActive(true);
        if (!PlayerPrefsController.ShowInformation())
        {
            startMenuParent.SetActive(false);
            parentalInformationMenu.SetActive(true);
        }
        else
        {
            startMenuParent.SetActive(true);
            parentalInformationMenu.SetActive(false);
        }
    }
}
