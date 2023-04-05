using System;
using UnityEngine;

// progress step for storing and executing story events
[CreateAssetMenu(fileName = "new Scripted Progress Step", menuName = "Progress/ScriptedProgressStep")]
public class ScriptedProgressStep : ProgressStep
{
    [Header("Scripted Actions")]
    [SerializeField] private EventContainer[] scriptedEvents;

    public void Execute()
    {
        Debug.Log("Executing scripted progress step " + this.name);
        foreach (EventContainer container in scriptedEvents)
        {
            try
            {
                Type targetType = Type.GetType(container.targetClass);
                targetType.GetMethod(container.targetMethod).Invoke(FindObjectOfType(targetType), container.parameters);
            }
            catch (Exception e)
            {
                Debug.LogError("Execution of EventContainer " + container + " was not possible! An Exception occured:\n" + e.Message);
            }
        }
    }

    [Serializable]
    private class EventContainer
    {
        public string targetClass;
        public string targetMethod;
        public string[] parameters;

        public override string ToString()
        {
            string ret = targetClass + "." + targetMethod + "(";
            foreach (string s in parameters)
            {
                ret += s + ",";
            }
            ret += ")";
            return ret;
        }
    }
}
