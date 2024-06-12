using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Maze2DSkeleton;




public class MazeGenerator2D : MonoBehaviour
{
   
    public enum MazeGeneratorMode { WithPrefabs, WihtoutPrefabs }

    public MazeGeneratorMode MazeMode = MazeGeneratorMode.WithPrefabs;

    public GameObject WallPrefab;
    public GameObject teeWallPrefab, crossWallPrefab, impWallPrefab, cornerWallPrefab;



    public int MazeWidth = 10, MazeHeight = 10;
    public Vector2 StartPosition = new Vector2(0, 0);
    public float WallWidth = 1, TunnelWidth = 1;
    public bool IsUseSeed = false;
    public int MazeSeed = 1000;

    public bool UseStartWall = false, UseEndWall = false;
    public bool CreateOnStart = true;
    public GameObject StartWallPrefab, EndWallPrefab;

    private Maze2DSkeleton maze;
    private List<Vector2> walls, startWallArr, endWallArr;
    private GameObject startWall, endWall;
    private bool isDrawed = false;
    private List<GameObject> wallsObj;
    public GameObject DoorPrefab;
  //  List<Maze2DSkeleton.cell> cellsWithDoors = maze.maze.GenerateRandomDoorPositions();
        
    void Start()
    {
        maze = new Maze2DSkeleton(StartPosition, MazeWidth, MazeHeight, TunnelWidth, WallWidth, IsUseSeed, MazeSeed, CreateOnStart, UseStartWall);
        if (CreateOnStart)
        {
            DrawMaze();
            SpawnDoors();
        }


    }
    public void DrawMaze()
    {
        if (isDrawed) UnDrawMaze();
        List<Vector2> walls = maze.GetWallArray();
        List<Vector2> startWallArr = maze.GetStartWallArray();
        List<Vector2> endWallArr = maze.GetEndWallArray();
        List<Maze2DSkeleton.WALL_TYPES> types;

        wallsObj = new List<GameObject>();
        startWall = new GameObject();
        startWall.transform.parent = transform;
        endWall = new GameObject();
        endWall.transform.parent = transform;
        if (MazeMode == MazeGeneratorMode.WithPrefabs)
        {
            int cnt = 0;


            for (int j = 0; j < walls.Count; j++)
            {

                if (UseEndWall || UseStartWall)
                {
                    if (maze.IsPosInArray(walls[j], endWallArr) && UseEndWall)
                    {
                        GameObject obj = (GameObject)Instantiate(EndWallPrefab, walls[j], Quaternion.identity);
                        obj.transform.parent = endWall.transform;
                        obj.name = "EndWall_" + j.ToString();
                        wallsObj.Add(obj);
                        cnt++;
                    }
                    else if (maze.IsPosInArray(walls[j], startWallArr) && UseStartWall)
                    {
                        GameObject obj = (GameObject)Instantiate(StartWallPrefab, walls[j], Quaternion.identity);
                        obj.transform.parent = startWall.transform;
                        obj.name = "StartWall_" + j.ToString();
                        wallsObj.Add(obj);
                        cnt++;
                    }
                    else
                    {
                        GameObject pref = WallPrefab;
                        GameObject wall = (GameObject)Instantiate(pref, new Vector3(walls[j].x, walls[j].y, 0), Quaternion.identity);
                        wall.transform.parent = transform;
                        wall.name = "Wall_" + j.ToString();
                        wallsObj.Add(wall);
                        cnt++;


                    }
                }
                else
                {
                    GameObject pref = WallPrefab;


                    GameObject wall = (GameObject)Instantiate(pref, new Vector3(walls[j].x, walls[j].y, 0), Quaternion.identity);
                    wall.transform.parent = transform;
                    wall.name = "Wall_" + j.ToString();
                    wallsObj.Add(wall);
                    cnt++;

                }
            }
            isDrawed = true;
        }

    }
    //private void SpawnDoors()
    //{
    //    // Reduced minimum distance for a smaller maze
    //    float minDistance = 2; // Adjust based on your maze's cell size and gameplay considerations
    //    List<Vector2> doorPositions = maze.GenerateRandomDoorPositions(8, minDistance);
    //    foreach (Vector2 pos in doorPositions)
    //    {
    //        Instantiate(DoorPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
    //    }
    //}
    public void SpawnDoors()
    {
        // Assuming you have a method like GenerateRandomDoorPositions that generates a list of door positions
        List<Vector2> doorPositions = maze.GenerateRandomDoorPositions(8, 2); // For example, generate 8 door positions with a minimum distance of 2 units

        foreach (Vector2 doorPosition in doorPositions)
        {
            Maze2DSkeleton.cell doorCell = maze.FindCellByCoordinates(doorPosition.x, doorPosition.y);
            Quaternion doorRotation = maze.GetDoorOrientation(doorCell);

            // Now instantiate the door with the position and rotation
            Instantiate(DoorPrefab, doorPosition, doorRotation);
        }
    }



