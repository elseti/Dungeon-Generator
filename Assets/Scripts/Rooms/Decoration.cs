using System.Linq;
using UnityEngine;

public class Decoration : MonoBehaviour {
    public bool conflictsWithStairs = false;

    private GameObject[] lights;
    private bool lightsOn = true;

    private void Update() {
        if (MazeGen.doLighting == lightsOn) {
            return;
        }

        lights ??= GetComponentsInChildren<Light>().Select(t => t.gameObject).ToArray();
        foreach (var _light in lights) {
            _light.SetActive(MazeGen.doLighting);
        }
        lightsOn = MazeGen.doLighting;
    }
}
