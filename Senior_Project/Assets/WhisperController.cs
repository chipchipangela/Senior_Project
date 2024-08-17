using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhisperController : MonoBehaviour
{
    public AudioClip sound1;
    public AudioClip sound2;
    public GameObject speakerPrefab;
    private List<AudioSource> allSpeakers = new List<AudioSource>();
    private bool isSpawning = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlaySound1();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SpawnSpeakers());
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StopSpawningAndMuteAllSounds();
        }
    }

    void PlaySound1()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sound1;
        audioSource.Play();
    }

    IEnumerator SpawnSpeakers()
    {
        while (isSpawning)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-10f, 10f),
                0f,
                Random.Range(-10f, 10f)
            );

            if (!(spawnPosition.x >= -10f && spawnPosition.x <= -6f &&
                  spawnPosition.z >= -10f && spawnPosition.z <= -6f))
            {
                GameObject newSpeaker = Instantiate(speakerPrefab, spawnPosition, Quaternion.identity);
                AudioSource speakerAudio = newSpeaker.GetComponent<AudioSource>();
                speakerAudio.clip = sound2;
                speakerAudio.Play();
                allSpeakers.Add(speakerAudio);
            }

            yield return new WaitForSeconds(0.5f); // 控制生成間隔
        }
    }

    void StopSpawningAndMuteAllSounds()
    {
        // 停止生成喇叭
        isSpawning = false;

        // 停止所有喇叭的播放
        foreach (AudioSource speaker in allSpeakers)
        {
            speaker.Stop();
        }
    }
}