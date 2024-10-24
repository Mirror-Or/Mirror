using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T>
{
    void OperateEnter(T sender);
    void OperateUpdate(T sender);
    void OperateExit(T sender);
}
