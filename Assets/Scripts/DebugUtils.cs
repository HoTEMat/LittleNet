using System;
using System.Collections.Generic;

class TestGrid : SimulationGrid {
    public int Width => 20;

    public int Height => 20;

    public void DoIteration() {
        throw new NotImplementedException();
    }

    public void DoSwap() {
        throw new NotImplementedException();
    }

    public State Get(int x, int y) {
        return State.Nothing;
    }

    public void Set(int x, int y, State state) {
        throw new NotImplementedException();
    }

    public GridContainer GetContainerAt(int x, int y) {
        throw new NotImplementedException();
    }

    public List<GridContainer> GetContainers() {
        throw new NotImplementedException();
    }

    public IEnumerable<IPort> GetPorts() {
        throw new NotImplementedException();
    }

    public void InsertContainer(GridContainer container) {
        throw new NotImplementedException();
    }

    public void RemoveContainerAt(GridContainer container) {
        throw new NotImplementedException();
    }

    public void AddPort(IPort port) {
        throw new NotImplementedException();
    }

    public void Reset() {
        throw new NotImplementedException();
    }
}
