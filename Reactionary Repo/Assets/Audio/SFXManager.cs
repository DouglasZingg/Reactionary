using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************
 * Filename:  		SFXManager.cs
 * Date:      		1/6/2021
 * Mod. Date: 		1/20/2021
 * Mod. Initials:	KK
 * Author:    		Kaden Kugal
 * Purpose:   		Used to handle playing sound effects for any purpose in the game
 *                  and feeding the sound into the proper mixer channel.
 ************************************************/

[System.Serializable] public struct SoundClip
{
    public SFXManager.eSounds eSound;
    public AudioClip clip;
}
public class SFXManager : MonoBehaviour
{
    public enum eSounds
    {
        PlayerFiresMagicBolt,
        PlayerFiresFireball,
        PlayerFiresConeOfCold,
        PlayerGetsHurt,
        PlayerJumps,
        PlayerDied,
        EnemyHitByFireball,
        EnemyHitByMagicBolt,
        EnemyDied,
        Headshot,
        TerrainHitByMagicBolt,
        FireballExplosion,
        WaveCompleted,
        WaveStarted
    }

    //member variables
    public static SFXManager g_cInstance;

    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioSource m_pitchAudioSource;
    [SerializeField] private AudioSource m_loopAudioSource;
    [SerializeField] private SoundClip[] m_soundClips;

    //called from external scripts whenever a sound effect is triggered
    public void PlaySoundEffect(eSounds eSound)
    {
        switch (eSound)
        {
            case eSounds.EnemyDied:
                {
                    m_pitchAudioSource.pitch = GetRandomPitch(0.3f, 1.2f);
                    m_pitchAudioSource.PlayOneShot(GetAudioClip(eSound));
                }
                break;
            case eSounds.PlayerJumps:
                {
                    m_pitchAudioSource.pitch = GetRandomPitch(0.9f, 1.1f);
                    m_pitchAudioSource.PlayOneShot(GetAudioClip(eSound));
                }
                break;
            case eSounds.PlayerGetsHurt:
                {
                    m_pitchAudioSource.pitch = GetRandomPitch(0.9f, 1.1f);
                    m_pitchAudioSource.PlayOneShot(GetAudioClip(eSound));
                }
                break;
            case eSounds.PlayerFiresConeOfCold:
                {
                    m_loopAudioSource.clip = GetAudioClip(eSound);
                    m_loopAudioSource.Play();
                }
                break;
            default:
                {
                    m_audioSource.pitch = 1.0f;
                    m_audioSource.PlayOneShot(GetAudioClip(eSound));
                }
                break;
        }
    }

    //used to stop looping sound effects
    public void StopSoundEffect(eSounds eSound)
    {
        switch (eSound)
        {
            case eSounds.PlayerFiresConeOfCold:
                {
                    m_loopAudioSource.Stop();
                }
                break;
            default:
                Debug.Log("Something not looping called loop functions");
                break;
        }
    }

    //Gets an AudioClip based on whatever sound is assigned in the inspector
    private AudioClip GetAudioClip(eSounds eSound)
    {
        for (int i = 0; i < m_soundClips.Length; i++)
        {
            if (m_soundClips[i].eSound == eSound)
            {
                return m_soundClips[i].clip;
            }
        }
        Debug.Log("Sound Not Found");
        return null;
    }

    private float GetRandomPitch(float min, float max)
    {
        return Random.Range(min, max);
    }

    //Sets up the Singleton
    private void Awake()
    {
        if (g_cInstance == null)
        {
            g_cInstance = this;
        }
        else if (g_cInstance != null)
        {
            Destroy(gameObject);
        }
    }
}
