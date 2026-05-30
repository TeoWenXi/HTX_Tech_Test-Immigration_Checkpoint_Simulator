using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class StationManager : MonoBehaviour
{
    public enum Direction
    {
        D_LEFT,
        D_RIGHT,
        D_UP,
        D_DOWN
    };

    public GameObject spawnAreaObj;
    public GameObject stationPrefabObj;
    GameObject parentStationObj;
    public WorldManager.WorldSegments stationManagerSegment;
    public Direction directionStationFaces;

    [HideInInspector]
    public List<GameObject> stationList = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentStationObj = transform.Find("Stations").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateStations(int rows, int staionsPerRow, int disabledStationsCount)
    {
        parentStationObj = transform.Find("Stations").gameObject;

        //Variables
        Vector2 spawnLocPivot = spawnAreaObj.transform.position;
        Vector2 spawnZoneScale = spawnAreaObj.transform.lossyScale;

        //Distance between stations
        float xDist = spawnZoneScale.x / staionsPerRow;
        float yDist = spawnZoneScale.y / rows;

        //Get top left corner of the spawn area
        spawnLocPivot.x = spawnLocPivot.x - spawnZoneScale.x * 0.5f + xDist * 0.5f;
        spawnLocPivot.y = spawnLocPivot.y + spawnZoneScale.y * 0.5f - yDist * 0.5f;


        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < staionsPerRow; ++j)
            {
                Vector2 pos = spawnLocPivot;
                pos.x += j * xDist;
                pos.y -= i * yDist;

                //Create station objects
                GameObject go = Instantiate(stationPrefabObj, pos, Quaternion.identity);

                //Rotate station to face the correct direction
                switch (directionStationFaces)
                {
                    case Direction.D_RIGHT:
                        go.transform.rotation = Quaternion.Euler(0, 0, 180);
                        break;
                    case Direction.D_UP:
                        go.transform.rotation = Quaternion.Euler(0, 0, -90);
                        break;
                    case Direction.D_DOWN:
                        go.transform.rotation = Quaternion.Euler(0, 0, 90);
                        break;
                    //Default is left, so no rotation needed
                    default:
                        break;
                }

                //Update position and parent
                go.transform.position = pos;
                go.transform.SetParent(parentStationObj.transform);
                go.GetComponent<Station>().stationSegment = stationManagerSegment;
                if(disabledStationsCount > 0)
                {
                    go.GetComponent<Station>().isActive = false;
                    --disabledStationsCount;
                }

                //Add to list
                stationList.Add(go);
            }
        }
    }

    public GameObject GetEmptyStation()
    {
        for(int i = 0; i < stationList.Count; ++i)
        {
            if (stationList[i].GetComponent<Station>().isActive && 
                stationList[i].GetComponent<Station>().currAgentObj == null)
            {
                return stationList[i];
            }
        }

        //No empty station found
        return null;
    }

    public void UpdateActiveStations(int targetActiveStations)
    {
        //Count number of active stations
        int activeStationCount = 0;
        for(int i = 0; i < stationList.Count; ++i)
        {
            if (stationList[i].GetComponent<Station>().isActive)
                ++activeStationCount;
        }

        //Less active stations than required (Activate Stations)
        if(activeStationCount < targetActiveStations)
        {
            int amountToAdjust = targetActiveStations - activeStationCount;
            for(int i = 0; i < stationList.Count; ++i)
            {
                //Adjusted
                if (amountToAdjust <= 0)
                    break;

                //Station already active
                if (stationList[i].GetComponent<Station>().isActive)
                    continue;

                //Activate station
                stationList[i].GetComponent<Station>().isActive = true;
                --amountToAdjust;
            }
        }

        //More active stations than required (Deactive Stations)
        else if (activeStationCount > targetActiveStations)
        {
            int amountToAdjust = activeStationCount - targetActiveStations;
            for (int i = 0; i < stationList.Count; ++i)
            {
                //Adjusted
                if (amountToAdjust <= 0)
                    break;

                //Station already inactive
                if (!stationList[i].GetComponent<Station>().isActive)
                    continue;

                //Deactivate station
                stationList[i].GetComponent<Station>().isActive = false;
                --amountToAdjust;
            }
        }
    }
}
