using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessing : MonoBehaviour
{
    public PostProcessVolume volume;

    public Vignette v;
    public float a, b, f;
    public GameObject player;
    public GameObject[] enemies;
    public Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGetSettings(out v);
        v.intensity.value = 0.0f;
        
        // (1 - f) * a + f * b
        a = 0.0f;
        b = 1.0f; // 1?
        f = 0.0f;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = player.transform.position;
        Vector3[] enemiesPos = new Vector3[enemies.Length];
        float distance;
        float minDistance = Mathf.Infinity;
        int itter = enemies.Length;
        for (int i = 0; i < itter; i++)
        {
            enemiesPos[i] = enemies[i].transform.position;
            distance = Vector3.Distance(pos, enemiesPos[i]);
            minDistance = Mathf.Min(minDistance, distance);
        }

        f = 1 - ((minDistance - 1) / 6.0f);
        v.intensity.value = (1 - f) * a + f * b;
    }
}
