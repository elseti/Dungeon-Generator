using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Direction;

public class BaseRoom : MonoBehaviour, Room {
    public float sizeX => MazeGen.GRID_UNIT_SIZE;
    public float sizeY => MazeGen.GRID_UNIT_SIZE;
    public float sizeZ => MazeGen.GRID_UNIT_SIZE;

    // [HideInInspector]
    public Direction startDirection = NULL;

    public bool hasLeftWall = true;
    public bool hasRightWall = true;
    public bool hasFrontWall = true;
    public bool hasBackWall = true;

    private GameObject _leftWall;
    private GameObject _rightWall;
    private GameObject _frontWall;
    private GameObject _backWall;
    private GameObject _arrow;

    private Dictionary<Direction, Quaternion> rotation = new() {
        {NULL, Quaternion.Euler(90, 0, 0)},
        {LEFT, Quaternion.Euler(90, 180, 0)},
        {RIGHT, Quaternion.Euler(90, 0, 0)},
        {FRONT, Quaternion.Euler(90, -90, 0)},
        {BACK, Quaternion.Euler(90, 90, 0)}
    };

    public void SetWallsDir(Node node) {
        hasLeftWall = !node.connectionLeft;
        hasRightWall = !node.connectionRight;
        hasFrontWall = !node.connectionFront;
        hasBackWall = !node.connectionBack;
        startDirection = node.direction;
    }

    private void ToggleWalls() {
        _leftWall.SetActive(hasLeftWall);
        _rightWall.SetActive(hasRightWall);
        _frontWall.SetActive(hasFrontWall);
        _backWall.SetActive(hasBackWall);
        _arrow.SetActive(MazeGen.showPath && startDirection != NULL);
        _arrow.transform.rotation = rotation[startDirection];
    }

    private void OnEnable() {
        var components = gameObject.GetComponentsInChildren<Transform>()
            .Select(t => t.gameObject)
            .ToArray();

        _leftWall = components[1];
        _rightWall = components[2];
        _frontWall = components[3];
        _backWall = components[4];
        _arrow = components[7];
        _arrow.SetActive(false);
    }

    private void FixedUpdate() {
        ToggleWalls();
    }
}
