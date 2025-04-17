using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class contains all settings and health update logic
/// </summary>
public class HealthPreferences : MonoBehaviour
{
    public GameObject fullHeartsContainer;
    public GameObject emptyHeartsContainer;
 
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    public int imagesAmount;

    public float baseHealth = 100;
    public float currentHealth;

    private float valuePerImage;
    private bool isInvincible = false;

    public Image.FillMethod fillMethod;

    [HideInInspector]
    public Image.OriginHorizontal horizontalDirection;
    [HideInInspector]
    public Image.OriginVertical verticalDirection;
    [HideInInspector]
    public Image.Origin90 radial90Direction;
    [HideInInspector]
    public Image.Origin180 radial180Direction;
    [HideInInspector]
    public Image.Origin360 radial360Direction;

    private Coroutine regenerationCoroutine = null, poisonCoroutine = null;

    /// <summary>
    /// Update health object with inspector changes while not playing 
    /// </summary>
    private void OnValidate()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, baseHealth);

        if (gameObject.scene.IsValid())
        {
            if (!Application.isPlaying)
            {
                RemoveAll();
#if UNITY_EDITOR
                for (int i = 0; i < imagesAmount; i++)
                    EditorApplication.delayCall += () => CreateImage(i);
#endif
            }
#if UNITY_EDITOR
            EditorApplication.delayCall += () => UpdateHealth();
#endif
        }
    }

    /// <summary>
    /// Delete all images from health object
    /// </summary>
    private void RemoveAll()
    {
        if (Application.isPlaying)
        {
            foreach(Transform child in fullHeartsContainer.transform)
                Destroy(child.gameObject);

            foreach (Transform child in emptyHeartsContainer.transform)
                Destroy(child.gameObject);
        }
        else
        {
#if UNITY_EDITOR
            foreach (Transform child in fullHeartsContainer.transform)
                EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);

            foreach (Transform child in emptyHeartsContainer.transform)
                EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);
