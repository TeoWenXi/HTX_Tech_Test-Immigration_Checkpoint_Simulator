using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using NavMeshPlus.Components;
using NUnit.Framework.Internal;

public class WorldManager : MonoBehaviour
{
    //Checkpoint segments
    public enum WorldSegments
    {
        //Entry area
        WS_ENTRANCE,
        WS_PRE_GATE_WAITING_ZONE,
        WS_GATE,

        //Security Area
        WS_SECURITY_QUEUE,
        WS_SECURITY_CHECKPOINT,
        WS_SECURITY_CHECKPOINT_SECONDARY_CHECK,
        WS_SECURITY_CHECKPOINT_EXIT,

        //Immigration Area
        WS_PRE_IMMIGRATION_WAITING_ZONE,
        WS_IMMIGRATION_QUEUE,
        WS_IMMIGRATION_CHECKPOINT,
        WS_IMMIGRATION_CHECKPOINT_SECONDARY_CHECK,

        //Finish
        WS_EXIT,
        WS_COMPLETE,

        //Extra
        WS_COUNT
    }

    [Header("Map Layout Objects")]
    public GameObject spawnerObj;
    public GameObject preGateWaitingAreaObj;
    public GameObject entranceGateAreaObj;

    //Security Area
    public GameObject securityQueueAreaObj;
    public GameObject securityZoneObj;
    public GameObject securitySecondaryCheckParentObj;
    public GameObject securityZoneEndWalls;

    //Immigration Area
    public GameObject preImmigrationWaitingAreaObj;
    public GameObject immigrationQueueAreaObj;
    public GameObject immigrationAreaObj;
    public GameObject immigrationSecondaryCheckParentObj;
    public GameObject immigrationZoneEndWalls;

    //Complete
    public GameObject exitAreaObj;

    //Navmesh
    public GameObject navMeshObj;
    
    //Queue Manager Lists
    [HideInInspector] public List<QueueManager> securityQueueManagers = new List<QueueManager>();
    [HideInInspector] public List<List<GameObject>> securitySecondaryCheckQueueList = new List<List<GameObject>>();

    [HideInInspector] public List<QueueManager> immigrationQueueManagers = new List<QueueManager>();
    [HideInInspector] public List<List<GameObject>> immigrationSecondaryCheckQueueList = new List<List<GameObject>>();

    //Data Collection
    public float simulationTime = 0f;
    DataCollector dataLogger = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dataLogger = GetComponent<DataCollector>();

        //Reset seed
        GlobalValues globalVars = GetComponent<GlobalValues>();
        if (globalVars.seed == "")
            globalVars.seed = UnityEngine.Random.value.ToString();
        UnityEngine.Random.InitState(globalVars.seed.GetHashCode());
        globalVars.newSeed = globalVars.seed;

        //Generate Map
        GenerateMap();

        //Update navmesh
        navMeshObj.GetComponent<NavMeshSurface>().UpdateNavMesh(navMeshObj.GetComponent<NavMeshSurface>().navMeshData);

