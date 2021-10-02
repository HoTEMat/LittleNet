using System.Collections.Generic;
using UnityEngine;

public enum State {
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
    CrossHOffVDead
}

public enum Rotation {
    By0,
    By90,
    By180,
    By270
}

interface IPort {
    State GetState();

    int InnerX { get; }
    int InnerY { get; }

    IPort Clone(SimulationGrid newGrid);
}

interface IAutomaton {
    State NextState(State up, State down, State left, State right, State center);
}

public enum ILevelState {
    Nothing,
    Success,
    Failure
}

interface ILevelValidator {
    int InputCount { get; }
    int OutputCount { get; }

    ILevelState ValidateStates(State[] states);

    State[] GetInputStates();

    void MoveToNextInputState();

    void Reset();
}

interface ILevel {
    SimulationGrid Grid { get; }
    ILevelValidator Validator { get; }

    ILevelState DoIteration();

    int GetIteration { get; }

    void Reset();
}

interface ICloneable<T> where T : ICloneable<T> {
    T Clone();
}