using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Container : MonoBehaviour
{
    public GridTile GridTilePrefab { get; set; }

    public event Action<Container> OnClicked;
    public event Action<Container> OnDragged;
    public event Action<Container> OnMouseRaised;
    public GridContainer GridContainer { get; set; }

    private List<GridTile> ports;

    private void Start() {
        ports = new List<GridTile>();
    }

    public static Container InstantiateContainer(GridContainer c) {
        var container = new GameObject();
        container.name = "Container";
        container.transform.localScale = new Vector3(c.OuterWidth, c.OuterHeight, 1);

        container.AddComponent<SpriteRenderer>();

        BoxCollider bc = container.AddComponent<BoxCollider>();
        bc.isTrigger = true;

        Container containerController = container.AddComponent<Container>();
        containerController.GridContainer = c;

        containerController.InstantiatePorts();

        return containerController;
    }

    private void InstantiatePorts() {
        foreach (IPort port in GridContainer.Grid.GetPorts()) {
            if (!GridContainer.GetPortOuterPlacement(port, out int gridX, out int gridY))
                continue;

            float relativeX = gridX * 1f;
            float relativeY = gridY * 1f;

            GridTile tile = Instantiate(GridTilePrefab, transform);
            tile.SetTopLeft(new Vector3(relativeX, relativeY, -0.1f));
            tile.ShowColor(State.WireOn);

            ports.Add(tile);
        }
    }

    public void SetTopLeft(Vector3 topLeft) {
        Vector3 scale = transform.localScale;
        (float w, float h) = (scale.x, scale.y);
        transform.position = new Vector3(topLeft.x + w / 2, topLeft.y - h / 2, topLeft.z);
    }

    private void OnMouseDown() {
        OnClicked?.Invoke(this);
    }

    private void OnMouseDrag() {
        OnDragged?.Invoke(this);
    }

    private void OnMouseUp() {
        OnMouseRaised?.Invoke(this);
    }
}
