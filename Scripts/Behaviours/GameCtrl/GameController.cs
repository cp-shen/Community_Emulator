using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum GameState {
        Paused,
        Running,
    }

    [SerializeField] private GameState gameState = GameState.Paused;
    public GameState GameSate { get => gameState; }

    [SerializeField] private CameraMoveCtrl camMove;
    [SerializeField] private SmoothMouseLook mouseLook;
    [SerializeField] private int maxCount = 200;
        
    /// <summary>
    /// GameObject to spawn
    /// </summary>
    [SerializeField] private GameObject obj;

    /// <summary>
    /// total time in seconds in one game
    /// </summary>
    [SerializeField] private float totalTime = 100f;

    private float timeRemain;

    // UI components
    [SerializeField] private Button btn;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text stateText;

    private readonly List<PersonBehaviour> persons = new List<PersonBehaviour>();

    private void Spawn() {
        if(obj != null) {
            GameObject g = Instantiate(obj);
            PersonBehaviour p = g.GetComponent<PersonBehaviour>();
            if(p != null) {
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

    private void SpawnByInputField() {
        try {
            int count = int.Parse(inputField.text);
            SpawnMultiple(count);
            BeginGame();
        }
        catch(System.Exception e) {
            Debug.Log(e.Message);
        }
    }

    private void Start() {
        SetupGUIComponents();
        SetupGUIEvents();

        timeRemain = totalTime;

    }

    private void OnDestroy() {
        RemoveGUIEvents();

    }

    private void SetupGUIComponents() {
        inputField.contentType = InputField.ContentType.IntegerNumber;
    }

    private void SetupGUIEvents() {
        if(btn != null) {
            btn.onClick.AddListener(SpawnByInputField);
        }
    }

    private void RemoveGUIEvents() {
        if(btn != null) {
            btn.onClick.RemoveListener(SpawnByInputField);
        }
    }

    private void Update() {
        // pause or reset game by keys
        if (Input.GetKeyDown(KeyCode.Escape)) {
            gameState = GameState.Paused;
            RemoveAllPersons();
        }

        SetCursorState();
        SetCamState();
        SetGameStateText();
    }

    private void SetCursorState() {
        switch (gameState) {
            case GameState.Paused:
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
            case GameState.Running:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }

    private void BeginGame() {
        gameState = GameState.Running;
    }

    private void PauseGame() {
        gameState = GameState.Paused;
    }

    private void SetCamState() {
        switch (gameState) {
            case GameState.Paused:
                if(camMove != null) {
                    camMove.enabled = false;
                }
                if(mouseLook != null) {
                    mouseLook.enabled = false;
                }
                break;
            case GameState.Running:
                if(camMove != null) {
                    camMove.enabled = true;
                }
                if(mouseLook != null) {
                    mouseLook.enabled = true;
                }
                break;
        }
    }

    private void SetGameStateText() {
        if (stateText == null) {
            return;
        }

        switch (gameState) {
            case GameState.Paused:
                stateText.text = "GameState: Paused";
                break;
            case GameState.Running:
                stateText.text = "GameState: Running";
                break;
        }
    }

    private void RemoveAllPersons() {
        foreach (PersonBehaviour p in persons) {
            Destroy(p.gameObject);
        }
        persons.Clear();
    }
}