    public void UnDrawMaze()
    {
        if (isDrawed)
        {
            for (int j = 0; j < wallsObj.Count; j++)
            {
                Destroy(wallsObj[j]);

            }
            if (UseStartWall)
            {
                Destroy(startWall);

            }
            if (UseEndWall)
            {
                Destroy(endWall);
            }
            wallsObj.Clear();
            isDrawed = false;
        }
    }

    // Delete all prefabs and skeleton of maze
    public void DeleteMaze()
    {
        if (isDrawed) UnDrawMaze();
        maze.DeleteMaze();
    }

    public void GenerateNewMaze(int newMazeWidth, int newMazeHeight)
    {
        MazeWidth = newMazeWidth;
        MazeHeight = newMazeHeight;
        //    maze.GenerateNewMaze(newMazeWidth, newMazeHeight);
        DrawMaze();
    }
    // Returns list of Vector2 of all impasses
    public List<Vector2> GetImpasses()
    {
        return maze.GetImpasses();
    }
    // Returns list of Vector2 of all CrossRoads
    public List<Vector2> GetCrossRoads()
    {
        return maze.GetCrossRoads();
    }
    // Returns width and height of maze in cells
    public Vector2 GetMazeSize()
    {
        return new Vector2(MazeWidth, MazeHeight);
    }
    // Returns array Vector2 of all cell centers
    public Vector2[,] GetGridCenters()
    {
        return maze.GetGridCenters();
    }
    // Returns position of start cell
    public Vector2 GetCenterPositionOfStartCell()
    {

        return maze.GetCenterPositionOfStartCell();
    }

    // Returns array of Vector2 of way froms start cell to end cell
    public Vector2[] FindWayToEnd()
    {
        return maze.FindWayToEnd();
    }
    // Returns List of Vector3 of all walls position

    public Vector2[] FindWayFromTo(Vector2 point1, Vector2 point2)
    {
        return maze.FindWayBetweenPoints(point1, point2);
    }


}
public class Maze2DSkeleton
{
    private int MazeWidth = 10, MazeHeight = 10;
    private float TunnelWidth = 1;
    private float WallWidth = 1;


   
   // private float MazeDepth = 1;
    private Vector2 startPos = new Vector3(0, 0);

    private bool UseSeed = false, generateOnInit;
    private int MazeSeed = 1000;

    private bool UseStartEndWall = false;
   
    public struct cell
    {
        public bool left;
        public bool right;
        public bool top;
        public bool bot;
        public bool isBorder;
        public bool isVisited;
        public float x;
        public float y;
        public int id;


    };
    public enum WALL_TYPES
    {
        HORIZONTAL,
        VERTICAL,
        TEE_LEFT,
        TEE_RIGHT,
        TEE_TOP,
        TEE_BOT,
        CROSSROAD,
        VERTICAL_IMPASS_TOP,
        VERTICAL_IMPASS_BOT,
        HORIZONTAL_IMPASS_LEFT,
        HORIZONTAL_IMPASS_RIGHT,
        CORNER_TOP_LEFT,
        CORNER_BOT_LEFT,
        CORNER_TOP_RIGHT,
        CORNER_BOT_RIGHT,
        NONE
    }
    public enum WALL_SIDE
    {
        TOP,
        LEFT,
        BOT,
        RIGHT
    }
    private float cell_w, shag;
    private List<cell> cells;

