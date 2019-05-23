using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// should be an singleton
/// </summary>
public class GameController : MonoBehaviour
{
    // UI components
    [SerializeField] private Button btn;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text stateText;
    [SerializeField] private Text timeText;

    public enum GameState {
        Waiting,
        Mixing,
        Running,
    }

    [SerializeField] private GameState gameState = GameState.Waiting;
    public GameState GameSate { get => gameState; }

    [SerializeField] private CameraMoveCtrl camMove;
    [SerializeField] private SmoothMouseLook mouseLook;
    [SerializeField] private int maxCount = 200;
        
    /// <summary>
    /// GameObject to spawn
    /// </summary>
    [SerializeField] private GameObject personObj;

    /// <summary>
    /// total time in seconds in one game
    /// </summary>
    [SerializeField] private float totalProductionTime = 101f;
    [SerializeField] private float mixTime = 15f;

    private float timeRemain;

    private float Cycle { get => Person.productionCycle; }

    private readonly List<PersonBehaviour> persons = new List<PersonBehaviour>();

    private bool readyForProduction = true;

    #region Spawn Funcions
    private void Spawn() {
        if(personObj != null && persons.Count < maxCount) {
            GameObject g = Instantiate(personObj);
            PersonBehaviour p = g.GetComponent<PersonBehaviour>();
            if(p != null) {
                // ID begins from 0
                p.Person.ID = persons.Count;
                persons.Add(p);
            }
        }
    }

    private void SpawnMultiple(int count) {
        count = Mathf.Clamp(count, 0, maxCount);
        for(int i = 0; i < count; i++) {
            Spawn();
        }
    }

    private void SpawnByKey() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Spawn();
        }
    }

    /// <exception cref="System.Exception">
    /// failed to spawn persons
    /// </exception>
    private void SpawnByInputField() {
        int count = int.Parse(inputField.text);
        SpawnMultiple(count);
    }
    #endregion

    #region Unity Callbacks
    private void Start() {
        SetupGUIComponents();
        SetupGUIEvents();

        timeRemain = totalProductionTime;

    }

    private void OnDestroy() {
        RemoveGUIEvents();

    }
    private void Update() {
        // pause or reset game by keys
        if (Input.GetKeyDown(KeyCode.Q)) {
            EndGame();
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            camMove.ResetPosition();
            mouseLook.ResetRotation();
        }

        SetCursorState();
        SetCamState();
        UpdateGUIComponents();

        // Update time counter
        if(gameState == GameState.Running) {
            if (timeRemain > 0f) {
                if(timeRemain > Cycle && readyForProduction) {
                    StartCoroutine(MakeProduction());
                }
                timeRemain -= Time.deltaTime;
            }
            else {
                EndGame();
            }
        }
    }
    #endregion

    #region GUI functions

    private void SetupGUIComponents() {
        inputField.contentType = InputField.ContentType.IntegerNumber;
    }

    private void SetupGUIEvents() {
        if(btn != null) {
            btn.onClick.AddListener(StartGame);
        }
    }

    private void RemoveGUIEvents() {
        if(btn != null) {
            btn.onClick.RemoveListener(StartGame);
        }
    }
    
    private void UpdateTimeText() {
        if(timeText != null) {
            timeText.text = "Time Remain: " + timeRemain;
        }
    }

    private void UpdateGameStateText() {
        if (stateText == null) {
            return;
        }
        stateText.text = "GameState: " + gameState;

        //switch (gameState) {
        //    case GameState.Waiting:
        //        stateText.text = "GameState: ";
        //        break;
        //    case GameState.Running:
        //        stateText.text = "GameState: ";
        //        break;
        //    case GameState.Mixing:
        //        stateText.text = "GameState: ";
        //        break;
        //}
    }
    private void UpdateGUIComponents() {
        UpdateTimeText();
        UpdateGameStateText();
    }
    #endregion

    private void SetCursorState() {
        switch (gameState) {
            case GameState.Waiting:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            default:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }

    private void SetCamState() {
        switch (gameState) {
            case GameState.Waiting:
                if(camMove != null) {
                    camMove.enabled = false;
                }
                if(mouseLook != null) {
                    mouseLook.enabled = false;
                }
                break;
            default:
                if(camMove != null) {
                    camMove.enabled = true;
                }
                if(mouseLook != null) {
                    mouseLook.enabled = true;
                }
                break;
        }
    }

    private void RemoveAllPersons() {
        foreach (PersonBehaviour p in persons) {
            Destroy(p.gameObject);
        }
        persons.Clear();
    }

    /// <summary>
    /// Write result to log files
    /// Clear all persons
    /// Reset game state and game time
    /// </summary>
    private void EndGame() {
        gameState = GameState.Waiting;
        timeRemain = totalProductionTime;

        SetSurviveState();
        WriteLog();
        RemoveAllPersons();
    }

    /// <summary>
    /// Spawn persons and set game state
    /// </summary>
    private void StartGame() {
        try {
            SpawnByInputField();
            StartCoroutine(MixPersons());
        }
        catch (System.Exception e) {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// Write game log to files
    /// </summary>
    private void WriteLog() {
        Debug.Log("Writing result to log files");
        JsonLogWriter.WriteLog(persons);
    }

    private IEnumerator MakeProduction() {
        readyForProduction = false;
        yield return new WaitForSeconds(Cycle);
        Debug.Log("Make Production");
        foreach(PersonBehaviour p in persons) {
            p.Person.MakeProduction();
        }
        readyForProduction = true;
    }

    /// <summary>
    /// Mix the spheres before we begin the simulation process
    /// After mixing, set gameState to running
    /// </summary>
    private IEnumerator MixPersons() {
        gameState = GameState.Mixing;
        yield return new WaitForSeconds(mixTime);
        gameState = GameState.Running;
    }

    private void SetSurviveState() {
        float avg = 0f;
        foreach(PersonBehaviour p in persons) {
            avg += p.Person.Resources;
        }
        avg = avg / (float) persons.Count;

        foreach(PersonBehaviour p in persons) {
            if(p.Person.Resources > avg) {
                p.Person.CanSurvive = true;
            }
            else {
                p.Person.CanSurvive = false;
            }
        }
    }
}
