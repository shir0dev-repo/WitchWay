using System;
using System.Linq;

[System.Flags]
public enum AlchemicalSymbol : sbyte
{
    None = 0,
    Abjuration = 1,
    Necromancy = 2,
    Enchantment = 4,
    Divination = 8,
    Evocation = 16,
    All = ~0b0
}