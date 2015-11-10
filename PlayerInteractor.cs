using UnityEngine;
using System.Collections;
public enum InteractionEventType
{
    OnMouseEnter,OnMouseExit,RightClick,LeftClick
}
public class InteractionEvent
{
    public InteractionEventType type;
    public Unit targetUnit;
    public Vector3 targetPoint;
    public GameObject targetObject;
   
    public InteractionEvent(InteractionEventType type, Unit targetUnit, Vector3 targetPoint)
    {
        this.type = type;
        this.targetUnit = targetUnit;
        this.targetPoint = targetPoint;        
    }
    public InteractionEvent(InteractionEventType type, Unit targetUnit)
    {
        this.type = type;
        this.targetUnit = targetUnit;
    }
    public InteractionEvent(InteractionEventType type, GameObject targetObject, Vector3 targetPoint)
    {
        this.type = type;
        this.targetObject = targetObject;
        this.targetPoint = targetPoint;
    }
    public InteractionEvent(InteractionEventType type, GameObject targetObject)
    {
        this.type = type;
        this.targetObject = targetObject;
    }
    public InteractionEvent(InteractionEventType type, Vector3 targetPoint)
    {
        this.type = type;
        this.targetPoint = targetPoint;
    }
    public override string ToString()
    {
        return this.type+"-"+this.targetUnit + "-" + this.targetObject+"-"+this.targetPoint;
    }
}

public delegate void PlayerInteractorHandler(InteractionEvent interactionEvent);
enum InteractionStatus
{
    NORMAL, TARGET_SELECT, AOE_SELECT
}

/// <summary>
/// 管理玩家的鼠标与各种物体的交互
/// </summary>
public class PlayerInteractor : MonoBehaviour
{

    public Texture2D m_pointer;
    public Texture2D m_attack;
    public Texture2D m_select;
    
    public float smooth = 4f;
    Shader rimLight;
    
    bool needDetect = true;
    bool forceDetect = false;
    InteractionStatus interactionStatus = InteractionStatus.NORMAL;
  
