﻿using System;
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
    public Face PreviousDirection { get; private set; }
    public float directionAngle;
    public enum Face{Forward,Backward,Left,Right};
    public Animator AC;
    public float RotationSpeed;//Speed of the rotation for the turning
    private Vector3 GoalRotation;//Goal rotation that will be set to every update
    public float shootCooldown = 0.7f;
    bool canShoot = true;

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
        turn();
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
        bool isMoving = false;
        //Get the current position for the key inputs
        Vector3 Position = gameObject.transform.position;

        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(hInput) > 0.3f || Mathf.Abs(vInput) > 0.3f)
        {
            isMoving = true;
            Position.x += hInput * MovementSpeed * Time.deltaTime;
            Position.z += vInput * MovementSpeed * Time.deltaTime;
            directionAngle = Vector3.SignedAngle(Vector3.forward, new Vector3(hInput, 0, vInput).normalized, Vector3.up);
            GoalRotation = new Vector3(0, directionAngle, 0);
        }

        /*if (Input.GetAxisRaw("Horizontal") < -0.4f)
        {
            PreviousDirection = DirectionFacing;
            Position.x -= MovementSpeed * Time.deltaTime;
            GoalRotation = new Vector3(0, 270, 0);
            DirectionFacing = Face.Left;
            isMoving = true;
        }
        if (Input.GetAxisRaw("Horizontal") > 0.4f)
        {
            PreviousDirection = DirectionFacing;
            Position.x += MovementSpeed * Time.deltaTime;
            GoalRotation = new Vector3(0, 90, 0);
            DirectionFacing = Face.Right;
            isMoving = true;
        }
        if (Input.GetAxisRaw("Vertical") > 0.4f)
        {
            PreviousDirection = DirectionFacing;
            Position.z += MovementSpeed * Time.deltaTime;
            //Set the rotation to 0,0,0 since start
            GoalRotation = new Vector3(0, 0, 0);
            DirectionFacing = Face.Forward;
            isMoving = true;
        }
        if (Input.GetAxisRaw("Vertical") < -0.4f)
        {
            PreviousDirection = DirectionFacing;
            Position.z -= MovementSpeed * Time.deltaTime;
            GoalRotation = new Vector3(0, -180, 0);
            DirectionFacing = Face.Backward;
            isMoving = true;
        }*/

        AC.SetBool("IsMoving", isMoving);
        //Check for if both w and a are pressed for rotation
        //gameObject.transform.position = Position;
        //Move down as well to prevelt climbing
        Position.y = 0f;
        GetComponent<CharacterController>().Move(Position-transform.position);
    }
    private void turn()
    {
        Body.transform.rotation = Quaternion.Slerp(Body.transform.rotation, Quaternion.Euler(GoalRotation), Time.deltaTime * RotationSpeed);
    }
    private void FireInputs()
    {
        // Jump includes space, Z, and joystick trigger
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            GameObject NewBullet = Instantiate(BulletPrefab, Barrel.transform.position, Barrel.transform.rotation);
            NewBullet.transform.eulerAngles = new Vector3(0, directionAngle, 0);
            NewBullet.GetComponent<ShootingScript>().fire(BulletSpeed);

            // Shooting cooldown
            canShoot = false;
            Invoke("EnableShooting", shootCooldown);

            /*switch(DirectionFacing)
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
            }*/
        }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (hit.gameObject.tag == "Enemy")
        if(hit.gameObject.name == "spider")
        {
            HitCount--;
            //take the RigidBody from teh enemy and apply the force of a less powerful shotgun
            hit.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed/2);
        }
    }

    void EnableShooting()
    {
        canShoot = true;
    }
}
