using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Linq;

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

    [SerializeField] private Health.FillDirection fillDirection = Health.FillDirection.LeftToRight;
    [SerializeField] private float heartSpacing = 0.5f;

    private float valuePerImage;
    private bool isInvincible = false;

    private SpriteRenderer[] fullHeartRenderers;
    private SpriteRenderer[] emptyHeartRenderers;
    private Material[] fullHeartMaterials;

    private Coroutine regenerationCoroutine = null, poisonCoroutine = null;

    private void Awake()
    {
        InitializeArrays();
    }

    private void InitializeArrays()
    {
        if (fullHeartRenderers == null || fullHeartRenderers.Length != imagesAmount)
        {
            fullHeartRenderers = new SpriteRenderer[imagesAmount];
            emptyHeartRenderers = new SpriteRenderer[imagesAmount];
            fullHeartMaterials = new Material[imagesAmount];
        }
    }

    /// <summary>
    /// Update health object with inspector changes while not playing 
    /// </summary>
    private void OnValidate()
    {
        if (!gameObject.scene.IsValid()) return;

        currentHealth = Mathf.Clamp(currentHealth, 0, baseHealth);

        // 필수 컴포넌트 체크
        if (fullHeartsContainer == null || emptyHeartsContainer == null || 
            fullHeartSprite == null || emptyHeartSprite == null)
        {
            Debug.LogWarning("HealthPreferences: 필수 컴포넌트가 할당되지 않았습니다.");
            return;
        }

        if (!Application.isPlaying)
        {
            InitializeArrays();
            RemoveAll();

#if UNITY_EDITOR
            for (int i = 0; i < imagesAmount; i++)
            {
                int index = i;
                EditorApplication.delayCall += () => 
                {
                    if (this != null)
                        CreateHeart(index);
                };
            }
#endif
        }
    }

    /// <summary>
    /// Delete all images from health object
    /// </summary>
    private void RemoveAll()
    {
        if (fullHeartsContainer == null || emptyHeartsContainer == null) return;

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
            // 프리팹 체크
            bool isPrefab = PrefabUtility.IsPartOfPrefabInstance(gameObject);

            if (isPrefab)
            {
                // 프리팹인 경우 기존 하트들을 비활성화
                foreach (Transform child in fullHeartsContainer.transform)
                    if (child != null) child.gameObject.SetActive(false);
                foreach (Transform child in emptyHeartsContainer.transform)
                    if (child != null) child.gameObject.SetActive(false);
            }
            else
            {
                // 프리팹이 아닌 경우 직접 삭제
                var fullHearts = fullHeartsContainer.transform.Cast<Transform>().ToList();
                var emptyHearts = emptyHeartsContainer.transform.Cast<Transform>().ToList();

                foreach (var child in fullHearts)
                    if (child != null)
                        EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);
                
                foreach (var child in emptyHearts)
                    if (child != null)
                        EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);
            }
