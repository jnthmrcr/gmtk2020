using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] float inputInertiaLerp = 5f;
    [SerializeField] float cameraTrackingLerp = 20f;

    Vector2 camPosition;

    Vector2 input;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire2"))
		{
            input = new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
		} else
		{
            input = Vector2.Lerp(input, Vector2.zero, Time.deltaTime * inputInertiaLerp);
		}

        camPosition += input;
        transform.position = Vector3.Lerp(transform.position, camPosition.toV3(35f), Time.deltaTime * cameraTrackingLerp);
    }
}
