using UnityEngine;

public class StationTriggerBox : MonoBehaviour
{
    public GameObject stationObjRef = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stationObjRef = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Station stationRef = stationObjRef.GetComponent<Station>();
        if (stationRef.currAgentObj == null)
            return;

        if (collision.gameObject == stationRef.currAgentObj)
            stationRef.isOccupied = true;
    }
}
