using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptOut)]
public class Person {
    public static float initialResources = 1f;
    public static float maxWorkAbility = 1.2f;
    public static float minWorkAbility = 1.1f;
    public static float maxComAbility = 1f;
    public static float minComAbility = .1f;
    public static int maxProdCount = 50;

    /// <summary>
    /// time in seconds between two production actions
    /// </summary>
    public static float productionInterval = 10f;

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
    [JsonIgnore] public Person Collaborator {
        get => collaborator;
    }

    private readonly List<ProductioRecord> productions = new List<ProductioRecord>();
    public List<ProductioRecord> Productions {
        get => productions;
    }

    private readonly Dictionary<Person, float> borrowedRes = new Dictionary<Person, float>();
    [JsonIgnore] public Dictionary<Person, float> BorrowedRes {
        get => borrowedRes;
    }

    public Person() {
        ValidateArgs();

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
        if(Productions.Count == maxProdCount) {
            return;
        }

        ProductioRecord pr = new ProductioRecord() {
            PersonA = this.id,
            ResourcesFormerA = resources,
            ResourcesNewA = AssessIndieProduction(),
            Time = Time.timeSinceLevelLoad,
        };

        resources = pr.ResourcesNewA;
        productions.Add(pr);

        // clear current collaborator
        collaborator = null;
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
        //float benefit = 0f;

        return AssessIndieProduction() + benefit * comAbility / (comAbility + another.comAbility);
    }

    public void MakeJointProduction(Person another) {
        if(Productions.Count == maxProdCount) {
            return;
        }

        ProductioRecord pr = new ProductioRecord() {
            PersonA = this.id,
            PersonB = another.id,
            ResourcesFormerA = resources,
            ResourcesFormerB = another.Resources,
            ResourcesNewA = AssessJointProduction(another),
            ResourcesNewB = another.AssessJointProduction(this),
            Time = Time.timeSinceLevelLoad,
            IsJointProduction = true,
        };

        resources = pr.ResourcesNewA;
        productions.Add(pr);

        // clear current collaborator
        collaborator = null;
    }

    public void MakeProduction() {
        //if(collaborator != null && collaborator.Collaborator == this) {
        if(collaborator != null ) {
            MakeJointProduction(collaborator);
        }
        else {
            MakeIndieProduction();
        }
    }

    public void SetCollaborator(Person another) {
        collaborator = another;
        //if(collaborator == null) {
        //    collaborator = another;
        //}
        //else if(AssessJointProduction(another) > AssessJointProduction(collaborator)) {
        //    collaborator = another;
        //}
    }

    private static void ValidateArgs() {

        if (
            initialResources < 0f ||
            minWorkAbility < 0f ||
            minComAbility < 0f ||
            maxWorkAbility < minWorkAbility ||
            maxComAbility < minComAbility ||
            productionInterval < 0f
        ){
            throw new System.ArgumentException("Person static args is illegal");
        }
    }

    public void BorrowRes(Person another) {
        if(comAbility <= another.comAbility) {
            return;
        }

        float ratio = comAbility - another.comAbility;
        float borrowed = another.resources * ratio;

        another.resources -= borrowed;
        resources += borrowed;

        borrowedRes.Add(another, borrowed);
    }

    public void ReturnBorrowed() {
        foreach(Person p in borrowedRes.Keys) {
            p.resources += borrowedRes[p];
            resources -= borrowedRes[p];
        }

        borrowedRes.Clear();
    }
}
