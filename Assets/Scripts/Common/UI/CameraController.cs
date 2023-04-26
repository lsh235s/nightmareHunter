using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 5.0f;


    [SerializeField]
    private GameObject player;

    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / (float)Screen.height) / ((float)16 / 9);
        float scalewidth = 1f / scaleheight;

        if(scaleheight < 1) {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        } else {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        camera.rect = rect;
    }

    private void Update()
    {
        if(player != null) {
            Vector3 dir = player.transform.position - this.transform.position;
            Vector3 moveVector = new Vector3(dir.x * cameraSpeed * Time.deltaTime, dir.y * cameraSpeed * Time.deltaTime, 0.0f);
            this.transform.Translate(moveVector);
        }
    }
}