        CalculateGateArea(entranceGateAreaObj);
        CalculateGateArea(securityZoneEndWalls);
    }

    // Update is called once per frame
    void Update()
    {
        simulationTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        //Update Queue Managers
        UpdateOrderedQueue(securityQueueManagers, securityZoneObj.GetComponent<StationManager>(), WorldSegments.WS_SECURITY_QUEUE);
        UpdateOrderedQueue(immigrationQueueManagers, immigrationAreaObj.GetComponent<StationManager>(), WorldSegments.WS_IMMIGRATION_QUEUE);

        //Update secondary check waiting zones
        for (int i = 0; i < securitySecondaryCheckQueueList.Count; ++i)
        {
            UpdateSimpleQueue(securitySecondaryCheckQueueList[i],
                                 securitySecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>(), WorldSegments.WS_SECURITY_CHECKPOINT_SECONDARY_CHECK);
        }
        for (int i = 0; i < immigrationSecondaryCheckQueueList.Count; ++i)
        {
            UpdateSimpleQueue(immigrationSecondaryCheckQueueList[i],
                                 immigrationSecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>(), WorldSegments.WS_IMMIGRATION_CHECKPOINT_SECONDARY_CHECK);
        }
    }

    void GenerateMap()
    {
        GlobalValues globalVars = GetComponent<GlobalValues>();

        //===================
        //SECURITY CHECKPOINT
        //===================

        //Find and add Queue Managers
        GameObject queueParObj;
        switch (globalVars.SecurityQueueLayout)
        {
            case QueueLayout.QL_ZIGZAG:
                queueParObj = securityQueueAreaObj.transform.Find("Queue Managers").Find("ZigZag Layout").gameObject;
                break;
            case QueueLayout.QL_SINGLE_LINES:
                queueParObj = securityQueueAreaObj.transform.Find("Queue Managers").Find("Single Lane Layout").gameObject;
                break;
            default:
                queueParObj = securityQueueAreaObj.transform.Find("Queue Managers").Find("ZigZag Layout").gameObject;
                break;
        }

        for (int i = 0; i < queueParObj.transform.childCount; ++i)
        {
            if (queueParObj.transform.GetChild(i).gameObject.activeInHierarchy == false)
                continue;

            QueueManager newQueueManager = queueParObj.transform.GetChild(i).GetComponent<QueueManager>();
            newQueueManager.CreateQueue();
            securityQueueManagers.Add(newQueueManager);
        }

        //Create security stations
        securityZoneObj.GetComponent<StationManager>().CreateStations(globalVars.SecurityStations.Rows, 
                                                                      globalVars.SecurityStations.StationsPerRow, 
                                                                      globalVars.SecurityStations.totalDisabledStations);

        //Create security secondary check areas
        for (int i = 0; i < securitySecondaryCheckParentObj.transform.childCount; ++i)
        {
            securitySecondaryCheckQueueList.Add(new List<GameObject>());
            securitySecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>().CreateStations(globalVars.SecuritySecondaryStations.Rows, 
                                                                                                                globalVars.SecuritySecondaryStations.StationsPerRow, 
                                                                                                                globalVars.SecuritySecondaryStations.totalDisabledStations);
        }

        //======================
        //IMMIGRATION CHECKPOINT
        //======================
        switch (globalVars.ImmigrationQueueLayout)
        {
            case QueueLayout.QL_ZIGZAG:
                queueParObj = immigrationQueueAreaObj.transform.Find("Queue Managers").Find("ZigZag Layout").gameObject;
                break;
            case QueueLayout.QL_SINGLE_LINES:
                queueParObj = immigrationQueueAreaObj.transform.Find("Queue Managers").Find("Single Lane Layout").gameObject;
                break;
            default:
                queueParObj = immigrationQueueAreaObj.transform.Find("Queue Managers").Find("ZigZag Layout").gameObject;
                break;
        }

        for (int i = 0; i < queueParObj.transform.childCount; ++i)
        {
            if (queueParObj.transform.GetChild(i).gameObject.activeInHierarchy == false)
                continue;

            QueueManager newQueueManager = queueParObj.transform.GetChild(i).GetComponent<QueueManager>();
            newQueueManager.CreateQueue();
            immigrationQueueManagers.Add(newQueueManager);
        }

        //Create security stations
        immigrationAreaObj.GetComponent<StationManager>().CreateStations(globalVars.ImmigrationStations.Rows, 
                                                                         globalVars.ImmigrationStations.StationsPerRow, 
                                                                         globalVars.ImmigrationStations.totalDisabledStations);

        //Create security secondary check areas
        for (int i = 0; i < immigrationSecondaryCheckParentObj.transform.childCount; ++i)
        {
            immigrationSecondaryCheckQueueList.Add(new List<GameObject>());
            immigrationSecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>().CreateStations(globalVars.ImmigrationSecondaryStations.Rows, 
                                                                                                                   globalVars.ImmigrationSecondaryStations.StationsPerRow, 
                                                                                                                   globalVars.ImmigrationSecondaryStations.totalDisabledStations);
        }
    }

    void CalculateGateArea(GameObject dividerObj)
    {
        GameObject centerGateAreaObj = dividerObj.transform.Find("Center Area Collider").gameObject;
        //centerGateAreaObj.transform.position = GateAreaObj.transform.position;
        Transform topGate = dividerObj.transform.Find("Top Gate");
        Transform botGate = dividerObj.transform.Find("Bottom Gate");

        centerGateAreaObj.transform.localScale = new Vector3(centerGateAreaObj.transform.localScale.x, 
                                                             (topGate.position.y - topGate.localScale.y * 0.5f) - (botGate.position.y + botGate.localScale.y * 0.5f) - 0.5f, 
                                                             1f);
    }

    public Vector2 GetClosestGateAreaPosition(GameObject dividerObj, Vector2 _inputPos)
    {
        GameObject centerGateAreaObj = dividerObj.transform.Find("Center Area Collider").gameObject;
        return centerGateAreaObj.GetComponent<BoxCollider2D>().ClosestPoint(_inputPos);
    }

    public void UpdateAgentNextState(GameObject agentObj, bool secondaryCheck = false)
    {
        Agent agentInfo = agentObj.GetComponent<Agent>();
        switch (agentInfo.currAgentWorldSegment)
        {
            case WorldSegments.WS_ENTRANCE:
                //Move agent to gate area if not waiting
                if (agentInfo.lastClearedWorldSegment == WorldSegments.WS_COUNT)
                {
                    //agentObj.GetComponent<Agent>().SetTarget(GetClosestGateAreaPosition(entranceGateAreaObj, agentObj.transform.position));
                    agentObj.GetComponent<Agent>().SetTarget(GetRandomPositionInArea(entranceGateAreaObj.transform.Find("Center Area Collider").gameObject));
                    agentObj.GetComponent<Agent>().lastClearedWorldSegment = WorldSegments.WS_ENTRANCE;
                }
                break;
            case WorldSegments.WS_PRE_GATE_WAITING_ZONE:
                if (agentInfo.lastClearedWorldSegment == WorldSegments.WS_COUNT)
                {
                    //agentObj.GetComponent<Agent>().SetTarget(GetClosestGateAreaPosition(entranceGateAreaObj, agentObj.transform.position));
                    agentObj.GetComponent<Agent>().SetTarget(GetRandomPositionInArea(entranceGateAreaObj.transform.Find("Center Area Collider").gameObject));
                    agentObj.GetComponent<Agent>().lastClearedWorldSegment = WorldSegments.WS_PRE_GATE_WAITING_ZONE;
                }
                break;
            case WorldSegments.WS_GATE:
                {
                    if (agentInfo.lastClearedWorldSegment != WorldSegments.WS_PRE_GATE_WAITING_ZONE)
                        return;

                    //Get smallest index
                    int index = GetSmallestOrderedQueueIndex(securityQueueManagers);
                    if (index == -1)
                        return;

                    //Move agent to smallest queue
                    securityQueueManagers[index].MoveToStartOfQueue(agentObj);
                    agentInfo.lastClearedWorldSegment = WorldSegments.WS_GATE;
                    agentObj.GetComponent<Agent>().securityCheckpointStartTime = simulationTime;
                    agentObj.GetComponent<Agent>().queueStartTime = simulationTime;
                }
                break;

            //============================
            //      SECURIITY AREA
            //============================
            case WorldSegments.WS_SECURITY_QUEUE:
                //Processed in Update function
                break;

            case WorldSegments.WS_SECURITY_CHECKPOINT:
            case WorldSegments.WS_SECURITY_CHECKPOINT_SECONDARY_CHECK:
                //Move agent to secondary check
                if (secondaryCheck)
                    agentObj.GetComponent<Agent>().SetTarget(GetClosestSecondaryCheckArea(agentObj.GetComponent<Agent>().currAgentWorldSegment, agentObj.transform.position));
                //Move agent to next area
                else
                    agentObj.GetComponent<Agent>().SetTarget(GetRandomPositionInArea(securityZoneEndWalls.transform.Find("Center Area Collider").gameObject));

                agentObj.GetComponent<Agent>().currAgentState = AgentStates.AS_MOVING;
                break;

            case WorldSegments.WS_SECURITY_CHECKPOINT_EXIT:
                if (agentInfo.lastClearedWorldSegment >= WorldSegments.WS_SECURITY_CHECKPOINT_EXIT)
                    return;
                agentInfo.lastClearedWorldSegment = WorldSegments.WS_SECURITY_CHECKPOINT_EXIT;

                agentObj.GetComponent<Agent>().SetTarget(GetRandomPositionInArea(preImmigrationWaitingAreaObj.transform.Find("Area Sprite").GetChild(0).gameObject));
                dataLogger.clearedSecurity.Add(simulationTime - agentObj.GetComponent<Agent>().securityCheckpointStartTime);
                agentObj.GetComponent<Agent>().immigrationCheckpointStartTime = simulationTime;
                break;

            //============================
            //   END OF SECURIITY AREA
            //============================

            //============================
            //      IMMIGRATION AREA
            //============================
            case WorldSegments.WS_PRE_IMMIGRATION_WAITING_ZONE:
                {
                    if (agentInfo.lastClearedWorldSegment != WorldSegments.WS_SECURITY_CHECKPOINT_EXIT)
                        return;

                    //Get smallest index
                    int index = GetSmallestOrderedQueueIndex(immigrationQueueManagers);
                    if (index == -1)
                        return;

                    //Move agent to smallest queue
                    immigrationQueueManagers[index].MoveToStartOfQueue(agentObj);
                    agentInfo.lastClearedWorldSegment = WorldSegments.WS_PRE_IMMIGRATION_WAITING_ZONE;
                    agentObj.GetComponent<Agent>().queueStartTime = simulationTime;
                }
                break;
            case WorldSegments.WS_IMMIGRATION_QUEUE:
                //Processed in Update function
                break;
            case WorldSegments.WS_IMMIGRATION_CHECKPOINT:
            case WorldSegments.WS_IMMIGRATION_CHECKPOINT_SECONDARY_CHECK:
                //Move agent to secondary check
                if (secondaryCheck)
                    agentObj.GetComponent<Agent>().SetTarget(GetClosestSecondaryCheckArea(agentObj.GetComponent<Agent>().currAgentWorldSegment, agentObj.transform.position));
                //Move agent to next area
                else
                    agentObj.GetComponent<Agent>().SetTarget(GetRandomPositionInArea(immigrationZoneEndWalls.transform.Find("Center Area Collider").gameObject));

                agentObj.GetComponent<Agent>().currAgentState = AgentStates.AS_MOVING;
                break;
            case WorldSegments.WS_EXIT:
                if (agentInfo.lastClearedWorldSegment >= WorldSegments.WS_EXIT)
                    return;

                agentInfo.lastClearedWorldSegment = WorldSegments.WS_EXIT;
                agentObj.GetComponent<Agent>().SetTarget(GetRandomPositionInArea(exitAreaObj.transform.Find("Area Sprite").GetChild(0).gameObject));
                dataLogger.clearedImmigration.Add(simulationTime - agentObj.GetComponent<Agent>().immigrationCheckpointStartTime);
                break;

            //============================
            //  END OF IMMIGRATION AREA
            //============================

            case WorldSegments.WS_COMPLETE:
                spawnerObj.GetComponent<Spawner>().DespawnAgent(agentObj);
                dataLogger.clearedSimulation.Add(simulationTime - agentObj.GetComponent<Agent>().spawnTime);
                break;
        }
    }

    void UpdateOrderedQueue(List<QueueManager> inputQueueManager, StationManager stationManagerRef, WorldSegments worldSegment)
    {
        //Check if there is any person in the queue
        bool hasPersonInQueue = false;
        for (int i = 0; i < inputQueueManager.Count; ++i)
        {
            if (inputQueueManager[i].queueList[0].GetComponent<QueueSlot>().isOccupied)
            {
                hasPersonInQueue = true;
                break;
            }
        }

        if (!hasPersonInQueue)
            return;

        //Gather front of queue agents that have arrived
        List<(QueueSlot, GameObject, int QueueIndex)> arrivedAgents = new List<(QueueSlot, GameObject, int)>();
        for (int i = 0; i < inputQueueManager.Count; ++i)
        {
            QueueSlot currQueueSlot = inputQueueManager[i].queueList[0].GetComponent<QueueSlot>();
            if (inputQueueManager[i].queueList[0].GetComponent<QueueSlot>().isOccupied)
                arrivedAgents.Add((inputQueueManager[i].queueList[0].GetComponent<QueueSlot>(), currQueueSlot.agentObject, i));
        }

        //Sort by waiting time
        arrivedAgents.Sort((a, b) => b.Item1.timer.CompareTo(a.Item1.timer));

        //Get empty stations and move agents to stations
        for (int i = 0; i < arrivedAgents.Count; ++i)
        {
            GameObject emptyStation = stationManagerRef.GetEmptyStation();

            //If no empty station, break
            if (emptyStation == null)
                break;

            //Move agent to station
            emptyStation.GetComponent<Station>().currAgentObj = arrivedAgents[i].Item2;
            emptyStation.GetComponent<Station>().MoveAgentToStationPosition(arrivedAgents[i].Item2);
            arrivedAgents[i].Item2.GetComponent<Agent>().currAgentState = AgentStates.AS_MOVING;

            //Data logger
            if (worldSegment == WorldSegments.WS_SECURITY_QUEUE)
                dataLogger.SCP_QueueTimes.Add(simulationTime - arrivedAgents[i].Item2.GetComponent<Agent>().queueStartTime);
            else if (worldSegment == WorldSegments.WS_IMMIGRATION_QUEUE)
                dataLogger.ICP_QueueTimes.Add(simulationTime - arrivedAgents[i].Item2.GetComponent<Agent>().queueStartTime);

            //Reset queue slot
            arrivedAgents[i].Item1.ResetVars();
            inputQueueManager[arrivedAgents[i].Item3].UpdateQueue();
        }
    }

    void UpdateSimpleQueue(List<GameObject> simpleQueueList, StationManager stationManagerRef, WorldSegments worldSegment)
    {
        //Get empty stations and move agents to stations
        while(simpleQueueList.Count != 0)
        {
            GameObject emptyStation = stationManagerRef.GetEmptyStation();

            //If no empty station, return
            if (emptyStation == null)
                return;

            //Move agent to station
            emptyStation.GetComponent<Station>().currAgentObj = simpleQueueList[0];
            emptyStation.GetComponent<Station>().MoveAgentToStationPosition(simpleQueueList[0]);
            simpleQueueList[0].GetComponent<Agent>().currAgentState = AgentStates.AS_MOVING;

            //Data logger
            if (worldSegment == WorldSegments.WS_SECURITY_CHECKPOINT_SECONDARY_CHECK)
                dataLogger.SCPS_QueueTimes.Add(simulationTime - simpleQueueList[0].GetComponent<Agent>().queueStartTime);
            else if (worldSegment == WorldSegments.WS_IMMIGRATION_CHECKPOINT_SECONDARY_CHECK)
                dataLogger.ICPS_QueueTimes.Add(simulationTime - simpleQueueList[0].GetComponent<Agent>().queueStartTime);

            //Clear queue slot
            simpleQueueList.RemoveAt(0);
        }
    }

    public void AddToSimpleQueue(GameObject newObj, WorldSegments segment, int index)
    {
        switch(segment)
        {
            case WorldSegments.WS_SECURITY_CHECKPOINT_SECONDARY_CHECK:
                securitySecondaryCheckQueueList[index].Add(newObj);
                break;
            case WorldSegments.WS_IMMIGRATION_CHECKPOINT_SECONDARY_CHECK:
                immigrationSecondaryCheckQueueList[index].Add(newObj);
                break;
            default:
                break;
        }
    }

    Vector2 GetClosestSecondaryCheckArea(WorldSegments segment, Vector3 inputPos)
    {
        GameObject parentObj;
        switch(segment)
        {
            case WorldSegments.WS_SECURITY_CHECKPOINT:
                parentObj = securitySecondaryCheckParentObj;
                break;
            case WorldSegments.WS_IMMIGRATION_CHECKPOINT:
                parentObj = immigrationSecondaryCheckParentObj;
                break;
            default:
                parentObj = securitySecondaryCheckParentObj;
                break;
        }

        List<(float, GameObject)> secondaryCheckAreas = new List<(float, GameObject)>();
        for (int i = 0; i < parentObj.transform.childCount; ++i)
        {
            GameObject waitingZone = parentObj.transform.GetChild(i).Find("Waiting Zone").gameObject;
            float dist = Vector2.Distance(inputPos, waitingZone.GetComponent<BoxCollider2D>().ClosestPoint(inputPos));
            secondaryCheckAreas.Add((dist, waitingZone));
        }

        //Sort by distance
        secondaryCheckAreas.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        return new Vector2(inputPos.x, secondaryCheckAreas[0].Item2.transform.position.y);
    }

    Vector2 GetRandomPositionInArea(GameObject areaObj)
    {
        Vector2 areaPos = areaObj.transform.position;
        Vector2 areaScale = areaObj.transform.lossyScale;
        float randX = UnityEngine.Random.Range(areaPos.x - areaScale.x * 0.5f, areaPos.x + areaScale.x * 0.5f);
        float randY = UnityEngine.Random.Range(areaPos.y - areaScale.y * 0.5f, areaPos.y + areaScale.y * 0.5f);
        return new Vector2(randX, randY);
    }

    int GetSmallestOrderedQueueIndex(List<QueueManager> inputQueueManager)
    {
        //Get smallest queue index
        int smallestQueueIndex = -1;
        int smallestQueueCount = 100000;

        for (int i = 0; i < inputQueueManager.Count; ++i)
        {
            int count = 0;
            for (int j = 0; j < inputQueueManager[i].queueList.Count; ++j)
            {
                if (inputQueueManager[i].queueList[j].GetComponent<QueueSlot>().agentObject != null)
                    ++count;
            }

            if (count < smallestQueueCount)
            {
                smallestQueueIndex = i;
                smallestQueueCount = count;
            }
        }

        //If all queues are full, return
        if (smallestQueueCount >= inputQueueManager[0].queueList.Count)
            return -1;

        return smallestQueueIndex;
    }

    public void ResetWorld()
    {
        //Reset seed
        GlobalValues globalVars = GetComponent<GlobalValues>();
        UnityEngine.Random.InitState(globalVars.seed.GetHashCode());
        
        Spawner spawnerRef = spawnerObj.GetComponent<Spawner>();

        //Clear spawn lists
        spawnerRef.waitingToSpawnList.Clear();

        //Despawn all agents
        while (spawnerRef.parentObj.transform.childCount > 0)
            spawnerRef.DespawnAgent(spawnerRef.parentObj.transform.GetChild(0).gameObject);


        //Clear all stations and queues
        for (int i = 0; i < securityQueueManagers.Count; ++i)
            for (int j = 0; j < securityQueueManagers[i].queueList.Count; ++j)
                securityQueueManagers[i].queueList[j].GetComponent<QueueSlot>().ResetVars();

        for (int i = 0; i < securityZoneObj.GetComponent<StationManager>().stationList.Count; ++i)
            securityZoneObj.GetComponent<StationManager>().stationList[i].GetComponent<Station>().ClearStation();

        for (int i = 0; i < immigrationQueueManagers.Count; ++i)
            for (int j = 0; j < immigrationQueueManagers[i].queueList.Count; ++j)
                immigrationQueueManagers[i].queueList[j].GetComponent<QueueSlot>().ResetVars();

        for (int i = 0; i < immigrationAreaObj.GetComponent<StationManager>().stationList.Count; ++i)
            immigrationAreaObj.GetComponent<StationManager>().stationList[i].GetComponent<Station>().ClearStation();

        for (int i = 0; i < securitySecondaryCheckQueueList.Count; ++i)
        {
            securitySecondaryCheckQueueList[i].Clear();

            //Clear stations
            for (int j = 0; j < securitySecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>().stationList.Count; ++j)
                securitySecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>().stationList[j].GetComponent<Station>().ClearStation();
        }
        for (int i = 0; i < immigrationSecondaryCheckQueueList.Count; ++i)
        {
            immigrationSecondaryCheckQueueList[i].Clear();

            //Clear stations
            for (int j = 0; j < immigrationSecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>().stationList.Count; ++j)
                immigrationSecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>().stationList[j].GetComponent<Station>().ClearStation();
        }

        simulationTime = 0;
        dataLogger.ResetData();
    }
}