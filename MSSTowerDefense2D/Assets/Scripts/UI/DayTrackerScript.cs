using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayTrackerScript : MonoBehaviour
{
    public GameObject Mon, Tue, Wed, Thu, Fri, Sat, Sun;
    public GameManager gameManager;
    public List<GameObject> week;
    public bool changeDay;

    private Image icon;
    public GameObject currDay;
    public float revenue, passableRevenue, rent;
    private int day;
    public int dayTracker;
    private Vector2 ogPosition, upPosition;
    public TextMeshProUGUI dayBannerText;


    void Awake()
    {
        day = -1;
        dayTracker = 0;
        currDay = Mon;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        revenue = gameManager.money;
        rent = gameManager.rent;
        passableRevenue = rent/7;
        week = new List<GameObject> { Mon, Tue, Wed, Thu, Fri, Sat, Sun };
        DontDestroyOnLoad(gameObject);
    }

    void changeTheDay()
    {
        day += 1;
        dayTracker++;
        if (day == 7) day = 0;
        currDay = week[day];
        ogPosition = currDay.GetComponent<RectTransform>().position;
        upPosition = new Vector2(ogPosition.x, ogPosition.y + 3);
        doUI();
        //if (currDay == Sun) { checkForPassing(); }

    }

    void doUI()
    {
        icon = currDay.transform.GetChild(3).GetComponent<Image>();
        icon.enabled = true;
        Debug.Log(currDay.name);
        if (currDay == Mon) { dayBannerText.text = "Tuesday"; }
        if (currDay == Tue) { dayBannerText.text = "Wednesday"; }
        if (currDay == Wed) { dayBannerText.text = "Thursday"; }
        if (currDay == Thu) { dayBannerText.text = "Friday"; }
        if (currDay == Fri) { dayBannerText.text = "Saturday"; }
        if (currDay == Sat) { dayBannerText.text = "Sunday"; }
        if (currDay == Sun) { dayBannerText.text = "Monday"; }

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
