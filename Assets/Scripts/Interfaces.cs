using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State {
    Nothing,
    WireOn,
    WireDead,
    WireOff,
    LampOn,
    LampDead,
    LampOff,
    NotOn,
    NotDead,
    NotOff,
    CrossHOnVOn,
    CrossHOnVOff,
    CrossHOffVOn,
    CrossHOffVOff,
    CrossHDeadVOn,
    CrossHOnVDead,
    CrossHDeadVDead,
    CrossHDeadVOff,
    CrossHOffVDead,
}

enum Rotation {
    By0, By90, By180, By270
}

interface IGrid {
    State Get(int x, int y);

    void Set(int x, int y, State state);

    void InsertContainer(IGridContainer container);
    
    // returns null when no container is found
    IGridContainer GetContainerAt(int x, int y);
    
    void RemoveContainerAt(IGridContainer container);

    List<IGridContainer> GetContainers();

    // volá se jen na tom vnějším
    void DoIteration();

    int Width { get; }
    int Height { get; }
}

interface IGridContainer {
    int X { get; set; }
    int Y { get; set; }

    int OuterWidth { get; }
    int OuterHeight { get; }

    Rotation Rotation { get; set; }

    IGrid Grid { get; }
}

interface IAutomaton {
    State NextState(State up, State down, State left, State right, State center);
}

public class Interfaces : MonoBehaviour { }