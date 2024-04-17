using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static Direction;
using Random = System.Random;

public class MazeGen : MonoBehaviour {
    [Header("Dimensions")]
    public int numNodesX = 10;
    public int numNodesY = 2;
    public int numNodesZ = 10;

    [Header("Probabilities (sum of room chances must be <= 100)")]
    public int deadEndChance = 20;

    public int squareRoomChance = 15;
    public int cornerRoomChance = 15;
    public int teeRoomChance = 15;
    public int throughRoomChance = 15;

    [Header("Misc")]
    public bool allowOverlappingRooms = false;
    public bool debugShowPath = false;


    [Header("Level Settings")]
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject tunnelPrefab;
    [SerializeField] private GameObject env;
    [SerializeField] private Transform player;

    public static bool showPath = false;

    public const float GRID_UNIT_SIZE = 9;

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

    private void InitNodes() {
        nodes = new Node[numNodesZ, numNodesY, numNodesX];
        for (var i = 0; i < numNodesZ; ++i) {
            for (var j = 0; j < numNodesY; ++j) {
                for(var k = 0; k < numNodesX; ++k) {
                    nodes[i, j, k] = new Node(k, j, i);
                }
            }
        }
    }

    private void CreateSquareRoom(Node node) {
        if (node.gridX + 1 >= numNodesX || node.gridZ + 1 >= numNodesZ) {
            return;
        }

        var botLeft = node;
        var topLeft = nodes[node.gridZ+1, node.gridY, node.gridX];
        var topRight = nodes[node.gridZ+1, node.gridY, node.gridX+1];
        var botRight = nodes[node.gridZ, node.gridY, node.gridX+1];

        if (
            botLeft.HasStairs()
            || botRight.HasStairs()
            || !allowOverlappingRooms
            && (
                botLeft.IsMerged()
                || topLeft.IsMerged()
                || topRight.IsMerged()
                || botRight.IsMerged()
            )
        ) {
            return;
        }

        botLeft.SetMerge(FRONT);
        botLeft.SetMerge(RIGHT);
        topLeft.SetMerge(RIGHT);
        topLeft.SetMerge(BACK);

        topRight.SetMerge(LEFT);
        topRight.SetMerge(BACK);
        botRight.SetMerge(FRONT);
        botRight.SetMerge(LEFT);
    }

    private bool CreateVerticalLine(Node node) {
        if (node.gridZ + 3 >= numNodesZ) {
            return false;
        }

        var bot = node;
        var mid = nodes[node.gridZ + 1, node.gridY, node.gridX];
        var top = nodes[node.gridZ + 2, node.gridY, node.gridX];

        if (
            !allowOverlappingRooms
            && (
                bot.IsMerged()
                || mid.IsMerged()
                || top.IsMerged()
            )
        ) {
            return false;
        }

        top.SetMerge(BACK);
        mid.SetMerge(FRONT);
        mid.SetMerge(BACK);
        bot.SetMerge(FRONT);
        return true;
    }

    private bool CreateHorizontalLine(Node node) {
        if (node.gridX + 3 >= numNodesX) {
            return false;
        }

        var bot = node;
        var mid = nodes[node.gridZ, node.gridY, node.gridX + 1];
        var top = nodes[node.gridZ, node.gridY, node.gridX + 2];

        if (
            bot.HasStairs()
            || mid.HasStairs()
            || !allowOverlappingRooms
            && (
                bot.IsMerged()
                || mid.IsMerged()
                || top.IsMerged()
            )
        ) {
            return false;
        }

        top.SetMerge(LEFT);
        mid.SetMerge(RIGHT);
        mid.SetMerge(LEFT);
        bot.SetMerge(RIGHT);
        return true;
    }

    private void CreateCornerRoom(Node node) {
        if (rnd.Next(100) < 50 && node.gridZ + 1 < numNodesZ) {
            var topNode = nodes[node.gridZ + 1, node.gridY, node.gridX];
            if (rnd.Next(100) < 50) {
                if ((!topNode.IsMerged() || allowOverlappingRooms) && CreateHorizontalLine(node)) {
                    topNode.SetMerge(BACK);
                    node.SetMerge(FRONT);
                    return;
                }
            } else if ((!node.IsMerged() || allowOverlappingRooms) && CreateHorizontalLine(topNode)) {
                topNode.SetMerge(BACK);
                node.SetMerge(FRONT);
                return;
            }
        }

        if (node.gridX + 1 >= numNodesX) {
            return;
        }

        var rightNode = nodes[node.gridZ, node.gridY, node.gridX + 1];
        if (rnd.Next(100) < 50) {
            if ((!rightNode.IsMerged() || allowOverlappingRooms) && !node.HasStairs() && CreateVerticalLine(node)) {
                rightNode.SetMerge(LEFT);
                node.SetMerge(RIGHT);
            }
        } else if ((!node.IsMerged() || allowOverlappingRooms) && !node.HasStairs() && CreateVerticalLine(rightNode)) {
            rightNode.SetMerge(LEFT);
            node.SetMerge(RIGHT);
        }
    }

