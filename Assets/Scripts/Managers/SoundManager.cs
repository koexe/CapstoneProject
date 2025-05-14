using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSourcePrefab;
    [SerializeField] private Transform seContainer;

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

    public void PlaySE(AudioClip _clip)
    {
        if (_clip == null) return;

        AudioSource t_SE = Instantiate(seSourcePrefab, seContainer);
        t_SE.gameObject.SetActive(true);
        t_SE.clip = _clip;
        t_SE.Play();
        Destroy(t_SE.gameObject, _clip.length);
    }

    public void SetBGMVolume(float _volume)
    {
        bgmSource.volume = Mathf.Clamp01(_volume);
    }

    public void SetSEVolume(float _volume)
    {
        foreach (Transform child in seContainer)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source != null) source.volume = Mathf.Clamp01(_volume);
        }
    }
}