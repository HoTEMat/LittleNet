using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridHolder))]
class ContainerMovement : MonoBehaviour
{
    [SerializeField] private Sprite WhiteSprite;
    [SerializeField] private Color ContainerColor;
    [SerializeField] private Camera Camera;
    [SerializeField] private GridTile GridTilePrefab;

    private GridHolder grid;
    private List<Container> containers;
    private bool lateStarted;

    private void Start() {
        grid = GetComponent<GridHolder>();
        lateStarted = false;
    }

    // Needs to be executed after GridHolder's Start().
    private void LateStart() {
        containers = new List<Container>();
        var clickHandler = new Action<Container>(HandleContainerClicked);
        var dragHandler = new Action<Container>(HandleContainerDragged);
        var mouseUpHandler = new Action<Container>(HandleMouseUp);
        foreach (GridContainer c in grid.Level.Grid.GetContainers()) {
            Container container = Container.InstantiateContainer(c);
            container.OnClicked += clickHandler;
            container.OnDragged += dragHandler;
            container.OnMouseRaised += mouseUpHandler;
            container.transform.parent = transform;
            container.GridTilePrefab = GridTilePrefab;
            var sr = container.GetComponent<SpriteRenderer>();
            sr.color = ContainerColor;
            sr.sprite = WhiteSprite;
            containers.Add(container);
        }
        UpdateContainersPositions();
        lateStarted = true;
    }

    private void Update() {
        if (!lateStarted) {
            LateStart();
        }

        UpdateContainersPositions();
    }

    private void UpdateContainersPositions() {
        foreach (Container c in containers) {
            c.SetTopLeft(grid.CoordinateToPosition(c.GridContainer.X, c.GridContainer.Y, -1));
        }
    }

    private Container draggedContainer;
    private Vector3 draggingMouseOffset;
    private bool dragging => draggedContainer != null;

    private void HandleContainerClicked(Container c) {
        Vector3 worldMousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 containerWorldPos = grid.CoordinateToPosition(c.GridContainer.X, c.GridContainer.Y, 0);
        draggedContainer = c;
        draggingMouseOffset = worldMousePos - containerWorldPos;
    }
    private void HandleContainerDragged(Container c) {
        if (!dragging)
            return;

        Vector3 worldMousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 containerTargetWorldPos = worldMousePos - draggingMouseOffset;

        Vector3 containerWorldPos = grid.CoordinateToPosition(c.GridContainer.X, c.GridContainer.Y, 0);
        if ((containerWorldPos - containerTargetWorldPos).magnitude < 0.2)
            return;

        (int containerTargetX, int containerTargetY) = grid.PositionToCoordinate(containerTargetWorldPos);
        if (c.GridContainer.X != containerTargetX || c.GridContainer.Y != containerTargetY) {
            grid.Level.Grid.RemoveContainer(c.GridContainer);
            c.GridContainer.X = containerTargetX;
            c.GridContainer.Y = containerTargetY;
            grid.Level.Grid.InsertContainer(c.GridContainer);
        }
    }

    private void HandleMouseUp(Container c) {
        draggedContainer = null;
        draggingMouseOffset = Vector3.zero; // for determinism
    }
}
