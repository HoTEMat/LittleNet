using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GridHolder : MonoBehaviour {
    public GridTile GridTilePrefab;
    public GameObject GridLinePrefab;

    [SerializeField]
    private float gridLineWidth = 0.01f;
    private IGrid grid { get; set; }
    private GridTile[,] gridTiles;

    private static readonly IReadOnlyDictionary<State, Color> stateToColor = new Dictionary<State, Color> {
        { State.Nothing, Color.blue }
    };

    private void SetGrid(IGrid grid) {
        this.grid = grid;
        gridTiles = new GridTile[grid.Width, grid.Height];
        InitGridTiles();
    }

    private void InitGridTiles() {
        for (int gridX = 0; gridX < grid.Width; gridX++) {
            for (int gridY = 0; gridY < grid.Height; gridY++) {
                GridTile tile = Instantiate<GridTile>(GridTilePrefab, transform);
                tile.SetTopLeft(GridPositionToPosition(gridX, gridY, 0));
                gridTiles[gridX, gridY] = tile;
            }
        }
    }

    private Vector3 GridPositionToPosition(int gridX, int gridY, float z) {
        (float w, float h) = GetTileSize();
        return new Vector3((w + gridLineWidth) * gridX, -(h + gridLineWidth) * gridY, z);
    }

    private void Start() {
        SetGrid(new TestGrid());
    }

    private void Update() {
        UpdateGridTiles();
    }

    private void UpdateGridTiles() {
        for (int gridX = 0; gridX < grid.Width; gridX++) {
            for (int gridY = 0; gridY < grid.Height; gridY++) {
                State state = grid.Get(gridX, gridY);
                Color color = stateToColor[state];
                GridTile tile = gridTiles[gridX, gridY];

                tile.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
            }
        }
    }

    public (float width, float height) GetTileSize() {
        Vector3 tileScale = GridTilePrefab.transform.localScale;
        return (tileScale.x, tileScale.y);
    }

    public Vector3 GetGridCenter() {
        (float w, float h) = GetGridSize();
        return new Vector3(w / 2, -h / 2, 0);
    }

    public (float, float) GetGridSize() {
        (float tileWidth, float tileHeight) = GetTileSize();
        return (grid.Width * (tileWidth + gridLineWidth), grid.Height * (tileHeight + gridLineWidth));
    }
}
