using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFollow : MonoBehaviour
{
    public Transform CarTransform;
    public Transform CameraPos;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        
    }

    void Update()
    {
        transform.LookAt(CarTransform);
        transform.position = Vector3.SmoothDamp(transform.position,CameraPos.position,ref velocity,5f*Time.deltaTime);        
    }
}
