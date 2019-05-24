using UnityEngine;
using System.Collections;

public class PersonBehaviour : MonoBehaviour {

    private Person person;
    public Person Person {
        get => person;
    }

    private GameController.GameStateEnum GameState {
        get {
            if(gc != null) {
                return gc.GameState;
            }
            else {
                return GameController.GameStateEnum.Waiting;
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
        if (GameState == GameController.GameStateEnum.Running
            && collision.gameObject.CompareTag("Player")) {

            //Debug.Log("Person Collision Enter");

            var p = collision.gameObject.GetComponent<PersonBehaviour>();
            if(p != null) {
                //SetCollaborator(p);
                //p.SetCollaborator(this);

                person.MakeJointProduction(p.person);
            }
        }
    }
}
