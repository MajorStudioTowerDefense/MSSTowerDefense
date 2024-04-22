using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //0:U, 1:D, 2:L, 3:R
    public GameObject[] walls;
    [HideInInspector] public Vector3 roomPos;
public List<List<string>> roomLayout; 

    public void Init(Vector3 position, List<List<string>> layout)
    {
        Debug.Log(position);
        roomPos = position;
        transform.position = position;
        roomLayout = layout;
    }

    public int GetWidth()
    {
        if (roomLayout != null && roomLayout.Count > 0)
        {
            return roomLayout[0].Count;
        }
        return 0;
    }

    public int GetHeight()
    {
        if (roomLayout != null)
        {
            return roomLayout.Count;
        }
        return 0;
    }

    public Vector3 AddRoom(Room current)
    {
        int index = Random.Range(0, walls.Length);
        GameObject wall = walls[index];
        if (!wall.activeSelf || index == 2) return AddRoom(current);
        else if (wall.activeSelf)
        {
            GameManager.instance.CloseExits(exitPos);
            switch (index)
            {
                case 0:
                    walls[0].SetActive(false);
                    current.walls[1].SetActive(false);
                    return new Vector3(transform.position.x, transform.position.y + GameManager.instance.gridCellHeight);
                case 1:
                    walls[1].SetActive(false);
                    current.walls[0].SetActive(false);
                    return new Vector3(transform.position.x, transform.position.y - GameManager.instance.gridCellHeight);
                case 2:
                    walls[2].SetActive(false);
                    current.walls[3].SetActive(false);
                    return new Vector3(transform.position.x - GameManager.instance.gridCellLength, transform.position.y);
                case 3:
                    walls[3].SetActive(false);
                    current.walls[2].SetActive(false);
                    return new Vector3(transform.position.x + GameManager.instance.gridCellLength, transform.position.y);
            }
        }
        return Vector3.zero;

    }

}
