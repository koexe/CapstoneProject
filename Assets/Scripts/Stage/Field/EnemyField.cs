using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyField : MonoBehaviour
{
    [Header("설정값")]
    [SerializeField] int[] enemyPool;
    [SerializeField] Vector3 fieldSize = new Vector3(5, 2, 5);
    [SerializeField] float checkInterval = 2f;
    [SerializeField] float encounterChance = 0.2f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] int minLevel;
    [SerializeField] int maxLevel;
    
    [Header("쿨다운 설정")]
    [SerializeField] float initialCooldown = 3f; // 씬 전환 후 초기 쿨다운
    [SerializeField] float encounterCooldown = 5f; // 조우 후 쿨다운

    private Coroutine checkCoroutine;
    private float lastEncounterTime = -100f; // 마지막 조우 시간
    private bool isSceneJustLoaded = true; // 씬이 방금 로드되었는지 확인
    private float enableTimestamp; // OnEnable이 호출된 시간

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        enableTimestamp = Time.time; // OnEnable 호출 시간 기록
        isSceneJustLoaded = true;
        checkCoroutine = StartCoroutine(CheckPlayerInField());
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        if (checkCoroutine != null)
            StopCoroutine(checkCoroutine);
    }

    private IEnumerator CheckPlayerInField()
    {
        // OnEnable 시점부터 initialCooldown 시간이 지날 때까지 대기
        while (Time.time - enableTimestamp < initialCooldown)
        {
            yield return CoroutineUtil.WaitForFixedUpdate;
        }
        isSceneJustLoaded = false;
        
        while (true)
        {
            yield return CoroutineUtil.WaitForSeconds(checkInterval);
            
            // 게임 상태가 Field가 아니면 대기
            while (GameManager.instance.GetGameState() != GameState.Field)
            {
                yield return CoroutineUtil.WaitForSeconds(checkInterval);
            }

            // 조우 쿨다운 체크
            if (Time.time - lastEncounterTime < encounterCooldown)
            {
                continue;
            }

            Collider[] t_hits = Physics.OverlapBox(
                center: transform.position,
                halfExtents: fieldSize * 0.5f,
                orientation: transform.rotation,
                layerMask: playerLayer
            );

            if (t_hits.Length > 0)
            {
                float t_roll = Random.value;
                if (t_roll <= encounterChance)
                {
                    TriggerEncounter();
                }
            }
        }
    }

    private void TriggerEncounter()
    {
        lastEncounterTime = Time.time; // 마지막 조우 시간 갱신
        
        int t_randomIndex = Random.Range(0, enemyPool.Length);
        int t_enemyID = enemyPool[t_randomIndex];

        Debug.Log($"[OverlapBox] 조우된 Enemy ID: {t_enemyID}");

        int t_enemyCount = Random.Range(1, enemyPool.Length);

        SOBattleCharacter[] t_enemy = new SOBattleCharacter[t_enemyCount];
        int[] t_level = new int[t_enemyCount];

        for (int i = 0; i < t_enemy.Length; i++)
        {
            t_enemy[i] = DataLibrary.instance.GetSOCharacter(t_enemyID);
            t_level[i] = Random.Range(this.minLevel, this.maxLevel);
        }

        GameManager.instance.ChangeSceneFieldToBattle(t_enemy, t_level);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, fieldSize);
    }
}