using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nameplate : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float fixedHeight = -2247.2f;
    private Camera camera;

    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = target.transform.position + offset;
        Vector3 targetPosOffset = new Vector3(targetPos.x, fixedHeight, targetPos.z);
        transform.position = targetPosOffset;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, camera.transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
