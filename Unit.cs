using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitType
{
    Normal,Missle, Civilian
}
public class Unit : MonoBehaviour
{

    Animator mc_animator;

    public AnimationManager animationManager;
    public Dictionary<int, Skill> skillDictionary = new Dictionary<int, Skill>();

    public UnitType type;
    public Player player;
    public UnitAttribute unitAttribute;
    void Awake()
    {
        UnitManager.add(gameObject, this);
    }

    void Start()
    {
        mc_animator = GetComponent<Animator>();
        PlayerInteractor.Instance.ClickObject += onclickGround;
        PlayerInteractor.Instance.ClickUnit += onclickGround;

    }
    void onclickGround(InteractionEvent interactionEvent)
    {
        Debug.Log(interactionEvent);
    }
    void OnMouseEnter()
    {
         PlayerInteractor.doMouseEnterUnit(this);
      
    }

    void OnMouseExit()
    {
        Debug.Log(111);
         PlayerInteractor.doMouseExitUnit(this);
    }
    public IEnumerator followUnit(Unit target,float distace = 2f)
    {
        yield return MotionController.followUnit(gameObject, target.gameObject, distace);
    }
    

}
