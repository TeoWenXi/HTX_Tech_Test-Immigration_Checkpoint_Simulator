using UnityEngine;

[System.Serializable]
public class QueueSlot : MonoBehaviour
{
    public GameObject agentObject = null;
    public bool isOccupied = false; //Will be true if there is an agent PHYSICALLY in the slot, false otherwise

    [HideInInspector]
    public float timer = 0f; //Timer for how long the agent has been in the slot

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isOccupied)
            timer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (agentObject == null)
            return;

        if (collision.gameObject == agentObject)
            isOccupied = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (agentObject == null || isOccupied)
            return;

        if (collision.gameObject == agentObject)
            isOccupied = true;
    }

    public void ResetVars()
    {
        agentObject = null;
        timer = 0f;
        isOccupied = false;
    }
}
