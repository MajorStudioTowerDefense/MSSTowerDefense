using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    // use the slashed out stuff if instead want the players still in the store to keep moving (alternate universe pause)
    //private PathProcessor.GraphUpdateLock graphLock;

    //graphLock = AstarPath.active.PausePathfinding();
    // Here we can modify the graphs safely. For example by increasing the penalty of a node
    //AstarPath.active.data.gridGraph.GetNode(0, 0).Penalty += 1000;

    // Allow pathfinding to resume
    //graphLock.Release();

    [SerializeField] GameObject gameManager;

    public void pauseGame()
    {
        //stopAI();
        //disableShelves();
        Time.timeScale = 0.0f;
    }

    public void resumeGame()
    {
        //startAI();
        //enableShelves();
        Time.timeScale = 1.0f;
    }

    private void stopAI()
    {
        var ai = FindObjectsOfType<CircleCollider2D>();
        foreach (var bot in ai)
        {
            bot.gameObject.GetComponent<IAstarAI>().canMove = false;
        }
    }

    private void startAI()
    {
        var ai = FindObjectsOfType<CircleCollider2D>();
        foreach (var bot in ai)
        {
            bot.gameObject.GetComponent<IAstarAI>().canMove = true;
        }
    }

    private void disableShelves()
    {
        gameManager.GetComponent<ShelfPlacementManager>().enabled = false;
    }

    private void enableShelves()
    {
        gameManager.GetComponent<ShelfPlacementManager>().enabled = true;
    }
}
