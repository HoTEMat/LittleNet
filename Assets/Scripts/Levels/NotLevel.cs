using System.Collections.Generic;

static partial class Levels {
    public static Level NotLevel() {
        var level = new Level(
            new StrictCalendalValidator(
                new List<List<bool>> {
                    new List<bool> {false},
                    new List<bool> {true},
                },
                new List<List<bool>> {
                    new List<bool> {true},
                    new List<bool> {false},
                }
            ), 15, 10, "Not level", "A very nice not level.");
        return level;
    }
}
