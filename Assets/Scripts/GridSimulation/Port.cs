using System.Diagnostics;
using System.Linq;

class Port : IPort {
    private readonly SimulationGrid Grid;

    public Port(SimulationGrid grid, int x, int y) {
        Grid = grid;

        InnerX = x;
        InnerY = y;
    }

    public State GetState() {
        return Grid.Get(InnerX, InnerY);
    }

    public State[] GetNeighbors(SimulationGrid parentGrid) {
        int i = 0;
        int gridWidth = Grid.Width;
        int gridHeight = Grid.Height;

        State[] neighbors = new State[4];

        // walk neighbors
        (int x, int y)[] offsets = { (0, -1), (0, 1), (-1, 0), (1, 0) };
        foreach (var offset in offsets) {
            int x = InnerX + offset.x;
            int y = InnerY + offset.y;

            bool isInside = x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;

            if (isInside) {
                neighbors[i] = Grid.Get(x, y);
            } else {
                // TODO: rotations
                neighbors[i] = parentGrid.Get(OuterX + offset.x, OuterY + offset.y);
            }

            i++;
        }

        return neighbors;
    }


    public IPort Clone(SimulationGrid newGrid) {
        var clone = new Port(newGrid, InnerX, InnerY);
        return clone;
    }


    /// <summary>
    /// Placement inside the <see cref="SimulationGrid"/>
    /// </summary>
    public int InnerX { get; }
    /// <summary>
    /// Placement inside the <see cref="SimulationGrid"/>
    /// </summary>
    public int InnerY { get; }

    /// <summary>
    /// Placement relative to the <see cref="GridContainer"/>
    /// </summary>
    public int OuterX { get; set; }
    /// <summary>
    /// Placement relative to the <see cref="GridContainer"/>
    /// </summary>
    public int OuterY { get; set; }
}