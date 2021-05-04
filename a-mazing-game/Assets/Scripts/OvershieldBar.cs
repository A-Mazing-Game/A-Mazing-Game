using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OvershieldBar : MonoBehaviour
{
    public Slider slider;
    private SphereCollider potion;
    
    // Start is called before the first frame update
    public void SetOvershield(int overshield)
    {
        slider.value = overshield;
        print(slider.value);
    }

    public void SetMaxOvershield(int overshield)
    {
        slider.maxValue = overshield;
        slider.value = overshield;
    }
}
