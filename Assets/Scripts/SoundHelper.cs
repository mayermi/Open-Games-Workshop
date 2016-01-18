﻿using UnityEngine;
using System.Collections;

public class SoundHelper : MonoBehaviour {


    public void SwitchFade(AudioSource source, AudioClip from, AudioClip to, float dur)
    {
        StartCoroutine(Switch(source, from, to, dur));
    }

    IEnumerator Switch(AudioSource source, AudioClip from, AudioClip to, float dur)
    {
        Fade(source, false, dur / 2);
        yield return new WaitForSeconds(dur / 2);
        source.clip = to;
        Fade(source, true, dur / 2);
    }

    public void Fade(AudioSource source, bool fadeIn,  float dur)
    {
        if (fadeIn)
        {
            source.volume = 0f;
            StartCoroutine(CoFadeIn(source,  dur));
        }
        if (!fadeIn)
        {
            source.volume = 1f;
            StartCoroutine(CoFadeOut(source, dur));
        }
        source.Play();
    }

    IEnumerator CoFadeIn(AudioSource source,  float dur)
    {
        float start = Time.time;
        while(Time.time-start <= dur)
        {
            source.volume = ( (Time.time - start) / dur );
            Debug.Log(source.volume);
            yield return null;
        }
    }

    IEnumerator CoFadeOut(AudioSource source, float dur)
    {
        float start = Time.time;
        while (Time.time - start <= dur)
        {
            source.volume = 1f - ((Time.time - start) / dur);
            Debug.Log(source.volume);
            yield return null;
        }
    }
}