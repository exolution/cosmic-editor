using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DamageController : MonoBehaviour {
    private List<GameObject> target = new List<GameObject>();
    public bl_HUDText HUDRoot;
    Func<List<GameObject>> getTarget;
    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }
    public void doDamage()
    {
        List<GameObject> targetList= getTarget();
        foreach(GameObject target in targetList)
        {
            Debug.Log(target);
            createDamageText(target);
        }
    }
    public void register(Func<List<GameObject>> getTarget)
    {
        this.getTarget = getTarget;
    }
    void createDamageText(GameObject target)
    {
        HUDRoot.NewText(UnityEngine.Random.Range(100, 1000).ToString(), target.transform, Color.red, -40, 20f, UnityEngine.Random.Range(-1f, -2f), UnityEngine.Random.Range(4f, 6f), UnityEngine.Random.Range(0, 2) == 0 ? bl_Guidance.LeftDown : bl_Guidance.RightDown);
    }
}
