using static Direction;

public class Node {
    public int gridX;
    public int gridY;

    public bool visited = false;
    public bool connectionLeft = false;
    public bool connectionRight = false;
    public bool connectionFront = false;
    public bool connectionBack = false;
    public Direction direction = NULL;
    public Node(int gridX, int gridY) {
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public bool isEmpty() {
        return connectionLeft || connectionRight || connectionFront || connectionBack;
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
        }
    }
}
