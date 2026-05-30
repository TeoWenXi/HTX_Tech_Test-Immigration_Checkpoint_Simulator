using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class DataCollector : MonoBehaviour
{
    #region UI_GAMEOBJECTS
    /////////// UI ELEMENTS DIRECT REFERENCE (RIP I HAVE NO TIME SO I HAVE TO DO THIS)
    [Header("System Overview")]
    public GameObject SO_SimTime;
    public GameObject SO_ActiveTravellers;
    public GameObject SO_TravellersProcessed;
    public GameObject SO_AvgTimeInSys;

    [Header("Flow Throughput Rate")]
    public GameObject FTR_System;
    public GameObject FTR_SecurityCP;
    public GameObject FTR_ImmigrationCP;

    [Header("Queue Performance")]
    public GameObject QPS_Security_PeopleInQueue;
    public GameObject QPS_Security_AvgQueueLength;
    public GameObject QPS_Security_PeakQueueLength;
    public GameObject QPS_Security_AvgWaitTime;
    public GameObject QPS_Immigration_PeopleInQueue;
    public GameObject QPS_Immigration_AvgQueueLength;
    public GameObject QPS_Immigration_PeakQueueLength;
    public GameObject QPS_Immigration_AvgWaitTime;

    [Header("Secondary Check System")]
    public GameObject SC_Security_PeopleInQueue;
    public GameObject SC_Security_AvgWaitTime;
    public GameObject SC_Security_SecondaryCheckRate;
    public GameObject SC_Immigration_PeopleInQueue;
    public GameObject SC_Immigration_AvgWaitTime;
    public GameObject SC_Immigration_SecondaryCheckRate;

    [Header("Resource Utilisation")]
    public GameObject RU_Security_AvgUtil;
    public GameObject RU_Security_IdleCounters;
    public GameObject RU_Security_ProcessingTime;
    public GameObject RU_SecuritySecondary_AvgUtil;
    public GameObject RU_SecuritySecondary_IdleCounters;
    public GameObject RU_SecuritySecondary_ProcessingTime;
    public GameObject RU_Immigration_AvgUtil;
    public GameObject RU_Immigration_IdleCounters;
    public GameObject RU_Immigration_ProcessingTime;
    public GameObject RU_ImmigrationSecondary_AvgUtil;
    public GameObject RU_ImmigrationSecondary_IdleCounters;
    public GameObject RU_ImmigrationSecondary_ProcessingTime;

    [Header("Bottleneck Area")]
    public float timeThresholdForBottleneck = 30f;
    public GameObject BA_BottleneckAreaName;
    public GameObject BA_Security_AvgPplInQueue;
    public GameObject BA_Security_AvgWaitTime;
    public GameObject BA_Security_UtilisationRate;
    public GameObject BA_SecuritySecondary_AvgPplInQueue;
    public GameObject BA_SecuritySecondary_AvgWaitTime;
    public GameObject BA_SecuritySecondary_UtilisationRate;
    public GameObject BA_Immigration_AvgPplInQueue;
    public GameObject BA_Immigration_AvgWaitTime;
    public GameObject BA_Immigration_UtilisationRate;
    public GameObject BA_ImmigrationSecondary_AvgPplInQueue;
    public GameObject BA_ImmigrationSecondary_AvgWaitTime;
    public GameObject BA_ImmigrationSecondary_UtilisationRate;
    #endregion

    Spawner spawnerRef = null;
    WorldManager worldManagerRef = null;
    GlobalValues globalVars = null;

    ///////////Data lists
    //Security
    int SCP_peakQueueLength = 0;
    [HideInInspector] public List<float> SCP_QueueTimes         = new List<float>();
    [HideInInspector] public List<float> SCP_ProcessingTimes    = new List<float>();
    [HideInInspector] public List<float> SCPS_QueueTimes        = new List<float>();
    [HideInInspector] public List<float> SCPS_ProcessingTimes   = new List<float>();

    //Immigration
    int ICP_peakQueueLength = 0;
    [HideInInspector] public List<float> ICP_QueueTimes         = new List<float>();
    [HideInInspector] public List<float> ICP_ProcessingTimes    = new List<float>();
    [HideInInspector] public List<float> ICPS_QueueTimes        = new List<float>();
    [HideInInspector] public List<float> ICPS_ProcessingTimes   = new List<float>();

    //Completion
    [HideInInspector] public List<float> clearedSecurity        = new List<float>();
    [HideInInspector] public List<float> clearedImmigration     = new List<float>();
    [HideInInspector] public List<float> clearedSimulation      = new List<float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        worldManagerRef = GetComponent<WorldManager>();
        globalVars = GetComponent<GlobalValues>();
        spawnerRef = worldManagerRef.spawnerObj.GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        /////////////System Overview
        SO_SimTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(worldManagerRef.simulationTime);
        SO_ActiveTravellers.GetComponent<TMP_Text>().text = spawnerRef.parentObj.transform.childCount.ToString();
        SO_TravellersProcessed.GetComponent<TMP_Text>().text = clearedSimulation.Count.ToString();
        SO_AvgTimeInSys.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(clearedSimulation));

        /////////////Flow Throughput Rate
        FTR_System.GetComponent<TMP_Text>().text = (clearedSimulation.Count / (worldManagerRef.simulationTime / 60f)).ToString("F2") + " / min";
        FTR_SecurityCP.GetComponent<TMP_Text>().text = (clearedSecurity.Count / (worldManagerRef.simulationTime / 60f)).ToString("F2") + " / min";
        FTR_ImmigrationCP.GetComponent<TMP_Text>().text = (clearedImmigration.Count / (worldManagerRef.simulationTime / 60f)).ToString("F2") + " / min";

        /////////////Queue Performance
        //Security Queue
        int queueCounter = CountQueue(worldManagerRef.securityQueueManagers);
        int queueAvg = queueCounter / worldManagerRef.securityQueueManagers.Count;
        if (queueAvg > SCP_peakQueueLength)
            SCP_peakQueueLength = queueAvg;

        QPS_Security_PeopleInQueue.GetComponent<TMP_Text>().text = queueCounter.ToString();
        QPS_Security_AvgQueueLength.GetComponent<TMP_Text>().text = queueAvg.ToString();
        QPS_Security_PeakQueueLength.GetComponent<TMP_Text>().text = SCP_peakQueueLength.ToString();
        QPS_Security_AvgWaitTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(SCP_QueueTimes));

        queueCounter = CountQueue(worldManagerRef.immigrationQueueManagers);
        queueAvg = queueCounter / worldManagerRef.immigrationQueueManagers.Count;
        if (queueAvg > ICP_peakQueueLength)
            ICP_peakQueueLength = queueAvg;

        //Immigration Queue
        QPS_Immigration_PeopleInQueue.GetComponent<TMP_Text>().text = queueCounter.ToString();
        QPS_Immigration_AvgQueueLength.GetComponent<TMP_Text>().text = queueAvg.ToString();
        QPS_Immigration_PeakQueueLength.GetComponent<TMP_Text>().text = ICP_peakQueueLength.ToString();
        QPS_Immigration_AvgWaitTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(ICP_QueueTimes));

        /////////////Secondary Check System
        //Security Secondary Check
        queueCounter = CountQueue(worldManagerRef.securitySecondaryCheckQueueList);
        SC_Security_PeopleInQueue.GetComponent<TMP_Text>().text = queueCounter.ToString();
        SC_Security_AvgWaitTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(SCPS_QueueTimes));
        if (SCP_ProcessingTimes.Count > 0)
            SC_Security_SecondaryCheckRate.GetComponent<TMP_Text>().text = ConvertFloatToPercentage((float)SCPS_ProcessingTimes.Count / SCP_ProcessingTimes.Count);
        else
            SC_Security_SecondaryCheckRate.GetComponent<TMP_Text>().text = "0%";

        //Immigration Secondary Check
        queueCounter = CountQueue(worldManagerRef.immigrationSecondaryCheckQueueList);
        SC_Immigration_PeopleInQueue.GetComponent<TMP_Text>().text = queueCounter.ToString();
        SC_Immigration_AvgWaitTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(ICPS_QueueTimes));
        if (ICP_ProcessingTimes.Count > 0)
            SC_Immigration_SecondaryCheckRate.GetComponent<TMP_Text>().text = ConvertFloatToPercentage((float)ICPS_ProcessingTimes.Count / ICP_ProcessingTimes.Count);
        else
            SC_Immigration_SecondaryCheckRate.GetComponent<TMP_Text>().text = "0%";

        /////////////Resource Utilisation
        //Security Stations
        StationManager stationManagerRef = worldManagerRef.securityZoneObj.GetComponent<StationManager>();
        int activeStationsCount = CountActiveStations(stationManagerRef);
        RU_Security_AvgUtil.GetComponent<TMP_Text>().text = ConvertFloatToPercentage((float)activeStationsCount / stationManagerRef.stationList.Count);
        RU_Security_IdleCounters.GetComponent<TMP_Text>().text = (stationManagerRef.stationList.Count - activeStationsCount).ToString();
        RU_Security_ProcessingTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(SCP_ProcessingTimes));

        //Security Secondary Check Stations
        int totalStations = 0;
        activeStationsCount = 0;
        for (int i = 0; i < worldManagerRef.securitySecondaryCheckQueueList.Count; ++i)
        {
            totalStations += worldManagerRef.securitySecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>().stationList.Count;
            activeStationsCount += CountActiveStations(worldManagerRef.securitySecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>());
        }
        RU_SecuritySecondary_AvgUtil.GetComponent<TMP_Text>().text = ConvertFloatToPercentage((float)activeStationsCount / totalStations);
        RU_SecuritySecondary_IdleCounters.GetComponent<TMP_Text>().text = (totalStations - activeStationsCount).ToString();
        RU_SecuritySecondary_ProcessingTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(SCPS_ProcessingTimes));

        //Immigration Stations
        stationManagerRef = worldManagerRef.immigrationAreaObj.GetComponent<StationManager>();
        activeStationsCount = CountActiveStations(stationManagerRef);
        RU_Immigration_AvgUtil.GetComponent<TMP_Text>().text = ConvertFloatToPercentage((float)activeStationsCount / stationManagerRef.stationList.Count);
        RU_Immigration_IdleCounters.GetComponent<TMP_Text>().text = (stationManagerRef.stationList.Count - activeStationsCount).ToString();
        RU_Immigration_ProcessingTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(ICP_ProcessingTimes));

        //Immigration Secondary Check Stations
        totalStations = 0;
        activeStationsCount = 0;
        for (int i = 0; i < worldManagerRef.immigrationSecondaryCheckQueueList.Count; ++i)
        {
            totalStations += worldManagerRef.immigrationSecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>().stationList.Count;
            activeStationsCount += CountActiveStations(worldManagerRef.immigrationSecondaryCheckParentObj.transform.GetChild(i).GetComponent<StationManager>());
        }
        RU_ImmigrationSecondary_AvgUtil.GetComponent<TMP_Text>().text = ConvertFloatToPercentage((float)activeStationsCount / totalStations);
        RU_ImmigrationSecondary_IdleCounters.GetComponent<TMP_Text>().text = (totalStations - activeStationsCount).ToString();
        RU_ImmigrationSecondary_ProcessingTime.GetComponent<TMP_Text>().text = ConvertFloatToTime(GetAverage(SCPS_ProcessingTimes));

        /////////////Bottleneck Area
        //Security
        BA_Security_AvgPplInQueue.GetComponent<TMP_Text>().text = QPS_Security_AvgQueueLength.GetComponent<TMP_Text>().text;
        BA_Security_AvgWaitTime.GetComponent<TMP_Text>().text = QPS_Security_AvgWaitTime.GetComponent<TMP_Text>().text;
        BA_Security_UtilisationRate.GetComponent<TMP_Text>().text = RU_Security_AvgUtil.GetComponent<TMP_Text>().text;

        //Security Secondary
        BA_SecuritySecondary_AvgPplInQueue.GetComponent<TMP_Text>().text = (int.Parse(SC_Security_PeopleInQueue.GetComponent<TMP_Text>().text) / 2).ToString();
        BA_SecuritySecondary_AvgWaitTime.GetComponent<TMP_Text>().text = SC_Security_AvgWaitTime.GetComponent<TMP_Text>().text;
        BA_SecuritySecondary_UtilisationRate.GetComponent<TMP_Text>().text = RU_SecuritySecondary_AvgUtil.GetComponent<TMP_Text>().text;

        //Immigration
        BA_Immigration_AvgPplInQueue.GetComponent<TMP_Text>().text = QPS_Immigration_AvgQueueLength.GetComponent<TMP_Text>().text;
        BA_Immigration_AvgWaitTime.GetComponent<TMP_Text>().text = QPS_Immigration_AvgWaitTime.GetComponent<TMP_Text>().text;
        BA_Immigration_UtilisationRate.GetComponent<TMP_Text>().text = RU_Immigration_AvgUtil.GetComponent<TMP_Text>().text;

        //Immigration Secondary
        BA_ImmigrationSecondary_AvgPplInQueue.GetComponent<TMP_Text>().text = (int.Parse(SC_Immigration_PeopleInQueue.GetComponent<TMP_Text>().text) / 2).ToString();
        BA_ImmigrationSecondary_AvgWaitTime.GetComponent<TMP_Text>().text = SC_Immigration_AvgWaitTime.GetComponent<TMP_Text>().text;
        BA_ImmigrationSecondary_UtilisationRate.GetComponent<TMP_Text>().text = RU_ImmigrationSecondary_AvgUtil.GetComponent<TMP_Text>().text;

        //Bottleneck area
        List<(float, string)> avgQueueTimes = new List<(float, string)>();
        avgQueueTimes.Add((GetAverage(SCP_QueueTimes), "Security Checkpoint"));
        avgQueueTimes.Add((GetAverage(SCPS_QueueTimes), "Security Secondary Check"));
        avgQueueTimes.Add((GetAverage(ICP_QueueTimes), "Immigration Checkpoint"));
        avgQueueTimes.Add((GetAverage(ICPS_QueueTimes), "Immigration Secondary Check"));

        //Sort to get highest value
        avgQueueTimes.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        //Ignore if less than 5mins
        if (avgQueueTimes[0].Item1 > timeThresholdForBottleneck)
            BA_BottleneckAreaName.GetComponent<TMP_Text>().text = avgQueueTimes[0].Item2;
        else
            BA_BottleneckAreaName.GetComponent<TMP_Text>().text = "None";
    }

    public void ResetData()
    {
        //Clear all data lists
        SCP_QueueTimes.Clear();
        SCP_ProcessingTimes.Clear();
        SCPS_QueueTimes.Clear();
        SCPS_ProcessingTimes.Clear();
        ICP_QueueTimes.Clear();
        ICP_ProcessingTimes.Clear();
        ICPS_QueueTimes.Clear();
        ICPS_ProcessingTimes.Clear();
        clearedSecurity.Clear();
        clearedImmigration.Clear();
        clearedSimulation.Clear();

        SCP_peakQueueLength = 0;
        ICP_peakQueueLength = 0;
    }

    string ConvertFloatToTime(float inputVal)
    {
        string retStr = "";

        float hours, minutes, seconds;

        seconds = inputVal % 60;
        minutes = inputVal / 60f;
        hours = minutes / 60f;

        if ((int)hours > 0)
            retStr += ((int)hours).ToString() + "hrs ";
        if ((int)minutes > 0)
            retStr += ((int)minutes).ToString() + "mins ";
        retStr += ((int)seconds).ToString() + "s";

        return retStr;
    }

    float GetAverage(List<float> dataList)
    {
        if(dataList.Count == 0) return 0;

        float totalTime = 0;
        for(int i = 0; i < dataList.Count; ++i)
            totalTime += dataList[i];

        return totalTime / dataList.Count;
    }

    int CountQueue (List<QueueManager> queueManager)
    {
        int counter = 0;

        for (int i = 0; i < queueManager.Count; ++i)
            for (int j = 0; j < queueManager[i].queueList.Count; ++j)
                if (queueManager[i].queueList[j].GetComponent<QueueSlot>().agentObject != null)
                    ++counter;

        return counter;
    }

    int CountQueue (List<List<GameObject>> queueManager)
    {
        int counter = 0;

        for (int i = 0; i < queueManager.Count; ++i)
            counter += queueManager[i].Count;

        return counter;
    }

    string ConvertFloatToPercentage(float inputVal)
    {
        inputVal *= 100;
        return ((int)inputVal) + "%";
    }

    int CountActiveStations(StationManager stationManager)
    {
        int counter = 0;
        for (int i = 0; i < stationManager.stationList.Count; ++i)
            if (stationManager.stationList[i].GetComponent<Station>().currAgentObj != null)
                ++counter;

        return counter;
    }
}