    public event PlayerInteractorHandler MouseEnterUnit;
    public event PlayerInteractorHandler MouseExitUnit;
    public event PlayerInteractorHandler ClickUnit;
    public event PlayerInteractorHandler MouseEnterObject;
    public event PlayerInteractorHandler MouseExitObject;
    public event PlayerInteractorHandler ClickObject;
    Vector3 hitPoint;
    public static PlayerInteractor Instance
    {
        get;
        private set;
    }
    // Use this for initialization
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            PlayerManager.init();
        }
        else
        {
            Debug.LogError("ActionManager must be added to just only one gameobject");
        }
    }
    void Start()
    {

        Cursor.SetCursor(m_pointer, Vector2.zero, CursorMode.ForceSoftware);
        rimLight = Shader.Find("Custom/RimLight");
        //MouseOverUnit += onMouseOverUnit;
        //MouseOutUnit += new PlayerInteractorHandler(onMouseOutUnit);
        MouseEnterUnit += onMouseEnterUnit;
        MouseEnterObject += onMouseEnterObject;

    }

  

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKey("e"))
        {
            interactionStatus = InteractionStatus.AOE_SELECT;
            forceDetect = true;
        }
        if (forceDetect)
        {

            Cursor.visible = false;
            Vector3 targetPositon = new Vector3(hitPoint.x, gameObject.transform.position.y, hitPoint.z);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPositon, Time.deltaTime * smooth);
        }
        bool leftMouseButtonDown = Input.GetMouseButtonDown(0);
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);
        if (leftMouseButtonDown || rightMouseButtonDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //判断射线是否发生碰撞                
            if (Physics.Raycast(ray, out hit, 100))
            {
                hitPoint = hit.point;
                Unit hitUnit = UnitManager.getUnit(hit.collider.gameObject);
                if (hitUnit)
                {
                    if (ClickUnit != null) {
                        ClickUnit(new InteractionEvent(leftMouseButtonDown ? InteractionEventType.LeftClick : InteractionEventType.RightClick, hitUnit, hit.point));
                    }
                }
                else
                {
                    if (ClickObject != null)
                    {
                        ClickObject(new InteractionEvent(leftMouseButtonDown ? InteractionEventType.LeftClick : InteractionEventType.RightClick, hit.collider.gameObject, hitPoint));
                    }
                }
            }
              
        }
       
    }
    void FixedUpdate()
    {
        if (needDetect || forceDetect)
        {
            switch (interactionStatus)
            {
                case InteractionStatus.NORMAL:
                    doNormalDetect();
                    break;
                case InteractionStatus.TARGET_SELECT:
                    doTargetSelectDetect();
                    break;
                case InteractionStatus.AOE_SELECT:
                    doAoeTargetSelectDetect();

                    break;
            }
        }
    }
    void doNormalDetect()
    {
            
       
    }

    void doTargetSelectDetect()
    {

    }
    void doAoeTargetSelectDetect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //判断射线是否发生碰撞                
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Terrain")))
        {
            hitPoint = hit.point;

            //gameObject.transform.position.z = hit.point.z;
        }
    }
    public static void doMouseEnterUnit(Unit targetUnit)
    {
        if (Instance.MouseEnterUnit != null)
        {
            Instance.MouseEnterUnit(new InteractionEvent(InteractionEventType.OnMouseEnter, targetUnit));
        }
    }

    public static void doMouseExitUnit(Unit targetUnit)
    {
        if (Instance.MouseExitUnit != null)
        {
            Instance.MouseExitUnit(new InteractionEvent(InteractionEventType.OnMouseExit, targetUnit));
        }
    }
    public static void doClickUnit(InteractionEventType iet,Unit targetUnit,Vector3 clickPoint)
    {
        if (Instance.ClickUnit != null)
        {
            Instance.ClickUnit(new InteractionEvent(iet, targetUnit, clickPoint));
        }
    }

    public static void doMouseEnterObject(GameObject targetObject)
    {
        if (Instance.MouseEnterObject != null)
        {
            Instance.MouseEnterObject(new InteractionEvent(InteractionEventType.OnMouseEnter, targetObject));
        }
    }
    public static void doMouseExitObject(GameObject targetObject)
    {
        if (Instance.MouseExitObject != null)
        {
            Instance.MouseExitObject(new InteractionEvent(InteractionEventType.OnMouseExit, targetObject));
        }
    }
    public static void doClickObject(InteractionEventType iet, GameObject targetUnit, Vector3 clickPoint)
    {
        if (Instance.MouseExitObject != null)
        {
            Instance.MouseExitObject(new InteractionEvent(iet, targetUnit, clickPoint));
        }
    }





    void onMouseEnterUnit(InteractionEvent e)
    {

        if (PlayerManager.isOpponent(PlayerManager.localPlayer, e.targetUnit.player))
        {
            Cursor.SetCursor(m_attack, Vector2.zero, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(m_pointer, Vector2.zero, CursorMode.ForceSoftware);
        }
        //    Renderer renderer = e.target.GetComponentInChildren<Renderer>();
        //    Debug.Log("over:"+renderer.material.shader.name);
        //    if (renderer)
        //    {
        //        hoveredTargetShader = renderer.material.shader;
        //        renderer.material.shader = rimLight;
        //    }
    }
    void onMouseEnterObject(InteractionEvent e)
    {

      
       
            Cursor.SetCursor(m_pointer, Vector2.zero, CursorMode.ForceSoftware);
        
        //    Renderer renderer = e.target.GetComponentInChildren<Renderer>();
        //    Debug.Log("over:"+renderer.material.shader.name);
        //    if (renderer)
        //    {
        //        hoveredTargetShader = renderer.material.shader;
        //        renderer.material.shader = rimLight;
        //    }
    }
    //void onMouseOutUnit(InteractionEvent e)
    //{
    //    Renderer renderer = e.target.GetComponentInChildren<Renderer>();
    //    //debug.log(renderer.material.shader.name);
    //    Debug.Log("out:"+hoveredTargetShader.name);
    //    if (renderer)
    //    {

    //        renderer.material.shader = hoveredTargetShader;
    //    }
    //}
    public static Action SkillSelect(SkillSelectMode skillSelectMode)
    {
        return null;
    }
}
