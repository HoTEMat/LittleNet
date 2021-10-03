using System.Collections.Generic;

static partial class Levels {
    public static Level AndLevel() {
        var level = new Level(
            new StrictCalendalValidator(
                new List<List<bool>> {
                    new List<bool> {false, false},
                    new List<bool> {true, false},
                    new List<bool> {false, true},
                    new List<bool> {true, true},
                },
                new List<List<bool>> {
                    new List<bool> {false},
                    new List<bool> {false},
                    new List<bool> {false},
                    new List<bool> {true},
                }
            ), 15, 10, "And level", "A very nice and level.");
        return level;
    }
}
