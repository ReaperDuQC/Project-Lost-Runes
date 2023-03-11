using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Table", menuName = "Sounds/Sound Table")]
public class SoundTable : ScriptableObject 
{
    public Dictionary<string, AudioClip> _audio = new();
}
