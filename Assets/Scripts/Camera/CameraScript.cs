using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private bool is3D = true;

    public Vector3 offset;
    public float sensitivityZoom = 3; // чувствительность мышки
    public float limit = 90; // ограничение вращения по Y

    public float zoom = 5f; // чувствительность при увеличении, колесиком мышки
    public float zoomMax = 150; // макс. увеличение
    public float zoomMin = 70; // мин. увеличение

    public float sensitivitySize = 1;
    public float sensetivityMouse2D = 10f;

    public float size = 60f;
    public float minimumSize = 50f;
    public float maximumSize = 150f;

    private float X, Y;


    public Vector3 start2DPosition;


    void Start()
    {
        Is3D = is3D;
    }

    void Update()
    {
        if (is3D)
        {
            update3D();
        }
        else
        {
            update2D();
        }
    }

    private void update3D()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoom;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoom;
        offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));

        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityZoom;
            Y += Input.GetAxis("Mouse Y") * sensitivityZoom;
            Y = Mathf.Clamp(Y, -limit, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        transform.localEulerAngles = new Vector3(-Y, X, 0);
        transform.position = transform.localRotation * offset;
    }

    private void update2D()
    {
        Camera camera = GetComponent<Camera>();
        float z = camera.orthographicSize;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) z -= sensitivitySize;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) z += sensitivitySize;

        camera.orthographicSize = Mathf.Clamp(z, minimumSize, maximumSize);

        float x = transform.position.x;
        float y = transform.position.z;

        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            x -= Input.GetAxis("Mouse X") * sensetivityMouse2D * camera.orthographicSize;
            y -= Input.GetAxis("Mouse Y") * sensetivityMouse2D * camera.orthographicSize;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        x = Mathf.Clamp(x, -camera.orthographicSize * camera.aspect + 45, camera.orthographicSize * camera.aspect - 45);
        y = Mathf.Clamp(y, -camera.orthographicSize + 45, camera.orthographicSize - 45);
        transform.position = new Vector3(x, transform.position.y, y);
    }

    public bool Is3D
    {
        get
        {
            return is3D;
        }
        set
        {
            Camera camera = GetComponent<Camera>();
            if (!value)
            {
                transform.position = start2DPosition;
                transform.eulerAngles = new Vector3(90, 0, 0);
                camera.orthographic = true;
                camera.orthographicSize = size;
            }
            else
            {

                limit = Mathf.Abs(limit);
                if (limit > 90) limit = 90;
                offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax) / 2);
                transform.position = offset;
                camera.orthographic = false;
            }
        }
    }
}
