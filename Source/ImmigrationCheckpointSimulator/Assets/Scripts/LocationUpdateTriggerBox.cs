using UnityEngine;

public class LocationUpdateTriggerBox : MonoBehaviour
{
    public WorldManager.WorldSegments worldSegment;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Agent agentObj = collision.GetComponent<Agent>();
        if (agentObj == null)
            return;
        if (agentObj.GetComponent<Agent>().currAgentWorldSegment >= worldSegment)
            return;

        agentObj.lastClearedWorldSegment = agentObj.currAgentWorldSegment;
        agentObj.currAgentWorldSegment = worldSegment;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Agent agentObj = collision.GetComponent<Agent>();
        if (agentObj == null)
            return;
        if (agentObj.GetComponent<Agent>().currAgentWorldSegment >= worldSegment)
            return;

        agentObj.lastClearedWorldSegment = agentObj.currAgentWorldSegment;
        agentObj.currAgentWorldSegment = worldSegment;
    }
}
