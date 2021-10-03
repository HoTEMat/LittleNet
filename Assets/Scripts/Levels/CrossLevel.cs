using System.Collections.Generic;
using UnityEngine;


static partial class Levels {
    public static Level CrossLevel() {
        var level = new Level(
            new StrictCalendalValidator(
                new List<List<bool>> {
                    new List<bool> {false, false},
                    new List<bool> {true, false},
                    new List<bool> {false, true},
                    new List<bool> {true, true},
                },
                new List<List<bool>> {
                    new List<bool> {false, false},
                    new List<bool> {false, true},
                    new List<bool> {true, false},
                    new List<bool> {true, true},
                }
            ), 15, 10, "Cross level", "A very nice cross level.");
        return level;
    }
}

class StrictCalendalValidator : CalendarValidator {
    private int testInterval;
    private int testCaseN;

    private List<List<bool>> inputs;
    private List<List<bool>> outputs;
    
    private int repeat;


    public StrictCalendalValidator(List<List<bool>> inputs, List<List<bool>> outputs, int repeat = 2, int testInterval = 50) {
        InputCount = inputs[0].Count;
        OutputCount = outputs[0].Count;
        
        this.repeat = repeat;

        this.inputs = inputs;
        this.outputs = outputs;

        this.testInterval = testInterval;
    }
    
    public override int InputCount { get; }
    public override int OutputCount { get; }
    
    public override void Reset() {
        base.Reset();
        testCaseN = 0;
    }
    
    public override bool[] GetInputs() {
        return inputs[testCaseN % inputs.Count].ToArray();
    }
    
    public override void MoveToNextInputState() {
        if (Iterations == 0) {
            RegisterNextTest();
        }

        base.MoveToNextInputState();
    }
    
    private void RegisterNextTest() {
        Expect(testInterval, outputs => {
            var data = this.outputs[testCaseN % this.outputs.Count];

            bool ok = true;
            for (int i = 0; i < outputs.Length; i++) {
                if (outputs[i] != data[i])
                    ok = false;
                
            }

            testCaseN++;
            bool lastTest = testCaseN >= inputs.Count * repeat;

            if (lastTest)
                return ok ? ILevelState.Success : ILevelState.Failure;

            RegisterNextTest();
            return ok ? ILevelState.Nothing : ILevelState.Failure;
        });
    }
}
