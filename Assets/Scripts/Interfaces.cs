using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State {
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

interface IGridContainer {
    State Get(int x, int y);

    void Set(int x, int y, State state);

    int Width { get; }
    int Height { get; }

    void DoIteration();
}

interface IAutomaton {
    State NextState(State up, State down, State left, State right, State center);
}

public class Interfaces : MonoBehaviour {
}