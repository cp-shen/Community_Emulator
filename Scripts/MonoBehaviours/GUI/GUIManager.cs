using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameController))]
public class GUIManager : MonoBehaviour
{
    private GameController gameController;

    // UI components
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnReadArgs;
    [SerializeField] private Button btnSetArgs;
    [SerializeField] private Text stateText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text ArgumentText;
    [SerializeField] private InputField countInput;

    [SerializeField] private InputField minComInput;
    [SerializeField] private InputField maxComInput;
    [SerializeField] private InputField minWorkInput;
    [SerializeField] private InputField maxWorkInput;
    [SerializeField] private InputField mixTimeInput;
    [SerializeField] private InputField totalProdInput;
    [SerializeField] private InputField intervalInput;
    [SerializeField] private InputField initResInput;
    [SerializeField] private InputField maxProdCountInput;

    public int SpawnCount {
        get {
            try {
                return int.Parse(countInput.text);
            }
            catch (System.Exception) {
                return 1;
            }
        }
    }

    void Start() {
        gameController = GetComponent<GameController>();
        SetupGUIComponents();
        SetupGUIEvents();
    }

    private void OnDestroy() {
        RemoveGUIEvents();
    }

    private void SetupGUIComponents() {
        countInput.contentType = InputField.ContentType.IntegerNumber;

        minComInput.contentType = InputField.ContentType.DecimalNumber;
        maxComInput.contentType = InputField.ContentType.DecimalNumber;
        minWorkInput.contentType = InputField.ContentType.DecimalNumber;
        maxWorkInput.contentType = InputField.ContentType.DecimalNumber;
        mixTimeInput.contentType = InputField.ContentType.DecimalNumber;
        totalProdInput.contentType = InputField.ContentType.DecimalNumber;
        intervalInput.contentType = InputField.ContentType.DecimalNumber;
        initResInput.contentType = InputField.ContentType.DecimalNumber;

        maxProdCountInput.contentType = InputField.ContentType.IntegerNumber;
    }

    void Update() {
        UpdateGUIComponents();
    }

    private void SetupGUIEvents() {
        btnStart?.onClick.AddListener(gameController.StartGame);
        btnReadArgs?.onClick.AddListener(ReadArgs);
        btnSetArgs?.onClick.AddListener(SetArgs);
    }

    private void RemoveGUIEvents() {
        btnStart?.onClick.RemoveListener(gameController.StartGame);
        btnReadArgs?.onClick.RemoveListener(ReadArgs);
        btnSetArgs?.onClick.RemoveListener(SetArgs);
    }

    private void ReadArgs() {
        minComInput.text = Person.minComAbility.ToString();
        maxComInput.text = Person.maxComAbility.ToString();
        minWorkInput.text = Person.minWorkAbility.ToString();
        maxWorkInput.text = Person.maxWorkAbility.ToString();

        mixTimeInput.text = gameController.MixTime.ToString();
        totalProdInput.text = gameController.TotalProdTime.ToString();
        intervalInput.text = Person.productionInterval.ToString();
        initResInput.text = Person.initialResources.ToString();
        maxProdCountInput.text = Person.maxProdCount.ToString();
    }

    private void SetArgs() {
        var minComAbility = float.Parse(minComInput.text);
        var maxComAbility = float.Parse(maxComInput.text);
        var minWorkAbility = float.Parse(minWorkInput.text);
        var maxWorkAbility = float.Parse(maxWorkInput.text);

        var mixTime = float.Parse(mixTimeInput.text);
        var totalProdTime = float.Parse(totalProdInput.text);
        var productionInterval = float.Parse(intervalInput.text);
        var initialResources = float.Parse(initResInput.text);
        var maxProdCount = int.Parse(maxProdCountInput.text);

        // validate before settings
        if (
            minWorkAbility < 0f ||
            minComAbility < 0f ||
            maxWorkAbility < minWorkAbility ||
            maxComAbility < minComAbility ||

            mixTime < 0f ||
            totalProdTime < 0f ||
            productionInterval < 0f ||
            initialResources < 0f ||
            maxProdCount < 0
        ){
            throw new System.ArgumentException("Person static args is illegal");
        }

        Person.minComAbility = minComAbility;
        Person.maxComAbility = maxComAbility;
        Person.minWorkAbility = minWorkAbility;
        Person.maxWorkAbility = maxWorkAbility;

        gameController.MixTime = mixTime;
        gameController.TotalProdTime = totalProdTime;
        Person.productionInterval = productionInterval;
        Person.initialResources = initialResources;
        Person.maxProdCount = maxProdCount;
    }

    private void UpdateTimeText() {
        if(timeText != null) {
            timeText.text = "Time Remain: " + gameController.TimeRemain;
        }
    }

    private void UpdateGameStateText() {
        if (stateText == null) {
            return;
        }
        stateText.text = "GameState: " + gameController.GameState;
    }

    private void UpdateArgumentText() {
        string lb = System.Environment.NewLine;
        ArgumentText.text =
            "Arguments" + lb
            + "Min Com: " + Person.minComAbility + lb
            + "Max Com: " + Person.maxComAbility + lb
            + "Min Work: " + Person.minWorkAbility + lb
            + "Max Work: " + Person.maxWorkAbility + lb
            + "Mix Time: " + gameController.MixTime + lb
            + "Total Prod Time: " + gameController.TotalProdTime + lb
            + "Production Interval: " + Person.productionInterval + lb
            + "Init Resources: " + Person.initialResources + lb 
            + "Max Person Count: " + gameController.MaxCount + lb
            + "Max Prod Count: " + Person.maxProdCount + lb;
    }

    private void UpdateGUIComponents() {
        UpdateTimeText();
        UpdateGameStateText();
        UpdateArgumentText();
    }
}
