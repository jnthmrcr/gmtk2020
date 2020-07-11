using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] float inputInertiaLerp = 5f;
    [SerializeField] float cameraTrackingLerp = 20f;
    [SerializeField] float cameraZoomSensitivity = 2.5f;

    Vector2 camPosition;
    float camZoom = 0;

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
            input = new Vector2(-Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
		} else
		{
            input = Vector2.Lerp(input, Vector2.zero, Time.deltaTime * inputInertiaLerp);
		}

        camZoom -= Input.GetAxisRaw("Mouse ScrollWheel") * cameraZoomSensitivity;

        camPosition += input;
        transform.position = Vector3.Lerp(transform.position, camPosition.toV3(35f + camZoom), Time.deltaTime * cameraTrackingLerp);
    }
}
