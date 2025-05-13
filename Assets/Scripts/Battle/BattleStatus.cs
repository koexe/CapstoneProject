using UnityEngine;

[System.Serializable]
public class BattleStatus
{

    [SerializeField] int level;
    [SerializeField] int exp;
    [SerializeField] float baseHp;
    [SerializeField] float baseMp;
    [SerializeField] float baseAtk;
    [SerializeField] float baseDef;
    [SerializeField] float baseSpeed;
    const int maxLevel = 50;

    public BattleStatus(BattleStatus _copy)
    {
        this.level = _copy.level;
        this.exp = _copy.exp;
        this.baseHp = _copy.baseHp;
        this.baseMp = _copy.baseMp;
        this.baseAtk = _copy.baseAtk;
        this.baseDef = _copy.baseDef;
        this.baseSpeed = _copy.baseSpeed;
    }


    public int GetHp()
    {
        float t_result = this.baseHp + ((this.level - 1) * 1.5f);
        return (int)t_result;
    }

    public int GetMp()
    {
        float t_result = this.baseHp + ((this.level - 1) * 0.6f);
        return (int)t_result;
    }

    public int GetAtk()
    {
        float t_result = this.baseHp + ((this.level - 1) * 0.6f);
        return (int)t_result;
    }

    public int GetDef()
    {
        float t_result = this.baseHp + ((this.level - 1) * 0.3f);
        return (int)t_result;
    }

    public int GetSpd()
    {
        float t_result = this.baseHp + ((this.level - 1) * 0.3f);
        return (int)t_result;
    }

    public bool GainExp(int _exp)
    {
        this.exp += _exp;
        bool t_levelUpOccurred = false;

        while (this.level < maxLevel)
        {
            int t_requiredExp = GetRequiredExp(this.level);
            if (this.exp >= t_requiredExp)
            {
                this.exp -= t_requiredExp;
                this.level++;
                t_levelUpOccurred = true;
            }
            else
            {
                break;
            }
        }

        // 만렙이면 초과 경험치 잘라주기 (선택사항)
        if (this.level >= maxLevel)
        {
            this.exp = 0;
        }

        return t_levelUpOccurred;
    }

    private int GetRequiredExp(int _level)
    {
        if (_level <= 10)
        {
            return 40 + (_level - 1) * 15;
        }
        else
        {
            return (int)(40 * Mathf.Pow(_level, 1.6f));
        }
    }

}