using UnityEngine;

public class CustomMatcher : MonoBehaviour
{
    public string targetObjectTag;
    private GameObject satisfactionUI;
    void Update()
    {
        GameObject targetObject = GameObject.FindWithTag(targetObjectTag);

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