using UnityEngine;

public class TriggerBoxNextAction : MonoBehaviour
{
    public WorldManager.WorldSegments worldSegment;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Move agent to next state when entering trigger box
        Agent agentObj = other.GetComponent<Agent>();
        if (agentObj == null)
            return;
        if (agentObj.GetComponent<Agent>().lastClearedWorldSegment >= worldSegment || agentObj.GetComponent<Agent>().currAgentWorldSegment >= worldSegment)
            return;

        if (agentObj.GetComponent<Agent>().currAgentWorldSegment < worldSegment)
        {
            agentObj.lastClearedWorldSegment = agentObj.currAgentWorldSegment;
            agentObj.currAgentWorldSegment = worldSegment;
        }

        //Update agent state in world manager
        FindAnyObjectByType<WorldManager>().UpdateAgentNextState(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Agent agentObj = other.GetComponent<Agent>();
        if (agentObj == null)
            return;
        if (agentObj.GetComponent<Agent>().lastClearedWorldSegment >= worldSegment)
            return;

        if (agentObj.GetComponent<Agent>().currAgentWorldSegment < worldSegment)
        {
            agentObj.lastClearedWorldSegment = agentObj.currAgentWorldSegment;
            agentObj.currAgentWorldSegment = worldSegment;
        }

        //Update agent state in world manager
        FindAnyObjectByType<WorldManager>().UpdateAgentNextState(other.gameObject);
    }
}
