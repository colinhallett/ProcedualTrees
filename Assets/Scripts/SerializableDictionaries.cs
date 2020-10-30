using UnityEngine;
using System.Collections;
using System;
using RotaryHeart.Lib.SerializableDictionary;

namespace CKZH.ProcedualTree
{
    [Serializable]
    public class ImplementationDict : SerializableDictionaryBase<char, Module>
    {

    }

    [Serializable]
    public class RulesDict : SerializableDictionaryBase<char, string>
    {

    }
}
