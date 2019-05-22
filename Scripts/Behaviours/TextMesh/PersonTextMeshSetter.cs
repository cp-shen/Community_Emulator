using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonTextMeshSetter : BaseTextMeshSetter
{
    private Person person;
    protected override string StringToDisplay {
        get => person.Resources.ToString();
    }

    protected override void Start() {
        base.Start();
        var personBehav = GetComponentInParent<PersonBehaviour>();
        person = personBehav?.Person;
        if (person == null) {
            Debug.LogError("can not get person data");
        }
    }
}
