using UnityEngine;
using UnityEngine.UI;

public class UI_SliderUpdater : MonoBehaviour
{
    public string targetElement = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Slider sliderRef = GetComponent<Slider>();
        GlobalValues globalVars = FindAnyObjectByType<GlobalValues>();
        Spawner spawnerRef = FindAnyObjectByType<Spawner>();

        if (targetElement == "Arrival Rate")
        {
            sliderRef.value = spawnerRef.spawnRate;
            FindAnyObjectByType<UI_Controller>().UpdateSliderTextInt(gameObject);
        }
        else if (targetElement == "Security Secondary Modifier")
        {
            sliderRef.value = globalVars.SCPS_AddtionalModifier;
            FindAnyObjectByType<UI_Controller>().UpdateSliderTextPercentage(gameObject);
        }
        else if (targetElement == "Immigration Secondary Modifier")
        {
            sliderRef.value = globalVars.ICPS_AddtionalModifier;
            FindAnyObjectByType<UI_Controller>().UpdateSliderTextPercentage(gameObject);
        }
    }
}
