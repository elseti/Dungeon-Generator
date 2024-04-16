using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Direction;

public class Tunnel : MonoBehaviour, IRoom {
    public float sizeX => MazeGen.GRID_UNIT_SIZE;
    public float sizeY => MazeGen.GRID_UNIT_SIZE;
    public float sizeZ => MazeGen.GRID_UNIT_SIZE;

    // [HideInInspector]
    public Direction startDirection = NULL;

    public bool hasLeftConnection;
    public bool hasRightConnection;
    public bool hasFrontConnection;
    public bool hasBackConnection;

    private GameObject _tunnelThrough;
    private GameObject _tunnelCorner;
    private GameObject _tunnelTee;
    private GameObject _tunnelCross;
    private GameObject _arrow;
    private GameObject _wallFront;
    private GameObject _wallBack;

    private Dictionary<Direction, Quaternion> rotation = new() {
        {NULL, Quaternion.Euler(90, 0, 0)},
        {LEFT, Quaternion.Euler(90, 180, 0)},
        {RIGHT, Quaternion.Euler(90, 0, 0)},
        {FRONT, Quaternion.Euler(90, -90, 0)},
        {BACK, Quaternion.Euler(90, 90, 0)}
    };

    public void SetWallsDir(Node node) {
        hasLeftConnection = node.connectionLeft;
        hasRightConnection = node.connectionRight;
        hasFrontConnection = node.connectionFront;
        hasBackConnection = node.connectionBack;
        startDirection = node.direction;
    }

    private void through() {
        _tunnelThrough.SetActive(true);
        _wallBack.SetActive(false);
        _wallFront.SetActive(false);
        if (hasLeftConnection || hasRightConnection) {
            _tunnelThrough.transform.rotation = Quaternion.Euler(0, 90, 0);
            switch (hasLeftConnection, hasRightConnection) {
                case (true, false): _wallFront.SetActive(true); break;
                case (false, true): _wallBack.SetActive(true); break;
            }
        } else {
            _tunnelThrough.transform.rotation = Quaternion.Euler(0, 0, 0);
            switch (hasBackConnection, hasFrontConnection) {
                case (true, false): _wallFront.SetActive(true); break;
                case (false, true): _wallBack.SetActive(true); break;
            }
        }
    }

    private void throughOrCorner() {
        if ((hasFrontConnection && hasBackConnection) || (hasLeftConnection && hasRightConnection)) {
            through();
            return;
        }
        _tunnelCorner.SetActive(true);
        if (hasBackConnection && hasRightConnection) {
            _tunnelCorner.transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (hasRightConnection && hasFrontConnection) {
            _tunnelCorner.transform.rotation = Quaternion.Euler(0, 270, 0);
        } else if (hasFrontConnection && hasLeftConnection) {
            _tunnelCorner.transform.rotation = Quaternion.Euler(0, 180, 0);
        } else {
            _tunnelCorner.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void tee() {
        _tunnelTee.SetActive(true);
        if (hasLeftConnection && hasRightConnection) {
            _tunnelTee.transform.rotation = Quaternion.Euler(0, hasBackConnection ? 0 : 180, 0);
        } else {
            _tunnelTee.transform.rotation = Quaternion.Euler(0, hasLeftConnection ? 90 : 270, 0);
        }
    }

    private void cross() {
        _tunnelCross.SetActive(true);
    }

    private void ToggleWalls() {
        _tunnelThrough.SetActive(false);
        _tunnelCorner.SetActive(false);
        _tunnelTee.SetActive(false);
        _tunnelCross.SetActive(false);

        var connections = (hasLeftConnection ? 1 : 0)
                           + (hasRightConnection ? 1 : 0)
                           + (hasFrontConnection ? 1 : 0)
                           + (hasBackConnection ? 1 : 0);
        switch (connections) {
            case 0: break;
            case 1: through(); break;
            case 2: throughOrCorner(); break;
            case 3: tee(); break;
            case 4: cross(); break;
        }

        _arrow.SetActive(MazeGen.showPath && startDirection != NULL);
        _arrow.transform.rotation = rotation[startDirection];
    }

    private void OnEnable() {
        var components = gameObject.GetComponentsInChildren<Transform>()
            .Where(t => t.parent == transform)
            .Select(t => t.gameObject)
            .ToArray();

        _tunnelThrough = components[0]; // default orientation FB
        _tunnelCorner = components[1];  // default orientation RB
        _tunnelTee = components[2];     // default orientation LRB
        _tunnelCross = components[3];   // default orientation LRFB
        _arrow = components[4];

        var walls = _tunnelThrough.gameObject.GetComponentsInChildren<Transform>()
            .Where(t => t.parent == _tunnelThrough.transform)
            .Select(t => t.gameObject)
            .ToArray();
        _wallBack = walls[0];
        _wallFront = walls[1];

        _tunnelThrough.SetActive(false);
        _tunnelCorner.SetActive(false);
        _tunnelTee.SetActive(false);
        _tunnelCross.SetActive(false);
        _arrow.SetActive(false);
        _wallFront.SetActive(false);
        _wallBack.SetActive(false);
    }

    private void FixedUpdate() {
        ToggleWalls();
    }
}
