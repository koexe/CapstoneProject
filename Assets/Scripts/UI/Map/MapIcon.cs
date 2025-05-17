using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIcon : MonoBehaviour
{
    [SerializeField] MapEntity mappingEntity;
    public string GetMapId()
    { 
        if(this.mappingEntity!= null)
        {
            return this.mappingEntity.GetID();    
        }
        else
        {
            LogUtil.Log($"No Mapping Entitiy! Check {this.name} Object");
            return null;
        }
    }
}
