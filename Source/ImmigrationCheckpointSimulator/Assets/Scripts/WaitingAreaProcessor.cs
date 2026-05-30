using UnityEngine;

public class WaitingAreaProcessor : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        Agent agentInfo = collision.gameObject.GetComponent<Agent>();
        if (agentInfo == null)
            return;

        //Check if waiting for groups or idling


        //Else move to next section
        FindAnyObjectByType<WorldManager>().UpdateAgentNextState(collision.gameObject);
    }
}
