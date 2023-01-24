using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[6];
    }
    public static MapManager Inst = null;
    // µðÆúÆ® ÇÁ·ÎÆÛÆ¼
    [field:SerializeField] public Transform StartPoint
    {
        get;
        private set;
    }
    [field:SerializeField] public Transform EndPoint
    {
        get;
        private set;
    }
    [SerializeField] Vector3Int mapSize = Vector3Int.zero;
    [SerializeField] int startPos = 0;
    public Vector3 offset;
    List<Cell> board;
    [SerializeField] GameObject map;
    NavMeshPath myPath = null;
    [ContextMenu("¸Ê »ý¼º")]
    void CreateMap()
    {
        while(transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        for(int y = 0; y < mapSize.y; ++y)
        {
            for (int z = 0; z < mapSize.z; ++z)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    GameObject obj = Instantiate(map, transform);
                    obj.transform.localPosition = new Vector3(x, y, z);
                    obj.name = $"Tile{x},{y},{z}";
                }
            }
        }
        transform.localPosition = new Vector3(-mapSize.x / 2.0f + 0.5f, 0, -mapSize.z / 2.0f + 0.5f);
    }

    
    public bool PathCheck()
    {
        NavMesh.CalculatePath(StartPoint.position, EndPoint.position, NavMesh.AllAreas, myPath);
        return myPath.status == NavMeshPathStatus.PathComplete ? true : false;
    }
    private void Awake()
    {
        Inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        myPath = new NavMeshPath();
        //MazeGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateDungeon()
    {
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int z = 0; z < mapSize.z; ++z)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    int floor = mapSize.x * mapSize.z;
                    GameObject obj = Instantiate(map, transform);
                    obj.transform.localPosition = new Vector3(offset.x * x, offset.y * y, offset.z * z);
                    obj.name = $"Tile{x},{y},{z}";

                    if (obj.TryGetComponent<TileBehaviour>(out var newRoom))
                    {
                        newRoom.UpdateRoom(board[x + z * mapSize.x + y * floor].status);
                    }
                }
            }
        }
    }
    [ContextMenu("·£´ý ¸Ê »ý¼º")]
    void MazeGenerator()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        board = new List<Cell>();
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int z = 0; z < mapSize.z; ++z)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    board.Add(new Cell());
                }
            }
        }
        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while(k < 1000)
        {
            k++;

            if (board.Count == 0) return;

            board[currentCell].visited = true;

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);
            if(neighbors.Count == 0)
            {
                if(path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    
                    if (newCell - 1 == currentCell)
                    {
                        //right
                        board[currentCell].status[(int)Wall.right] = true;
                        currentCell = newCell;
                        board[currentCell].status[(int)Wall.left] = true;
                    }
                    else if(newCell - mapSize.x == currentCell)
                    {
                        //forward
                        board[currentCell].status[(int)Wall.forward] = true;
                        currentCell = newCell;
                        board[currentCell].status[(int)Wall.backward] = true;
                    }
                    else
                    {
                        //up
                        board[currentCell].status[(int)Wall.up] = true;
                        currentCell = newCell;
                        board[currentCell].status[(int)Wall.down] = true;
                    }
                }
                else
                {
                    if (newCell + 1 == currentCell)
                    {
                        //left, down
                        board[currentCell].status[(int)Wall.left] = true;
                        currentCell = newCell;
                        board[currentCell].status[(int)Wall.right] = true;
                    }
                    else if(newCell + mapSize.x == currentCell)
                    {
                        //backward
                        board[currentCell].status[(int)Wall.backward] = true;
                        currentCell = newCell;
                        board[currentCell].status[(int)Wall.forward] = true;
                    }
                    else
                    {
                        //down
                        board[currentCell].status[(int)Wall.down] = true;
                        currentCell = newCell;
                        board[currentCell].status[(int)Wall.up] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }
    List<int> CheckNeighbors(int cell)
    { 
        if (mapSize.x == 0) return null;
        if (mapSize.y == 0) return null;
        if (mapSize.z == 0) return null;

        List<int> neighbors = new List<int>();
        int floor = mapSize.x * mapSize.z;
        //check forward neighbor
        if (cell % floor < floor - mapSize.x && !board[Mathf.FloorToInt(cell + mapSize.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + mapSize.x));
        }
        //check backward neighbor
        if (cell % floor >= mapSize.x && !board[Mathf.FloorToInt(cell- mapSize.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - mapSize.x));
        }
        //check right neighbor
        if ((cell+1) % mapSize.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }
        //check left neighbor
        if (cell % mapSize.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - 1));
        }
        //check up neighbor
        if (cell + floor < board.Count && !board[Mathf.FloorToInt(cell + floor)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + floor));
        }
        //check down neighbor
        if (cell - floor >= 0 && !board[Mathf.FloorToInt(cell - floor)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - floor));
        }
        return neighbors;
    }
    [ContextMenu("¸Ê »èÁ¦")]
    void DestroyMap()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
