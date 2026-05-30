using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public enum AgentStates
{
    //Entry area
    AS_NONE = 0,
    AS_MOVING,
    AS_QUEUEING,
    AS_PROCESSING,
    AS_REGROUPING   // For groups to wait for each other after every segment
}

public enum TravellerType
{
    TT_LEISURE = 0,
    TT_BUSINESS,
    TT_VIP
}

public enum RiskProfile
{
    RR_LOW = 0,
    RR_MEDIUM,
    RR_HIGH
}

public class Agent : MonoBehaviour
{
    [Header("Agent Movement Values")]
    private NavMeshAgent agentNavMeshRef;
    public Vector3 targetPos;

    //Processing time may vary by traveller type, baggage, document readiness, or risk profile.
    public AgentData agentInfo;

    //States for the agent workflow
    public WorldManager.WorldSegments currAgentWorldSegment;
    public WorldManager.WorldSegments lastClearedWorldSegment;
    public AgentStates currAgentState = AgentStates.AS_NONE;

    [HideInInspector] public float spawnTime = 0;
    [HideInInspector] public float queueStartTime = 0;
    [HideInInspector] public float securityCheckpointStartTime = 0;
    [HideInInspector] public float immigrationCheckpointStartTime = 0;

    void Start()
    {
    }

    void Update()
    {
        if (!agentNavMeshRef.pathPending && agentNavMeshRef.remainingDistance <= agentNavMeshRef.stoppingDistance)
        {
            //Set agent to moving if not queueing
            if (currAgentState != AgentStates.AS_QUEUEING)
                currAgentState = AgentStates.AS_MOVING;

            agentNavMeshRef.SetDestination(targetPos);
        }

        //Agent has completed the entire area
        if (agentNavMeshRef.remainingDistance - agentNavMeshRef.stoppingDistance < 1f 
            && currAgentWorldSegment == WorldManager.WorldSegments.WS_COMPLETE)
        {
            FindAnyObjectByType<WorldManager>().UpdateAgentNextState(gameObject);
        }
    }
    public void SetTarget(Vector3 _targetPos)
    {
        targetPos = _targetPos;
        agentNavMeshRef.SetDestination(targetPos);

        //Set agent to moving if not queueing
        if (currAgentState != AgentStates.AS_QUEUEING)
            currAgentState = AgentStates.AS_MOVING;
    }

    public void Init(AgentData newAgentData)
    {
        //Update agent data
        agentInfo = newAgentData;

        //Nav Mesh Setup
        agentNavMeshRef = GetComponent<NavMeshAgent>();
        agentNavMeshRef.speed = agentInfo.moveSpeed;
        agentNavMeshRef.updateRotation = false; // Disable automatic rotation
        agentNavMeshRef.updateUpAxis = false;

        //Reset positions
        targetPos = new Vector3(0, 0, 0);
        agentNavMeshRef.avoidancePriority = UnityEngine.Random.Range(0, 100);
        transform.position = targetPos;

        //Reset enums
        currAgentWorldSegment = 0;
        lastClearedWorldSegment = WorldManager.WorldSegments.WS_COUNT;
        currAgentState = 0;
    }
}

[Serializable]
public class AgentData
{
    public float moveSpeed = 8f;
    public float spawnDelay = 0f;

    [Header("Agent Processing Values")]
    public TravellerType travellerType = TravellerType.TT_LEISURE;
    public int baggageCount = 1;
    public float documentReadiness = 0.5f;   // 0 to 1, where 1 is fully ready
    public RiskProfile riskProfile = RiskProfile.RR_LOW;

    public void PureRandomInitValues()
    {
        moveSpeed = UnityEngine.Random.Range(5f, 20f);
        travellerType = (TravellerType)UnityEngine.Random.Range(1, (int)TravellerType.TT_VIP);
        baggageCount = UnityEngine.Random.Range(1, 5);
        documentReadiness = UnityEngine.Random.Range(0f, 1f);
        riskProfile = (RiskProfile)UnityEngine.Random.Range(1, (int)RiskProfile.RR_HIGH);
    }
    public void RandomInitValuesDefault()
    {
        moveSpeed = UnityEngine.Random.Range(5f, 20f);
        baggageCount = UnityEngine.Random.Range(1, 3);
        documentReadiness = UnityEngine.Random.Range(0.7f, 1f);

        float randomChance = UnityEngine.Random.Range(0f, 1f);
        //50% of holiday
        if (randomChance < 0.5f)
            travellerType = TravellerType.TT_LEISURE;
        //40% of business
        else if (randomChance < 0.9f)
            travellerType = TravellerType.TT_BUSINESS;
        //10% of VIP
        else
            travellerType = TravellerType.TT_VIP;

        randomChance = UnityEngine.Random.Range(0f, 1f);
        //70% low
        if (randomChance < 0.7f)
            riskProfile = RiskProfile.RR_LOW;
        //20% medium
        else if (randomChance < 0.9f)
            riskProfile = RiskProfile.RR_MEDIUM;
        //10% high
        else
            riskProfile = RiskProfile.RR_HIGH;
    }

    public void RandomInitValuesHoliday()
    {
        moveSpeed = UnityEngine.Random.Range(5f, 8f);
        baggageCount = UnityEngine.Random.Range(2, 5);
        documentReadiness = UnityEngine.Random.Range(0.5f, 1f);

        float randomChance = UnityEngine.Random.Range(0f, 1f);
        //70% of holiday
        if (randomChance < 0.7f)
            travellerType = TravellerType.TT_LEISURE;
        //20% of business
        else if (randomChance < 0.9f)
            travellerType = TravellerType.TT_BUSINESS;
        //10% of VIP
        else
            travellerType = TravellerType.TT_VIP;

        randomChance = UnityEngine.Random.Range(0f, 1f);
        //70% low
        if (randomChance < 0.7f)
            riskProfile = RiskProfile.RR_LOW;
        //20% medium
        else if (randomChance < 0.9f)
            riskProfile = RiskProfile.RR_MEDIUM;
        //10% high
        else
            riskProfile = RiskProfile.RR_HIGH;
    }

    public void RandomInitValuesHighRisk()
    {
        moveSpeed = UnityEngine.Random.Range(10f, 16f);
        baggageCount = UnityEngine.Random.Range(1, 3);
        documentReadiness = UnityEngine.Random.Range(0f, 1f);

        float randomChance = UnityEngine.Random.Range(0f, 1f);
        //70% of holiday
        if (randomChance < 0.7f)
            travellerType = TravellerType.TT_LEISURE;
        //20% of business
        else if (randomChance < 0.9f)
            travellerType = TravellerType.TT_BUSINESS;
        //10% of VIP
        else
            travellerType = TravellerType.TT_VIP;

        randomChance = UnityEngine.Random.Range(0f, 1f);
        //40% low
        if (randomChance < 0.4f)
            riskProfile = RiskProfile.RR_LOW;
        //35% medium
        else if (randomChance < 0.75f)
            riskProfile = RiskProfile.RR_MEDIUM;
        //25% high
        else
            riskProfile = RiskProfile.RR_HIGH;
    }
}
