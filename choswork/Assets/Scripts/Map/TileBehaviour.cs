using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Wall
{
    forward, backward, right, left, up, down
}
public enum StairsAndDoor
{
    RoomWall, RoomDoorWall, Stairs
}
public class TileBehaviour : MonoBehaviour
{
    public GameObject[] walls;
    public GameObject[] stairsAndDoors;
    public bool[] mapStatus;
    public void UpdateRoom(bool[] status)
    {
        mapStatus = status;
        for (int i = 0; i< mapStatus.Length; ++i)
        {
            walls[i].SetActive(!mapStatus[i]);
        }
        bool up = mapStatus[(int)Wall.up];
        bool down = mapStatus[(int)Wall.down];
        stairsAndDoors[(int)StairsAndDoor.Stairs].SetActive(up);
        stairsAndDoors[(int)StairsAndDoor.RoomWall].SetActive(down || up);
        stairsAndDoors[(int)StairsAndDoor.RoomDoorWall].SetActive(!(down || up));
    }
    public void StairPlusOffset()
    {
        Vector3 offset = new Vector3(0, -0.17f, 0);
        stairsAndDoors[(int)StairsAndDoor.Stairs].transform.position += offset;
    }
}
