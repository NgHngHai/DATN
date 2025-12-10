using System;

[Flags]
public enum DamageType
{
    Normal = 0,
    Heavy  = 1 << 0,
    Poison = 1 << 1,
    Counter = 1 << 2
    // Add more as needed
}