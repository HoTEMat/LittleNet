using System.Collections.Generic;


static partial class Levels {
    public static Level NotLevel() {
        var level = new Level(new NotLevelValidator(), 15, 10, "Not level", "A very nice not level.");
        return level;
    }
}

class NotLevelValidator : CalendarValidator {
    int testCaseN = 0;
    const int totalTestCases = 4;

    const int testInterval = 100;

    static List<(bool in0, bool out0)> cases = new List<(bool, bool)> {
        (false, true),
        (true, false),
    };

    public override int InputCount => 1;
    public override int OutputCount => 1;

    public override void Reset() {
        base.Reset();
        testCaseN = 0;
    }

    public override bool[] GetInputs() {
        var c = cases[testCaseN % 2];
        return new[] {c.in0};
    }

    public override void MoveToNextInputState() {
        if (Iterations == 0) {
            RegisterNextTest();
        }

        base.MoveToNextInputState();
    }

    private void RegisterNextTest() {
        Expect(testInterval, outputs => {
            var data = cases[testCaseN % 2];
            bool ok = data.out0 == outputs[0];

            testCaseN++;
            bool lastTest = testCaseN >= totalTestCases;

            if (lastTest) {
                return ok ? ILevelState.Success : ILevelState.Failure;
            }
            else {
                RegisterNextTest();
                return ok ? ILevelState.Nothing : ILevelState.Failure;
            }
        });
    }
}