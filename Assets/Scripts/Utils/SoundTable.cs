using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Table", menuName = "LostRunes/Sounds/Sound Table Data")]
public class SoundTable : ScriptableObject 
{
    public Dictionary<string, AudioClip> _audio = new();
}
