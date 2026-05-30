using UnityEngine;
using UnityEngine.UI;

public class UI_StationSlider : MonoBehaviour
{
    public string targetStationName = "";
    bool Init = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!Init)
        {
            Slider sliderRef = GetComponent<Slider>();
            StationManager objRef = null;
            if (targetStationName == "Security Checkpoint")
                objRef = FindAnyObjectByType<WorldManager>().securityZoneObj.GetComponent<StationManager>();
            else if (targetStationName == "Security Secondary Check Bot")
                objRef = FindAnyObjectByType<WorldManager>().securitySecondaryCheckParentObj.transform.GetChild(0).GetComponent<StationManager>();
            else if (targetStationName == "Security Secondary Check Top")
                objRef = FindAnyObjectByType<WorldManager>().securitySecondaryCheckParentObj.transform.GetChild(1).GetComponent<StationManager>();

            else if (targetStationName == "Immigration Checkpoint")
                objRef = FindAnyObjectByType<WorldManager>().immigrationAreaObj.GetComponent<StationManager>();
            else if (targetStationName == "Immigration Secondary Check Bot")
                objRef = FindAnyObjectByType<WorldManager>().immigrationSecondaryCheckParentObj.transform.GetChild(0).GetComponent<StationManager>();
            else if (targetStationName == "Immigration Secondary Check Top")
                objRef = FindAnyObjectByType<WorldManager>().immigrationSecondaryCheckParentObj.transform.GetChild(1).GetComponent<StationManager>();


            if (objRef != null)
            {
                sliderRef.maxValue = objRef.stationList.Count;

                int counter = 0;
                for (int i = 0; i < objRef.stationList.Count; i++)
                    if (objRef.stationList[i].GetComponent<Station>().isActive)
                        ++counter;

                sliderRef.value = counter;
            }

            Init = true;
        }
    }
}
