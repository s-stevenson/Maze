using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Maze : MonoBehaviour {
    public GameObject floor;
    public GameObject wall;
    public GameObject player;
    public Material wallMat;
    public Material floorMat;
    
    static readonly int WIDTH = 50;
    static readonly int HEIGHT = 50;
    static readonly int SCALE = 4;
    Cell[,] Map = new Cell[WIDTH, HEIGHT];

    // offset to reach [north, east, south, west] neighboring cell
    int[] DX = {0, 1, 0, -1};
    int[] DY = {1, 0, -1, 0};
    
    // Procedurally generates and instantiates a maze
    // Maze generation logic should probably be separated from game object instantiation at some point
	void Start () {

        // initialize cells of maze
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Map[x, y] = new Cell(x, y);
            }
        }

        // pick start cell for maze generation algorithm
        int startx = 25;
        int starty = 25;
        Map [startx, starty].Visited = true;

        List<Cell> Cells = new List<Cell>();
        Cells.Add(Map [startx, starty]);

        // procedurally generate maze
        while (Cells.Count > 0)
        {
            // choose a cell to expand the maze from, determines how "branchy" the maze will be
            // a 50/50 chance of expanding from previous cell vs new random cell leads to a good balance
            Cell current = Random.Range(0,2) == 0 ? Cells[Cells.Count - 1] : Cells[Random.Range(0, Cells.Count)];

            // try to expand in random direction
            int startdir = Random.Range(0, 4);
            bool FoundUnvisitedNeighbor = false;
            for (int i = startdir; i < startdir + 4 && !FoundUnvisitedNeighbor; i++)
            {
                int dir = i % 4;
                int newx = current.x + DX[dir];
                int newy = current.y + DY[dir];
                if ( newx >= 0 && newx < WIDTH && newy >= 0 && newy < HEIGHT &&  !Map[newx, newy].Visited ) {
                    current.Connections[dir] = true;
                    Map[newx, newy].Connections[(dir + 2) % 4] = true;
                    Map[newx, newy].Visited = true;
                    Cells.Add(Map[newx, newy]);
                    FoundUnvisitedNeighbor = true;
                }
            }
            // if no expansion possible from this cell then done with this cell
            if (!FoundUnvisitedNeighbor)
                Cells.Remove(current);
        }

        // instantiate the maze
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.transform.position = new Vector3(SCALE*x, 0, SCALE*y);
                cell.transform.localScale = new Vector3(SCALE, SCALE, 1);
                cell.transform.Rotate(new Vector3(90, 0, 0));
                cell.renderer.material = floorMat;
                for (int dir = 0; dir < 4; dir++)
                {
                    if (!Map[x,y].Connections[dir] && !(x == 0 && y == 0 && dir == 2)) {
                        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        wall.transform.localScale = new Vector3(SCALE, SCALE, 1);
                        wall.transform.Rotate(new Vector3(0, 90 * dir, 0));
                        wall.transform.position = new Vector3(SCALE*x + (DX[dir] * (SCALE / 2)), (SCALE / 2), SCALE*y + (DY[dir] * (SCALE / 2)));
                        wall.renderer.material = wallMat;
                    }
                }

            }
        }

        // instantiate player in middle of maze
        Instantiate(player, new Vector3(SCALE*startx, 1.01f, SCALE*starty), Quaternion.identity);
	
    }

}

// cell in maze
public class Cell {
                               // north, east, south, west
    public bool[] Connections = { false, false, false, false};
    public bool Visited = false;
    public int x;
    public int y;

    public Cell(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

