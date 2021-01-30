using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    public GameObject heartPrefab;
    int cachedHealth = 1;
    GameObject[] hearts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(int health)
    {
        if (health != cachedHealth)
        {
            // Clear old hearts
            if (hearts != null && hearts.Length > 0)
            {
                foreach (GameObject o in hearts)
                    Destroy(o);
            }

            // Make new hearts
            cachedHealth = health;
            hearts = new GameObject[cachedHealth];
            for (int i = 0; i < cachedHealth; i++)
            {
                float heartOffset = 60.0f;
                GameObject newHeart = Instantiate(heartPrefab, transform);
                RectTransform uiPosition = newHeart.GetComponent<RectTransform>();
                uiPosition.anchoredPosition = new Vector2(uiPosition.anchoredPosition.x - heartOffset * i, uiPosition.anchoredPosition.y);
                newHeart.transform.SetParent(transform);
                hearts[i] = newHeart;
            }
        }
    }
}
