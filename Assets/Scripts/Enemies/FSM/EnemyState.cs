using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState<T> : MonoBehaviour
{
    protected T controller;

    public virtual void OnEnterState(T controller) 
    {
        this.controller = controller;
    }

    public virtual void OnUpdateState()
    {

    }

    public virtual void OnExitState()
    {
    
    }
}