    private bool isGen = false;


    private List<int> points_to_return;

    private Vector2[,] grid;
    
   
    private List<int> IDs;
    
   
    private List<Vector2> wallPosArray, startWallPos, endWallPos, CellCenters;   
    
    private Vector2 localStartPos;
    
    
    private cell startCell, endCell;
    public Maze2DSkeleton(  Vector2 mazePosition, int mazeWidth = 10, int mazeHeight = 10, float tunnelWidth = 1, float wallWidth = 1, 
                            bool useSeed = false, int seed = 1000, bool createOnInit = false, bool useStartEndWalls=false)
    {
        startPos = mazePosition;
        MazeWidth = mazeWidth;
        MazeHeight = mazeHeight;
        WallWidth = wallWidth;
        TunnelWidth = tunnelWidth;
        UseSeed = useSeed;
        MazeSeed = seed;
        generateOnInit = createOnInit;
       
        UseStartEndWall = useStartEndWalls;


        cell_w = TunnelWidth;
        shag = cell_w + WallWidth;
        points_to_return = new List<int>();
        cells = new List<cell>();
        IDs = new List<int>();
       
        wallPosArray = new List<Vector2>();
        endWallPos = new List<Vector2>();
        
        
        startWallPos = new List<Vector2>();
        CellCenters = new List<Vector2>();
        
        
        localStartPos = new Vector2(startPos.x - MazeWidth / 2 * shag, startPos.y - MazeHeight / 2 * shag);

        if (generateOnInit)
        {
            GenerateNewMaze(MazeWidth, MazeHeight);
        }
    }
   
    public void SetMazeTunnelWidth(float w)
    {
        TunnelWidth = w;
    }
    public void SetMazeWallWidth(float w)
    {
        WallWidth = w;
    }
    public void SetStartPosition(Vector2 pos)
    {
        startPos = pos;
    }
    
    
   
    public void DeleteMaze()
    {
        
        CellCenters.Clear();
        cells.Clear();
        
        wallPosArray.Clear();
        endWallPos.Clear();
        points_to_return.Clear();
       
        isGen = false;


    }
    public void ClearAndGenerateNewMaze(int newMazeWidth, int newMazeHeight)
    {
        DeleteMaze();
        MazeWidth = newMazeWidth;
        MazeHeight = newMazeHeight;
        GenerateNewMaze(MazeWidth, MazeHeight);
    }


    public void GeneratemazeWithSeed(int newMazeWidth, int newMazeHeight, Vector2 position, int seed)
    {
        UseSeed = true;
        startPos = position;
        MazeSeed = seed;
        if (isGen) DeleteMaze();
        MazeWidth = newMazeWidth;
        MazeHeight = newMazeHeight;
        GenerateNewMaze(MazeWidth, MazeHeight);

    }
    public void GenerateNewMaze(int newMazeWidth, int newMazeHeight)
    {
        MazeWidth = newMazeWidth;
        MazeHeight = newMazeHeight;
        cell_w = TunnelWidth;
        shag = cell_w + WallWidth;
        localStartPos = new Vector2(startPos.x - MazeWidth / 2 * shag, startPos.y - MazeHeight / 2 * shag);
        grid = new Vector2[newMazeWidth, newMazeHeight];
        // creating maze
        cell c = new cell();

        for (int i = 0; i < MazeWidth; i++)
        {
            for (int j = 0; j < MazeHeight; j++)
            {
                c.bot = true;
                c.top = true;
                c.left = true;
                c.right = true;
                c.isVisited = false;
                if (cells.Count == 0)
                {
                    c.isVisited = true;
                }

                if (j == 0 || j == (MazeHeight - 1) || i == 0 || i == (MazeWidth - 1)) c.isBorder = true;
                else c.isBorder = false;

                c.x = i;
                c.y = j;


                grid[i, j] = new Vector2(startPos.x + i * shag - (MazeWidth / 2) * shag, startPos.y + j * shag - (MazeHeight / 2) * shag);
                CellCenters.Add(grid[i, j]);
                c.id = cells.Count;


                cells.Add(c);
            }
        }
        startCell = cells[0];
        endCell = cells[cells.Count - 1];

        generateMazeByBackTrack();


        isGen = true;


    }

