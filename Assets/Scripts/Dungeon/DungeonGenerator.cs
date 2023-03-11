using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{

    // Public variables
    public GameObject[] roomPrefabs; // An array of room prefabs to choose from
    public int dungeonWidth = 10; // The width of the dungeon in number of rooms
    public int dungeonLength = 10; // The length of the dungeon in number of rooms
    public int roomWidth = 10; // The width of a room in game units
    public int roomHeight = 5; // The height of a room in game units
    public float hallwayWidth = 2f; // The width of hallways between rooms
    public int numFloors = 1; // The number of floors in the dungeon
    public float floorHeight = 10f; // The height between floors

    // Private variables
    private List<GameObject> rooms = new List<GameObject>();
    private List<Vector3> roomPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateRooms();
        GenerateHallways();
    }

    // Generate the rooms for the dungeon
    void GenerateRooms()
    {
        for (int i = 0; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonLength; j++)
            {
                // Choose a random room prefab from the array
                int index = Random.Range(0, roomPrefabs.Length);
                GameObject room = Instantiate(roomPrefabs[index]);
                room.transform.position = new Vector3(i * (roomWidth + hallwayWidth), 0, j * (roomWidth + hallwayWidth));
                rooms.Add(room);
                roomPositions.Add(room.transform.position);
            }
        }
    }

    // Generate hallways between the rooms
    void GenerateHallways()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                if (Vector3.Distance(rooms[i].transform.position, rooms[j].transform.position) < (roomWidth + hallwayWidth))
                {
                    // Generate a hallway between the two rooms
                    GameObject hallway = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    float distance = Vector3.Distance(rooms[i].transform.position, rooms[j].transform.position) - roomWidth;
                    hallway.transform.localScale = new Vector3(hallwayWidth, roomHeight, distance);
                    hallway.transform.position = (rooms[i].transform.position + rooms[j].transform.position) / 2;
                    hallway.transform.LookAt(rooms[j].transform.position);
                    Destroy(hallway.GetComponent<BoxCollider>());
                }
            }
        }
    }
}