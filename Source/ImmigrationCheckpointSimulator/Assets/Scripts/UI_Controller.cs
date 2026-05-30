using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    public GameObject spawnerObjRef;
    public GameObject spawnCounterRef;
    AgentData agentData = new AgentData();
    public int agentEditorCount = 0;
    public int agentRandomCount = 0;

    string targetElement = "";
    GameObject lastInteractedObject;

    float tempSpawnRate = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnCounterRef.GetComponent<TextMeshProUGUI>().text = spawnerObjRef.GetComponent<Spawner>().spawnQueue.Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Toggling Menus
    public void ToggleMenu(GameObject go)
    {
        go.SetActive(!go.activeInHierarchy);
    }
    
    public void CloseItem(GameObject go)
    {
        go.SetActive(false);
    }

    public void OpenItem(GameObject go)
    {
        go.SetActive(true);
    }

    //Spawning
    public void AddAgentsToSpawnQueue()
    {
        for(int i = 0; i <  agentEditorCount; ++i)
            spawnerObjRef.GetComponent<Spawner>().AddAgentToSpawnQueue(agentData);

        spawnCounterRef.GetComponent<TextMeshProUGUI>().text = spawnerObjRef.GetComponent<Spawner>().spawnQueue.Count.ToString();
    }

    public void AddRandomAgentsToSpawnQueue()
    {
        for(int i = 0; i < agentRandomCount; ++i)
        {
            AgentData newAgentInfo = new AgentData();
            newAgentInfo.PureRandomInitValues();

            spawnerObjRef.GetComponent<Spawner>().AddAgentToSpawnQueue(newAgentInfo);
        }

        spawnCounterRef.GetComponent<TextMeshProUGUI>().text = spawnerObjRef.GetComponent<Spawner>().spawnQueue.Count.ToString();
    }

    public void SpawnAgents()
    {
        spawnerObjRef.GetComponent<Spawner>().CreateAgents();
        spawnCounterRef.GetComponent<TextMeshProUGUI>().text = spawnerObjRef.GetComponent<Spawner>().spawnQueue.Count.ToString();
    }

    //Updating Elements
    public void UpdateElementButtonInt(int value)
    {
        if (targetElement == "Traveller Type")
            agentData.travellerType = (TravellerType)value;
        else if (targetElement == "Risk Profile")
            agentData.riskProfile = (RiskProfile)value;
    }
    public void UpdateElementSliderInt()
    {
        Slider sliderObj = lastInteractedObject.GetComponent<Slider>();

        if (targetElement == "Move Speed")
            agentData.moveSpeed = sliderObj.value;
        else if (targetElement == "Baggage Count")
            agentData.baggageCount = (int)sliderObj.value;
        else if (targetElement == "Agent Editor Quantity")
            agentEditorCount = (int)sliderObj.value;
        else if (targetElement == "Agent Randomizer Quantity")
            agentRandomCount = (int)sliderObj.value;

        else if (targetElement == "Security Checkpoint")
            FindAnyObjectByType<WorldManager>().securityZoneObj.GetComponent<StationManager>().UpdateActiveStations((int)sliderObj.value);
        else if (targetElement == "Security Secondary Check Bot")
            FindAnyObjectByType<WorldManager>().securitySecondaryCheckParentObj.transform.GetChild(0).GetComponent<StationManager>().UpdateActiveStations((int)sliderObj.value);
        else if (targetElement == "Security Secondary Check Top")
            FindAnyObjectByType<WorldManager>().securitySecondaryCheckParentObj.transform.GetChild(1).GetComponent<StationManager>().UpdateActiveStations((int)sliderObj.value);

        else if (targetElement == "Immigration Checkpoint")
            FindAnyObjectByType<WorldManager>().immigrationAreaObj.GetComponent<StationManager>().UpdateActiveStations((int)sliderObj.value);
        else if (targetElement == "Immigration Secondary Check Bot")
            FindAnyObjectByType<WorldManager>().immigrationSecondaryCheckParentObj.transform.GetChild(0).GetComponent<StationManager>().UpdateActiveStations((int)sliderObj.value);
        else if (targetElement == "Immigration Secondary Check Top")
            FindAnyObjectByType<WorldManager>().immigrationSecondaryCheckParentObj.transform.GetChild(1).GetComponent<StationManager>().UpdateActiveStations((int)sliderObj.value);
    }
    public void UpdateElementSliderFloat()
    {
        Slider sliderObj = lastInteractedObject.GetComponent<Slider>();

        if (targetElement == "Document Readiness")
            agentData.documentReadiness = sliderObj.value;
        else if (targetElement == "Security Secondary Modifier")
            FindAnyObjectByType<GlobalValues>().SCPS_AddtionalModifier = sliderObj.value;
        else if (targetElement == "Immigration Secondary Modifier")
            FindAnyObjectByType<GlobalValues>().ICPS_AddtionalModifier = sliderObj.value;
    }

    //For referencing
    public void UpdateTargetElement(string targetElementName)
    {
        targetElement = targetElementName;
    }
    public void UpdateLastInteractedObj(GameObject interactedObj)
    {
        lastInteractedObject = interactedObj;
    }

    //For updating displays
    public void UpdateSliderTextFloat(GameObject sliderObj)
    {
        sliderObj.transform.parent.Find("Value").GetComponent<TextMeshProUGUI>().text = (sliderObj.GetComponent<Slider>().value).ToString();
    }
    public void UpdateSliderTextInt(GameObject sliderObj)
    {
        sliderObj.transform.parent.Find("Value").GetComponent<TextMeshProUGUI>().text = sliderObj.GetComponent<Slider>().value.ToString();
    }
    public void UpdateSliderTextPercentage(GameObject sliderObj)
    {
        sliderObj.transform.parent.Find("Value").GetComponent<TextMeshProUGUI>().text = ((int)(sliderObj.GetComponent<Slider>().value * 100)).ToString() + "%";
    }
    public void UpdateButtonOverlayPosition(GameObject overlayObj)
    {
        overlayObj.transform.position = lastInteractedObject.transform.position;
        overlayObj.GetComponent<RectTransform>().sizeDelta = lastInteractedObject.GetComponent<RectTransform>().sizeDelta + new Vector2(6, 6);
    }


    //==== Scenario Menu ======================================
    public void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
    }

    public void SetSpawnRateButton(float spawnRate)
    {
        tempSpawnRate = spawnRate;
    }
    public void SetSpawnRateSlider()
    {
        Slider sliderRef = lastInteractedObject.GetComponent<Slider>();
        spawnerObjRef.GetComponent<Spawner>().spawnRate = sliderRef.value;
    }
    public void LoadScenario()
    {
        spawnerObjRef.GetComponent<Spawner>().spawnRate = tempSpawnRate;

        GlobalValues globalVars = FindAnyObjectByType<GlobalValues>();
        globalVars.currScenarioMode = globalVars.newScenarioMode;
        globalVars.seed = globalVars.newSeed;

        //Hardcoded secondary check values
        if(globalVars.currScenarioMode == "Peak Holiday")
        {
            globalVars.SCPS_AddtionalModifier = 0.1f;
            globalVars.ICPS_AddtionalModifier = 0.1f;
        }
        else if (globalVars.currScenarioMode == "High Risk")
        {
            globalVars.SCPS_AddtionalModifier = 0.25f;
            globalVars.ICPS_AddtionalModifier = 0.25f;
        }

        ResetScenario();
    }
    public void ResetScenario()
    {
        GlobalValues globalVars = FindAnyObjectByType<GlobalValues>();
        globalVars.newSeed = globalVars.seed;

        FindAnyObjectByType<WorldManager>().ResetWorld();
    }
    public void UpdateScenario(string scenarioName)
    {
        GlobalValues globalVars = FindAnyObjectByType<GlobalValues>();
        globalVars.newScenarioMode = scenarioName;
    }

    public void ResetTextField(GameObject inputTextObj)
    {
        GlobalValues globalVars = FindAnyObjectByType<GlobalValues>();
        if(targetElement == "Seed")
            inputTextObj.GetComponent<TMP_InputField>().text = globalVars.seed;
    }
}
