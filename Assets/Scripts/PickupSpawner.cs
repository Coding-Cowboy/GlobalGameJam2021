using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public GameObject[] pickups;
    private bool spawnChancePassed = false;
    private int activeIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < pickups.Length; i++)
        {
            pickups[i].SetActive(false);
        }
        if (spawnChancePassed)
            pickups[activeIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnPickup()
    {
        //assumes that this function will get called in the awake phase
        if (pickups.Length > 0)
        {
            activeIndex = UnityEngine.Random.Range(0, pickups.Length - 1);
            spawnChancePassed = true;
        }
    }
}
