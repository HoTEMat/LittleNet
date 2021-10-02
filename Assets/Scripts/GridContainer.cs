using System;
using System.Collections.Generic;

class GridContainer : IGridContainer {
    private Dictionary<(int x, int y), IPort> relativePortPlacement = new Dictionary<(int x, int y), IPort>();
    
    public GridContainer(int x, int y, List<IPort> ports, IGrid grid, Rotation rotation) {
        Grid = grid;
        
        X = x;
        Y = y;

        Rotation = rotation; 
        
        // whether there are ports in the corners
        IPort lu = null;
        IPort ru = null;
        IPort ld = null;
        IPort rd = null;

        // the ports on each of the sides
        List<IPort> l = new List<IPort>();
        List<IPort> r = new List<IPort>();
        List<IPort> u = new List<IPort>();
        List<IPort> d = new List<IPort>();
        
        // Yuck!
        foreach (IPort port in ports) {
            if (port.InnerX == 0 && port.InnerY == 0) lu = port;
            if (port.InnerX == grid.Width - 1 && port.InnerY == 0) ru = port;
            if (port.InnerX == grid.Width - 1 && port.InnerY == grid.Height - 1) rd = port;
            if (port.InnerX == 0 && port.InnerY == grid.Height - 1) ld = port; 
            
            if (port.InnerX == 0) l.Add(port); 
            if (port.InnerX == grid.Width - 1) r.Add(port); 
            if (port.InnerY == 0) u.Add(port); 
            if (port.InnerY == grid.Height - 1) d.Add(port);
        }
        
        // offsets - either 1 if the port is not there or 2 if it is
        int tOffset = (lu != null || ru != null) ? 2 : 1;
        int lOffset = (lu != null || ld != null) ? 2 : 1;
        int dOffset = (ld != null || rd != null) ? 2 : 1;
        int rOffset = (rd != null || ru != null) ? 2 : 1;

        // lengths of each side, calculated from offsets
        int lMin = tOffset + dOffset + (l.Count - 1) * 2 + 1;
        int rMin = tOffset + dOffset + (r.Count - 1) * 2 + 1;
        int uMin = lOffset + rOffset + (u.Count - 1) * 2 + 1;
        int dMin = lOffset + rOffset + (d.Count - 1) * 2 + 1;
        
        // determine the dimensions so that both sides fit
        OuterWidth = Math.Max(uMin, dMin);
        OuterHeight = Math.Max(lMin, rMin);

        // place the corner ports
        if (lu != null) relativePortPlacement[(0, 0)] = lu;
        if (ru != null) relativePortPlacement[(OuterWidth - 1, 0)] = ru;
        if (rd != null) relativePortPlacement[(OuterWidth - 1, OuterHeight - 1)] = rd;
        if (ld != null) relativePortPlacement[(0, OuterHeight - 1)] = rd;
        
        // place the rest of the ports
        // TODO
    }
    
    public int X { get; set; }
    public int Y { get; set; }
    
    public int OuterWidth { get; }
    public int OuterHeight { get; }

    public IReadOnlyDictionary<(int x, int y), IPort> RelativePortPlacement => relativePortPlacement;
    
    public Rotation Rotation { get; set; }
    public IGrid Grid { get; }
}
