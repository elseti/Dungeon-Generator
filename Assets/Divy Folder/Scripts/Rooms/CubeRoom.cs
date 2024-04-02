using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Direction;

public class CubeRoom : MonoBehaviour, IRoom {
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
    private GameObject _leftWallDoor;
    private GameObject _rightWallDoor;
    private GameObject _frontWallDoor;
    private GameObject _backWallDoor;
    private GameObject _stairs;
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
        _leftWallDoor.SetActive(!hasLeftWall);
        _rightWallDoor.SetActive(!hasRightWall);
        _frontWallDoor.SetActive(!hasFrontWall);
        _backWallDoor.SetActive(!hasBackWall);
        _arrow.SetActive(MazeGen.showPath && startDirection != NULL);
        _arrow.transform.rotation = rotation[startDirection];
    }

    private void OnEnable() {
        var components = gameObject.GetComponentsInChildren<Transform>()
            .Where(t => t.parent == transform)
            .Select(t => t.gameObject)
            .ToArray();

        _leftWall = components[0];
        _rightWall = components[1];
        _frontWall = components[2];
        _backWall = components[3];
        // 4 and 5 are floor and ceiling
        components[5].SetActive(false); // todo: temporary
        _leftWallDoor = components[6];
        _rightWallDoor = components[7];
        _frontWallDoor = components[8];
        _backWallDoor = components[9];
        // 10 and 11 are floor and ceiling with stairs
        components[10].SetActive(false);
        components[11].SetActive(false);
        _stairs = components[12];
        _arrow = components[13];
        _stairs.SetActive(false);
        _arrow.SetActive(false);
        _leftWallDoor.SetActive(false);
        _rightWallDoor.SetActive(false);
        _frontWallDoor.SetActive(false);
        _backWallDoor.SetActive(false);
    }

    private void FixedUpdate() {
        ToggleWalls();
    }
}
