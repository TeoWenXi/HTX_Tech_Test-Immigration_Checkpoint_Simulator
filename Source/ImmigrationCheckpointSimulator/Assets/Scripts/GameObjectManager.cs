using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public int numStartObjs = 1000;
    public GameObject defaultAgentObj;
    public GameObject inactiveParentObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < numStartObjs; ++i)
        {
            GameObject go = Instantiate(defaultAgentObj);
            go.SetActive(false);
            go.transform.SetParent(inactiveParentObj.transform);
        }
    }

    public GameObject GetNewGO()
    {
        if (inactiveParentObj.transform.childCount > 0)
            return inactiveParentObj.transform.GetChild(0).gameObject;
        
        return(Instantiate(defaultAgentObj));
    }
}
