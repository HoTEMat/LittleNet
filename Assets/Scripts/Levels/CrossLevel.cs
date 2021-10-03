using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static partial class Levels {
    public static Level CrossLevel() {

        int size = 10;
        var level = new Level(new CrossLevelValidator(), size, size);
        return level;
    }
}

class CrossLevelValidator : CalendarValidator {

    int caseN = 0;
    static List<(bool in0, bool in1, bool out1, bool out0)> cases = new List<(bool, bool, bool, bool)> {
        (false, false, false, false),
        (true, false, false, true),
        (false, true, true, false),
        (false, false, false, false),
    };

    public override int InputCount => 2;
    public override int OutputCount => 2;


    public override State[] GetInputStates() {
        var c = cases[caseN];
        return new State[] { c.in0.AsWire(), c.in1.AsWire() };
    }

    public override void MoveToNextInputState() {
        if (Iterations % 200 == 0) {
            if (caseN < cases.Count) {
                Expect(100, outputs =>
                    (outputs[0] == State.WireOn) == cases[caseN].out0 && (outputs[1] == State.WireOn) == cases[caseN].out1
                    ? ILevelState.Nothing
                    : ILevelState.Failure    
                );
            } else {
                Succeed();
            }
        }

        base.MoveToNextInputState();
    }
}