    public List<Vector2> GetImpasses()
    {
        List<Vector2> impasses = new List<Vector2>();
        if (isGen)
        {
            int cnt = 0;

            for (int i = 0; i < cells.Count; i++)
            {

                if (cells[i].bot) cnt++;
                if (cells[i].top) cnt++;
                if (cells[i].left) cnt++;
                if (cells[i].right) cnt++;
                if (cnt == 3) impasses.Add(GetCellCenter(cells[i]));
                cnt = 0;
            }
        }
        return impasses;
    }
    public List<Vector2> GetCrossRoads()
    {
        int cnt = 0;
        List<Vector2> crossRoads = new List<Vector2>();
        if (isGen)
        {
            for (int i = 0; i < cells.Count; i++)
            {

                if (cells[i].bot) cnt++;
                if (cells[i].top) cnt++;
                if (cells[i].left) cnt++;
                if (cells[i].right) cnt++;
                if (cnt == 1) crossRoads.Add(GetCellCenter(cells[i]));
                cnt = 0;
            }
        }
        return crossRoads;
    }
    public Vector2[,] GetGridCenters()
    {
        return grid;
    }
    public List<Vector2> GetCellPositionsList()
    {
        return CellCenters;
    }
    public Vector2 GetCenterPositionOfStartCell()
    {
        Vector2 toRet = Vector2.zero;
        if (isGen)
        {
            toRet = grid[0, 0];
        }
       
        return toRet;
    }
    public Vector2 GetCenterPositionOfEndCell()
    {

        Vector2 toRet = Vector2.zero;
        if (isGen)
        {
            toRet = grid[MazeWidth-1,MazeHeight-1];
        }
        return toRet;
    }

    public void SetSeedForRandom(int s)
    {
        MazeSeed = s;
    }
    public void SetUseSeed(bool useSeed)
    {
        UseSeed = useSeed;
    }
    public bool IsThereWall(Vector2 position)
    {
        bool toRet = false;
        
        if (isGen)
        {
            
            for (int i = 0; i < wallPosArray.Count; i++)
            {
                float dist = Vector2.Distance(position, wallPosArray[i]);
                
                if (dist<=WallWidth/2)
                {
                    
                    toRet = true;
                    break;
                }

            }
        }
        return toRet;
    }

