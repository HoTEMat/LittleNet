using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static partial class Levels {
    public static Level HappyLevel() {

        var validator = new HappyValidator();
        int size = 10;

        var level = new Level(validator, size, size);
        return level;
    }
}
