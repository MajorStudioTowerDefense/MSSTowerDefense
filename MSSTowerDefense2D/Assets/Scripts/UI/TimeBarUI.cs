using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeBarUI : MonoBehaviour
{
    float shrinkUnit, growUnit;
    RectTransform timeBarRect;

    private void Start()
    {
        timeBarRect = GetComponent<RectTransform>();
        shrinkUnit = timeBarRect.sizeDelta.x / (120*4);
        growUnit = timeBarRect.sizeDelta.x / (5400/2.3f);
        StartCoroutine(shrink());
    }

    public void startCo()
    {
        StartCoroutine(shrink());
    }

    IEnumerator shrink()
    {
        yield return new WaitForSeconds(0.04f);
        timeBarRect.sizeDelta = new Vector2(timeBarRect.sizeDelta.x-shrinkUnit, timeBarRect.sizeDelta.y);
        timeBarRect = GetComponent<RectTransform>();
        if (timeBarRect.sizeDelta.x <= 0f)
        {
            timeBarRect.sizeDelta = new Vector3(0, timeBarRect.sizeDelta.y);
            StartCoroutine(waitASec());
        }
        else
        {
            StartCoroutine(shrink());
        }
    }

    IEnumerator waitASec()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(grow());
    }

    IEnumerator grow()
    {
        timeBarRect = GetComponent<RectTransform>();
        yield return new WaitForSeconds(0.04f);
        timeBarRect.sizeDelta = new Vector2(timeBarRect.sizeDelta.x + growUnit, timeBarRect.sizeDelta.y);
        if (timeBarRect.sizeDelta.x >= 481.5711f)
        {
            timeBarRect.sizeDelta = new Vector2(481.5711f, timeBarRect.sizeDelta.y);
        }
        else
        {
            StartCoroutine(grow());
        }
    }
}
