using UnityEngine;
using System.Collections;

public class PersonBehaviour : MonoBehaviour {

    private Person person;
    public Person Person {
        get => person;
    }

    void Awake() {
        person = new Person();
    }

    void Update() {

    }

    private void SetCollaborator(PersonBehaviour another) {
        Person.Collaborator = another.Person;
    }
}
