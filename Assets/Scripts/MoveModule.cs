using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveModule : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] float speed = 10;
    void Update()
    {
        Vector3 t_currentPosition = transform.localPosition;
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            t_currentPosition.x -=  Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            t_currentPosition.x += Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            t_currentPosition.z -= Time.deltaTime * speed; 
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            t_currentPosition.z += Time.deltaTime * speed;
        }
        this.transform.localPosition = t_currentPosition;
        
    }
}
