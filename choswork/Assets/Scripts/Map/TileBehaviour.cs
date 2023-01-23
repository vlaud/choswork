using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Wall
{
    forward, backward, right, left, up, down
}
public enum StairsAndDoor
{
    RoomWall, RoomDoorWall, Stairs, StairWall
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
        stairsAndDoors[(int)StairsAndDoor.StairWall].SetActive(down);
        stairsAndDoors[(int)StairsAndDoor.RoomWall].SetActive(down || up);
        stairsAndDoors[(int)StairsAndDoor.RoomDoorWall].SetActive(!(down || up));
    }
}
