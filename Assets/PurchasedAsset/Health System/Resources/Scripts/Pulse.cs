using System.Collections;
using UnityEngine;

/// <summary>
/// This class is used for pulse effect
/// </summary>
public class Pulse : MonoBehaviour {

    private Coroutine lastRoutine = null;

    public float approachSpeed = 0.0005f;
    public float growthBound = 1.05f;
    public float shrinkBound = 0.95f;
    private float currentRatio = 1;

    private Vector3 originalSize;

    public float threshold;
    public bool isActivated = false;

    private HealthPreferences prefs;

    public void Start()
    {
        prefs = GetComponent<HealthPreferences>();
    }

    /// <summary>
    /// Mark pulse effect as enabled 
    /// </summary>
    public void Run(float threshold = 0)
    {
        this.threshold = threshold;
        originalSize = transform.localScale;

        Stop();

        lastRoutine = StartCoroutine(PulseEffect());
    }

    /// <summary>
    /// Stop pulse effect
    /// </summary>
    public void Stop()
    {
        if (lastRoutine != null)
            StopCoroutine(lastRoutine);

        transform.localScale = originalSize;
    }

    private IEnumerator PulseEffect()
    {
        while (true)
        {
            if (prefs.GetCurrentHealth() < threshold)
            {
                while (currentRatio != growthBound)
                {
                    currentRatio = Mathf.MoveTowards(currentRatio, growthBound, approachSpeed);
                    transform.localScale = Vector3.one * currentRatio;
                    yield return new WaitForEndOfFrame();
                }

                while (currentRatio != shrinkBound)
                {
                    currentRatio = Mathf.MoveTowards(currentRatio, shrinkBound, approachSpeed);
                    transform.localScale = Vector3.one * currentRatio;
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                transform.localScale = originalSize;
                yield return new WaitForEndOfFrame();
            }  
        }
    }
}
