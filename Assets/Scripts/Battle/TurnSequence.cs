using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class TurnSequence
{
    public enum BattleSequenceType
    {
        InitializeSequence,
        ChooseSequence,
        ExecuteSequence,
        SummarySequence,
        EndSequence,
    }

    protected Action BeforeSequence;
    protected Action AfterSequence;
    protected BattleManager battleManager;
    protected BattleSequenceType sequenceType;



    public SequenceState currentState = SequenceState.Initialize;
    public TurnSequence(BattleManager _battleManager, Action _beforeAction = null, Action _afterAction = null)
    {
        this.BeforeSequence = null;
        this.AfterSequence = null;

        this.currentState = SequenceState.BeforeAction;
        this.battleManager = _battleManager;

        this.BeforeSequence += _beforeAction;
        this.AfterSequence += _afterAction;

    }
    public virtual void SequenceUpdate()
    {

    }

    public virtual async void SequenceAction()
    {
        this.currentState = SequenceState.InAction;

    }

    public void ExecuteBeforeAction()
    {
        BeforeSequence?.Invoke();
        this.currentState = SequenceState.InAction;
    }
    public void ExecuteAfterAction()
    {
        AfterSequence?.Invoke();
        this.currentState = SequenceState.Done;
    }

    public enum SequenceState
    {
        Initialize,
        BeforeAction,
        InAction,
        AfterAction,
        Done,
    }
}

[System.Serializable]
public class InitializeSequence : TurnSequence
{

    public InitializeSequence(BattleManager _battleManager, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.sequenceType = BattleSequenceType.InitializeSequence;

        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }

    public override async void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("초기화 진행중");
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        this.battleManager.NextSequence();
    }
}

[System.Serializable]
public class ChooseSequence : TurnSequence
{
    public enum ChooseState
    {
        SelectSkill,
        SelectEnemy,
        None,
    }
    BattleCharacterBase[] players;
    BattleCharacterBase[] enemys;

    public ChooseState state;

    int currentPlayerIndex;

    int currentEnemyIndex;
    public ChooseSequence(BattleManager _battleManager, BattleCharacterBase[] _players, BattleCharacterBase[] _enemys, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.players = _players;
        this.sequenceType = BattleSequenceType.ChooseSequence;
        this.state = ChooseState.None;
        this.enemys = _enemys;
        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }

