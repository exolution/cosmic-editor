using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public interface Action
{
    bool isDone();
    void pause();
    void resumue();
    void finish();
}

public class ActionManager : MonoBehaviour {


	class ActionExecutor:Action
    {
        IEnumerator enumerator;
        ActionExecutor parent;
		bool done=false;
        public ActionExecutor(IEnumerator enumerator,ActionExecutor parent=null)
        {
            this.enumerator = enumerator;
            this.parent = parent;
        }
        public bool run()
        {
            
            bool flag;
            object current = this.enumerator.Current;
            if(current is Action)
            {
                if((current as Action).isDone())
                {                   
                    flag = this.enumerator.MoveNext();                    
                }
                else
                {
                    return true;
                }
            }
            else if(current is IEnumerator)
            {
                this.parent = new ActionExecutor(this.enumerator, this.parent);
                this.enumerator = current as IEnumerator;
                return this.run();
                
            }
           else
            {
                Debug.Log("run");
                flag = this.enumerator.MoveNext();
            }
            if (!flag&&this.parent!=null)
            {
                this.enumerator = this.parent.enumerator;
                this.parent = this.parent.parent;              
                return this.enumerator.MoveNext();
            }
            return flag;           
        }
		public bool isDone(){
			return done;
		}

        public void pause(){

		}
        public void resumue(){

		}
        public void finish(){
			done = true;
		}
    }


    public static ActionManager Instance
    {
        get;
        private set;
    }
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("ActionManager must be added to just only one gameobject");
        }
    }
    List<ActionExecutor> executorList = new List<ActionExecutor>(); 
	// Use this for initialization
	void Start () {
	
	}
	void Update()
    {
      
    }

	void LateUpdate () {
        Debug.Log("late");
        for(int i = 0; i < executorList.Count; i++)
        {
            if (!executorList[i].run()) {
				executorList[i].finish();
                executorList.RemoveAt(i);
                i--;
            };
        }	
	}
    public static Action excuteAction(IEnumerator enumerator)
    {
        ActionExecutor actionExecutor = new ActionExecutor(enumerator);
		Instance.executorList.Add (actionExecutor);
		return actionExecutor;
    }

}
