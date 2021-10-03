using System;
using System.Collections.Generic;

class GatePrototype {

    public GatePrototype(SimulationGrid protoGrid, string name) {
        Name = name;
        ProtypeGrid = protoGrid;
    }

    public string Name { get; private set; }
    public int OuterWidth => throw new NotImplementedException();
    public int OuterHeight => throw new NotImplementedException();
    public SimulationGrid ProtypeGrid { get; }

    public GridContainer CreateInstance(int x, int y, Rotation rotation) {

        var gridClone = ProtypeGrid.Clone();
        var result = new GridContainer(x, y, gridClone, rotation);

        return result;
    }
}
