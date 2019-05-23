using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class JsonLogWriter {
    public static void WriteLog(List<PersonBehaviour> persons) {
        if(persons == null || persons.Count == 0) {
            return;
        }

        List<Person> personList = new List<Person>();
        foreach (PersonBehaviour p in persons) {
            personList.Add(p.Person);
        }
        personList.Sort((p1, p2) => {
            if (p1.Resources > p2.Resources) {
                return 1;
            }
            else if (p1.Resources == p2.Resources) {
                return 0;
            }
            else {
                return -1;
            }
        });

        string output = JsonConvert.SerializeObject(personList, Formatting.Indented);
        string filename = "Log_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";

        try {
            File.WriteAllText(filename, output);
        }
        catch(System.Exception e) {
            Debug.Log(e);
        }
    }
}
