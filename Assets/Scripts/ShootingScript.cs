using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public GameObject[] Bullets = new GameObject[4];//List of bullets for if we need more of a spread
    
    public float BulletOffset;//This is the size of the offest between bullets
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Fire(GameObject BulletPrefab, Vector3 StartPosition, Vector3 Rotation, float BulletSpeed)
    {
        //Instantiate(BulletPrefab);
        transform.eulerAngles = Rotation;
        //Add Force
        GetComponent<Rigidbody>().AddForce(Bullets[0].transform.forward * BulletSpeed);
    }
}
