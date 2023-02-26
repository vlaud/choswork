using System.Collections.Generic;
using Unity.AI.Navigation;
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
    // 디폴트 프로퍼티
    [field: Header("플레이어 위치 설정")]
    [SerializeField] Vector3 PlayerStartPos = Vector3.zero;
    [field: SerializeField] public Transform PlayerStart
    {
        get;
        private set;
    }
    [field: Header("몬스터 위치 설정")]
    [SerializeField] Vector3 MobPos = Vector3.zero;
    [SerializeField] GameObject StartObj;
    [field:SerializeField] public List<Transform> StartPoint
    {
        get;
        private set;
    }
    public List<int> StartNum;
    [SerializeField] GameObject EndObj;
    [field:SerializeField] public List<Transform> EndPoint
    {
        get;
        private set;
    }
    public List<int> EndNum;
    [field: Header("크롤러 위치 설정")]
    [SerializeField] Vector3 Cr_MobPos = Vector3.zero;
    [field: SerializeField] public Transform Cr_StartPoint
    {
        get;
        private set;
    }
    public int Cr_StartNum;
    [field: SerializeField] public Transform Cr_EndPoint
    {
        get;
        private set;
    }
    public int Cr_EndNum;
    [field: Header("아이템 위치 설정")]
    [field: SerializeField] public Transform ItemPoint
    {
        get;
        private set;
    }
    [SerializeField] Vector3 ItemStartPos = Vector3.zero;
    [Header("오브젝트 위치 설정")]
    [SerializeField] Vector3 doorPos = Vector3.zero;
    [SerializeField] Vector3 keypadPos = Vector3.zero;
    [SerializeField] Vector3 hintNotePos = Vector3.zero;
    [Header("맵 크기 설정")]
    [SerializeField] Vector3Int mapSize = Vector3Int.zero;
    [SerializeField] int startPos = 0;
    public NavMeshSurface surfaces;
    public Transform ceilingSurfParent;
    public NavMeshSurface[] ceilingSurface;
    public Vector3 offset;
    List<Cell> board;
    [Header("오브젝트 설정")]
    [SerializeField] GameObject map;
    [SerializeField] GameObject keyPad;
    [SerializeField] Transform item;
    [SerializeField] Transform doorObj;
    NavMeshPath myPath = null;
    [ContextMenu("맵 생성")]
    void CreateMap()
    {
        DestroyMap();
        for (int y = 0; y < mapSize.y; ++y)
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
    private void Awake()
    {
        Inst = this;
        MazeGenerator();
        MobSpawning();
        CrawlerSpawning();
        KeySpawning();
        DoorSpawn();
        PlayerSpawn();
        HintNoteSpawn();
    }
    // Start is called before the first frame update
    void Start()
    {
        myPath = new NavMeshPath();
        surfaces.BuildNavMesh();

        if(ceilingSurfParent != null && ceilingSurface != null)
        {
            ceilingSurface = ceilingSurfParent.GetComponentsInChildren<NavMeshSurface>();
            foreach (var surf in ceilingSurface) surf?.BuildNavMesh();
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            surfaces.BuildNavMesh();
            if (ceilingSurfParent != null && ceilingSurface != null)
            {
                ceilingSurface = ceilingSurfParent.GetComponentsInChildren<NavMeshSurface>();
                foreach (var surf in ceilingSurface) surf?.BuildNavMesh();
            }
        }
    }
    #region ObjectSpawn
    void DoorSpawn()
    {
        doorObj.SetParent(transform.GetChild(Random.Range(1, mapSize.x)));
        doorObj.localPosition = doorPos;
    }
    void KeySpawning()
    {
        ItemPoint.SetParent(transform.GetChild(Random.Range(1, transform.childCount)));
        ItemPoint.transform.localPosition = ItemStartPos;
        item.transform.position = ItemPoint.position;
    }
    void HintNoteSpawn()
    {
        GameObject obj = Instantiate(keyPad, transform.GetChild(0));
        obj.transform.localPosition = keypadPos;
        Transform hintnote = obj.GetComponent<Keypad>().myHintNote;
        hintnote.SetParent(transform.GetChild(Random.Range(1, transform.childCount)));
        hintnote.localPosition = hintNotePos;
        hintnote.SetParent(null);
    }
    #endregion
    #region MobAndPlayer
    void PlayerSpawn()
    {
        int floor = mapSize.x * mapSize.z; // 아래층에서만 소환

        PlayerStart.SetParent(transform.GetChild(0));
        PlayerStart.localPosition = PlayerStartPos;
    }
    void CrawlerSpawning()
    {
        int floor = mapSize.x * mapSize.z; // 맨 위층에서만 소환

        // 중복 숫자 방지
        List<int> spawnNums = GetRandomNumber.GetRanNum(transform.childCount - floor, transform.childCount, 2, false);
        //시작 위치
        GameObject obj = Instantiate(StartObj);
        Cr_StartPoint = obj.transform;
        Cr_StartNum = spawnNums[0];
        Cr_StartPoint.SetParent(transform.GetChild(Cr_StartNum));
        Cr_StartPoint.localPosition = Cr_MobPos;
        //끝 위치
        obj = Instantiate(EndObj);
        Cr_EndPoint = obj.transform;
        Cr_EndNum = spawnNums[1];
        Cr_EndPoint.SetParent(transform.GetChild(Cr_EndNum));
        Cr_EndPoint.localPosition = Cr_MobPos;
    }
    public void CrawlerChangePath(bool isStart)
    {
        int floor = mapSize.x * mapSize.z; // 맨 위층에서만 소환

        int[] remove = { Cr_StartNum, Cr_EndNum };
        List<int> spawnNums = GetRandomNumber.GetRanNum(transform.childCount - floor, transform.childCount, 2, true, remove);

        if (isStart)
        {
            Cr_EndNum = spawnNums[1];
            Cr_EndPoint.transform.SetParent(transform.GetChild(spawnNums[1]));
            Cr_EndPoint.transform.localPosition = Cr_MobPos;
        }
        else
        {
            Cr_StartNum = spawnNums[0];
            Cr_StartPoint.transform.SetParent(transform.GetChild(spawnNums[0]));
            Cr_StartPoint.transform.localPosition = Cr_MobPos;
        }
    }
    void MobSpawning()
    {
        int floor = mapSize.x * mapSize.z; // 1층에선 소환 안되게끔

        for (int i = 0; i < GameManagement.Inst.myMonsters.Length; ++i)
        {
            Debug.Log(GetMonsterTypeClass.GetRagdollAction(GameManagement.Inst.myMonsters[i].transform));
        }
        for (int i = 0; i < GameManagement.Inst.myMonsters.Length; ++i)
        {
            // 중복 숫자 방지
            List<int> spawnNums = GetRandomNumber.GetRanNum(floor, transform.childCount, 2, false);
            //시작 위치
            GameObject obj = Instantiate(StartObj);
            StartPoint.Add(obj.transform);
            StartNum.Add(spawnNums[0]);
            StartPoint[i].SetParent(transform.GetChild(StartNum[i]));
            StartPoint[i].localPosition = MobPos;
            //끝 위치
            obj = Instantiate(EndObj);
            EndPoint.Add(obj.transform);
            EndNum.Add(spawnNums[1]);
            EndPoint[i].SetParent(transform.GetChild(EndNum[i]));
            EndPoint[i].localPosition = MobPos;
        }
    }
    public void MobChangePath(bool isStart, int mobindex)
    {
        int floor = mapSize.x * mapSize.z; // 1층에선 소환 안되게끔

        int[] remove = { StartNum[mobindex], EndNum[mobindex] };
        List<int> spawnNums = GetRandomNumber.GetRanNum(floor, transform.childCount, 2, true, remove);

        if (isStart)
        {
            EndNum[mobindex] = spawnNums[1];
            EndPoint[mobindex].transform.SetParent(transform.GetChild(spawnNums[1]));
            EndPoint[mobindex].transform.localPosition = MobPos;
        }
        else
        {
            StartNum[mobindex] = spawnNums[0];
            StartPoint[mobindex].transform.SetParent(transform.GetChild(spawnNums[0]));
            StartPoint[mobindex].transform.localPosition = MobPos;
        }

    }
    public Transform GetDestination(bool IsStart, int mobindex)
    {
        if (IsStart) return EndPoint[mobindex];
        else return StartPoint[mobindex];
    }
    public Transform GetCrDestination(bool IsStart)
    {
        if (IsStart) return Cr_EndPoint;
        else return Cr_StartPoint;
    }
    #endregion
    #region MapGenerator
    void GenerateDungeon()
    {
        List<TileBehaviour> tiles = new List<TileBehaviour>();

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
                        tiles.Add(newRoom);
                    }
                }
            }
        }
        for(int i = 0; i < tiles.Count; ++i)
        {
            tiles[i].StairPlusOffset();
        }
    }
    [ContextMenu("랜덤 맵 생성")]
    void MazeGenerator()
    {
        DestroyMap();
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
    [ContextMenu("맵 삭제")]
    void DestroyMap()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    #endregion
}
