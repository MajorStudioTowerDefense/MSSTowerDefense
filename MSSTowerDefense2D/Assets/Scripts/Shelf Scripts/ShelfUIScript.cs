using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfUIScript : MonoBehaviour
{
    public Button shopToggle;
    public GameObject currentShelf;
    public GameObject gridBlock;

    private void Update()
    {
        if (currentShelf != null)
        {
            if (Input.GetKeyDown("Q")) { currentShelf.GetComponent<ShelfScript>().rotateLeft(); }
            if (Input.GetKeyDown("E")) { currentShelf.GetComponent<ShelfScript>().rotateRight(); }
            if (Input.GetKeyDown(KeyCode.Delete)) { currentShelf.GetComponent<ShelfScript>().deleteShelf(); }
            if (Input.GetKeyDown(KeyCode.Return)) { currentShelf.GetComponent<ShelfScript>().verifyPlacement(); }

            currentShelf.transform.position = gridBlock.transform.position;
        }
    }

}
