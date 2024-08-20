using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Boss Tracks")]
    [SerializeField] AudioSource bossIntroPlayer;
    [SerializeField] AudioSource bossLoopPlayer;

    [Header("Damage Sounds")]
    public AudioClip[] physicalDamageSFX;

    [Header("Action Sounds")]
    public AudioClip pickUpItemSFX;
    public AudioClip rollSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] sfxList)
    {
        int index = Random.Range(0, sfxList.Length);

        return sfxList[index];
    }

    /* Method 2 for footStep sfx maker
    public AudioClip ChooseRandomFootStepSoundBaseOnGround(GameObject steppedOnObject, CharacterManager character)
    {
        if (steppedOnObject.tag == "Dirt")
        {
            return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepDirt);
        }
        else if (steppedOnObject.tag == "Stone")
        {
            return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepStone);
        }

        return null;
    }
    */

    public void PlayBossTrack(AudioClip introTrack, AudioClip loopTrack)
    {
        if (introTrack == null || loopTrack == null)
        {
            return;
        }

        bossIntroPlayer.volume = 1;
        bossIntroPlayer.clip = introTrack;
        bossIntroPlayer.loop = false;
        bossIntroPlayer.Play();

        bossLoopPlayer.volume = 1;
        bossLoopPlayer.clip = loopTrack;
        bossLoopPlayer.loop = true;
        bossLoopPlayer.PlayDelayed(bossIntroPlayer.clip.length);
    }

    public void StopBossMusic()
    {
        StartCoroutine(FadeOutBossMusic());
    }

    private IEnumerator FadeOutBossMusic()
    {
        while (bossLoopPlayer.volume > 0)
        {
            bossIntroPlayer.volume -= 0.5f * Time.deltaTime;
            bossLoopPlayer.volume -= 0.5f * Time.deltaTime;
            yield return null;
        }

        bossIntroPlayer.Stop();
        bossLoopPlayer.Stop();
    }
}
