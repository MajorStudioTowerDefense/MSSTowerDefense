using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayTrackerScript : MonoBehaviour
{
    public Sprite greenArrow, redArrow, X;
    public GameObject Mon, Tue, Wed, Thu, Fri, Sat, Sun;
    public GameManager gameManager;
    public List<GameObject> week;
    public bool changeDay;

    private Image icon;
    private GameObject currDay;
    public float revenue, passableRevenue, rent;
    private int day;
    private Vector2 ogPosition, upPosition;


    void Awake()
    {
        day = -1;
        currDay = Mon;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        revenue = gameManager.total;
        rent = gameManager.rent;
        passableRevenue = rent/7;
        week = new List<GameObject> { Mon, Tue, Wed, Thu, Fri, Sat, Sun };
        DontDestroyOnLoad(gameObject);
    }

    void changeTheDay()
    {
        day += 1;
        if (day == 7) day = 0;
        currDay = week[day];
        ogPosition = currDay.GetComponent<RectTransform>().position;
        upPosition = new Vector2(ogPosition.x, ogPosition.y + 3);
        doUI();
        if (currDay == Sun) { checkForPassing(); }
    }

    void doUI()
    {
        currDay.GetComponent<RectTransform>().position = upPosition;
        icon = currDay.transform.GetChild(2).GetComponent<Image>();
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

    private void checkForPassing()
    {
        if (revenue >= rent)
        {
            //pass
        }
        else
        {
            //fail
        }
    }

    private void OnEnable()
    {
        changeTheDay();
    }

    private void OnDisable()
    {
        currDay.GetComponent<RectTransform>().position = ogPosition;

        if (currDay == Sun)
        {
            foreach (GameObject day in week)
            {
                icon = day.transform.GetChild(2).GetComponent<Image>();
                icon.enabled = false;
            }
        }
    }
}
