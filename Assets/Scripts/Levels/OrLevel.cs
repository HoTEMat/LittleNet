using System.Collections.Generic;

static partial class Levels {
    public static Level OrLevel() {
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
                    new List<bool> {true},
                    new List<bool> {true},
                    new List<bool> {true},
                }
            ), 15, 10, "Or level", "A very nice or level.");
        return level;
    }
}
