using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject SpawningZoneObj;
    public GameObject AgentPrefab;
    public GameObject parentObj;

    public float spawnRate = 4f; // Number of agents spawned per second
    private float defaultSpawnTimer = 0;

    [HideInInspector]
    public List<AgentData> spawnQueue;
    [HideInInspector]
    public List<(float, GameObject)> waitingToSpawnList = new List<(float, GameObject)>();

    private GameObjectManager goManager;

    private void Start()
    {
        goManager = GetComponent<GameObjectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Spawning logic
        for(int i = 0; i < waitingToSpawnList.Count;)
        {
            (float, GameObject) val = waitingToSpawnList[i];
            val.Item1 -= Time.deltaTime;
            waitingToSpawnList[i] = val;

            //Spawn item and remove it from list
            if (waitingToSpawnList[i].Item1 < 0)
            {
                GameObject go = waitingToSpawnList[i].Item2;
                go.SetActive(true);

                Vector2 randomPos = GetRandomSpawnPos();
                go.transform.position = new Vector3(randomPos.x, randomPos.y, go.transform.position.z);
                go.GetComponent<Agent>().spawnTime = FindAnyObjectByType<WorldManager>().simulationTime;

                //Ask world manager to update agent state
                FindAnyObjectByType<WorldManager>().UpdateAgentNextState(go);

                waitingToSpawnList.RemoveAt(i);
                continue;
            }

            //Increase only if no spawns
            ++i;
        }

        //Default spawn rate
        if(spawnRate > 0)
        {
            defaultSpawnTimer += Time.deltaTime;
            if (defaultSpawnTimer > 1f / spawnRate)
            {
                AgentData newAgentData = new AgentData();

                newAgentData = RandomAgentData(newAgentData);
                SpawnAgent(newAgentData);

                defaultSpawnTimer = 0;
            }
        }
    }

    public void SpawnAgent(AgentData agentInfo)
    {
        //Spawn new object
        GameObject newObj = goManager.GetNewGO();
        newObj.GetComponent<Agent>().Init(agentInfo);
        newObj.transform.SetParent(parentObj.transform);

        waitingToSpawnList.Add((agentInfo.spawnDelay, newObj));
    }

    public void AddAgentToSpawnQueue(AgentData agentInfo)
    {
        spawnQueue.Add(agentInfo);
    }

    public void CreateAgents()
    {
        for (int i = 0; i < spawnQueue.Count; ++i)
            SpawnAgent(spawnQueue[i]);

        spawnQueue.Clear();
    }

    Vector2 GetRandomSpawnPos()
    {
        //Set random position in the zone
        Vector2 spawnZonePos = SpawningZoneObj.GetComponent<Transform>().position;
        Vector2 spawnZoneScale = SpawningZoneObj.GetComponent<Transform>().lossyScale;
        Vector2 agentPrefabScale = AgentPrefab.GetComponent<Transform>().lossyScale;

        float randX = Random.Range(spawnZonePos.x - spawnZoneScale.x * 0.5f + agentPrefabScale.x * 0.5f, spawnZonePos.x + spawnZoneScale.x * 0.5f - agentPrefabScale.x * 0.5f);
        float randY = Random.Range(spawnZonePos.y - spawnZoneScale.y * 0.5f + agentPrefabScale.y * 0.5f, spawnZonePos.y + spawnZoneScale.y * 0.5f - agentPrefabScale.y * 0.5f);

        return new Vector2(randX, randY);
    }

    public void DespawnAgent(GameObject agentObj)
    {
        //Data collection
        CollectData();

        //Move agent to inactive zone
        agentObj.SetActive(false);
        agentObj.transform.SetParent(goManager.inactiveParentObj.transform);
    }

    AgentData RandomAgentData(AgentData agentInfo)
    {
        GlobalValues globalVars = FindAnyObjectByType<GlobalValues>();
        if (globalVars.currScenarioMode == "Default")
            agentInfo.RandomInitValuesDefault();
        else if (globalVars.currScenarioMode == "Peak Holiday")
                agentInfo.RandomInitValuesHoliday();
        else if (globalVars.currScenarioMode == "High Risk")
            agentInfo.RandomInitValuesHighRisk();

        return agentInfo;
    }

    public void CollectData()
    {

    }
}
