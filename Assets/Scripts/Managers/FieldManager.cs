using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    public void Initialization()
    {
        if (GameManager.instance.GetPlayer() == null)
        {
            var t_player = Instantiate(this.playerPrefab, Vector3.zero, Quaternion.identity, MapManager.instance.transform);
            GameManager.instance.SetPlayer(t_player);
            GameManager.instance.GetCamera().SetTarget(t_player.transform);
        }
    }
}
