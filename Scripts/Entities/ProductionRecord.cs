using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[JsonObject()]
public class ProductioRecord {
    /// <summary>
    /// ID of person A, 0 means null
    /// </summary>
    public int PersonA { get; set; }
    /// <summary>
    /// ID of person B, 0 means null
    /// </summary>
    public int PersonB { get; set; }

    public float ResourcesFormerA { get; set; }
    public float ResourcesFormerB { get; set; }

    public float ResourcesNewA { get; set; }
    public float ResourcesNewB { get; set; }

    public bool IsJointProduction { get; set; } = false;

    /// <summary>
    /// time in seconds since the game started
    /// </summary>
    public float Time { get; set; }
}
