﻿using UnityEngine;
using System.Collections;

public class PersonBehaviour : MonoBehaviour {

    private Person person;
    public Person Person {
        get => person;
    }

    private GameController.GameState GameState {
        get {
            if(gc != null) {
                return gc.GameSate;
            }
            else {
                return GameController.GameState.Waiting;
            }
        }
    }

    private GameController gc;

    void Awake() {
        person = new Person();
    }

    void Start() {
        gc = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameController>();
    }

    private void SetCollaborator(PersonBehaviour another) {
        Person.SetCollaborator(another.person);
    }

    private void OnCollisionEnter(Collision collision) {
        if (GameState == GameController.GameState.Running && collision.gameObject.CompareTag("Player")) {
            var p= collision.gameObject.GetComponent<PersonBehaviour>();
            if(p != null) {
                SetCollaborator(p);
            }
        }
    }
}