    public Vector2[] FindWayBetweenPoints(Vector2 point1, Vector2 point2)
    {
        Vector2[] toRetWay=new Vector2[0];
        if (isGen)
        {
            unVisitAll();
            points_to_return.Clear();
            cell currentCell = new cell();
            cell c = new cell();
            List<cell> neig = new List<cell>();
            List<cell> cells_2 = cells;
            int firstID = FindClosestCell(point1);
            int lastID = FindClosestCell(point2);
            //Debug.Log("Start: "+firstID + "last: "+lastID);
            IDs.Clear();
            int id = firstID;
            c = cells_2[firstID];
            c.isVisited = true;
            cells_2.RemoveAt(firstID);
            cells_2.Insert(firstID, c);
            bool done = false;
            int count = 0;
            while (!done)
            {
                count++;
                if (count > MazeHeight * MazeWidth * 20)
                {
                    Debug.Log("Error! Can't find way! So much iterations!");
                    break;
                }
                neig = getWays(cells_2[id]);
                //Debug.Log("currentCell " + id);
                if (neig.Count > 0)
                {

                    if (neig.Count > 1)
                    {
                        points_to_return.Add(id);
                    }
                    currentCell = neig[0];

                    id = neig[0].id;

                    IDs.Add(id);

                    currentCell.isVisited = true;

                    cells_2.RemoveAt(id);
                    cells_2.Insert(id, currentCell);
                    neig.Clear();

                }
                else
                {
                    if (points_to_return.Count > 0)
                    {
                        id = points_to_return[points_to_return.Count - 1];
                        points_to_return.RemoveAt(points_to_return.Count - 1);

                        //Debug.Log("RemFrom: "+IDs[IDs.Count - 1]);
                        if (IDs.Count > 0)
                        {
                            while (IDs[IDs.Count - 1] != id)
                            {
                                //Debug.Log("Removed: " + IDs[IDs.Count - 1]);
                                IDs.RemoveAt(IDs.Count - 1);
                                if (IDs.Count == 0) break;
                            }
                           //if(IDs.Count > 0) Debug.Log("RemTo: " + IDs[IDs.Count - 1]);
                        }
                        

                    }
                    else
                    {

                    }
                }
                if (id == lastID)
                {
                    c = cells[lastID];

                    cells.RemoveAt(lastID);
                    cells.Add(c);
                    done = true;
                    //Debug.Log("res: " + id);

                    toRetWay = new Vector2[IDs.Count];
                    for (int i = 0; i < IDs.Count; i++)
                    {
                        toRetWay[i] = GetCellCenter(cells[IDs[i]]);
                        //Debug.Log("resWay: " + IDs[i]);
                    }
                    IDs.Clear();


                }

            }
        }
        unVisitAll();
        return toRetWay;
    }
    public Vector2[] FindWayToEnd()
    {        
        return FindWayBetweenPoints(GetCellCenter(startCell),GetCellCenter(endCell));
    }
    public List<Vector2> GetWallArray()
    {
        if (isGen)
        {
            return wallPosArray;
        }
        else return null;
    }
   
    
    public List<Vector2> GetStartWallArray()
    {
        if (isGen)
        {
            return startWallPos;
        }
        else return null;
    }
    public List<Vector2> GetEndWallArray()
    {
        if (isGen)
        {
            return endWallPos;
        }
        else return null;
    }
    
  

    
    public bool IsPosInArray(Vector2 pos, List<Vector2> array)
    {
        bool toRet = false;
        for (int i = 0; i < array.Count; i++)
        {
            if (pos == array[i])
            {
                toRet = true;
                break;
            }

        }
        return toRet;
    }
    private void writeWallPositions(cell c)
    {

        float dx = startPos.x - (MazeWidth / 2) * shag;
        float dy = startPos.y - (MazeHeight / 2) * shag;

        int wallsPerCell = Mathf.RoundToInt(TunnelWidth / WallWidth) + 2;

        float wallStartCenterX = c.x*shag - cell_w / 2 - WallWidth / 2;
        float wallStartCenterY = c.y * shag - cell_w / 2 - WallWidth / 2;
        if (c.top)
        {

            for (int i = 0; i < wallsPerCell; i++)
            {
                Vector2 pos = new Vector2(localStartPos.x + wallStartCenterX + i * WallWidth, localStartPos.y + wallStartCenterY + WallWidth + cell_w);
                if (!IsPosInArray(pos, wallPosArray))
                {
                    wallPosArray.Add(pos);

                }
                if (c.id == (endCell.id) && endWallPos.Count < wallsPerCell)
                {
                    endWallPos.Add(pos);
                }
                
            }


        }

        if (c.bot)
        {

            for (int i = 0; i < wallsPerCell; i++)
            {
                Vector2 pos = new Vector2(localStartPos.x + wallStartCenterX + i * WallWidth, localStartPos.y + wallStartCenterY);
                if (!IsPosInArray(pos, wallPosArray))
                {
                    wallPosArray.Add(pos);

                }
                if (c.id == (endCell.id) && endWallPos.Count < wallsPerCell)
                {
                    endWallPos.Add(pos);
                }
                if (c.id == startCell.id && startWallPos.Count < wallsPerCell)
                {
                    startWallPos.Add(pos);
                }
            }

        }
        if (c.left)
        {

            for (int i = 0; i < wallsPerCell; i++)
            {
                Vector2 pos = new Vector2(localStartPos.x + wallStartCenterX, localStartPos.y + wallStartCenterY + i * WallWidth);
                if (!IsPosInArray(pos, wallPosArray))
                {
                    wallPosArray.Add(pos);

                }
                if (c.id == (endCell.id) && endWallPos.Count < wallsPerCell)
                {
                    endWallPos.Add(pos);
                }
                if (c.id == startCell.id && startWallPos.Count < wallsPerCell)
                {
                    startWallPos.Add(pos);
                }
            }

        }
        if (c.right)
        {
            for (int i = 0; i < wallsPerCell; i++)
            {
                Vector2 pos = new Vector2(localStartPos.x + wallStartCenterX + WallWidth + cell_w, localStartPos.y + wallStartCenterY + i * WallWidth);
                if (!IsPosInArray(pos, wallPosArray))
                {
                    wallPosArray.Add(pos);

                }
                if (c.id == (endCell.id) && endWallPos.Count < wallsPerCell)
                {
                    endWallPos.Add(pos);
                }
                if (c.id == startCell.id && startWallPos.Count < wallsPerCell)
                {
                    startWallPos.Add(pos);
                }
            }

        }
    }
    private void generateMazeByBackTrack()
    {
        cell currentCell = new cell();
        List<cell> neig = new List<cell>();
        int id = 0;
        bool done = false;
        var rnd = new System.Random(MazeSeed);
        int count = 0;
        while (!done)
        {
            count++;
            if(count>(MazeWidth*MazeHeight*20))
            {
                Debug.Log("Generation is fault!");
                break;
            }

            if (haveNeighbors(cells[id]))
            {

                neig = getNeighborsList(cells[id]);
                if (neig.Count > 0)
                {
                    if (neig.Count > 1) points_to_return.Add(id);

                    int side;
                    if (UseSeed) side = rnd.Next(0, neig.Count);
                    else side = UnityEngine.Random.Range(0, neig.Count);


                    removeWallBetween(id, neig[side].id);
                    id = neig[side].id;

                    currentCell = cells[id];
                    currentCell.isVisited = true;

                    cells.RemoveAt(id);
                    cells.Insert(id, currentCell);
                    neig.Clear();
                }
                else
                {

                }
            }
            else
            {

                if (points_to_return.Count > 0)
                {
                    id = points_to_return[points_to_return.Count - 1];
                    points_to_return.RemoveAt(points_to_return.Count - 1);
                }
            }

            done = isAllCellsVisited();
            if (done)
            {
                points_to_return.Clear();
                for (int i = 0; i < cells.Count; i++)
                {
                    writeWallPositions(cells[i]);

                }
                isGen = true; 
            }

        }
    }
    private void unVisitAll()
    {
        cell c = new cell();
        List<cell> cc = new List<cell>();
        for (int i = 0; i < cells.Count; i++)
        {
            c = cells[i];
            c.isVisited = false;
            cc.Add(c);
        }
        cells = cc;
        startCell.isVisited = false;
        endCell.isVisited = false;
    }
    private List<cell> getWays(cell c)
    {
        List<cell> toRet = new List<cell>();
        float shag2 = 1;
        int id = startCell.id;

        id = findCellByPos(c.x, c.y + shag2);

        if (!c.top && !cells[id].bot && !cells[id].isVisited)
        {
            if (id != startCell.id) toRet.Add(cells[id]);
        }

        id = findCellByPos(c.x, c.y - shag2);

        if (!c.bot && !cells[id].top && !cells[id].isVisited)
        {
            if (id != startCell.id) toRet.Add(cells[id]);
        }

        id = findCellByPos(c.x + shag2, c.y);

        if (!c.right && !cells[id].left && !cells[id].isVisited)
        {
            if (id != startCell.id) toRet.Add(cells[id]);
        }

        id = findCellByPos(c.x - shag2, c.y);

        if (!c.left && !cells[id].right && !cells[id].isVisited)
        {
            if (id != startCell.id) toRet.Add(cells[id]);
        }

        return toRet;
    }

