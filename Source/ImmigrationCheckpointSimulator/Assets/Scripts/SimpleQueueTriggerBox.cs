using UnityEngine;

public class SimpleQueueTriggerBox : MonoBehaviour
{
    void Start()
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check for agent
        if (collision.GetComponent<Agent>() == null)
            return;

        //Check if agent has already cleared this segment
        WorldManager.WorldSegments segment = transform.parent.Find("Area Sprite").GetComponent<LocationUpdateTriggerBox>().worldSegment;
        if(collision.GetComponent<Agent>().lastClearedWorldSegment >= segment || collision.GetComponent<Agent>().currAgentState == AgentStates.AS_QUEUEING)
            return;

        //Add agent to the correct queue in world manager
        WorldManager worldManagerRef = FindAnyObjectByType<WorldManager>();
        int index = gameObject.transform.parent.GetSiblingIndex();
        worldManagerRef.AddToSimpleQueue(collision.gameObject, segment, index);

        //Update agent state
        collision.GetComponent<Agent>().currAgentState = AgentStates.AS_QUEUEING;
        collision.GetComponent<Agent>().queueStartTime = worldManagerRef.simulationTime;
    }
}
