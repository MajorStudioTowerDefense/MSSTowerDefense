using UnityEngine;

public class SatisfactionMatcher : MonoBehaviour
{
    private GameObject satisfactionUI;
    void Update()
    {
        GameObject targetObject = GameObject.FindWithTag("Satisfaction");

        if (targetObject != null)
        {
            satisfactionUI = targetObject; 

            transform.position = satisfactionUI.transform.position;
            transform.rotation = satisfactionUI.transform.rotation;
            transform.localScale = satisfactionUI.transform.localScale;
        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}