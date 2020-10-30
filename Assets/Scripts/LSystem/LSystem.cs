using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using System;

namespace CKZH.ProcedualTree
{
    public class LSystem
    {
        public static string GenerateSentence(string axiom, int iterations, RulesDict rules)
        {
            var sentence = axiom;

            for (int i = 0; i < iterations; i++)
            {
                var replace = "";
                for (int c = 0; c < sentence.Length; c++)
                {
                    var test = sentence[c];

                    if (rules.ContainsKey(test))
                        replace += rules[test];
                    else replace += test;

                }

                sentence = replace;
            }

            return sentence;
        }
    }
}
