using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridHolder))]
public class ContainerMovement : MonoBehaviour
{
    [SerializeField] private Sprite WhiteSprite;
    [SerializeField] private Color ContainerColor;

    private GridHolder grid;
    private List<GameObject> containers;
    private bool lateStarted;

    private void Start() {
        grid = GetComponent<GridHolder>();
        lateStarted = false;
    }

    // Needs to be executed after GridHolder's Start().
    private void LateStart() {
        containers = new List<GameObject>();
        foreach (GridContainer c in grid.Level.Grid.GetContainers()) {
            GameObject containerObj = InstantiateContainer(c.X, c.Y, c.OuterWidth, c.OuterHeight);
            containers.Add(containerObj);
        }
        lateStarted = true;
    }

    private void Update() {
        if (!lateStarted) {
            LateStart();
        }
    }

    private GameObject InstantiateContainer(int gridX, int gridY, float width, float height) {
        var container = new GameObject();
        container.name = "Container";
        container.transform.parent = transform;
        container.transform.localScale = new Vector3(width, height, 1);

        SpriteRenderer sr = container.AddComponent<SpriteRenderer>();
        sr.color = ContainerColor;
        sr.sprite = WhiteSprite;

        Container containerController = container.AddComponent<Container>();
        containerController.SetTopLeft(grid.GridToWorldPosition(gridX, gridY, -1));

        return container;
    }
}
