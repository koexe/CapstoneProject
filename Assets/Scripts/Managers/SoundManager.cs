using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSourcePrefab;
    [SerializeField] private Transform seContainer;

    [SerializeField] float seVolume = 1f;

    List<AudioSource> seSources = new List<AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            if (bgmSource == null)
            {
                GameObject bgmObj = new GameObject("BGM_Source");
                bgmObj.transform.parent = transform;
                bgmSource = bgmObj.AddComponent<AudioSource>();
                bgmSource.loop = true;
            }

            if (seContainer == null)
            {
                GameObject seParent = new GameObject("SE_Container");
                seParent.transform.parent = transform;
                seContainer = seParent.transform;
            }

            if (seSourcePrefab == null)
            {
                GameObject seObj = new GameObject("SE_Source_Prefab");
                seObj.transform.parent = transform;
                var source = seObj.AddComponent<AudioSource>();
                seSourcePrefab = source;
                seSourcePrefab.playOnAwake = false;
                seSourcePrefab.loop = false;
                seObj.SetActive(false);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void PlayBGM(AudioClip _clip, bool _loop = true)
    {
        if (_clip == null) return;

        bgmSource.clip = _clip;
        bgmSource.loop = _loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PlaySE(AudioClip _clip , bool _isAllowMultiple = true)
    {
        if (_clip == null) return;
        if(_isAllowMultiple == false)
        {
            for(int i = seSources.Count - 1; i >= 0; i--)
            {
                var t_source = seSources[i];
                if(t_source == null)
                {
                    seSources.RemoveAt(i);
                    continue;
                }
                if(t_source.clip == _clip)
                {
                    return;
                }
            }
        }

        AudioSource t_SE = Instantiate(seSourcePrefab, seContainer);
        t_SE.gameObject.SetActive(true);
        t_SE.clip = _clip;
        t_SE.volume = seVolume;
        t_SE.Play();
        seSources.Add(t_SE);
        
        Destroy(t_SE.gameObject, _clip.length);
    }

    public void SetBGMVolume(float _volume)
    {
        bgmSource.volume = Mathf.Clamp01(_volume);
    }

    public void SetSEVolume(float _volume)
    {
        seVolume = Mathf.Clamp01(_volume);
        foreach (Transform child in seContainer)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source != null) source.volume = Mathf.Clamp01(_volume);
        }
    }
}