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

    int testCaseN = 0;
    const int totalTestCases = 16;

    const int testInterval = 100;

    static List<(bool in0, bool in1, bool out0, bool out1)> cases = new List<(bool, bool, bool, bool)> {
        (false, false, false, false),
        (true, false, false, true),
        (false, true, true, false),
        (true, true, true, true),
    };

    public override int InputCount => 2;
    public override int OutputCount => 2;

    public override void Reset() {
        base.Reset();
        testCaseN = 0;
    }

    public override State[] GetInputs() {
        var c = cases[testCaseN];
        return new State[] { c.in0.AsWire(), c.in1.AsWire() };
    }

    public override void MoveToNextInputState() {

        if (Iterations == 0) {
            RegisterNextTest();
        }

        base.MoveToNextInputState();
    }

    private void RegisterNextTest() {

        Expect(testInterval, outputs => {
            var data = cases[testCaseN];
            bool ok = data.out0 == outputs[0] && data.out1 == outputs[1];

            testCaseN++;
            bool lastTest = testCaseN >= totalTestCases;

            if (lastTest) {
                return ok ? ILevelState.Success : ILevelState.Failure;
            } else {
                RegisterNextTest();
                return ok ? ILevelState.Nothing : ILevelState.Failure;
            }

        });
    }
}
