using System;
using System.Collections.Generic;
using CesiumForUnity;
using RTS_Cam;
using SQLite4Unity3d;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class EntityTracker : MonoBehaviour
{
    public static EntityTracker instance;
    private IDataService ds;
    private List<TrackedEntity> trackedEntities = new List<TrackedEntity>();
    private Dictionary<String, TrackedEntity> trackedEntitiesById = new Dictionary<String, TrackedEntity>();
    private Dictionary<String, GameObject> trackedObjectById = new Dictionary<String, GameObject>();
    private Dictionary<String, DateTime> trackedEntityLastTimeUpdate = new Dictionary<String, DateTime>();
    private Dictionary<String, List<TrackedEntity>> trackedEntityPosList = new Dictionary<String, List<TrackedEntity>>();
    private Dictionary<String, int> trackedEntityCurId = new Dictionary<String, int>();
    public GameObject dummyTrackedEntity;
    public GameObject latLongCenter;
    public bool testing = false;
    private float lastTime = 0.0f;
    private float updateIntervalInSeconds = 1.0f;
    public RTS_Camera camera = null;
    public UIManager gui;
    public String bearerToken = "";
    public String url = "";
    public Material lineMaterial;
    public Color lineColor = Color.white;
    public GameObject nameplatePrefab;
    public GameObject nameplateParent;
    public const double START_HEIGHT = -25.7698;
    public bool REALTIME = true;

    public void ToggleRealtime()
    {
        ClearEntityList();
        REALTIME = !REALTIME;
        lastTime = 0.0f;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (testing)
            ds = new MockDataService();
        else
            ds = GetComponent<HttpDataService>();

		trackedEntities = (List<TrackedEntity>)ds.GetTrackedEntities ();
        instance = this;
    }

    void FixedUpdate() {
        if (Time.time - lastTime > updateIntervalInSeconds) {
            lastTime = Time.time;
            trackedEntities = (List<TrackedEntity>)ds.GetTrackedEntities ();
        }

        UpdateEntityList();
    }

    private void UpdateTrackedEntityData() {
        trackedEntities = (List<TrackedEntity>)ds.GetTrackedEntities ();
    }

    public void ClearEntityList()
    {
        gui.ClearEntities();
        var allNameplates = GameObject.FindGameObjectsWithTag("Nameplate");
        var childUis = GameObject.FindGameObjectsWithTag("ChildUI");
        var lines = GameObject.FindGameObjectsWithTag("Line");
        
        foreach (var line in lines)
        {
            Destroy(line);
        }
        
        foreach (var nameplate in allNameplates)
        {
            Destroy(nameplate);
        }
        
        foreach (var childUi in childUis)
        {
            Destroy(childUi);
        }
        
        foreach (var trackedObject in trackedObjectById)
        {
            Destroy(trackedObject.Value);
        }
        trackedEntities.Clear();
        trackedEntitiesById.Clear();
        trackedEntityCurId.Clear();
        trackedEntityLastTimeUpdate.Clear();
        trackedEntityPosList.Clear();
        trackedObjectById.Clear();
    }

    private void UpdateEntityList()
    {
        HashSet<String> hasUpdated = new HashSet<String>();
        foreach (var entity in trackedEntities) {
            if (!hasUpdated.Contains(entity.UUID))
            {
                DateTime curDateTime = DateTime.Parse(entity.Time);
                if (trackedEntitiesById.ContainsKey(entity.UUID))
                {
                    var trackedEntityInfo = trackedObjectById[entity.UUID].GetComponent<EntityInfo>();
                    bool isSameTime = (curDateTime == trackedEntityLastTimeUpdate[entity.UUID]);
                    if (curDateTime > trackedEntityLastTimeUpdate[entity.UUID] || (isSameTime && !trackedEntityInfo.hasReachedPosition))
                    {
                        var trackedEntity = trackedObjectById[entity.UUID]
                            .GetComponent<CesiumGlobeAnchor>();
                        UpdateTrackedEntity(ref trackedEntity, entity, trackedObjectById[entity.UUID], entity.UUID);
                        trackedEntityLastTimeUpdate[entity.UUID] = curDateTime;

                        hasUpdated.Add(entity.UUID);
                    }
                }
                else
                {
                    trackedEntityCurId.Add(entity.UUID, 0);
                    trackedEntitiesById.Add(entity.UUID, entity);
                    trackedEntityLastTimeUpdate.Add(entity.UUID, curDateTime);
                    trackedEntityPosList.Add(entity.UUID, new List<TrackedEntity>());
                    String fullName = entity.CompanyName + ": " + entity.FirstName + " " + entity.LastName;
                    GameObject newTrackedEntity = CreateTrackedEntity(entity.UUID, fullName, entity);
                    trackedObjectById.Add(entity.UUID, newTrackedEntity);
                    hasUpdated.Add(entity.UUID);
                }
            }
        }
    }

    private GameObject CreateTrackedEntity(String uuid, String fullName, TrackedEntity firstPos) {
        GameObject newTrackedEntity = Instantiate(dummyTrackedEntity, latLongCenter.transform);
        CesiumGlobeAnchor anchor = newTrackedEntity.AddComponent<CesiumGlobeAnchor>();
        var entityInfo = newTrackedEntity.AddComponent<EntityInfo>();
        entityInfo.truckId = uuid;
        TrailCreator trailCreator = newTrackedEntity.AddComponent<TrailCreator>();
        trailCreator.lineMaterial = lineMaterial;
        trailCreator.lineColor = lineColor;
        var nameplate = Instantiate(nameplatePrefab, nameplateParent.transform);
        nameplate.GetComponentInChildren<TextMeshPro>().text = fullName;
        nameplate.GetComponent<Nameplate>().target = newTrackedEntity.transform;

        anchor.longitudeLatitudeHeight = new double3(firstPos.Lon, firstPos.Lat, START_HEIGHT);
        
        if (camera != null)
            camera.SetTarget(newTrackedEntity.transform);
        
        // Update the UI for the new tracked entity
        gui.AddEntityToUI(uuid, fullName, newTrackedEntity);

        return newTrackedEntity;
    }

    private void UpdateTrackedEntity(ref CesiumGlobeAnchor existingEntity, TrackedEntity newEntity, GameObject obj, String curId)
    {
        DateTime targetTime = DateTime.Parse(newEntity.Time);
        DateTime curTime = obj.GetComponent<EntityInfo>().curTime;

        if (curTime == DateTime.MinValue)
            curTime = targetTime;

        TimeSpan timeSpan = new TimeSpan(targetTime.Ticks - curTime.Ticks);

        obj.GetComponent<EntityInfo>().targetPosition = new double3(newEntity.Lat, newEntity.Lon, newEntity.Alt);
        Vector3 pos = obj.transform.position;
        double3 cesiumNewPos = new double3(newEntity.Lon, newEntity.Lat, 0);
        double3 cesiumCurPos = existingEntity.longitudeLatitudeHeight;
        double3 cesiumPosDelta = (cesiumNewPos - cesiumCurPos) * Time.deltaTime;
        double3 heightAdjustedLocation = existingEntity.longitudeLatitudeHeight + cesiumPosDelta;
        heightAdjustedLocation.z = START_HEIGHT;

        // Only update the position and rotation if the entity has not reached its destination
        if (cesiumPosDelta.x > 0.000001 || cesiumPosDelta.y > 0.000001)
        {
            existingEntity.longitudeLatitudeHeight = heightAdjustedLocation;

            Vector3 newPos = obj.transform.position;
            Vector3 targetDirection = pos - newPos;
            float speed = 300.0f;
            // The step size is equal to speed times frame time.
            float singleStep = speed * Time.deltaTime;
            // Debug.Log(targetDirection);
            Vector3 normTargetDirection = Vector3.Normalize(new Vector3(targetDirection.x * 1000f,
                targetDirection.y * 1000f, targetDirection.z * 1000f));

            // Debug.Log(normTargetDirection);
            Vector3 newDirection = Vector3.RotateTowards(obj.transform.forward, -normTargetDirection, singleStep, 0.0f);
            // Debug.Log(newDirection);
            Quaternion newRotation = Quaternion.LookRotation(newDirection);
            existingEntity.rotationEastUpNorth =
                new quaternion(newRotation.x, newRotation.y, newRotation.z, newRotation.w);
            obj.GetComponent<EntityInfo>().hasReachedPosition = false;
        }
        else
        {
            obj.GetComponent<EntityInfo>().hasReachedPosition = true;
            obj.GetComponent<EntityInfo>().curTime = targetTime;
            obj.GetComponent<TrailCreator>().track = true;
        }
    }
}