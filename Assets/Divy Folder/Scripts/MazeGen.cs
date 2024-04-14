using System.Collections.Generic;
using UnityEngine;
using static Direction;
using Random = System.Random;

public class MazeGen : MonoBehaviour {
    public int numNodesX = 5;
    public int numNodesY = 5;

    public const float GRID_UNIT_SIZE = 9;

    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject tunnelPrefab;

    [SerializeField] private GameObject level;
    [SerializeField] private bool _showPath = false;

    public static bool showPath = false;

    private Node[,] nodes;
    private Stack<Node> lastNode;

    private static Random rnd = new();

    private static Dictionary<Direction, Direction> opposite = new() {
        { LEFT, RIGHT },
        { RIGHT, LEFT },
        { FRONT, BACK },
        { BACK, FRONT },
    };

    private void InitNodes() {
        nodes = new Node[numNodesY, numNodesX];
        for (var i = 0; i < numNodesY; ++i) {
            for (var j = 0; j < numNodesX; ++j) {
                nodes[i, j] = new Node(j, i);
            }
        }
    }

    private Node GetRandomNeighbour(Node current) {
        List<Node> neighbours = new();
        List<Direction> dirs = new();

        if(!current.connectionLeft && 0 < current.gridX && !nodes[ current.gridY, current.gridX - 1].visited) {
            neighbours.Add(nodes[ current.gridY, current.gridX - 1]);
            dirs.Add(LEFT);
        }
        if(!current.connectionRight && current.gridX + 1 < numNodesX && !nodes[current.gridY, current.gridX + 1].visited) {
            neighbours.Add(nodes[current.gridY, current.gridX + 1]);
            dirs.Add(RIGHT);
        }
        if(!current.connectionBack && 0 < current.gridY && !nodes[current.gridY - 1, current.gridX].visited) {
            neighbours.Add(nodes[current.gridY - 1, current.gridX]);
            dirs.Add(BACK);
        }
        if(!current.connectionFront && current.gridY + 1 < numNodesY && !nodes[current.gridY + 1, current.gridX].visited) {
            neighbours.Add(nodes[current.gridY + 1, current.gridX]);
            dirs.Add(FRONT);
        }

        if (neighbours.Count == 0) {
            return null;
        }

        var i = rnd.Next(neighbours.Count);
        if (rnd.Next(100) < 20) {
            neighbours[i].visited = true;
            return GetRandomNeighbour(current);
        }

        current.SetConnection(dirs[i]);

        var neighbour = neighbours[i];
        neighbour.direction = opposite[dirs[i]];
        neighbour.SetConnection(neighbour.direction);
        return neighbour;
    }

    private void GenMaze() {
        lastNode = new Stack<Node>();
        var current = nodes[0, numNodesX/2];

        do {
            current.visited = true;
            var next = GetRandomNeighbour(current);
            if (next != null) {
                lastNode.Push(current);
                current = next;
            } else {
                current = lastNode.Pop();
            }
        } while (lastNode.Count > 0);
    }

    private void CreateRooms() {
        foreach (var node in nodes) {
            if (node.isEmpty()) {
                continue;
            }
            var nodePrefab = roomPrefab;
            if (rnd.Next(2) == 1) {
                nodePrefab = tunnelPrefab;
            }

            var roomType = nodePrefab.GetComponent<IRoom>();
            var room = Instantiate(
                nodePrefab,
                new Vector3(node.gridX * roomType.sizeX, 0, node.gridY * roomType.sizeY),
                Quaternion.identity,
                level.transform
            );
            var objRoom = room.GetComponent<IRoom>();
            objRoom.SetWallsDir(node);
        }
    }

    private void Start() {
        InitNodes();
        GenMaze();
        CreateRooms();
    }

    private void FixedUpdate() {
        showPath = _showPath;
    }
}
