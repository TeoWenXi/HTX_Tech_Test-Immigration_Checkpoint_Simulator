using UnityEngine;

public class QueueTriggerBox : MonoBehaviour
{
    public GameObject queueManagerObj;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Move agent to next state when entering trigger box
        if (other.gameObject.GetComponent<Agent>() != null)
        {
            Agent agentData = other.gameObject.GetComponent<Agent>();

            //Ignore if not the targetted queue
            if (agentData.targetPos != transform.position)
                return;

            agentData.currAgentState = AgentStates.AS_QUEUEING;
            queueManagerObj.GetComponent<QueueManager>().MoveAgentToQueuePosition(other.gameObject);

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Move agent to next state when entering trigger box
        if (collision.gameObject.GetComponent<Agent>() != null)
        {
            Agent agentData = collision.gameObject.GetComponent<Agent>();

            //Ignore if not the targetted queue
            if (agentData.targetPos != transform.position)
                return;

            agentData.currAgentState = AgentStates.AS_QUEUEING;
            queueManagerObj.GetComponent<QueueManager>().MoveAgentToQueuePosition(collision.gameObject);

        }
    }
}