#endif
        }
    }

    /// <summary>
    /// Create stated amount of images
    /// </summary>
    public void Init(int amount)
    {
        imagesAmount = amount;
        InitializeArrays();
        RemoveAll();

        for (int i = 0; i < imagesAmount; i++)
            CreateHeart(i);

        UpdateHealth();
    }

    /// <summary>
    /// Create image sprite
    /// </summary>
    private void CreateHeart(int index)
    {
        if (fullHeartsContainer == null || emptyHeartsContainer == null ||
            fullHeartSprite == null || emptyHeartSprite == null ||
            fullHeartRenderers == null || emptyHeartRenderers == null ||
            fullHeartMaterials == null || index >= imagesAmount)
        {
            return;
        }

#if UNITY_EDITOR
        bool isPrefab = PrefabUtility.IsPartOfPrefabInstance(gameObject);
        Transform existingFullHeart = fullHeartsContainer.transform.Find($"HeartFull_{index}");
        Transform existingEmptyHeart = emptyHeartsContainer.transform.Find($"HeartEmpty_{index}");
        Vector3 heartPosition = new Vector3(index * 0.5f, 0, 0);

        if (isPrefab)
        {
            if (existingFullHeart != null)
            {
                existingFullHeart.gameObject.SetActive(true);
                existingFullHeart.localPosition = heartPosition;
                SpriteRenderer rendererFull = existingFullHeart.GetComponent<SpriteRenderer>();
                if (rendererFull != null)
                {
                    rendererFull.sprite = fullHeartSprite;
                    rendererFull.sortingOrder = 1;
                    fullHeartRenderers[index] = rendererFull;

                    if (!Application.isPlaying)
                    {
                        if (rendererFull.sharedMaterial == null)
                        {
                            Material matFull = new Material(Shader.Find("Sprites/Fill"));
                            matFull.SetFloat("_FillAmount", 1);
                            rendererFull.sharedMaterial = matFull;
                        }
                        fullHeartMaterials[index] = rendererFull.sharedMaterial;
                    }
                    else
                    {
                        Material matFull = new Material(Shader.Find("Sprites/Fill"));
                        matFull.SetFloat("_FillAmount", 1);
                        rendererFull.material = matFull;
                        fullHeartMaterials[index] = matFull;
                    }
                }
            }

            if (existingEmptyHeart != null)
            {
                existingEmptyHeart.gameObject.SetActive(true);
                existingEmptyHeart.localPosition = heartPosition;
                SpriteRenderer rendererEmpty = existingEmptyHeart.GetComponent<SpriteRenderer>();
                if (rendererEmpty != null)
                {
                    rendererEmpty.sprite = emptyHeartSprite;
                    rendererEmpty.sortingOrder = 0;
                    emptyHeartRenderers[index] = rendererEmpty;
                }
            }
        }
        else
        {
            GameObject heartFull = new GameObject($"HeartFull_{index}");
            if (heartFull != null && fullHeartsContainer != null)
            {
                heartFull.transform.SetParent(fullHeartsContainer.transform, false);
                heartFull.transform.localPosition = heartPosition;
                SpriteRenderer rendererFull = heartFull.AddComponent<SpriteRenderer>();
                rendererFull.sprite = fullHeartSprite;
                rendererFull.sortingOrder = 1;

                if (!Application.isPlaying)
                {
                    Material matFull = new Material(Shader.Find("Sprites/Fill"));
                    matFull.SetFloat("_FillAmount", 1);
                    rendererFull.sharedMaterial = matFull;
                    fullHeartMaterials[index] = matFull;
                }
                else
                {
                    Material matFull = new Material(Shader.Find("Sprites/Fill"));
                    matFull.SetFloat("_FillAmount", 1);
                    rendererFull.material = matFull;
                    fullHeartMaterials[index] = matFull;
                }
                
                fullHeartRenderers[index] = rendererFull;
            }

            GameObject heartEmpty = new GameObject($"HeartEmpty_{index}");
            if (heartEmpty != null && emptyHeartsContainer != null)
            {
                heartEmpty.transform.SetParent(emptyHeartsContainer.transform, false);
                heartEmpty.transform.localPosition = heartPosition;
                SpriteRenderer rendererEmpty = heartEmpty.AddComponent<SpriteRenderer>();
                rendererEmpty.sprite = emptyHeartSprite;
                rendererEmpty.sortingOrder = 0;
                
                emptyHeartRenderers[index] = rendererEmpty;
            }
        }
#else
        // 런타임 코드는 그대로 유지
        GameObject heartFull = new GameObject($"HeartFull_{index}");
        if (heartFull != null && fullHeartsContainer != null)
        {
            heartFull.transform.SetParent(fullHeartsContainer.transform, false);
            heartFull.transform.localPosition = new Vector3(index * 0.5f, 0, 0);
            SpriteRenderer rendererFull = heartFull.AddComponent<SpriteRenderer>();
            rendererFull.sprite = fullHeartSprite;
            rendererFull.sortingOrder = 1;

            Material matFull = new Material(Shader.Find("Sprites/Fill"));
            matFull.SetFloat("_FillAmount", 1);
            rendererFull.material = matFull;
            
            fullHeartRenderers[index] = rendererFull;
            fullHeartMaterials[index] = matFull;
        }
#endif

        ApplyFillDirection();

        valuePerImage = baseHealth / imagesAmount;

        if ((index + 1) * valuePerImage > currentHealth)
        {
            float temp = (index + 1) * valuePerImage - currentHealth;
            float value = 1 - temp / valuePerImage;
            if (fullHeartMaterials[index] != null)
                fullHeartMaterials[index].SetFloat("_FillAmount", value);
        }
        
        if (index * valuePerImage >= currentHealth)
        {
            fullHeartMaterials[index].SetFloat("_FillAmount", 0);
        }
    }

    private void ApplyFillDirection()
    {
        switch (fillDirection)
        {
            case Health.FillDirection.LeftToRight:
                transform.localScale = new Vector3(1, 1, 1);
                transform.localRotation = Quaternion.identity;
                break;
            case Health.FillDirection.RightToLeft:
                transform.localScale = new Vector3(-1, 1, 1);
                transform.localRotation = Quaternion.identity;
                break;
            case Health.FillDirection.TopToBottom:
                transform.localScale = new Vector3(1, 1, 1);
                transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case Health.FillDirection.BottomToTop:
                transform.localScale = new Vector3(1, 1, 1);
                transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
        }
    }

    /// <summary>
    /// Update image sprites according to current health amount
    /// </summary>
    private void UpdateHealth()
    {
        if (fullHeartRenderers == null || fullHeartMaterials == null)
        {
            InitializeArrays();
            return;
        }

        valuePerImage = baseHealth / imagesAmount;
        
        for (int i = 0; i < imagesAmount; i++)
        {
            if (fullHeartRenderers[i] == null || fullHeartMaterials[i] == null) continue;

            float heartStartHealth = i * valuePerImage;
            float heartEndHealth = (i + 1) * valuePerImage;
            float fillAmount;

            if (currentHealth >= heartEndHealth)
            {
                fillAmount = 1f;
            }
            else if (currentHealth <= heartStartHealth)
            {
                fillAmount = 0f;
            }
            else
            {
                fillAmount = (currentHealth - heartStartHealth) / valuePerImage;
            }
            
            if (!Application.isPlaying)
            {
                if (fullHeartRenderers[i].sharedMaterial != null)
                    fullHeartRenderers[i].sharedMaterial.SetFloat("_FillAmount", fillAmount);
            }
            else
            {
                fullHeartMaterials[i].SetFloat("_FillAmount", fillAmount);
            }
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
        InitializeArrays();
        Init(imagesAmount);
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
        Init(imagesAmount);
    }
}
