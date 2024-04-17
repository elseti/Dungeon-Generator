using UnityEngine;
using UnityEngine.Serialization;

public class Decoration : MonoBehaviour {
    [FormerlySerializedAs("conflictsWithStair")] public bool conflictsWithStairs = false;
}
