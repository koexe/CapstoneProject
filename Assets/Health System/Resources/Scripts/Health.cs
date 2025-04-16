using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health class is used as a bridge between health preferences and your game logic
/// </summary>
public class Health : MonoBehaviour
{
    public static Health instance;

    private HealthPreferences prefs;
    private Pulse pulse;

    void Start()
    {
        instance = this;
        prefs = GetComponent<HealthPreferences>();
        pulse = GetComponent<Pulse>();
    }

    /// <summary>
    /// Return current health amount
    /// </summary>
    public float GetCurrentHealth()
    {
        return prefs.GetCurrentHealth();
    }

    /// <summary>
    /// Set current health amount
    /// </summary>
    public void SetCurrentHealth(int currentHealth)
    {
        prefs.SetCurrentHealth(currentHealth);
    }

    /// <summary>
    /// Return base health amount
    /// </summary>
    public float GetTotalHealth()
    {
        return prefs.GetTotalHealth();
    }

    /// <summary>
    /// Set base health amount
    /// </summary>
    public void SetTotalHealth(float amount)
    {
        prefs.SetTotalHealth(amount);
    }

    /// <summary>
    /// Decrease health per amount
    /// </summary>
    public void AddDamage(float damage)
    {
        prefs.AddDamage(damage);
    }

    /// <summary>
    /// Increase health per amount
    /// </summary>
    public void AddHeal(float heal)
    {
        prefs.AddHeal(heal);
    }

    /// <summary>
    /// Enable health increasing for value per time
    /// </summary>
    public void EnableRegeneration(bool enabled, float amount = 1, float delay = 1)
    {
        if (enabled)
            prefs.EnableRegeneration(true, amount, delay);
        else
            prefs.EnableRegeneration(false);
    }

    /// <summary>
    /// Enable health decreasing for value per time
    /// </summary>
    public void EnablePoison(bool enabled, float amount = 1, float delay = 1)
    {
        if (enabled)
            prefs.EnablePoison(true, amount, delay);
        else
            prefs.EnablePoison(false);
    }

    /// <summary>
    /// Make health object pulse
    /// </summary>
    public void EnablePulse(bool enabled, float threshold = 0)
    {
        if (enabled)
            pulse.Run(threshold);
        else
            pulse.Stop();
    }

    /// <summary>
    /// Enable invincible mode
    /// </summary>
    public void EnableInvincibility(bool enabled)
    {
        prefs.EnableInvincibility(enabled);
    }

    /// <summary>
    /// Set amount of health images
    /// </summary>
    public void SetImagesAmount(int amount)
    {
        prefs.Init(amount);
    }

    /// <summary>
    /// Add one health image
    /// </summary>
    public void AddImage()
    {
        prefs.AddImage();
    }

    /// <summary>
    /// Remove one health image
    /// </summary>
    public void RemoveImage()
    {
        prefs.RemoveImage();
    }

    /// <summary>
    /// Set images fill method
    /// </summary>
    public void SetFillType(Image.FillMethod type) {
        prefs.SetFillType(type);
    }

    /// <summary>
    /// Reset all settings to default
    /// </summary>
    public void Reset()
    {
        prefs.Reset();
    }
}
