using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person {
    private static readonly float initialResources = 1f;
    private static readonly float maxWorkAbility = 1.4f;
    private static readonly float minWorkAbility = 1.1f;
    private static readonly float maxComAbility = 1f;
    private static readonly float minComAbility = .1f;

    /// <summary>
    /// time in seconds between two production actions
    /// </summary>
    public static readonly float productionCycle = 10f;

    private float resources;
    private float workAbility;
    private float comAbility;
    private bool canSurvive = false;
    private int id = 0;

    public float Resources { get => resources; }
    public float WorkAbility { get => workAbility; }
    public float ComAbility { get => comAbility; }
    public bool CanSurvive {
        get => canSurvive;
        set => canSurvive = value;
    }
    public int ID {
        get => id;
        set => id = value;
    }

    private Person collaborator = null;
    public Person Collaborator {
        get => collaborator;
    }

    /// <summary>
    /// used for json serialization
    /// </summary>
    public bool ShouldSerializeCollaborator() {
        return false;
    }

    public Person() {
        resources = initialResources;

        // random between 0f and 1f and has one decimal digit
        float factor = Mathf.Round(Random.Range(0f, 10f)) / 10f;

        workAbility = minWorkAbility + (maxWorkAbility - minWorkAbility) * factor;
        comAbility = minComAbility + (maxComAbility - minComAbility) * (1 - factor);
    }

    public float AssessIndieProduction() {
        return resources * workAbility;
    }

    private void MakeIndieProduction() {
        resources = AssessIndieProduction();
    }

    public float AssessJointProduction(Person another) {
        //float lendRatio = Mathf.Max(comAbility, another.comAbility);
        float lendRatio = (comAbility + another.comAbility) / 2f;

        Person borrower, lender;
        if(workAbility >= another.workAbility) {
            borrower = this;
            lender = another;
        }
        else {
            borrower = another;
            lender = this;
        }

        float benefit = lender.resources * lendRatio * (borrower.workAbility - lender.workAbility);

        return AssessIndieProduction() + benefit / 2;
    }

    private void MakeJointProduction(Person another) {
        resources = AssessJointProduction(another);

        // clear current collaborator
        collaborator = null;
    }

    public void MakeProduction() {
        if(collaborator != null && collaborator.Collaborator == this) {
            MakeJointProduction(collaborator);
        }
        else {
            MakeIndieProduction();
        }
    }

    public void SetCollaborator(Person another) {
        if(collaborator == null) {
            collaborator = another;
        }
        else if(AssessJointProduction(another) > AssessJointProduction(collaborator)) {
            collaborator = another;
        }
    }
}
