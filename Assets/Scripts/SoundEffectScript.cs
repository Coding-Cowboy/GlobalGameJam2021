﻿using System.Collections;
using UnityEngine;

public class SoundEffectScript : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;
    public float volume;
    private bool played = false;

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.8f;
        source.volume = volume;
        source.rolloffMode = AudioRolloffMode.Linear; // linear rolloff
    }

    // Update is called once per frame
    void Update()
    {
        // Self-destruct when done playing
        if (played && !source.isPlaying)
            Destroy(gameObject);
    }

    public void Play()
    {
        StartCoroutine(RealPlay());
    }

    private IEnumerator RealPlay()
    {
        yield return new WaitForSeconds(0.01f);
        if (clip != null)
            source.PlayOneShot(clip);
        played = true;
    }

    public static void PlaySoundEffect(Transform parent, AudioClip clip, float volume = 1.0f)
    {
        GameObject soundMaker = new GameObject("SoundEffect");
        soundMaker.transform.position = parent.position;
        soundMaker.transform.parent = parent;
        SoundEffectScript source = soundMaker.AddComponent<SoundEffectScript>();
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }
}
