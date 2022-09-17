using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float yaw;
    public float pitch;
    public float mouseSensitivity=10;
    public bool lockCursor;
    public Transform target;
    public float dstFromTarget=8;
    public Vector2 pitchMinMax = new Vector2(-30,85);
    public float pitchAdjustment = 0.7f;
    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    public bool freeze;
    public LayerMask mask;
    public Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        if(lockCursor){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Use mouse input to adjust rotation
        yaw+=Input.GetAxis("Mouse X")*mouseSensitivity;
        pitch-=Input.GetAxis("Mouse Y")*mouseSensitivity*pitchAdjustment; 
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;   
        transform.position = target.position - transform.forward * dstFromTarget;
        Vector2.moveTowards()
        // Ray casting to prevent peeking through walls
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        // if ray hits a layer included in the mask
        if(Physics.Raycast(ray, out hitInfo, 7, mask)) {
            // calculate new position from (hitinfo - target) *0.8f + target (target is what the camera is focusing on) 
            targetPosition = (hitInfo.point - target.transform.position) *  0.8f + target.transform.position;
            transform.position = targetPosition;
        }
    }
}
