using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLabel : MonoBehaviour
{
    [SerializeField] eHIT label = default;
    public eHIT GetLabel()
    {
        return label;
    }
    public void SetLabel(eHIT inLabel)
    {
        label = inLabel;
    }
}
