using System.Collections.Generic;
using static Direction;

public class Node {
    public int gridX;
    public int gridY;
    public int gridZ;

    public NodeType type;

    public bool visited = false;
    public bool connectionLeft = false;
    public bool connectionRight = false;
    public bool connectionFront = false;
    public bool connectionBack = false;
    public bool connectionUp = false;
    public bool connectionDown = false;

    public Direction direction = NULL;
    public Node(int gridX, int gridY, int gridZ) {
        this.gridX = gridX;
        this.gridY = gridY;
        this.gridZ = gridZ;
    }

    public void GetAllNeighbours(Node[,,] nodes, out List<Node> neighbours, out List<Direction> dirs) {
        neighbours = new List<Node>();
        dirs = new List<Direction>();
        var numNodesZ = nodes.GetLength(0);
        var numNodesY = nodes.GetLength(1);
        var numNodesX = nodes.GetLength(2);

        if(
            !connectionLeft
            && 0 < gridX
            && !nodes[gridZ, gridY, gridX - 1].hasStairs()
            && !nodes[gridZ, gridY, gridX - 1].visited
        ) {
            neighbours.Add(nodes[gridZ, gridY, gridX - 1]);
            dirs.Add(LEFT);
        }
        if(
            !connectionRight
            && !hasStairs()
            && gridX + 1 < numNodesX
            && !nodes[gridZ, gridY, gridX + 1].visited
        ) {
            neighbours.Add(nodes[gridZ, gridY, gridX + 1]);
            dirs.Add(RIGHT);
        }
        if(
            !hasStairs()
            && !connectionRight
            && 0 < gridY
            && !nodes[gridZ, gridY - 1, gridX].visited
        ) {
            neighbours.Add(nodes[gridZ, gridY - 1, gridX]);
            dirs.Add(DOWN);
        }
        if(
            !hasStairs()
            && !connectionRight
            && gridY + 1 < numNodesY
            && !nodes[gridZ, gridY + 1, gridX].visited
        ) {
            neighbours.Add(nodes[gridZ, gridY + 1, gridX]);
            dirs.Add(UP);
        }
        if(
            !connectionBack
            && 0 < gridZ
            && !nodes[gridZ - 1, gridY, gridX].visited
        ) {
            neighbours.Add(nodes[gridZ - 1, gridY, gridX]);
            dirs.Add(BACK);
        }
        if(
            !connectionFront
            && gridZ + 1 < numNodesZ
            && !nodes[gridZ + 1, gridY, gridX].visited
        ) {
            neighbours.Add(nodes[gridZ + 1, gridY, gridX]);
            dirs.Add(FRONT);
        }
    }

    public bool isEmpty() {
        return (
            !connectionLeft
            && !connectionRight
            && !connectionFront
            && !connectionBack
            && !connectionUp
            && !connectionDown
        );
    }

    public bool hasStairs() {
        return connectionUp || connectionDown;
    }

    public void SetConnection(Direction dir) {
        if (dir == NULL) {
            return;
        }
        switch (dir) {
            case LEFT:
                connectionLeft = true;
                break;
            case RIGHT:
                connectionRight = true;
                break;
            case FRONT:
                connectionFront = true;
                break;
            case BACK:
                connectionBack = true;
                break;
            case UP:
                connectionUp = true;
                break;
            case DOWN:
                connectionDown = true;
                break;
        }
    }
}
