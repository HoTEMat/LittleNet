using System.Diagnostics;
using System.Linq;

class Port : IPort {
    private readonly GridContainer Container;
    private readonly SimulationGrid Grid;

    public Port(GridContainer cont, int x, int y) {
        Container = cont;
        Grid = cont.Grid;

        InnerX = x;
        InnerY = y;

        var placement = cont.RelativePortPlacement.Where(kv => kv.Value == this).Single().Key;
        OuterX = placement.x;
        OuterY = placement.y;
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
        (int x, int y)[] offsets = {(0, -1), (0, 1), (-1, 0), (1, 0)};
        foreach (var offset in offsets) {
            int x = InnerX + offset.x;
            int y = InnerY + offset.y;

            bool isInside = x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;

            if (isInside) {
                neighbors[i] = Grid.Get(x, y);
            }
            else {
                // TODO: rotations
                neighbors[i] = parentGrid.Get(OuterX + offset.x, OuterY + offset.y);
            }

            i++;
        }

        return neighbors;
    }

    
    public Port Clone(GridContainer newGridCont) {
        var clone = new Port(newGridCont, InnerX, InnerY);
        return clone;
    }
    public IPort Clone(SimulationGrid newGrid) {
        throw new System.InvalidOperationException();
    }

    public int InnerX { get; }
    public int InnerY { get; }

    public int OuterX { get; }
    public int OuterY { get; }
}