using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// should be an singleton
/// </summary>
[RequireComponent(typeof(GUIManager))]
public class GameController : MonoBehaviour
{
    private GUIManager guiManager;

    public enum GameStateEnum {
        Waiting,
        Mixing,
        Running,
    }

    [SerializeField] private GameStateEnum gameState = GameStateEnum.Waiting;
    public GameStateEnum GameState { get => gameState; }

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
    [SerializeField] private float totalProdTime = 101f;
    public float TotalProdTime {
        get => totalProdTime;
        set => totalProdTime = value;
    }

    [SerializeField] private float mixTime = 15f;
    public float MixTime {
        get => mixTime;
        set => mixTime = value;
    }

    private float timeRemain;
    public float TimeRemain { get => timeRemain; }

    private float Cycle { get => Person.productionInterval; }

    private readonly List<PersonBehaviour> persons = new List<PersonBehaviour>();

    private bool readyForProduction = true;

    #region Spawn Funcions
    private void Spawn() {
        if(personObj != null && persons.Count < maxCount) {
            GameObject g = Instantiate(personObj);
            PersonBehaviour p = g.GetComponent<PersonBehaviour>();
            if(p != null) {
                persons.Add(p);
                // ID from 1 to maxCount
                p.Person.ID = persons.Count;
            }
        }
    }

    private void SpawnMultiple(int count) {
        if(count <= 0) {
            throw new System.ArgumentException("spawn count should larger than 0");
        }

        count = Mathf.Clamp(count, 1, maxCount);
        for(int i = 0; i < count; i++) {
            Spawn();
        }
    }

    private void SpawnByKey() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Spawn();
        }
    }

    #endregion

    #region Unity Callbacks
    private void Start() {
        timeRemain = totalProdTime;

        guiManager = GetComponent<GUIManager>();
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

        // Update time counter
        if(gameState == GameStateEnum.Running) {
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


    private void SetCursorState() {
        switch (gameState) {
            case GameStateEnum.Waiting:
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
            case GameStateEnum.Waiting:
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
        gameState = GameStateEnum.Waiting;
        timeRemain = totalProdTime;

        SetSurviveState();
        WriteLog();
        RemoveAllPersons();
    }

    /// <summary>
    /// Spawn persons and set game state
    /// </summary>
    public void StartGame() {
        try {
            SpawnMultiple(guiManager.SpawnCount);
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
        gameState = GameStateEnum.Mixing;
        yield return new WaitForSeconds(mixTime);
        gameState = GameStateEnum.Running;
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
