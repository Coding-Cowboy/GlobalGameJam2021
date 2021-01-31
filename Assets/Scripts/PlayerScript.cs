using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    AudioSource audioSource;
    public Camera PlayerCamera;//Player camera that we will comtrol
    public float MovementSpeed;//Speed that we will use to move the player
    public GameObject Body; //the body that we will rotate
    public GameObject Barrel;//The barrel of the gun which will be where the bullets are shot out of.
    public int NumberOfBullets;//The number of bullets that we would like to spawn
    public GameObject BulletPrefab;
    public float BulletSpeed;//Speed of the bullets
    public int HitCount;//The amount of times that a player can be hit.
    public bool isHit;
    public float hitTimer = 1f;
    public Face DirectionFacing { get; private set; }//The state of the direction that the player is facing
    public Face PreviousDirection { get; private set; }
    public float directionAngle;
    public enum Face{Forward,Backward,Left,Right};
    public Animator AC;
    public float RotationSpeed;//Speed of the rotation for the turning
    private Vector3 GoalRotation;//Goal rotation that will be set to every update
    bool isMoving = false;
    public float shootCooldown = 0.7f;
    bool canShoot = true;
    public AudioClip shootSoundEffect;
    public float shootSoundVolume = 0.7f;
    HeartManager heartManager;

    public ParticleSystem RightBarrel;
    public ParticleSystem LeftBarrel;
    private bool side = false; //0 is right and 1 is left

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        heartManager = FindObjectOfType<HeartManager>();

        //Disable the particles from playing
        RightBarrel.Pause();
        LeftBarrel.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        MovementInputs();
        FireInputs();
        EndGame();
        turn();
        if(isHit)
            HitCheck();

        heartManager.UpdateHealth(HitCount);
    }

    private void EndGame()
    {
        if(HitCount == 0)
        {
            Debug.Log("Spongebob, me boy, I am Overdosing on Ketamine");
        }
    }
    public void HitCheck()
    {
        hitTimer -= Time.deltaTime;
        if(hitTimer < 0)
        {
            hitTimer = 1f;
            isHit = false;
        }
    }
    private void MovementInputs()
    {
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
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            isMoving = false;
            if (audioSource.isPlaying)
                audioSource.Stop();
        }

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
        if ((Input.GetButtonDown("Fire1") || Input.GetAxisRaw("FireTrigger") > 0.4f) && canShoot)
        {
            GameObject NewBullet = Instantiate(BulletPrefab, Barrel.transform.position, Barrel.transform.rotation);
            NewBullet.transform.eulerAngles = new Vector3(0, directionAngle, 0);
            NewBullet.GetComponent<ShootingScript>().fire(BulletSpeed);
            SoundEffectScript.PlaySoundEffect(transform, shootSoundEffect, shootSoundVolume); // sound FX

            // Shooting cooldown
            canShoot = false;
            Invoke("EnableShooting", shootCooldown);
            if(side)
            {
                RightBarrel.Play();
                side = !side;
            }
            else
            {
                LeftBarrel.Play();
                side = !side;
            }
        }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
      
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && !isHit)
        {
            HitCount--;
            //take the RigidBody from teh enemy and apply the force of a less powerful shotgun
            other.gameObject.GetComponent<Rigidbody>().AddForce(-other.gameObject.transform.right * BulletSpeed/5);
            //set flag and disable collider
            isHit = true;
            Debug.Log("Ive been hit spongebob me boy");
            
        }
        else if (other.gameObject.tag == "MedKit")
        {
            Debug.Log($"Health before pickup:{HitCount}");
            HitCount = 3;
            Debug.Log($"Health after pickup:{HitCount}");
            Destroy(other.gameObject);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && !isHit)
        {
            HitCount--;
            //take the RigidBody from teh enemy and apply the force of a less powerful shotgun
            other.gameObject.GetComponent<Rigidbody>().AddForce(-other.gameObject.transform.right * BulletSpeed / 5);
            //set flag and disable collider
            isHit = true;
            Debug.Log("Ive been hit spongebob me boy");
        }
        
    }

    void EnableShooting()
    {
        canShoot = true;
    }
}
