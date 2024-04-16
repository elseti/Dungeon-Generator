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
