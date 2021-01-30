using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    private string side;
    // Start is called before the first frame update
    void Start()
    {
        // Auto despawn
        Invoke("Expire", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void fire(float Force)
    {
        float spawnSpread = 0.06f;
        float angleSpread = 0.2f;

        // Spawn with slight offset
        Vector3 spawnOffset = transform.right * spawnSpread;
        GameObject Child1 = Instantiate(gameObject, (transform.position + spawnOffset), transform.rotation);
        GameObject Child2 = Instantiate(gameObject, (transform.position - spawnOffset), transform.rotation);

        // Rotate and add force
        Vector3 angleOffset = transform.right * angleSpread;
        GetComponent<Rigidbody>().AddForce(transform.forward * Force);
        Child1.GetComponent<Rigidbody>().AddForce((transform.forward + angleOffset) * Force);
        Child2.GetComponent<Rigidbody>().AddForce((transform.forward - angleOffset) * Force);
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
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Enemy")
    //    {
    //        Destroy(gameObject);//destroy self regardless
    //    }
    //    else if(other.tag != "Player")
    //        Destroy(gameObject);//destroy self regardless
    //}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            Destroy(gameObject);//destroy self regardless
        }
        else if (collision.collider.tag != "Player")
            Destroy(gameObject);//destroy self regardless
    }

    void Expire()
    {
        // Projectile dies automatically
        Destroy(gameObject);
    }
}
