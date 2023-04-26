using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor.ShaderGraph.Serialization;

namespace SQLite4Unity3d
{
    public class HttpDataService : MonoBehaviour, IDataService
    {
        private List<TrackedEntity> loadedTrackedEntities = new List<TrackedEntity>();
 
        IEnumerator GetText() {
            UnityWebRequest www = UnityWebRequest.Get(EntityTracker.instance.url);
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + EntityTracker.instance.bearerToken);
            yield return www.SendWebRequest();
 
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.error);
            }
            else {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
 
                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }
        }

        public static List<TrackedEntity> getTrackedEntityFromJsonText(String jsonString)
        {
            JArray jsonStringVar = JArray.Parse(jsonString);
            List<TrackedEntity> deserializeObject = JsonConvert.DeserializeObject<List<TrackedEntity>>(jsonStringVar.ToString());
            return new List<TrackedEntity>(deserializeObject);
        }
        
        IEnumerator GetJsonData()
        {
            if (EntityTracker.instance == null)
                yield return null;

            UnityWebRequest www;
            
            if (EntityTracker.instance.REALTIME)
                www = UnityWebRequest.Get("http://ec2-3-80-228-237.compute-1.amazonaws.com:5000/get_latest_data"); 
            else
                www = UnityWebRequest.Get("http://ec2-3-80-228-237.compute-1.amazonaws.com:5000/get_data");
            
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + EntityTracker.instance.bearerToken);
            yield return www.SendWebRequest();
 
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.error);
            }
            else {
                // Show results as text
                // Debug.Log(www.downloadHandler.text);

                loadedTrackedEntities = getTrackedEntityFromJsonText(www.downloadHandler.text);
            }
        }

        public IEnumerable<TrackedEntity> GetTrackedEntities()
        {
            StartCoroutine(GetJsonData());
            return loadedTrackedEntities;
        }

        public IEnumerable<TrackedEntity> GetTrackedEntitiesByName(string Uuid)
        {
            return loadedTrackedEntities.FindAll(entity => entity.UUID == Uuid);
        }

        public TrackedEntity GetTrackedEntity(string Uuid)
        {
            return loadedTrackedEntities.Find(entity => entity.UUID == Uuid);
        }

        public TrackedEntity CreateTrackedEntity(string Uuid, double latitude, double longitude, double height)
        {
            throw new NotImplementedException();
        }
    }
}