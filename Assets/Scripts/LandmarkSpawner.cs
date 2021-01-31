using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkSpawner : MonoBehaviour
{
    public GameObject[] landmarks;
    private bool spawnChancePassed = false;
    private int activeIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < landmarks.Length; i++)
        {
            landmarks[i].SetActive(false);
        }
        if (spawnChancePassed)
            landmarks[activeIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnLandmark()
    {
        //assumes that this function will get called in the awake phase
        if (landmarks.Length > 0)
        {
            activeIndex = UnityEngine.Random.Range(0, landmarks.Length - 1);
            spawnChancePassed = true;
        }
    }
}
