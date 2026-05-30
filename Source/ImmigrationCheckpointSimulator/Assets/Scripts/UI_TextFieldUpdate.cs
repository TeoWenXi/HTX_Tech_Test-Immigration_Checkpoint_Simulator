using TMPro;
using UnityEngine;

public class UI_TextFieldUpdate : MonoBehaviour
{
    string text;
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
            GetComponent<TMP_InputField>().text = FindAnyObjectByType<GlobalValues>().seed.ToString();
            Init = true;
        }
    }

    public void UpdateTextData()
    {
        text = GetComponent<TMP_InputField>().text;
        FindAnyObjectByType<GlobalValues>().newSeed = text;
    }
}
