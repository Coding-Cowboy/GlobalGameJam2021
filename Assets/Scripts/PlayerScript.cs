using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Camera PlayerCamera;//Player camera that we will comtrol
    public float MovementSpeed;//Speed that we will use to move the player
    public GameObject Body; //the body that we will rotate
    public GameObject[] Bullets;
    public GameObject BulletPrefab;
    public float BulletSpeed;//Speed of the bullets
    public Face DirectionFacing { get; private set; }//The state of the direction that the player is facing
    public enum Face{Forward,Backward,Left,Right};
    //public Rigidbody rb;//RigidBody of the player
    // Start is called before the first frame update
    void Start()
    {
        //Set the Rotation to 0,0,0 for the start of the game

    }

    // Update is called once per frame
    void Update()
    {
        MovementInputs();
        FireInputs();
    }
    private void MovementInputs()
    {
        //Get the current position for the key inputs
        Vector3 Position = gameObject.transform.position;
        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            Position.z += MovementSpeed * Time.deltaTime;
            //Set the rotation to 0,0,0 since start
            Body.transform.eulerAngles = new Vector3(0, 180, 0);
            DirectionFacing = Face.Forward;
        }
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            Position.z -= MovementSpeed * Time.deltaTime;
            Body.transform.eulerAngles = new Vector3(0, 0, 0);
            DirectionFacing = Face.Backward;
        }
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            Position.x -= MovementSpeed * Time.deltaTime;
            Body.transform.eulerAngles = new Vector3(0, 90, 0);
            DirectionFacing = Face.Left;
        }
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            Position.x += MovementSpeed * Time.deltaTime;
            Body.transform.eulerAngles = new Vector3(0, 270, 0);
            DirectionFacing = Face.Right;
        }

        //Check for if both w and a are pressed for rotation
        gameObject.transform.position = Position;
    }
    private void FireInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            switch(DirectionFacing)
            {
                case Face.Forward:
                    foreach(GameObject Bullet in Bullets)
                    {
                        Bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    //Add Force
                    Bullets[0].GetComponent<Rigidbody>().AddForce(Bullets[0].transform.forward * BulletSpeed);
                    Bullets[1].GetComponent<Rigidbody>().AddForce(Bullets[1].transform.forward * BulletSpeed);
                    Bullets[2].GetComponent<Rigidbody>().AddForce(Bullets[2].transform.forward * BulletSpeed);
                    Bullets[3].GetComponent<Rigidbody>().AddForce(Bullets[3].transform.forward * BulletSpeed);
                    break;
                case Face.Backward:
                    foreach (GameObject Bullet in Bullets)
                    {
                        Bullet.transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                    //Add Force
                    Bullets[0].GetComponent<Rigidbody>().AddForce(Bullets[0].transform.forward * BulletSpeed);
                    Bullets[1].GetComponent<Rigidbody>().AddForce(Bullets[1].transform.forward * BulletSpeed);
                    Bullets[2].GetComponent<Rigidbody>().AddForce(Bullets[2].transform.forward * BulletSpeed);
                    Bullets[3].GetComponent<Rigidbody>().AddForce(Bullets[3].transform.forward * BulletSpeed);
                    break;
                case Face.Left:
                    foreach (GameObject Bullet in Bullets)
                    {
                        Bullet.transform.eulerAngles = new Vector3(0, 270, 0);
                    }
                    //Add Force
                    Bullets[0].GetComponent<Rigidbody>().AddForce(Bullets[0].transform.forward * BulletSpeed);
                    Bullets[1].GetComponent<Rigidbody>().AddForce(Bullets[1].transform.forward * BulletSpeed);
                    Bullets[2].GetComponent<Rigidbody>().AddForce(Bullets[2].transform.forward * BulletSpeed);
                    Bullets[3].GetComponent<Rigidbody>().AddForce(Bullets[3].transform.forward * BulletSpeed);
                    break;
                case Face.Right:
                    foreach (GameObject Bullet in Bullets)
                    {
                        Bullet.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    //Add Force
                    Bullets[0].GetComponent<Rigidbody>().AddForce(Bullets[0].transform.forward * BulletSpeed);
                    Bullets[1].GetComponent<Rigidbody>().AddForce(Bullets[1].transform.forward * BulletSpeed);
                    Bullets[2].GetComponent<Rigidbody>().AddForce(Bullets[2].transform.forward * BulletSpeed);
                    Bullets[3].GetComponent<Rigidbody>().AddForce(Bullets[3].transform.forward * BulletSpeed);
                    break;
            }
        }
    }
}
