using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Demo class. Here you can get an example how to use this health system with your game
/// </summary>
public class Demo : MonoBehaviour
{
    public InputField heartsAmountInput;
    public InputField totalHealthInput;
    public InputField healDamageInput;
    public InputField pulseThresholdInput;
    public InputField poisonRegenValueInput;
    public InputField poisonRegenDelayInput;
    public Toggle poisonToggle;
    public Toggle regenToggle;
    public Toggle pulseToggle;
    public Toggle invincibilityToggle;
    public Text totalHealthText;
    public Text currentHealthText;

    private int value;
    private float value2;

    public void Update()
    {
        totalHealthText.text = "Total Health: " + Health.instance.GetTotalHealth();
        currentHealthText.text = "Current Health: " + Health.instance.GetCurrentHealth();
    }

    public void SetHeartsAmount()
    {
        int.TryParse(heartsAmountInput.text, out value);
        Health.instance.SetImagesAmount(value);  
    }

    public void SetTotalHealth()
    {
        int.TryParse(totalHealthInput.text, out value);
        Health.instance.SetTotalHealth(value);
    }

    public void HealDamage(bool heal)
    {
        int.TryParse(healDamageInput.text, out value);

        if (heal)
            Health.instance.AddHeal(value);
        else
            Health.instance.AddDamage(value);
    }

    public void Pulse()
    {
        int.TryParse(pulseThresholdInput.text, out value);
        bool enable = pulseToggle.isOn;
        Health.instance.EnablePulse(enable, value);
    }

    public void AddImage(bool add)
    {
        if (add)
            Health.instance.AddImage();
        else
            Health.instance.RemoveImage();
    }

    public void SetFillType(string type)
    {
        switch (type)
        {
            case "horizontal": Health.instance.SetFillType(Image.FillMethod.Horizontal); break;
            case "vertical": Health.instance.SetFillType(Image.FillMethod.Vertical); break;
            case "radial90": Health.instance.SetFillType(Image.FillMethod.Radial90); break;
            case "radial180": Health.instance.SetFillType(Image.FillMethod.Radial180); break;
            case "radial360": Health.instance.SetFillType(Image.FillMethod.Radial360); break;
        }
    }

    public void StartPoisonRegen(bool isRegen)
    {
        int.TryParse(poisonRegenValueInput.text, out value);
        float.TryParse(poisonRegenDelayInput.text, out value2);

        if (isRegen)
            if (regenToggle.isOn)
                Health.instance.EnableRegeneration(true, value, value2);
            else
                Health.instance.EnableRegeneration(false);
        else
            if (poisonToggle.isOn)
                Health.instance.EnablePoison(true, value, value2);
            else
                Health.instance.EnablePoison(false);
    }

    public void Invincibility()
    {
        bool enable = invincibilityToggle.isOn;
        Health.instance.EnableInvincibility(enable);
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
