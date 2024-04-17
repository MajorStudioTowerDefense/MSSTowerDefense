using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCardStackingUIScript : MonoBehaviour
{
    public GameObject IDprefab;
    public GameObject firstID;
    public GameObject lastSkeleton;
    private List<GameObject> IDs;

    private Vector2 topright;
    private Vector2 bottomleft;
    private Vector3 middle;
    [SerializeField] float increment, marginX;

    void Start()
    {
        IDs = new List<GameObject>() { firstID };
        topright = GetComponentInParent<RectTransform>().offsetMax;
        bottomleft = GetComponentInParent<RectTransform>().offsetMin;
        middle = new Vector2((topright.x+bottomleft.x)/2, (topright.y+bottomleft.y)/2);
    }

    public void CreateNewCard(GameObject emp)
    {
        lastSkeleton = emp;
        GameObject newCard = Instantiate(IDprefab, Vector3.forward, Quaternion.identity);
        Instantiate(newCard, this.transform);
        IDs.Add(newCard);
        Refactor();
    }

    public void Refactor()
    {
        int length = IDs.Count;
        increment = (topright.y - bottomleft.y) / 8;
        int curr = length / 2;
        IDs[curr].GetComponent<RectTransform>().position = middle;
        while (curr > 0) {
            curr--;
            IDs[curr].GetComponent<RectTransform>().position = new Vector2(middle.x, IDs[curr + 1].GetComponent<RectTransform>().position.y + increment);
        }
        curr = length / 2;
        while (curr < length)
        {
            curr++;
            IDs[curr].GetComponent<RectTransform>().position = new Vector2(middle.x, IDs[curr - 1].GetComponent<RectTransform>().position.y - increment);
        }
        if (length%2 == 0)
        {
            int number = 10;
            foreach(GameObject id in IDs) {
                number -= 1;
                id.GetComponent<RectTransform>().position = new Vector3(id.GetComponent<RectTransform>().position.x, middle.z - number);
            }
        }
    }
}
