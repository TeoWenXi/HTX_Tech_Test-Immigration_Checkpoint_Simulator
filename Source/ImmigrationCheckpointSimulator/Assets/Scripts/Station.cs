using UnityEngine;

public class Station : MonoBehaviour
{
    public bool isActive = true; //For deactivation of stations
    public bool isOccupied = false;
    public GameObject currAgentObj = null;
    public WorldManager.WorldSegments stationSegment;
    float timer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Disable officer if station is inactive
        if (isActive) 
            transform.Find("Officer").gameObject.SetActive(true);
        else
            transform.Find("Officer").gameObject.SetActive(false);

        if (isOccupied)
        {
            timer += Time.deltaTime;
            currAgentObj.GetComponent<Agent>().currAgentState = AgentStates.AS_PROCESSING;

            if (timer >= GetProcessingTime())
            {
                WorldManager worldManagerRef = FindAnyObjectByType<WorldManager>();
                currAgentObj.GetComponent<Agent>().lastClearedWorldSegment = stationSegment;

                //Data logger
                switch(stationSegment)
                {
                    case WorldManager.WorldSegments.WS_SECURITY_CHECKPOINT:
                        worldManagerRef.gameObject.GetComponent<DataCollector>().SCP_ProcessingTimes.Add(timer);
                        break;
                    case WorldManager.WorldSegments.WS_SECURITY_CHECKPOINT_SECONDARY_CHECK:
                        worldManagerRef.gameObject.GetComponent<DataCollector>().SCPS_ProcessingTimes.Add(timer);
                        break;
                    case WorldManager.WorldSegments.WS_IMMIGRATION_CHECKPOINT:
                        worldManagerRef.gameObject.GetComponent<DataCollector>().ICP_ProcessingTimes.Add(timer);
                        break;
                    case WorldManager.WorldSegments.WS_IMMIGRATION_CHECKPOINT_SECONDARY_CHECK:
                        worldManagerRef.gameObject.GetComponent<DataCollector>().ICPS_ProcessingTimes.Add(timer);
                        break;
                }

                //Secondary checks are possible for security and immigration areas
                if (stationSegment == WorldManager.WorldSegments.WS_SECURITY_CHECKPOINT ||
                    stationSegment == WorldManager.WorldSegments.WS_IMMIGRATION_CHECKPOINT)
                {
                    //Chance of additional check
                    if (Random.Range(0f, 1f) < GetSecondaryCheckChance())
                    {
                        //Move agent to secondary check segment
                        worldManagerRef.UpdateAgentNextState(currAgentObj, true);
                        ClearStation();
                        return;
                    }
                }

                //Move agent forward to next segment
                worldManagerRef.UpdateAgentNextState(currAgentObj);
                ClearStation();
            }
        }
    }

    float GetProcessingTime()
    {
        AgentData agentInfo = currAgentObj.GetComponent<Agent>().agentInfo;
        GlobalValues globalVars = FindAnyObjectByType<GlobalValues>();
        float dura = 0;

        switch (stationSegment)
        {
            case WorldManager.WorldSegments.WS_SECURITY_CHECKPOINT:
                dura = globalVars.SCP_BaseTime
                       + globalVars.SCP_TravellerTypeModifier * (int)agentInfo.travellerType
                       + globalVars.SCP_BaggegeModifier * agentInfo.baggageCount
                       + globalVars.SCP_DocumentReadinessModifier * (1 - agentInfo.documentReadiness)
                       + globalVars.SCP_RiskProfileModifier * (int)agentInfo.riskProfile;
                break;
            case WorldManager.WorldSegments.WS_SECURITY_CHECKPOINT_SECONDARY_CHECK:
                //Base time
                dura = globalVars.SS_SecondaryCheckBaseTime;

                //Risk
                if (agentInfo.riskProfile == RiskProfile.RR_MEDIUM)
                    dura += globalVars.SS_RiskProfileMedium;
                else if (agentInfo.riskProfile == RiskProfile.RR_HIGH)
                    dura += globalVars.SS_RiskProfileHigh;

                //Baggage count
                dura += agentInfo.baggageCount * globalVars.SS_BaggageCountMultiplier;

                //Document Readiness
                dura += (1 - agentInfo.documentReadiness) * globalVars.SS_DocumentReadinessMultiplier;

                //Traveller type
                if (agentInfo.travellerType == TravellerType.TT_BUSINESS)
                    dura += globalVars.SS_TravellerTypeBusiness;
                else if (agentInfo.travellerType == TravellerType.TT_VIP)
                    dura += globalVars.SS_TravellerTypeVIP;

                //Random amount
                dura += Random.Range(0, globalVars.SS_RandomVariance);

                break;
            case WorldManager.WorldSegments.WS_IMMIGRATION_CHECKPOINT:
                dura = globalVars.ICP_BaseTime
                       + globalVars.ICP_TravellerTypeModifier * (int)agentInfo.travellerType
                       + globalVars.ICP_DocumentReadinessModifier * (1 - agentInfo.documentReadiness)
                       + globalVars.ICP_RiskProfileModifier * (int)agentInfo.riskProfile
                       + Random.Range(0, globalVars.ICP_RandomVariance);
                break;
            case WorldManager.WorldSegments.WS_IMMIGRATION_CHECKPOINT_SECONDARY_CHECK:
                //Base time
                dura = globalVars.IS_SecondaryCheckBaseTime;

                //Risk
                if (agentInfo.riskProfile == RiskProfile.RR_MEDIUM)
                    dura += globalVars.IS_RiskProfileMedium;
                else if (agentInfo.riskProfile == RiskProfile.RR_HIGH)
                    dura += globalVars.IS_RiskProfileHigh;

                //Document Readiness
                dura += (1 - agentInfo.documentReadiness) * globalVars.IS_DocumentReadinessMultiplier;

                //Traveller type
                if (agentInfo.travellerType == TravellerType.TT_BUSINESS)
                    dura += globalVars.IS_TravellerTypeBusiness;
                else if (agentInfo.travellerType == TravellerType.TT_VIP)
                    dura += globalVars.IS_TravellerTypeVIP;

                //Random amount
                dura += Random.Range(0, globalVars.IS_RandomVariance);
                break;
        }

        return dura;
    }

    float GetSecondaryCheckChance()
    {
        AgentData agentInfo = currAgentObj.GetComponent<Agent>().agentInfo;
        GlobalValues globalVars = FindAnyObjectByType<GlobalValues>();

        float chance = 0;
        if (stationSegment == WorldManager.WorldSegments.WS_SECURITY_CHECKPOINT)
        {
            //Base chance
            chance = globalVars.SCPS_SecondaryCheckBaseChance;

            //Risk profile modifier
            switch (agentInfo.riskProfile)
            {
                case RiskProfile.RR_LOW:
                    break;
                case RiskProfile.RR_MEDIUM:
                    chance += globalVars.SCPS_RiskProfileMedium;
                    break;
                case RiskProfile.RR_HIGH:
                    chance += globalVars.SCPS_RiskProfileHigh;
                    break;
            }

            //Baggage count modifier
            chance += agentInfo.baggageCount * globalVars.SCPS_BaggageCountMultiplier;

            //Document readiness modifier
            chance += (1f - agentInfo.documentReadiness) * globalVars.SCPS_DocumentReadinessMultiplier;

            //Traveller type modifier
            switch (agentInfo.travellerType)
            {
                case TravellerType.TT_LEISURE:
                    break;
                case TravellerType.TT_BUSINESS:
                    chance += globalVars.SCPS_TravellerTypeBusiness;
                    break;
                case TravellerType.TT_VIP:
                    chance += globalVars.SCPS_TravellerTypeVIP;
                    break;
            }

            chance += globalVars.SCPS_AddtionalModifier;
        }
        else if (stationSegment == WorldManager.WorldSegments.WS_IMMIGRATION_CHECKPOINT)
        {
            //Base chance
            chance = globalVars.ICPS_SecondaryCheckBaseChance;

            //Risk profile modifier
            switch (agentInfo.riskProfile)
            {
                case RiskProfile.RR_LOW:
                    break;
                case RiskProfile.RR_MEDIUM:
                    chance += globalVars.ICPS_RiskProfileMedium;
                    break;
                case RiskProfile.RR_HIGH:
                    chance += globalVars.ICPS_RiskProfileHigh;
                    break;
            }

            //Document readiness modifier
            chance += (1f - agentInfo.documentReadiness) * globalVars.ICPS_DocumentReadinessMultiplier;

            //Traveller type modifier
            switch (agentInfo.travellerType)
            {
                case TravellerType.TT_LEISURE:
                    break;
                case TravellerType.TT_BUSINESS:
                    chance += globalVars.ICPS_TravellerTypeBusiness;
                    break;
                case TravellerType.TT_VIP:
                    chance += globalVars.ICPS_TravellerTypeVIP;
                    break;
            }

            chance += globalVars.ICPS_AddtionalModifier;
        }

        //Return final chance, clamped between 0 and 1
        return Mathf.Clamp(chance, 0f, 1f);
    }

    public void MoveAgentToStationPosition(GameObject agentObj)
    {
        agentObj.GetComponent<Agent>().SetTarget(transform.Find("Standing Location").position);
    }

    public void ClearStation()
    {
        isOccupied = false;
        currAgentObj = null;
        timer = 0f;
    }
}
