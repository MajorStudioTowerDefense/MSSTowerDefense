using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayTrackerScript : MonoBehaviour
{
    public GameObject Monday, Tuesday, Wednesday, Thusday, Friday, Saturday, Sunday;
    public GameManager gameManager;
    public List<GameObject> week;
    public bool changeDay;
    public int weekNum = 1;

    private Image icon;
    private GameObject currDay;
    public float revenue, passableRevenue, rent;
    private int day;
    private Vector2 ogPosition, upPosition;
    public TextMeshProUGUI dayOfWeek, weekNumTMP, weekNumTMP2;


    void Awake()
    {
        day = -1;
        currDay = Monday;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        revenue = gameManager.total;
        rent = gameManager.rent;
        passableRevenue = rent/7;
        week = new List<GameObject> { Monday, Tuesday, Wednesday, Thusday, Friday, Saturday, Sunday };
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

        if (currDay == Sunday) { checkForPassing(); }
    }

    void doUI()
    {
        icon = currDay.transform.GetChild(3).GetComponent<Image>();
        icon.enabled = true;
        dayOfWeek.text = currDay.name;
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

        if (currDay == Sunday)
        {
            foreach (GameObject day in week)
            {
                icon = day.transform.GetChild(2).GetComponent<Image>();
                icon.enabled = false;
            }
            weekNum += 1;
            weekNumTMP.text = "Week " + weekNum;
            weekNumTMP2.text = "Week " + weekNum;


        }
    }
}
