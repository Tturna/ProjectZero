using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static MapScriptableObject mapSO;
    public static List<int> unlockedZones = new List<int>();

    private void Start()
    {
        unlockedZones.Add(0);
    }
}
