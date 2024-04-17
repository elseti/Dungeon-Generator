using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Direction;
using Random = System.Random;

public class MazeGen : MonoBehaviour {
    [Header("Dimensions")]
    public int numNodesX = 5;
    public int numNodesY = 5;
    public int numNodesZ = 5;

    [Header("Probabilities (sum of room chances must be <= 100)")]
    public int deadEndChance = 20;

    public int squareRoomChance = 10;
    public int cornerRoomChance = 10;
    public int teeRoomChance = 10;
    public int throughRoomChance = 10;

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
            botLeft.isMerged()
            || topLeft.isMerged()
            || topRight.isMerged()
            || botRight.isMerged()
            && !allowOverlappingRooms
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
            bot.isMerged()
            || mid.isMerged()
            || top.isMerged()
            && !allowOverlappingRooms
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
            bot.isMerged()
            || mid.isMerged()
            || top.isMerged()
            && !allowOverlappingRooms
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
                if ((!topNode.isMerged() || allowOverlappingRooms) && CreateHorizontalLine(node)) {
                    topNode.SetMerge(BACK);
                    node.SetMerge(FRONT);
                    return;
                }
            } else if ((!node.isMerged() || allowOverlappingRooms) && CreateHorizontalLine(topNode)) {
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
            if ((!rightNode.isMerged() || allowOverlappingRooms) && CreateVerticalLine(node)) {
                rightNode.SetMerge(LEFT);
                node.SetMerge(RIGHT);
            }
        } else if ((!node.isMerged() || allowOverlappingRooms) && CreateVerticalLine(rightNode)) {
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
                if ((!topMid.isMerged() || allowOverlappingRooms) && CreateHorizontalLine(node)) {
                    topMid.SetMerge(BACK);
                    botMid.SetMerge(FRONT);
                    return;
                }
            } else if ((!botMid.isMerged() || allowOverlappingRooms) && CreateHorizontalLine(topCorner)) {
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
            if ((!rightMid.isMerged() || allowOverlappingRooms) && CreateVerticalLine(node)) {
                leftMid.SetMerge(RIGHT);
                rightMid.SetMerge(LEFT);
            }
        } else if ((!leftMid.isMerged() || allowOverlappingRooms) && CreateVerticalLine(rightCorner)) {
            leftMid.SetMerge(RIGHT);
            rightMid.SetMerge(LEFT);
        }
    }

    private void CreateThroughRoom(Node node) {
        if (rnd.Next(100) < 50 && node.gridX + 4 < numNodesX) {
            var rightNode = nodes[node.gridZ, node.gridY, node.gridX + 1];
            if((!node.isMerged() || allowOverlappingRooms) && CreateHorizontalLine(rightNode)) {
                node.SetMerge(RIGHT);
                rightNode.SetMerge(LEFT);
            }
            return;
        }

        if (node.gridZ + 4 >= numNodesZ) {
            return;
        }

        var topNode = nodes[node.gridZ + 1, node.gridY, node.gridX];
        if((!node.isMerged() || allowOverlappingRooms) && CreateVerticalLine(topNode)) {
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
            if (node.hasStairs() || node.isMerged()) {
                nodePrefab = roomPrefab;
            }

            var roomType = nodePrefab.GetComponent<IRoom>();
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

    // private void FetchParams() {
    //     var document = GetComponent<UIDocument>();
    //     var lengthInput = document.rootVisualElement.Q("LengthInput") as IntegerField;
    //     var widthInput = document.rootVisualElement.Q("WidthInput") as IntegerField;
    //     var heightInput = document.rootVisualElement.Q("HeightInput") as IntegerField;
    //     var deadEndChanceInput = document.rootVisualElement.Q("DeadEndChanceInput") as FloatField;
    //
    // }

    private void RunGen() {
        // FetchParams();
        CleanGen();
        InitNodes();
        GenMaze();
        CreateRooms();
    }

    private void Start() {
        RunGen();
    }

    private void FixedUpdate() {
        showPath = debugShowPath;
    }
}
