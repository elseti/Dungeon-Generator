using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Direction;

public class CubeRoom : MonoBehaviour, IRoom {

    // [HideInInspector]
    public Direction startDirection = NULL;

    public bool hasLeftWall = true;
    public bool hasRightWall = true;
    public bool hasFrontWall = true;
    public bool hasBackWall = true;
    public bool hasFloor = true;
    public bool hasCeiling = true;

    public bool mergeLeft = false;
    public bool mergeRight = false;
    public bool mergeFront = false;
    public bool mergeBack = false;

    private GameObject _leftWall;
    private GameObject _rightWall;
    private GameObject _frontWall;
    private GameObject _backWall;
    private GameObject _floor;
    private GameObject _ceiling;
    private GameObject _leftWallDoor;
    private GameObject _rightWallDoor;
    private GameObject _frontWallDoor;
    private GameObject _backWallDoor;
    private GameObject _floorDoor;
    private GameObject _ceilingDoor;
    private GameObject _stairs;
    private GameObject _arrow;

    private Vector3 _arrowPos;

    private Dictionary<Direction, Quaternion> rotation = new() {
        {NULL, Quaternion.Euler(90, 0, 0)},
        {LEFT, Quaternion.Euler(90, 180, 0)},
        {RIGHT, Quaternion.Euler(90, 0, 0)},
        {FRONT, Quaternion.Euler(90, -90, 0)},
        {BACK, Quaternion.Euler(90, 90, 0)},
        {UP, Quaternion.Euler(0, 0, 90)},
        {DOWN, Quaternion.Euler(0, 0, -90)},
    };

    public void SetConnections(Node node) {
        hasLeftWall = !node.connectionLeft;
        hasRightWall = !node.connectionRight;
        hasFrontWall = !node.connectionFront;
        hasBackWall = !node.connectionBack;
        hasFloor = !node.connectionDown;
        hasCeiling = !node.connectionUp;

        mergeLeft = node.mergeLeft;
        mergeRight = node.mergeRight;
        mergeFront = node.mergeFront;
        mergeBack = node.mergeBack;

        startDirection = node.direction;
    }

    private void ToggleWalls() {
        _leftWall.SetActive(hasLeftWall && !mergeLeft);
        _rightWall.SetActive(hasRightWall && !mergeRight);
        _frontWall.SetActive(hasFrontWall && !mergeFront);
        _backWall.SetActive(hasBackWall && !mergeBack);
        _floor.SetActive(hasFloor);
        // _ceiling.SetActive(hasCeiling); // todo: temporarily commented

        _leftWallDoor.SetActive(!hasLeftWall && !mergeLeft);
        _rightWallDoor.SetActive(!hasRightWall && !mergeRight);
        _frontWallDoor.SetActive(!hasFrontWall && !mergeFront);
        _backWallDoor.SetActive(!hasBackWall && !mergeBack);
        _floorDoor.SetActive(!hasFloor);
        _ceilingDoor.SetActive(!hasCeiling);

        _stairs.SetActive(!hasCeiling);

        _arrow.SetActive(startDirection != NULL);
        var pos = new Vector3(_arrowPos.x, _arrowPos.y + (startDirection is UP or DOWN ? 3f : 0), _arrowPos.z);
        _arrow.transform.position = pos;
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
        _floor = components[4];
        _ceiling = components[5];
        _leftWallDoor = components[6];
        _rightWallDoor = components[7];
        _frontWallDoor = components[8];
        _backWallDoor = components[9];
        _floorDoor = components[10];
        _ceilingDoor = components[11];
        _stairs = components[12];
        _arrow = components[13];

        _stairs.SetActive(false);
        _arrow.SetActive(false);
        _leftWallDoor.SetActive(false);
        _rightWallDoor.SetActive(false);
        _frontWallDoor.SetActive(false);
        _backWallDoor.SetActive(false);
        _floorDoor.SetActive(false);
        _ceilingDoor.SetActive(false);

        _ceiling.SetActive(false); // todo: temporarily uncommented

        _arrowPos = _arrow.transform.position;
    }

    private void FixedUpdate() {
        ToggleWalls();
    }
}