    private Vector2 GetCellCenter(cell c)
    {
        Vector2 toRet = Vector2.zero;
        toRet = new Vector2(localStartPos.x + c.x*shag, localStartPos.y + c.y* shag);
        return toRet;
    }
    List<cell> getNeighborsList(cell c)
    {
        List<cell> not_visited_neighbors = new List<cell>();
        float shag2 = 1;
        int id = 0;
        if (c.x != 0)
        {
            id = findCellByPos(c.x - shag2, c.y);
            if (!cells[id].isVisited) not_visited_neighbors.Add(cells[id]);
        }
        if (c.y != 0)
        {
            id = findCellByPos(c.x, c.y - shag2);
            if (!cells[id].isVisited) not_visited_neighbors.Add(cells[id]);

        }
        if (c.x != MazeWidth * shag2 - shag2)
        {
            id = findCellByPos(c.x + shag2, c.y);
            if (!cells[id].isVisited) not_visited_neighbors.Add(cells[id]);

        }
        if (c.y != MazeHeight * shag2 - shag2)
        {
            id = findCellByPos(c.x, c.y + shag2);
            if (!cells[id].isVisited) not_visited_neighbors.Add(cells[id]);

        }

        return not_visited_neighbors;
    }
    
    
   
    void removeWallBetween(int id1, int id2)
    {
        cell c1, c2;
        c1 = cells[id1];
        c2 = cells[id2];
        int roomID1 = -1, roomID2 = -1;
        
        
        float dx, dy;
        dx = c1.x - c2.x;
        dy = c1.y - c2.y;
        if (dx == 0)
        {
            if (dy > 0)
            {
                c1.bot = false;
                c2.top = false;
                cells.RemoveAt(id1);
                cells.Insert(id1, c1);
                cells.RemoveAt(id2);
                cells.Insert(id2, c2);
                
            }
            else
            {                
                c1.top = false;
                c2.bot = false;
                cells.RemoveAt(id1);
                cells.Insert(id1, c1);
                cells.RemoveAt(id2);
                cells.Insert(id2, c2);
                
            }

        }
        else
        {
            if (dx > 0)
            {
                c1.left = false;
                c2.right = false;
                cells.RemoveAt(id1);
                cells.Insert(id1, c1);
                cells.RemoveAt(id2);
                cells.Insert(id2, c2);
                
            }
            else
            {
                c1.right = false;
                c2.left = false;
                cells.RemoveAt(id1);
                cells.Insert(id1, c1);
                cells.RemoveAt(id2);
                cells.Insert(id2, c2);
                
            }

        }

    }