    public override void SequenceUpdate()
    {
        base.SequenceUpdate();

        if (this.state == ChooseState.None)
        {
            ShadowUpdate(LayerMask.GetMask("Player"));
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.currentPlayerIndex += 1;
                if (this.currentPlayerIndex >= this.players.Length) this.currentPlayerIndex = 0;
                GameManager.instance.GetCamera().SetTarget(this.players[this.currentPlayerIndex].transform);
                this.battleManager.SetSelectedCharacter(this.players[this.currentPlayerIndex]);

            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.currentPlayerIndex -= 1;
                if (this.currentPlayerIndex < 0) this.currentPlayerIndex = 1;
                GameManager.instance.GetCamera().SetTarget(this.players[this.currentPlayerIndex].transform);
                this.battleManager.SetSelectedCharacter(this.players[this.currentPlayerIndex]);
            }


            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.parent.TryGetComponent<BattleCharacterBase>(out var t_character))
                    {
                        if (this.players.Contains(t_character))
                        {
                            this.currentPlayerIndex = System.Array.IndexOf(this.players, t_character);
                            GameManager.instance.GetCamera().SetTarget(hit.transform);
                            this.battleManager.SetSelectedCharacter(t_character);
                        }
                    }
                }
            }
        }
        else if (this.state == ChooseState.SelectSkill)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.battleManager.HidePlayerAction();
                this.state = ChooseState.None;
            }
        }
        else if (this.state == ChooseState.SelectEnemy)
        {
            ShadowUpdate(LayerMask.GetMask("Enemy"));
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.currentEnemyIndex += 1;
                if (this.currentEnemyIndex >= this.enemys.Length) this.currentEnemyIndex = 0;
                GameManager.instance.GetCamera().SetTarget(this.enemys[this.currentEnemyIndex].transform);
                this.battleManager.SetSelectedCharacter(this.enemys[this.currentEnemyIndex]);

            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.currentEnemyIndex -= 1;
                if (this.currentEnemyIndex < 0) this.currentEnemyIndex = this.enemys.Length - 1;
                GameManager.instance.GetCamera().SetTarget(this.enemys[this.currentEnemyIndex].transform);
                this.battleManager.SetSelectedCharacter(this.enemys[this.currentEnemyIndex]);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.players[this.currentPlayerIndex].GetSelectedSkill().target = new BattleCharacterBase[] { this.enemys[this.currentEnemyIndex] };
                GameManager.instance.GetCamera().SetTarget(null);
                this.state = ChooseState.None;
                this.battleManager.CheckAllReady();
            }
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.parent.TryGetComponent<BattleCharacterBase>(out var t_character))
                    {
                        if (this.enemys.Contains(t_character))
                        {
                            GameManager.instance.GetCamera().SetTarget(hit.transform);

                            this.currentEnemyIndex = System.Array.IndexOf(this.players, t_character);

                            this.players[this.currentPlayerIndex].GetSelectedSkill().target = new BattleCharacterBase[] { t_character };
                            GameManager.instance.GetCamera().SetTarget(null);
                            this.battleManager.SetChooseSequenceState(ChooseState.None);
                            this.battleManager.SetAllyArrow(false);
                            this.battleManager.SetEnemyArrow(false);
                            this.battleManager.CheckAllReady();
                        }

                    }
                }
            }
        }

    }

    void ShadowUpdate(LayerMask _layerMask)
    {
        // 모든 그림자 초기화
        foreach (var player in players)
        {
            player.SetShadow(false);
        }
        foreach (var enemy in enemys)
        {
            enemy.SetShadow(false);
        }

        // 마우스 호버 체크
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(mouseRay, Mathf.Infinity, _layerMask);

        foreach (var hit in hits)
        {
            var parent = hit.transform;
            while (parent != null)
            {
                if (parent.TryGetComponent<BattleCharacterBase>(out var character))
                {
                    if ((state == ChooseState.None && players.Contains(character)) ||
                        (state == ChooseState.SelectEnemy && enemys.Contains(character)))
                    {
                        character.SetShadow(true);
                        break;
                    }
                }
                parent = parent.parent;
            }
        }
    }

    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("플레이어 행동 선택");
    }
}
[System.Serializable]
public class ExecuteSequence : TurnSequence
{
    List<BattleCharacterBase> battleCharacters;
    public ExecuteSequence(BattleManager _battleManager, List<BattleCharacterBase> _battleCharacters, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.battleCharacters = _battleCharacters;

        this.sequenceType = BattleSequenceType.ChooseSequence;

        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }
    public override async void SequenceAction()
    {
        base.SequenceAction();
        await ActionTask();
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        this.battleManager.NextSequence();
    }
    public async UniTask ActionTask()
    {
        for (int i = 0; i < battleCharacters.Count; i++)
        {
            if (battleCharacters[i].GetAction() == CharacterActionType.Defence)
            {
                GameStatics.Swap(battleCharacters, i, 0);
            }
        }

        foreach (var t_battleCharacter in battleCharacters)
        {
            if (!t_battleCharacter.IsDie())
                await t_battleCharacter.StartAction();
        }

        battleManager.NextSequence();

    }
}

[System.Serializable]
public class SummarySequence : TurnSequence
{
    List<BattleCharacterBase> battleCharacters;
    public SummarySequence(BattleManager _battleManager, List<BattleCharacterBase> _battleCharacters, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.battleCharacters = _battleCharacters;
        this.sequenceType = BattleSequenceType.SummarySequence;

        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }
    public override async void SequenceAction()
    {
        base.SequenceAction();
        await SummaryTask();
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        this.battleManager.NextSequence();
    }
    async UniTask SummaryTask()
    {
        foreach (var t_battleCharacter in battleCharacters)
        {
            if (!t_battleCharacter.IsDie())
                await t_battleCharacter.Summary();
        }

        battleManager.NextSequence();


    }
}

[System.Serializable]
public class EndSequence : TurnSequence
{
    List<BattleCharacterBase> battleCharacters;
    public EndSequence(BattleManager _battleManager, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.sequenceType = BattleSequenceType.EndSequence;

        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }
    public override async void SequenceAction()
    {
        base.SequenceAction();
        if (this.battleManager.IsAllyAllDie())
        {
            //게임오버 작동 구현
        }
        else if (this.battleManager.IsEnemyAllDie())
        {
            await EndTask();
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            this.battleManager.NextSequence();
        }

    }
    async UniTask EndTask()
    {
        battleManager.ShowText("승리했다! 전투 종료!");
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        battleManager.ChangeToFieldScene();

    }
}