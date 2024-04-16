using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// This class is used store scene reference to save and load asset settings. If changing scene, the scene object reference may not work
    /// </summary>
    [ExecuteInEditMode]
    public class HSVExposeReferenceMgr : MonoBehaviour, IExposedPropertyTable
    {
        public List<PropertyName> listPropertyName;
        public List<UnityEngine.Object> listReference;

        private void Awake()
        {
            if(listPropertyName == null)
            {
                //Debug.Log("Initialized property");
                listPropertyName = new List<PropertyName>();
            }

            if(listReference == null)
            {
                //Debug.Log("Initialized reference");
                listReference = new List<Object>();
            }

            //Debug.Log(listPropertyName.Count + " " + listReference.Count);
        }

        public void ClearReferenceValue(PropertyName id)
        {
            int index = listPropertyName.IndexOf(id);
            if (index != -1)
            {
                //Debug.Log("Remove Reference " + id + " "+ index);
                listReference.RemoveAt(index);
                listPropertyName.RemoveAt(index);
            }
        }

        public Object GetReferenceValue(PropertyName id, out bool idValid)
        {
            int index = listPropertyName.IndexOf(id);
            if (index != -1)
            {
                //Debug.Log("Get Reference " + id + " " + index);
                idValid = true;
                return listReference[index];
            }
            idValid = false;
            return null;
        }
        public void SetReferenceValue(PropertyName id, Object value)
        {
            int index = listPropertyName.IndexOf(id);
            if (index != -1)
            {
                //Debug.Log("Set Reference " + id + " " + index);
                listReference[index] = value;
            }
            else
            {
                //Debug.Log("Set Reference " + id + " ");
                listPropertyName.Add(id);
                listReference.Add(value);
            }
        }
    }
}