    private void CreateTeeRoom(Node node) {
        if (rnd.Next(100) < 50 && node.gridZ + 1 < numNodesZ && node.gridX + 1 < numNodesX) {
            var topCorner = nodes[node.gridZ + 1, node.gridY, node.gridX];
            var topMid = nodes[node.gridZ + 1, node.gridY, node.gridX + 1];
            var botMid = nodes[node.gridZ, node.gridY, node.gridX + 1];
            if (rnd.Next(100) < 50) {
                if ((!topMid.IsMerged() || allowOverlappingRooms) && CreateHorizontalLine(node)) {
                    topMid.SetMerge(BACK);
                    botMid.SetMerge(FRONT);
                    return;
                }
            } else if ((!botMid.IsMerged() || allowOverlappingRooms) && CreateHorizontalLine(topCorner)) {
                topMid.SetMerge(BACK);
                botMid.SetMerge(FRONT);
                return;
            }
        }

        if (node.gridX + 1 >= numNodesX || node.gridZ + 1 >= numNodesZ) {
            return;
        }

        var rightCorner = nodes[node.gridZ, node.gridY, node.gridX + 1];
        var leftMid = nodes[node.gridZ + 1, node.gridY, node.gridX];
        var rightMid = nodes[node.gridZ + 1, node.gridY, node.gridX + 1];
        if (rnd.Next(100) < 50) {
            if ((!rightMid.IsMerged() || allowOverlappingRooms) && CreateVerticalLine(node)) {
                leftMid.SetMerge(RIGHT);
                rightMid.SetMerge(LEFT);
            }
        } else if ((!leftMid.IsMerged() || allowOverlappingRooms) && CreateVerticalLine(rightCorner)) {
            leftMid.SetMerge(RIGHT);
            rightMid.SetMerge(LEFT);
        }
    }

    private void CreateThroughRoom(Node node) {
        if (rnd.Next(100) < 50 && node.gridX + 4 < numNodesX) {
            var rightNode = nodes[node.gridZ, node.gridY, node.gridX + 1];
            if((!node.IsMerged() || allowOverlappingRooms) && !node.HasStairs() && CreateHorizontalLine(rightNode)) {
                node.SetMerge(RIGHT);
                rightNode.SetMerge(LEFT);
            }
            return;
        }

        if (node.gridZ + 4 >= numNodesZ) {
            return;
        }

        var topNode = nodes[node.gridZ + 1, node.gridY, node.gridX];
        if((!node.IsMerged() || allowOverlappingRooms) && CreateVerticalLine(topNode)) {
            node.SetMerge(FRONT);
            topNode.SetMerge(BACK);
        }
    }

    private Node GetRandomNeighbour(Node current) {
        current.GetAllNeighbours(nodes, out var neighbours, out var dirs);

        if (neighbours.Count == 0) {
            return null;
        }

        var i = rnd.Next(neighbours.Count);
        if (rnd.Next(100) < deadEndChance) {
            neighbours[i].visited = true;
            return GetRandomNeighbour(current);
        }

        current.SetConnection(dirs[i]);

        var neighbour = neighbours[i];
        neighbour.direction = opposite[dirs[i]];
        neighbour.SetConnection(neighbour.direction);

        var square  = 100    - squareRoomChance;
        var corner  = square - cornerRoomChance;
        var tee     = corner - teeRoomChance;
        var through = tee    - throughRoomChance;

        var type = rnd.Next(100);
        if      (type >= square)  CreateSquareRoom(neighbour);
        else if (type >= corner)  CreateCornerRoom(neighbour);
        else if (type >= tee)     CreateTeeRoom(neighbour);
        else if (type >= through) CreateThroughRoom(neighbour);

        return neighbour;
    }

    private void GenMaze() {
        lastNode = new Stack<Node>();
        var current = nodes[0, 0, numNodesX/2];
        player.transform.position = new Vector3(current.gridX * GRID_UNIT_SIZE, current.gridY + 2, current.gridZ * GRID_UNIT_SIZE);

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
            var nodePrefab = tunnelPrefab;
            if (node.HasStairs() || node.IsMerged()) {
                nodePrefab = roomPrefab;
            }

            var room = Instantiate(
                nodePrefab,
                new Vector3(node.gridX * GRID_UNIT_SIZE, node.gridY * GRID_UNIT_SIZE, node.gridZ * GRID_UNIT_SIZE),
                Quaternion.identity,
                env.transform
            );
            var objRoom = room.GetComponent<IRoom>();
            objRoom.SetConnections(node);
        }


    }

    public void StartGen() {
        CleanGen();
        InitNodes();
        GenMaze();
        CreateRooms();
    }
    
    private void OnEnable() {
        StartGen();
    }
    
    private void CleanGen() {
        // Iterate through each GameObject
        var children = env.GetComponentsInChildren<Transform>()
            .Where(t => t.parent == env.transform)
            .Select(t => t.gameObject)
            .ToArray();
        foreach (var child in children) {
            Destroy(child);
        }
    }

    private void FixedUpdate() {
        showPath = debugShowPath;
    }
}
