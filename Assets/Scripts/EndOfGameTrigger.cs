using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfGameTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
        if (other.tag == "Player")
        {
            Debug.Log("End of game sequence");
        }
    }
}
