using System;
using System.Collections.Generic;
using RTS_Cam;
using UnityEngine;
using UnityEngine.UI;

namespace SQLite4Unity3d
{
    [Serializable]
    public class UIManager
    {
        public GameObject entityList;
        public GameObject entityPrefab;
        public Dictionary<String, GameObject> uiEntityIdMap = new Dictionary<String, GameObject>();
        public Dictionary<String, GameObject> vehicleEntityIdMap = new Dictionary<String, GameObject>();
        public RTS_Camera camera;

        public void AddEntityToUI(String id, String entityName, GameObject vehicle)
        {
            GameObject newEntity = GameObject.Instantiate(entityPrefab, entityList.transform);
            
            Text newEntityText = newEntity.GetComponentInChildren<Text>();
            newEntityText.text = entityName;

            newEntity.GetComponent<Button>().onClick.AddListener(() =>
            {
                camera.SetTarget(vehicle.transform);
            });
            uiEntityIdMap.Add(id, newEntity);
            vehicleEntityIdMap.Add(id, vehicle);
        }

        public void ClearEntities()
        {
            foreach (var entity in uiEntityIdMap)
            {
                GameObject.Destroy(entity.Value);
            }
            uiEntityIdMap.Clear();
            vehicleEntityIdMap.Clear();
        }

        public void RemoveUIEntity(String id)
        {
            GameObject.Destroy(uiEntityIdMap[id]);
            uiEntityIdMap.Remove(id);
            vehicleEntityIdMap.Remove(id);
        }
    }
}