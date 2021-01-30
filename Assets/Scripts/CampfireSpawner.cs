using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireSpawner : MonoBehaviour
{

    public GameObject[] campfires;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < campfires.Length; i++)
        {
            campfires[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnCampfire()
    {
        //Debug.Log("Campfire spawn");
        if (campfires.Length > 0)
        {
            campfires[UnityEngine.Random.Range(0, campfires.Length - 1)].SetActive(true);
        }
    }
}
