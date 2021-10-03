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
    static List<(bool in0, bool in1, bool out0, bool out1)> cases = new List<(bool, bool, bool, bool)> {
        (false, false, false, false),
        (true, false, false, true),
        (false, true, true, false),
        (false, false, false, false),
    };

    public override int InputCount => 2;
    public override int OutputCount => 2;

    public override void Reset() {
        base.Reset();
        caseN = 0;
    }


    public override State[] GetInputStates() {
        var c = cases[caseN];
        return new State[] { c.in0.AsWire(), c.in1.AsWire() };
    }

    public override void MoveToNextInputState() {
        if (Iterations % 200 == 0) {
             if (caseN < cases.Count) {
                Expect(100, outputs => {
                    var c = cases[caseN++];
                    if ((outputs[0] == State.WireOn) == c.out0 && (outputs[1] == State.WireOn) == c.out1) { 
                        return ILevelState.Nothing;
                    }
                    return ILevelState.Failure;
                }
                );
            } else {
                Succeed();
            }
        }

        base.MoveToNextInputState();
    }
}
