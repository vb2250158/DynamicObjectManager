using System;
using System.Collections.Generic;
using DynamicObjectManager;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Serialization;
using Image = UnityEngine.UI.Image;

public class DynamicObject : MonoBehaviour
{
    [DynamicObjectKey] public string key;

    public List<DynamicObjectAction> dynamicDataList;

    /// <summary>
    /// 当前的状态
    /// </summary>
    public string condition;

    public event Action<string> OnUpdateConditionEvent;

    public virtual void OnUpdateCondition(string condition)
    {
        OnUpdateConditionEvent?.Invoke(condition);
    }

    private void Awake()
    {
        DynamicObjectController.AddDynamicObject(this);
    }

    private void Start()
    {
        UpdateCondition();
    }

    public void OnDestroy()
    {
        DynamicObjectController.RemoveDynamicObject(this);
    }

    public void UpdateCondition()
    {
        var condition = DynamicObjectController.GetDynamicCondition(key);
        if (condition != "")
            foreach (var item in dynamicDataList)
                if (item.condition == condition)
                {
                    this.condition = condition;
                    item.action.Invoke();
                    OnUpdateCondition(condition);
                    return;
                }
                else if (dynamicDataList.Count > 0)
                    //没有数据并且是有数据条件，设置第一个为默认数据条件，并且进行回调
                    SetCondition(dynamicDataList[0].condition);
    }

    public int GetIndex()
    {
        if (dynamicDataList.Count <= 0) return -1;
        var index = dynamicDataList.FindIndex(item => { return item.condition == condition; });
        if (index == -1)
        {
            index = 0;
            condition = dynamicDataList[0].condition;
        }

        return dynamicDataList.FindIndex(item => { return item.condition == condition; });
    }

    public void NextIndex()
    {
        var index = GetIndex();
        if (index != -1) index++;
        SetCondition(dynamicDataList[index].condition);
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="condition"></param>
    public void SetCondition(string condition)
    {
        DynamicObjectController.SetDynamicCondition(key, condition);
        UpdateCondition();
    }
}

public class DynamicObjectKeyAttribute : PropertyAttribute
{
}