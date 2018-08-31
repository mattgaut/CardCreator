using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StaticAbility), true)]
public class StaticAbilityEditor : Editor {
    // Class Only exists to allow CompareEntity classes to be drawn properly on static abilities
}
