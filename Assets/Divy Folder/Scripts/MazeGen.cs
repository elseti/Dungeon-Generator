using System.Collections.Generic;
using UnityEngine;
using static Direction;
using Random = System.Random;

public class MazeGen : MonoBehaviour {
    public int numNodesX = 5;
    public int numNodesY = 5;
    public int numNodesZ = 5;

    public const float GRID_UNIT_SIZE = 9;

    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject tunnelPrefab;

    [SerializeField] private GameObject level;
    [SerializeField] private bool _showPath = false;

    public static bool showPath = false;

    private Node[,,] nodes;
    private Stack<Node> lastNode;

    private static Random rnd = new();

    private static Dictionary<Direction, Direction> opposite = new() {
        { LEFT, RIGHT },
        { RIGHT, LEFT },
        { FRONT, BACK },
        { BACK, FRONT },
        { UP, DOWN },
        { DOWN, UP },
    };

    private void InitNodes()
    {
        nodes = new Node[numNodesZ, numNodesY, numNodesX];
        for (var i = 0; i < numNodesZ; ++i) {
            for (var j = 0; j < numNodesY; ++j) {
                for(var k = 0; k < numNodesX; ++k) {
                    nodes[i, j, k] = new Node(k, j, i);
                }
            }
        }
    }

    private Node GetRandomNeighbour(Node current) {
        current.GetAllNeighbours(nodes, out var neighbours, out var dirs);

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

    private void GenMaze()
    {
        lastNode = new Stack<Node>();
        var current = nodes[0, 0, numNodesX/2];

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

    private void CreateRooms()
    {
        foreach (var node in nodes) {
            if (node.isEmpty()) {
                continue;
            }
            var nodePrefab = roomPrefab;
            if (rnd.Next(5) < 3 && !node.hasStairs()) {
                nodePrefab = tunnelPrefab;
            }

            var roomType = nodePrefab.GetComponent<IRoom>();
            var room = Instantiate(
                nodePrefab,
                new Vector3(node.gridX * roomType.sizeX, node.gridY * roomType.sizeY, node.gridZ * roomType.sizeZ),
                Quaternion.identity,
                level.transform
            );
            var objRoom = room.GetComponent<IRoom>();
            objRoom.SetWallsDir(node);
        }
    }

    private void CleanGen()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        // Iterate through each GameObject
        foreach (GameObject obj in allObjects)
        {
            // Check if the name contains "(Clone)"
            if (obj.name.Contains("(Clone)"))
            {
                // Destroy the GameObject
                Destroy(obj);
            }
        }
    }

    public void RunGen()
    {
        // Debug.Log("Cleaning...");
        CleanGen();
        // Debug.Log("Generating...");
        InitNodes();
        GenMaze();
        CreateRooms();
    }

    private void Start() {
        // InitNodes();
        // GenMaze();
        // CreateRooms();
    }

    private void FixedUpdate() {
        showPath = _showPath;
    }
}
