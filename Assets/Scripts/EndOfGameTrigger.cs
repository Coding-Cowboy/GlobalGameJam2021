using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfGameTrigger : MonoBehaviour
{
    LevelManager lm;
    // Start is called before the first frame update
    void Start()
    {
        lm = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            lm.CompleteLevel();
        }
    }
}
