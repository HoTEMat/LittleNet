using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;



class WireeAutomaton : IAutomaton {

    Dictionary<(State, State, State, State, State), State> cache = new Dictionary<(State, State, State, State, State), State>();

    public State NextState(State up, State down, State left, State right, State center) {

        var key = (up, down, left, right, center);

        if (cache.TryGetValue(key, out var cachedState)) {
            return cachedState;
        }

        var computedState = ComputeNextState(up, down, left, right, center);
        cache[key] = computedState;
        return computedState;
    }

    public State ComputeNextState(State up, State down, State left, State right, State center) {

        var neighbors = new[] { up, down, left, right };
        var horizontal = new[] { left, right };
        var vertical = new[] { up, down };

        switch (center) {

            // Nothing
            case State.Nothing: return State.Nothing;

            // Wire
            case State.WireOff:
                return vertical.Any(v => v.EmitsVertical())
                    || horizontal.Any(h => h.EmitsHorizontal())
                        ? State.WireOn
                        : center;
            case State.WireOn:
                return vertical.Any(v => v.DeadVertical())
                    || horizontal.Any(h => h.DeadHorizontal())
                        ? State.WireDead
                        : center;
            case State.WireDead: return State.WireOff;

            // Lamp
            case State.LampOff:
                return vertical.Any(v => v.EmitsVertical())
                    || horizontal.Any(h => h.EmitsHorizontal())
                        ? State.LampOn
                        : center;
            case State.LampOn:
                return vertical.All(v => !v.EmitsVertical())
                    || horizontal.All(h => !h.EmitsHorizontal())
                    ? State.LampDead
                    : center;
            case State.LampDead: return State.LampOff;

            // Negator
            case State.NotOn:
                return neighbors.All(n => !n.IsLamp() || n == State.LampOn)
                    ? center
                    : State.NotDead;
            case State.NotOff:
                return neighbors.Any(n => n.IsLamp() && n != State.LampOn)
                    ? State.NotOn
                    : center;
            case State.NotDead: return State.NotOff;
        }

        Debug.Assert(center.IsCross());

        // Cross //

        { // horizontal
            if (center.DeadHorizontal()) {
                // -- is dead
                center = center.CrossSetOff(true);
            } else if (center.EmitsHorizontal()) {
                // -- is on
                if (horizontal.Any(h => h.DeadHorizontal() || h == State.LampDead))
                    center = center.CrossSetDead(true);
            } else {
                // -- is off
                if (horizontal.Any(h => h.EmitsHorizontal() || h == State.LampOn))
                    center = center.CrossSetOn(true);
            }
        }

        { // vertical
            if (center.DeadVertical()) {
                // | is dead
                center = center.CrossSetOff(false);
            } else if (center.EmitsVertical()) {
                // | is on
                if (vertical.Any(v => v.DeadVertical() || v == State.LampDead))
                    center = center.CrossSetDead(false);
            } else {
                // | is off
                if (vertical.Any(v => v.EmitsVertical() || v == State.LampOn))
                    center = center.CrossSetOn(false);
            }
        }



        return center;
    }
}

static class StateEx {

    public static State AsWire(this bool b) {
        return b ? State.WireOn : State.WireOff;
    }

    public static bool IsPlaceable(this State state) {
        switch (state) {
            case State.Nothing:
            case State.WireOn:
            case State.WireDead:
            case State.WireOff:
            case State.LampOn:
            case State.LampOff:
            case State.NotOn:
            case State.NotOff:
            case State.CrossHOnVOn:
            case State.CrossHOnVOff:
            case State.CrossHOffVOn:
            case State.CrossHOffVOff:
                return true;
            default:
                return false;
        }
    }

    public static bool IsWire(this State state) {
        switch (state) {
            case State.WireDead:
            case State.WireOn:
            case State.WireOff:
                return true;
            default:
                return false;
        }
    }
    public static bool IsLamp(this State state) {
        switch (state) {
            case State.LampDead:
            case State.LampOn:
            case State.LampOff:
                return true;
            default:
                return false;
        }
    }
    public static bool IsNot(this State state) {
        switch (state) {
            case State.NotDead:
            case State.NotOn:
            case State.NotOff:
                return true;
            default:
                return false;
        }
    }
    public static bool IsCross(this State state) {
        switch (state) {
            case State.CrossHOnVOn:
            case State.CrossHOnVOff:
            case State.CrossHOffVOn:
            case State.CrossHOffVOff:
            case State.CrossHDeadVOn:
            case State.CrossHOnVDead:
            case State.CrossHDeadVDead:
            case State.CrossHDeadVOff:
            case State.CrossHOffVDead:
                return true;
            default:
                return false;
        }
    }

