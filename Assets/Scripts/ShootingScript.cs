using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    private string side;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void fire(string side, float Force)
    {
        //Add to the Rotation
        //Vector3 EA = transform.eulerAngles;
        //EA.y += AdditionalRotation.y;
        //transform.eulerAngles = EA;
        //Add force to the bullet
        this.side = side;
        GetComponent<Rigidbody>().AddForce(transform.forward * Force);
        split(Force);
    }
    //Call once to create 3 bullets
    public void split(float Force)
    {
        //Create 2 children off the parent
        if (side == "Forward")
        { 
            GameObject Child1 = Instantiate(gameObject, (transform.position + new Vector3(.06f,0,0)), transform.rotation);
            GameObject Child2 = Instantiate(gameObject, (transform.position - new Vector3(.06f, 0, 0)), transform.rotation);
            //Rotate and add force
            Child1.GetComponent<Rigidbody>().AddForce((transform.forward + new Vector3(.5f,0,0)) * Force);
            Child2.GetComponent<Rigidbody>().AddForce((transform.forward - new Vector3(.5f, 0, 0)) * Force);
        }
        //Create 2 children off the parent
        else if (side == "Backward")
        {
            GameObject Child1 = Instantiate(gameObject, (transform.position - new Vector3(.06f, 0, 0)), transform.rotation);
            GameObject Child2 = Instantiate(gameObject, (transform.position + new Vector3(.06f, 0, 0)), transform.rotation);
            //Rotate and add force
            Child1.GetComponent<Rigidbody>().AddForce((transform.forward - new Vector3(.5f, 0, 0)) * Force);
            Child2.GetComponent<Rigidbody>().AddForce((transform.forward + new Vector3(.5f, 0, 0)) * Force);
        }
        else if (side == "Left")
        {
            GameObject Child1 = Instantiate(gameObject, (transform.position - new Vector3(0, 0, .06f)), transform.rotation);
            GameObject Child2 = Instantiate(gameObject, (transform.position + new Vector3(0, 0, .06f)), transform.rotation);
            //Rotate and add force
            Child1.GetComponent<Rigidbody>().AddForce((transform.forward - new Vector3(0, 0, .5f)) * Force);
            Child2.GetComponent<Rigidbody>().AddForce((transform.forward + new Vector3(0, 0, .5f)) * Force);
        }
        else if (side == "Right")
        {
            GameObject Child1 = Instantiate(gameObject, (transform.position + new Vector3(0, 0, .06f)), transform.rotation);
            GameObject Child2 = Instantiate(gameObject, (transform.position - new Vector3(0, 0, .06f)), transform.rotation);
            //Rotate and add force
            Child1.GetComponent<Rigidbody>().AddForce((transform.forward + new Vector3(0, 0, .5f)) * Force);
            Child2.GetComponent<Rigidbody>().AddForce((transform.forward - new Vector3(0, 0, .5f)) * Force);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        if (other.tag == "Enemy")
            Destroy(other.gameObject);
        Destroy(gameObject);//destroy self regardless
    }

}
