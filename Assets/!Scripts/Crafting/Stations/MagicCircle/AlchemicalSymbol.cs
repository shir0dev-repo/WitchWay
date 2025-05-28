using System;
using System.Linq;

[System.Flags]
public enum AlchemicalSymbol
{
    None = 0,
    Abjuration = 1,
    Necromancy = 2,
    Enchantment = 4,
    Divination = 8,
    Evocation = 16
}

public static class EnumUtils 
{
    /*public static bool HasFlag(this AlchemicalSymbol flag, AlchemicalSymbol checkedFlag)
    {
        
    }*/

}
