using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum QueueLayout
{
    QL_ZIGZAG,
    QL_SINGLE_LINES
}

public class QueueManager : MonoBehaviour
{
    [Header("Reference Game Objects")]
    public GameObject wallObj;
    public GameObject tempQueueSlotObj;
    public GameObject QueueEndPositionObj;
    public GameObject queueStartTriggerBoxObj;
    public GameObject queueStartTriggerBoxPrefab;

    //Grid Queue System
    [Header("Grid Queue System")]
    public int rows = 10;
    public int columns = 10;
    public float gridWidth = 1;
    public bool swap = false;   //For Zig-Zag pattern

    public List<GameObject> queueList = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateQueue();
    }

    public void CreateQueue()
    {
        //Create grid list
        for (int i = 0; i < columns; ++i)
        {
            // Connectors between rows
            if (i % 2 == 0)
            {
                Vector2 pos = new Vector2(QueueEndPositionObj.transform.position.x - i * gridWidth, 0);
                //Down
                if (swap)
                    pos.y = QueueEndPositionObj.transform.position.y;
                //Up
                else
                    pos.y = QueueEndPositionObj.transform.position.y + gridWidth * (rows - 1);

                GameObject go = Instantiate(tempQueueSlotObj, pos, Quaternion.identity);
                go.transform.SetParent(transform);
                go.transform.localScale = new Vector3(gridWidth, gridWidth, 1);
                go.transform.position = pos;

                queueList.Add(go);

                //Walls
                for (int j = 1; j < rows; ++j)
                {
                    pos = new Vector2(QueueEndPositionObj.transform.position.x - i * gridWidth, 0);

                    //From down to up
                    if (swap)
                        pos.y = QueueEndPositionObj.transform.position.y + gridWidth * j;
                    //From up to down
                    else
                        pos.y = QueueEndPositionObj.transform.position.y + gridWidth * (rows - j - 1);

                    //Instantiate wall object
                    GameObject wall = Instantiate(wallObj, pos, Quaternion.identity);
                    wall.transform.SetParent(transform);
                    wall.transform.localScale = new Vector3(gridWidth, gridWidth, 1);
                }
            }
            else
            {
                for (int j = 0; j < rows; ++j)
                {
                    Vector2 pos = new Vector2(QueueEndPositionObj.transform.position.x - i * gridWidth, 0);

                    //From down to up
                    if (swap)
                        pos.y = QueueEndPositionObj.transform.position.y + gridWidth * j;
                    //From up to down
                    else
                        pos.y = QueueEndPositionObj.transform.position.y + gridWidth * (rows - j - 1);

                    //Add new queue slot to list
                    GameObject go = Instantiate(tempQueueSlotObj, pos, Quaternion.identity);
                    go.transform.SetParent(transform);
                    go.transform.localScale = new Vector3(gridWidth, gridWidth, 1);
                    go.transform.position = pos;

                    queueList.Add(go);
                }

                //Swap direction for next row
                swap = !swap;
            }
        }

        for (int i = 0; i < columns; ++i)
        {
            //Top Walls
            Vector2 pos = new Vector2(QueueEndPositionObj.transform.position.x - i * gridWidth, QueueEndPositionObj.transform.position.y - gridWidth);
            GameObject wall = Instantiate(wallObj, pos, Quaternion.identity);
            wall.transform.SetParent(transform);
            wall.transform.localScale = new Vector3(gridWidth, gridWidth, 1);

            //Bottom Walls
            pos = new Vector2(QueueEndPositionObj.transform.position.x - i * gridWidth, QueueEndPositionObj.transform.position.y + gridWidth * rows);
            wall = Instantiate(wallObj, pos, Quaternion.identity);
            wall.transform.SetParent(transform);
            wall.transform.localScale = new Vector3(gridWidth, gridWidth, 1);
        }

        //Create triggerboxes at the start of the queue
        Vector2 tempPos = new Vector2(QueueEndPositionObj.transform.position.x - columns * gridWidth, 0);
        //Down
        if (swap)
            tempPos.y = QueueEndPositionObj.transform.position.y;
        //Up
        else
            tempPos.y = QueueEndPositionObj.transform.position.y + gridWidth * (rows - 1);

        GameObject newObj = Instantiate(queueStartTriggerBoxPrefab, tempPos, Quaternion.identity);
        newObj.transform.SetParent(transform);
        newObj.transform.localScale = new Vector3(gridWidth, gridWidth, 1);
        newObj.GetComponent<QueueTriggerBox>().queueManagerObj = gameObject;
        queueStartTriggerBoxObj = newObj;
    }

    public void AddAgentToQueue(GameObject agentObj)
    {
        //Add agent to the end of the queue
        if (queueList[queueList.Count - 1].GetComponent<QueueSlot>().agentObject == null)
            queueList[queueList.Count - 1].GetComponent<QueueSlot>().agentObject = agentObj;
    }

    public void MoveToStartOfQueue(GameObject agentObj)
    {
        agentObj.GetComponent<Agent>().SetTarget(queueStartTriggerBoxObj.transform.position);
        AddAgentToQueue(agentObj);
    }

    public void MoveAgentToQueuePosition(GameObject agentObj)
    {
        for (int i = 0; i < queueList.Count; ++i)
        {
            if (queueList[i].GetComponent<QueueSlot>().agentObject == agentObj)
            {
                agentObj.GetComponent<Agent>().SetTarget(queueList[i].transform.position);
                return;
            }
        }

        //If not working for some reason
        AddAgentToQueue(agentObj);
    }

    public void UpdateQueue()
    {
        //Move queue up if the front of the queue is empty
        for (int i = 1; i < queueList.Count; ++i)
        {
            if (queueList[i - 1].GetComponent<QueueSlot>().agentObject == null &&
                queueList[i].GetComponent<QueueSlot>().agentObject != null)
            {
                //Update agent new target position
                queueList[i].GetComponent<QueueSlot>().agentObject.GetComponent<Agent>().SetTarget(queueList[i - 1].transform.position);
                queueList[i - 1].GetComponent<QueueSlot>().agentObject = queueList[i].GetComponent<QueueSlot>().agentObject;
                queueList[i].GetComponent<QueueSlot>().ResetVars();
            }
        }
    }
}
