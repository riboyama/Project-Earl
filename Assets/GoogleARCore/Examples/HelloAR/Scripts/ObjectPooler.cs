using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler SharedInstance;

    public List<GameObject> pooledEmotions;
    public List<GameObject> pooledObjects;

    public GameObject objectToPool;
    public int amountToPool = 14;

    public GameObject emotionPanel;
    public GameObject objectPanel;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        //add emotion text to the list and panel
        pooledEmotions = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(objectToPool);
            obj.transform.parent = emotionPanel.transform;
            obj.SetActive(false);
            pooledEmotions.Add(obj);
        }
        //add object text to the list and panel
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(objectToPool);
            obj.transform.parent = objectPanel.transform;
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
    //Get pooled Emotions
    public GameObject GetPooledEmotions()
    {
        for (int i = 0; i < pooledEmotions.Count; i++)
        {
            if (!pooledEmotions[i].activeInHierarchy)
            {
                return pooledEmotions[i];
            }            
        }
        return null;
    }
    //Get pooled Objects
    public GameObject GetPooledObjects()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
