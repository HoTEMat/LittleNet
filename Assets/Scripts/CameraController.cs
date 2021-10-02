using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridHolder))]
class CameraController : MonoBehaviour
{
    public Camera Camera;
    private GridHolder gridHolder;

    private bool initializedToCenter = false;
    private Vector2 dragStartPosition;
    private Vector2 dragLastPosition;
    private bool isDragging = false;

    private void Start() {
        gridHolder = GetComponent<GridHolder>();
    }

    private void InitializeToCenter() {
        Vector3 gridCenter = gridHolder.GetGridCenter();
        Camera.transform.position = new Vector3(gridCenter.x, gridCenter.y, Camera.transform.position.z);
        initializedToCenter = true;
    }

    private void Update() {
        if (!initializedToCenter) {
            InitializeToCenter();
            return;
        }

        if (Input.GetMouseButtonDown(1)) {
            dragStartPosition = Input.mousePosition;
            dragLastPosition = dragStartPosition;
            isDragging = true;
        }

        if (Input.GetMouseButton(1) && isDragging) {
            Vector2 delta = (Vector2)Input.mousePosition - dragLastPosition;
            dragLastPosition = Input.mousePosition;

            if (delta != Vector2.zero) {
                Camera.transform.position -= (Camera.ScreenToWorldPoint(delta) - Camera.ScreenToWorldPoint(Vector2.zero));
            }
        }

        if (Input.GetMouseButtonUp(1) && isDragging) {
            isDragging = false;
        }
    }
}
