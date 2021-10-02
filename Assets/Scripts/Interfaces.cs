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
}

interface IGrid {
    State Get(int x, int y);

    void Set(int x, int y, State state);

    IEnumerable<IPort> GetPorts();

    void AddPort(IPort port);

    void InsertContainer(IGridContainer container);

    // Returns null when no container is found.
    IGridContainer GetContainerAt(int x, int y);

    void RemoveContainerAt(IGridContainer container);

    List<IGridContainer> GetContainers();

    // Is called on the outermost container.
    void DoIteration();
    void DoSwap();

    int Width { get; }
    int Height { get; }
}

interface IGridContainer {
    int X { get; set; }
    int Y { get; set; }

    int OuterWidth { get; }
    int OuterHeight { get; }

    IReadOnlyDictionary<(int x, int y), IPort> RelativePortPlacement { get; }
    Rotation Rotation { get; set; }

    IGrid Grid { get; }
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
}

interface ILevel {
    IGrid Grid { get; }
    ILevelValidator Validator { get; }

    ILevelState DoIteration();
}

public class Interfaces : MonoBehaviour {
}