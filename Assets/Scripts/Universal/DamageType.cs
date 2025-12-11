using System;

[Flags]
public enum DamageType
{
    None = 0,
    Normal = 1 << 0,
    Heavy  = 1 << 1,
    Poison = 1 << 2,
    Counter = 1 << 3,
    // Add more as needed
}