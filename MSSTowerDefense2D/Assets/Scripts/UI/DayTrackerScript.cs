using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayTrackerScript : MonoBehaviour
{
    public Sprite greenArrow, redArrow, X;
    public GameObject Mon, Tue, Wed, Thu, Fri, Sat, Sun;
    public List<GameObject> week;
    public bool changeDay;

    private Image icon;
    private GameObject currDay;
    private float day, revenue, passableRevenue;
    private Vector2 ogPosition, upPosition;

    void Start()
    {
        day = -1;
        currDay = Mon;
        ogPosition = Mon.GetComponent<RectTransform>().position;
        upPosition = new Vector2(ogPosition.x, 184f);
        week = new List<GameObject>{Mon, Tue, Wed, Thu, Fri, Sat, Sun};

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (changeDay == true)
        {
            changeTheDay();
            changeDay = false;
        }
    }

    void changeTheDay()
    {
        day += 1;
        if (day == 8) day = 0;

        switch (day)
        {
            case 0:
                currDay = Mon;
                doUI(currDay);
                break;
            case 1:
                currDay = Tue;
                doUI(currDay);
                break;
            case 2:
                currDay = Wed;
                doUI(currDay);
                break;
            case 3:
                currDay = Thu;
                doUI(currDay);
                break;
            case 4:
                currDay = Fri;
                doUI(currDay);
                break;
            case 5:
                currDay = Sat;
                doUI(currDay);
                break;
            case 6:
                currDay = Sun;
                doUI(currDay);
                checkForPassing();
                break;
        }
    }

    void doUI(GameObject currDay)
    {
        if (currDay == Mon)
        {
           foreach (GameObject day in week)
            {
                day.GetComponent<RectTransform>().position = ogPosition;

            }
        }
        foreach (GameObject day in week)
        {
            day.GetComponent<RectTransform>().position = ogPosition;
           
            if (day == currDay)
            {
                icon = day.transform.GetChild(2).gameObject.GetComponent<Image>();
                icon.enabled = true;

                if (revenue > passableRevenue)
                {
                    icon.sprite = greenArrow;
                }
                else if (revenue > 0)
                {
                    icon.sprite = redArrow;
                }
                else
                {
                    icon.sprite = X;
                }
            }
            break;

        }
        currDay.GetComponent<RectTransform>().position = upPosition;
    }

    private void checkForPassing()
    {
        if (revenue < 0)
        {
            //fail
        }
        //pass
    }

}