    bool haveNeighbors(cell c)
    {
        List<cell> not_visited_neighbors = new List<cell>();
        float shag2 = 1;
        int id = 0;
        if (c.x != 0)
        {
            id = findCellByPos(c.x - shag2, c.y);
            if (!cells[id].isVisited) not_visited_neighbors.Add(cells[id]);
        }
        if (c.y != 0)
        {
            id = findCellByPos(c.x, c.y - shag2);
            if (!cells[id].isVisited) not_visited_neighbors.Add(cells[id]);

        }
        if (c.x != MazeWidth * shag2 - shag2)
        {
            id = findCellByPos(c.x + shag2, c.y);
            if (!cells[id].isVisited) not_visited_neighbors.Add(cells[id]);

        }
        if (c.y != MazeHeight * shag2 - shag2)
        {
            id = findCellByPos(c.x, c.y + shag2);
            if (!cells[id].isVisited) not_visited_neighbors.Add(cells[id]);

        }
        
        if (not_visited_neighbors.Count > 0) return true;
        else return false;

    }
    
    int findCellByPos(float x, float y)
    {

        int id = 0;

        for (int i = 0; i < cells.Count; i++)
        {
            float dx = (cells[i].x - x);
            float dy = (cells[i].y - y);

            if ((cells[i].x == x) && ((cells[i].y == y)))
            {
                id = i;

                break;

            }

        }

        return id;
    }

