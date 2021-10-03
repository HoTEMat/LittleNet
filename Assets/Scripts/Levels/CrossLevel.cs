using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static partial class Levels {
    public static Level CrossLevel() {

        var level = new Level(new CrossLevelValidator(), 15, 10, "Cross level", "A very nice cross level.");
        return level;
    }
}

class CrossLevelValidator : CalendarValidator {

    private int testCaseN = 0;
    private int testCaseCurrent;
    private const int totalTestCases = 16;

    private Random r = new Random();

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

    public override bool[] GetInputs() {
        var c = cases[testCaseCurrent];
        return new[]{ c.in0, c.in1 };
    }

    public override void MoveToNextInputState() {

        if (Iterations == 0) {
            RegisterNextTest();
        }

        base.MoveToNextInputState();
    }

    private void RegisterNextTest() {
        testCaseCurrent = r.Next(0, cases.Count - 1);
            
        Expect(testInterval, outputs => {
            var data = cases[testCaseCurrent];
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
