using System.Collections.Generic;
using static Direction;

public class Node {
    public int gridX;
    public int gridY;
    public int gridZ;

    public bool visited = false;
    public bool connectionLeft = false;
    public bool connectionRight = false;
    public bool connectionFront = false;
    public bool connectionBack = false;
    public bool connectionUp = false;
    public bool connectionDown = false;

    public bool mergeLeft = false;
    public bool mergeRight = false;
    public bool mergeFront = false;
    public bool mergeBack = false;

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
            && !nodes[gridZ, gridY, gridX - 1].HasStairs()
            && !nodes[gridZ, gridY, gridX - 1].visited
        ) {
            neighbours.Add(nodes[gridZ, gridY, gridX - 1]);
            dirs.Add(LEFT);
        }
        if(
            !connectionRight
            && !HasStairs()
            && gridX + 1 < numNodesX
            && !nodes[gridZ, gridY, gridX + 1].visited
        ) {
            neighbours.Add(nodes[gridZ, gridY, gridX + 1]);
            dirs.Add(RIGHT);
        }
        if(
            !HasStairs()
            && !connectionRight
            && !mergeRight
            && 0 < gridY
            && !nodes[gridZ, gridY - 1, gridX].visited
            && !nodes[gridZ, gridY - 1, gridX].connectionRight
            && !nodes[gridZ, gridY - 1, gridX].mergeRight
        ) {
            neighbours.Add(nodes[gridZ, gridY - 1, gridX]);
            dirs.Add(DOWN);
        }
        if(
            !HasStairs()
            && !connectionRight
            && !mergeRight
            && gridY + 1 < numNodesY
            && !nodes[gridZ, gridY + 1, gridX].visited
            && !nodes[gridZ, gridY + 1, gridX].connectionRight
            && !nodes[gridZ, gridY + 1, gridX].mergeRight
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
            && !HasStairs()
            && !IsMerged()
        );
    }

    public bool HasStairs() {
        return connectionUp || connectionDown;
    }

    public bool IsMerged() {
        return mergeLeft || mergeRight || mergeFront || mergeBack;
    }

    public void SetConnection(Direction dir) {
        if (dir == NULL) {
            return;
        }
        switch (dir) {
            case LEFT: connectionLeft = true; break;
            case RIGHT: connectionRight = true; break;
            case FRONT: connectionFront = true; break;
            case BACK: connectionBack = true; break;
            case UP: connectionUp = true; break;
            case DOWN: connectionDown = true; break;
        }
    }

    public void SetMerge(Direction dir) {
        if (dir is NULL or UP or DOWN) {
            return;
        }
        switch (dir) {
            case LEFT: mergeLeft = true; break;
            case RIGHT: mergeRight = true; break;
            case FRONT: mergeFront = true; break;
            case BACK: mergeBack = true; break;
        }
    }
}
