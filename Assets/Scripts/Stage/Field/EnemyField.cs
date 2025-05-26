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
    [SerializeField] float minLevel;
    [SerializeField] float maxLevel;

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
        int t_randomIndex = Random.Range(0, enemyPool.Length);
        int t_enemyID = enemyPool[t_randomIndex];

        Debug.Log($"[OverlapBox] 조우된 Enemy ID: {t_enemyID}");

        SOBattleCharacter[] t_enemy = new SOBattleCharacter[Random.Range(1, 3)]; 

        for(int i  = 0; i < t_enemy.Length; i++)
        {
            t_enemy[i] = DataLibrary.instance.GetSOCharacter(t_enemyID);
        }

        GameManager.instance.ChangeSceneFieldToBattle(t_enemy);

        // TODO: 전투 씬 진입 또는 조우 연출
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, fieldSize);
    }
}