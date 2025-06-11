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

    private Coroutine checkCoroutine;

    private void OnEnable()
    {
        checkCoroutine = StartCoroutine(CheckPlayerInField());
    }

    private void OnDisable()
    {
        if (checkCoroutine != null)
            StopCoroutine(checkCoroutine);
    }

    private IEnumerator CheckPlayerInField()
    {
        while (true)
        {
            yield return CoroutineUtil.WaitForSeconds(checkInterval);
            while (GameManager.instance.GetGameState() != GameState.Field)
            {
                yield return CoroutineUtil.WaitForSeconds(checkInterval);
            }

            Collider[] t_hits = Physics.OverlapBox(
                center: transform.position,
                halfExtents: fieldSize * 0.5f,
                orientation: transform.rotation,
                layerMask: playerLayer
            );

            while (GameManager.instance.GetGameState() != GameState.Field)
            {
                yield return CoroutineUtil.WaitForSeconds(checkInterval);
            }

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