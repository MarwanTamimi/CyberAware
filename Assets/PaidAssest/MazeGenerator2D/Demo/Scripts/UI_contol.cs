using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_contol : MonoBehaviour {

    
    public GameObject newWall;
    public GameObject greenCube, redCube, yellowCube, blackCube, skyCube;
    private MazeGenerator2D mazeScript;
    
	void Start () {
        mazeScript = GameObject.Find("Maze2D").GetComponent<MazeGenerator2D>();
    }
    
	
    public void OnGenMaze()
    {

        mazeScript.GenerateNewMaze(15, 15);
        

    }
   
    

    public void OnShowImpases()
    {
        List<Vector2> impasses = mazeScript.GetImpasses();
        for(int i=0;i<impasses.Count;i++)
        {
            GameObject obj = (GameObject)Instantiate(redCube, impasses[i], Quaternion.identity);
        }
    }
    public void OnShowCenters()
    {
        Vector2 size= mazeScript.GetMazeSize();
        Vector2[,] centers = mazeScript.GetGridCenters();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject obj = (GameObject)Instantiate(greenCube, centers[i,j], Quaternion.identity);
                
            }
        
        }
    }
    public void OnShowCrossRoads()
    {
        List<Vector2> CrossRoads = mazeScript.GetCrossRoads();
        for (int i = 0; i < CrossRoads.Count; i++)
        {
            GameObject obj = (GameObject)Instantiate(yellowCube, CrossRoads[i], Quaternion.identity);
            
        }
    }
    public void clearMaze()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Cube");
        for (int i = 0;i<objs.Length;i++)
        {
            Destroy(objs[i]);
        }
    }
    public void FindWay()
    {
        
        Vector2[] way = mazeScript.FindWayToEnd();
        for (int i = 0; i < way.Length; i++)
        {
            GameObject obj = (GameObject)Instantiate(blackCube, way[i], Quaternion.identity);
        }
    }

    
}