    public static bool EmitsVertical(this State state) {
        switch (state) {
            case State.WireOn:
            case State.NotOn:
            case State.CrossHOnVOn:
            case State.CrossHOffVOn:
            case State.CrossHDeadVOn:
                return true;
            default:
                return false;
        }

        throw new NotImplementedException();
    }
    public static bool EmitsHorizontal(this State state) {
        switch (state) {
            case State.WireOn:
            case State.NotOn:
            case State.CrossHOnVOff:
            case State.CrossHOnVDead:
            case State.CrossHOnVOn:
                return true;
            default:
                return false;
        }

        throw new NotImplementedException();
    }

    public static bool DeadVertical(this State state) {
        switch (state) {
            case State.WireDead:
            case State.NotDead:
            case State.CrossHOnVDead:
            case State.CrossHDeadVDead:
            case State.CrossHOffVDead:
                return true;
            default:
                return false;
        }
    }

    public static bool DeadHorizontal(this State state) {
        switch (state) {
            case State.WireDead:
            case State.NotDead:
            case State.CrossHDeadVDead:
            case State.CrossHDeadVOff:
            case State.CrossHDeadVOn:
                return true;
            default:
                return false;
        }
    }

    public static State CrossSetDead(this State state, bool horizontal) {
        switch (state) {
            case State.CrossHOnVOn: return horizontal ? State.CrossHDeadVOn : State.CrossHOnVDead;
            case State.CrossHOnVOff: return horizontal ? State.CrossHDeadVOff : State.CrossHOnVDead;
            case State.CrossHOffVOn: return horizontal ? State.CrossHDeadVOn : State.CrossHOffVDead;
            case State.CrossHOffVOff: return horizontal ? State.CrossHDeadVOff : State.CrossHOffVDead;
            case State.CrossHDeadVOn: return horizontal ? State.CrossHDeadVOn : State.CrossHDeadVDead;
            case State.CrossHOnVDead: return horizontal ? State.CrossHDeadVDead : State.CrossHOnVDead;
            case State.CrossHDeadVDead: return horizontal ? State.CrossHDeadVDead : State.CrossHDeadVDead;
            case State.CrossHDeadVOff: return horizontal ? State.CrossHDeadVOff : State.CrossHDeadVDead;
            case State.CrossHOffVDead: return horizontal ? State.CrossHDeadVDead : State.CrossHOffVDead;
        }

        throw new InvalidOperationException();
    }


    public static State CrossSetOn(this State state, bool horizontal) {
        switch (state) {
            case State.CrossHOnVOn: return horizontal ? State.CrossHOnVOn : State.CrossHOnVOn;
            case State.CrossHOnVOff: return horizontal ? State.CrossHOnVOff : State.CrossHOnVOn;
            case State.CrossHOffVOn: return horizontal ? State.CrossHOnVOn : State.CrossHOffVOn;
            case State.CrossHOffVOff: return horizontal ? State.CrossHOnVOff : State.CrossHOffVOn;
            case State.CrossHDeadVOn: return horizontal ? State.CrossHOnVOn : State.CrossHDeadVOn;
            case State.CrossHOnVDead: return horizontal ? State.CrossHOnVDead : State.CrossHOnVOn;
            case State.CrossHDeadVDead: return horizontal ? State.CrossHOnVDead : State.CrossHDeadVOn;
            case State.CrossHDeadVOff: return horizontal ? State.CrossHOnVOff : State.CrossHDeadVOn;
            case State.CrossHOffVDead: return horizontal ? State.CrossHOnVDead : State.CrossHOffVOn;
        }

        throw new InvalidOperationException();
    }

    public static State CrossSetOff(this State state, bool horizontal) {
        switch (state) {
            case State.CrossHOnVOn: return horizontal ? State.CrossHOffVOn : State.CrossHOnVOff;
            case State.CrossHOnVOff: return horizontal ? State.CrossHOffVOff : State.CrossHOnVOff;
            case State.CrossHOffVOn: return horizontal ? State.CrossHOffVOn : State.CrossHOffVOff;
            case State.CrossHOffVOff: return horizontal ? State.CrossHOffVOff : State.CrossHOffVOff;
            case State.CrossHDeadVOn: return horizontal ? State.CrossHOffVOn : State.CrossHDeadVOff;
            case State.CrossHOnVDead: return horizontal ? State.CrossHOffVDead : State.CrossHOnVOff;
            case State.CrossHDeadVDead: return horizontal ? State.CrossHOffVDead : State.CrossHDeadVOff;
            case State.CrossHDeadVOff: return horizontal ? State.CrossHOffVOff : State.CrossHDeadVOff;
            case State.CrossHOffVDead: return horizontal ? State.CrossHOffVDead : State.CrossHOffVOff;
        }

        throw new InvalidOperationException();
    }
}