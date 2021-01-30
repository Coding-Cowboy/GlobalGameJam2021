using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject lightTheme;
    public GameObject darkTheme;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void CompleteLevel()
    {
        lightTheme.SetActive(true);
        darkTheme.SetActive(false);

        //destroy all enemies
        EnemyScript[] enemies = FindObjectsOfType<EnemyScript>();
        for (int i = 0; i < enemies.Length; i++)
        {
            UnityEngine.Object.Destroy(enemies[i].gameObject);
        }
    }
}
