using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DecorationManager : MonoBehaviour {

    public int numDecors = 1;

    private GameObject[] decors;
    private static Random rnd = new();
    private IRoom room;

    private void SelectDecors() {
        var toEnable = decors
            .Where(t => !t.GetComponent<Decoration>().conflictsWithStairs || !room.HasStairs())
            .OrderBy(_ => rnd.Next())
            .Take(numDecors)
            .ToArray();

        foreach (var decor in toEnable) {
            decor.SetActive(true);
            var lights = decor.GetComponentsInChildren<Light>().Select(t => t.gameObject).ToArray();
            foreach (var _light in lights) {
                _light.SetActive(false);
            }
        }
    }

    private void OnEnable() {
        room = transform.parent.parent.GetComponent<IRoom>();

        decors = GetComponentsInChildren<Decoration>()
            .Select(t => t.gameObject)
            .ToArray();

        foreach (var decor in decors) {
            decor.SetActive(false);
        }

        SelectDecors();
    }
}