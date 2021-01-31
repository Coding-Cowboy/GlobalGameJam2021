using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireSpawner : MonoBehaviour
{

    public GameObject[] campfires;
    private bool spawnChancePassed = false;
    private int activeIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < campfires.Length; i++)
        {
            campfires[i].SetActive(false);
        }
        if (spawnChancePassed)
            campfires[activeIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnCampfire()
    {
        //assumes that this function will get called in the awake phase
        if (campfires.Length > 0)
        {
            activeIndex = UnityEngine.Random.Range(0, campfires.Length - 1);
            spawnChancePassed = true;
        }
    }
}