    bool isAllCellsVisited()
    {
        int count = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].isVisited) count++;
        }
        if (count == 0)
        {
            return true;
        }
        else return false;
    }

    int FindClosestCell(Vector2 pos)
    {
        int id=-1;
        float dist = 9999999999;
        for(int i=0;i<cells.Count;i++)
        {
            float mdist = Vector2.Distance(pos, GetCellCenter(cells[i]));
            if(mdist<dist)
            {
                dist = mdist;
                id = i;
            }
        }

        return id;

    }
    public List<Vector2> GenerateRandomDoorPositions(int doorCount, float minDistanceBetweenDoors)
    {
        List<Vector2> potentialDoorPositions = new List<Vector2>();
        List<Vector2> selectedDoorPositions = new List<Vector2>();
        var rnd = new System.Random(); // Consider using MazeSeed for deterministic results

        // Function to check proximity to existing potential doors
        bool IsTooCloseToOtherDoors(Vector2 position, float minDistance, List<Vector2> doorsList)
        {
            return doorsList.Any(door => Vector2.Distance(position, door) < minDistance);
        }

        // Exclude border cells and generate a pool of potential positions
        var possibleCells = cells.Where(c => !c.isBorder).ToList();

        foreach (var cell in possibleCells)
        {
            Vector2 potentialPosition = GetCellCenter(cell);
            if (!IsTooCloseToOtherDoors(potentialPosition, minDistanceBetweenDoors, potentialDoorPositions))
            {
                potentialDoorPositions.Add(potentialPosition);
            }
        }

        // Randomly select from the potential positions if there are enough candidates
        if (potentialDoorPositions.Count >= doorCount)
        {
            while (selectedDoorPositions.Count < doorCount)
            {
                int randomIndex = rnd.Next(potentialDoorPositions.Count);
                Vector2 selectedPosition = potentialDoorPositions[randomIndex];
                if (!selectedDoorPositions.Contains(selectedPosition))
                {
                    selectedDoorPositions.Add(selectedPosition);
                }
            }
        }
        else
        {
            Debug.LogWarning("Not enough positions to place all doors with the specified minimum distance. Doors placed: " + potentialDoorPositions.Count);
            return potentialDoorPositions; // Returning what we have if not enough
        }

        return selectedDoorPositions;
    }
    // This method assumes your cell layout places walls on the exterior sides of cells
    public Quaternion GetDoorOrientation(cell doorCell)
    {
        // If a wall is to the left and right, it's a vertical wall
        if (doorCell.left && doorCell.right)
        {
            return Quaternion.Euler(0, 0, 90); // Door is rotated to fit vertical wall
        }
        // If a wall is to the top and bottom, it's a horizontal wall
        else if (doorCell.top && doorCell.bot)
        {
            return Quaternion.Euler(0, 0, 0); // Door is not rotated, default horizontal
        }

        // If the logic above doesn't determine the orientation,
        // you may want to check the adjacent cells or add more complex logic
        // Default to no rotation
        return Quaternion.identity;
    }

    public cell FindCellByCoordinates(float x, float y)
    {
        return cells.FirstOrDefault(c => Mathf.Approximately(c.x, x) && Mathf.Approximately(c.y, y));
    }


}