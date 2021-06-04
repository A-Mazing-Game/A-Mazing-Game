using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    public bool startFight;
    public MazeConstructor mz;
    private void OnTriggerEnter(Collider other)
    {