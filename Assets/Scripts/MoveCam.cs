using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveKeys : MonoBehaviour
{
    [SerializeField]private float speed = 3.0f;
    [SerializeField]private float scrollSpeed = 10.0f;

    private Camera zoom;

    void Start() {
        zoom = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(x, y, 0);
        movement = Vector3.ClampMagnitude(movement, 1);

        transform.Translate(movement * speed * Time.deltaTime);

        if (zoom.orthographic) {
            zoom.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        } else {
            zoom.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        }
    }
}
