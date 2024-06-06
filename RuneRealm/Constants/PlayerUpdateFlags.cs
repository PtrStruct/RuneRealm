﻿namespace RuneRealm.Constants;

[Flags]
public enum PlayerUpdateFlags
{
    None = 0,
    Graphics = 0x100,
    Animation = 0x8,
    InteractingEntity = 0x1,
    Appearance = 0x10,
    FaceDirection = 0x2,
    SingleHit = 0x20,
    DoubleHit = 0x200,
    FullMask = 0x140
}