#endif
        }
    }

    /// <summary>
    /// Create stated amount of images
    /// </summary>
    public void Init(int amount)
    {
        imagesAmount = amount;

        RemoveAll();

        for (int i = 0; i < imagesAmount; i++)
            CreateImage(i);
    }

    /// <summary>
    /// Create image sprite
    /// </summary>
    private void CreateImage(int index)
    {
        GameObject heartFull = new GameObject();
        heartFull.tag = "Heart Full";
        Image imgFull = heartFull.AddComponent<Image>();
        imgFull.sprite = fullHeartSprite;
        heartFull.GetComponent<RectTransform>().SetParent(fullHeartsContainer.transform);
        heartFull.transform.localScale = Vector3.one;

        imgFull.type = Image.Type.Filled;
        imgFull.fillMethod = fillMethod;
        switch (fillMethod)
        {
            case Image.FillMethod.Horizontal: imgFull.fillOrigin = (int)horizontalDirection; break;
            case Image.FillMethod.Vertical: imgFull.fillOrigin = (int)verticalDirection; break;
            case Image.FillMethod.Radial90: imgFull.fillOrigin = (int)radial90Direction; break;
            case Image.FillMethod.Radial180: imgFull.fillOrigin = (int)radial180Direction; break;
            case Image.FillMethod.Radial360: imgFull.fillOrigin = (int)radial360Direction; break;
        }

        valuePerImage = baseHealth / imagesAmount;

        if ((index + 1) * valuePerImage > currentHealth)
        {
            float temp = (index + 1) * valuePerImage - currentHealth;
            float value = 1 - temp / valuePerImage;

            imgFull.fillAmount = value;
        }
        else
            imgFull.fillAmount = 1;

        if (index * valuePerImage >= currentHealth)
            imgFull.fillAmount = 0;


        GameObject heartEmpty = new GameObject();
        heartEmpty.tag = "Heart Empty";
        Image imgEmpty = heartEmpty.AddComponent<Image>();
        imgEmpty.sprite = emptyHeartSprite;
        heartEmpty.GetComponent<RectTransform>().SetParent(emptyHeartsContainer.transform);
        heartEmpty.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Update image sprites according to current health amount
    /// </summary>
    private void UpdateHealth()
    {
        valuePerImage = baseHealth / imagesAmount;

        for (int i = 0; i < imagesAmount; i++)
        {
            if ((i + 1) * valuePerImage > currentHealth)
            {
                float temp = (i + 1) * valuePerImage - currentHealth;
                float value = 1 - temp / valuePerImage;

                fullHeartsContainer.transform.GetChild(i).GetComponent<Image>().fillAmount = value;
            }
            else
                fullHeartsContainer.transform.GetChild(i).GetComponent<Image>().fillAmount = 1;

            if (i * valuePerImage >= currentHealth)
                fullHeartsContainer.transform.GetChild(i).GetComponent<Image>().fillAmount = 0;
        }
    }

    /// <summary>
    /// Set current health amount
    /// </summary>
    public void SetCurrentHealth(float amount)
    {
        currentHealth = Mathf.Clamp(amount, 0, baseHealth);
        UpdateHealth();
    }

    /// <summary>
    /// Get current health amount
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Increase health amount for value per time
    /// </summary>
    public void EnableRegeneration(bool enable, float amountPerSecond = 1, float delay = 1)
    {
        if (enable)
        {
            if (regenerationCoroutine != null)
                StopCoroutine(regenerationCoroutine);

            regenerationCoroutine = StartCoroutine(RegenerationCoroutine(amountPerSecond, delay));
        }
        else
            StopCoroutine(regenerationCoroutine);
    }

    private IEnumerator RegenerationCoroutine(float amountPerSecond, float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            AddHeal(amountPerSecond);
        }
    }

    /// <summary>
    /// Decrease health amount for value per time
    /// </summary>
    public void EnablePoison(bool enable, float amountPerSecond = 1, float delay = 1)
    {
        if (enable)
        {
            if (poisonCoroutine != null)
                StopCoroutine(poisonCoroutine);

            poisonCoroutine = StartCoroutine(PoisonCoroutine(amountPerSecond, delay));
        }
        else
            StopCoroutine(poisonCoroutine);
    }

    private IEnumerator PoisonCoroutine(float amountPerSecond, float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            AddDamage(amountPerSecond);
        }
    }

    /// <summary>
    /// Get base health amount
    /// </summary>
    public float GetTotalHealth()
    {
        return baseHealth;
    }

    /// <summary>
    /// Set base health amount
    /// </summary>
    public void SetTotalHealth(float amount) 
    {
        baseHealth = amount;
        UpdateHealth();
    }

    /// <summary>
    /// Decrease current health by amount
    /// </summary>
    /// <param name="amount"></param>
    public void AddDamage(float amount)
    {
        if (!isInvincible)
        {
            currentHealth -= amount;

            if (currentHealth < 0)
                currentHealth = 0;

            UpdateHealth();
        }
    }

    /// <summary>
    /// Increase current health by amount
    /// </summary>
    public void AddHeal(float amount)
    {
        currentHealth += amount;

        if (currentHealth > baseHealth)
            currentHealth = baseHealth;

        UpdateHealth();
    }

    /// <summary>
    /// Add one image
    /// </summary>
    public void AddImage()
    {
        imagesAmount += 1;
        Init(imagesAmount);
    }

    /// <summary>
    /// Remove one image
    /// </summary>
    public void RemoveImage()
    {
        if (imagesAmount > 0)
        {
            imagesAmount -= 1;
            Init(imagesAmount);
        }
    }

    /// <summary>
    /// Select images fill method
    /// </summary>
    public void SetFillType(Image.FillMethod type)
    {
        fillMethod = type;
        Init(imagesAmount);
    }

    /// <summary>
    /// Enable invincible mode 
    /// </summary>
    public void EnableInvincibility(bool enable)
    {
        if (enable)
            isInvincible = true;
        else
            isInvincible = false;
    }

    /// <summary>
    /// Restore default settings
    /// </summary>
    public void Reset()
    {
        imagesAmount = 3;
        baseHealth = 100;
        currentHealth = 100;
        fillMethod = Image.FillMethod.Horizontal;
        Init(imagesAmount);
    }
}
