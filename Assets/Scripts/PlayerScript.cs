using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Camera PlayerCamera;//Player camera that we will comtrol
    public float MovementSpeed;//Speed that we will use to move the player
    public GameObject Body; //the body that we will rotate
    public GameObject Barrel;//The barrel of the gun which will be where the bullets are shot out of.
    public int NumberOfBullets;//The number of bullets that we would like to spawn
    public GameObject BulletPrefab;
    public float BulletSpeed;//Speed of the bullets
    public int HitCount;//The amount of times that a player can be hit.
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
        EndGame();
    }

    private void EndGame()
    {
        if(HitCount == 0)
        {
            Debug.Log("Spongebob, me boy, I am Overdosing on Ketamine");
        }
    }

    private void MovementInputs()
    {
        //Get the current position for the key inputs
        Vector3 Position = gameObject.transform.position;
        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            Position.z += MovementSpeed * Time.deltaTime;
            //Set the rotation to 0,0,0 since start
            Body.transform.eulerAngles = new Vector3(0, 0, 0);
            DirectionFacing = Face.Forward;
        }
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            Position.z -= MovementSpeed * Time.deltaTime;
            Body.transform.eulerAngles = new Vector3(0, -180, 0);
            DirectionFacing = Face.Backward;
        }
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            Position.x -= MovementSpeed * Time.deltaTime;
            Body.transform.eulerAngles = new Vector3(0, 270, 0);
            DirectionFacing = Face.Left;
        }
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            Position.x += MovementSpeed * Time.deltaTime;
            Body.transform.eulerAngles = new Vector3(0, 90, 0);
            DirectionFacing = Face.Right;
        }

        //Check for if both w and a are pressed for rotation
        //gameObject.transform.position = Position;
        GetComponent<CharacterController>().Move(Position-transform.position);
    }
    private void FireInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            GameObject NewBullet = Instantiate(BulletPrefab, Barrel.transform.position,Barrel.transform.rotation);
            switch(DirectionFacing)
            {
                case Face.Forward:
                        //Create the bullet for element i
                        //Give the NewBullet the Prefab for a bullet, the barrel transform position and the barrel transform rotation
                        //Set the Bullet eulerAngle to face the right way when firing
                        NewBullet.transform.eulerAngles = new Vector3(0, 0, 0);
                        NewBullet.GetComponent<ShootingScript>().fire( "Forward", BulletSpeed);
                    break;
                case Face.Backward:
                    for (int i = 0; i < NumberOfBullets; i++)
                    {
                        //Create the bullet for element i
                        //Give the NewBullet the Prefab for a bullet, the barrel transform position and the barrel transform rotation
                        //Set the Bullet eulerAngle to face the right way when firing
                        NewBullet.transform.eulerAngles = new Vector3(0, 180, 0);
                        NewBullet.GetComponent<ShootingScript>().fire("Backward", BulletSpeed);
                    }
                    break;
                case Face.Left:
                    for (int i = 0; i < NumberOfBullets; i++)
                    {
                        //Create the bullet for element i
                        //Give the NewBullet the Prefab for a bullet, the barrel transform position and the barrel transform rotation
                        //Set the Bullet eulerAngle to face the right way when firing
                        NewBullet.transform.eulerAngles = new Vector3(0, 270, 0);
                        NewBullet.GetComponent<ShootingScript>().fire("Left", BulletSpeed);
                    }
                    break;
                case Face.Right:
                    for (int i = 0; i < NumberOfBullets; i++)
                    {
                        //Create the bullet for element i
                        //Give the NewBullet the Prefab for a bullet, the barrel transform position and the barrel transform rotation
                        //Set the Bullet eulerAngle to face the right way when firing
                        NewBullet.transform.eulerAngles = new Vector3(0, 90, 0);
                        NewBullet.GetComponent<ShootingScript>().fire("Right", BulletSpeed);
                    }
                    break;
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
            Debug.Log("Ive been hit spongebob me boy");
            HitCount--;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemey")
        {
            Debug.Log("Ive been hit spongebob me boy");
            HitCount--;
        }
    }
}
