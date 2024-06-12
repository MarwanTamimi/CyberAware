using UnityEngine;

public class MazeRespawnManager : MonoBehaviour
{
    public MazeGenerator2D mazeGenerator; 
    public GameObject player;

    // Call this method to respawn the player at the start of the maze
    public void RespawnPlayerAtStart()
    {
        if (mazeGenerator != null && player != null)
        {
            Vector2 startPosition = mazeGenerator.StartPosition;
            player.transform.position = new Vector3(startPosition.x, startPosition.y, player.transform.position.z);

            // If the player has a Rigidbody2D and you need to reset its velocity
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
            Debug.Log("Respawning player at start.");

            
        }
    }
}
