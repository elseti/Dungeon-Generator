using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DecorationManager : MonoBehaviour {

    public int numDecors = 1;

    private GameObject[] decors;
    private static Random rnd = new();
    private IRoom room;

    private void SelectDecors() {
        var enabledDecors = decors
            .Where(t => !t.GetComponent<Decoration>().conflictsWithStairs || !room.HasStairs())
            .OrderBy(_ => rnd.Next())
            .Take(numDecors)
            .ToArray();

        foreach (var decor in enabledDecors) {
            decor.SetActive(true);
        }
    }

    private void Awake() {
        room = transform.parent.parent.GetComponent<IRoom>();

        decors = GetComponentsInChildren<Decoration>()
            .Select(t => t.gameObject)
            .ToArray();

        foreach (var decor in decors) {
            decor.SetActive(false);
        }
    }

    private void Start() {
        SelectDecors();
    }
}