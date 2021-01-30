using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void fire(Vector3 AdditionalRotation, Vector3 Force)
    {
        //Add to the Rotation
        transform.eulerAngles += AdditionalRotation;
        GetComponent<Rigidbody>().AddForce(transform.forward);
        Debug.Log("Firing");
    }
}
