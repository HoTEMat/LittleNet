using System.Collections.Generic;

class GridContainer : IGridContainer
{
    public GridContainer(int x, int y, List<IPort> ports, IGrid grid) {
        Grid = grid;
        
        X = x;
        Y = y;
        // TODO: calculate width and height from the position of ports
    }
    
    public int X { get; set; }
    public int Y { get; set; }
    
    public int OuterWidth { get; }
    public int OuterHeight { get; }
    
    public IReadOnlyDictionary<(int x, int y), IPort> RelativePortPlacement { get; }
    
    public Rotation Rotation { get; set; }
    public IGrid Grid { get; }
}
