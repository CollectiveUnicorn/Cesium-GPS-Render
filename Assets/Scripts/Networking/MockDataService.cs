using System;
using System.Collections.Generic;
using CesiumForUnity;
using SQLite4Unity3d;
using Unity.Mathematics;
using UnityEngine;

public class MockDataService : IDataService {
    private static double3 lastLatLongHeight = new double3(28.41878f, -80.60952f, 0f);
    private static List<double3> fakeLatLongs = new List<double3>() {
        new double3(28.41878f, -80.60952f, 0f),
        new double3(28.41878f, -80.60785f, 0f),
        new double3(28.41878f, -80.60501f, 0f),
        new double3(28.41878f, -80.60242f, 0f),
        new double3(28.41878f, -80.60242f, 0f),
        new double3(28.41877f, -80.6019f, 0f),
        new double3(28.41879f, -80.60154f, 0f),
        new double3(28.41883f, -80.60133f, 0f),
        new double3(28.41889f, -80.60113f, 0f),
        new double3(28.41902f, -80.60086f, 0f),
        new double3(28.41916f, -80.60066f, 0f),
        new double3(28.41936f, -80.60043f, 0f),
        new double3(28.41952f, -80.6003f, 0f),
        new double3(28.41968f, -80.6002f, 0f),
        new double3(28.41986f, -80.60012f, 0f),
        new double3(28.42004f, -80.60007f, 0f),
        new double3(28.42109f, -80.59985f, 0f),
        new double3(28.42137f, -80.59978f, 0f),
        new double3(28.42517f, -80.59908f, 0f),
        new double3(28.42696f, -80.59875f, 0f),
        new double3(28.42705f, -80.59873f, 0f),
        new double3(28.42866f, -80.59843f, 0f),
        new double3(28.42946f, -80.59826f, 0f),
        new double3(28.4311f, -80.59789f, 0f),
        new double3(28.43292f, -80.59734f, 0f),
        new double3(28.43438f, -80.59692f, 0f),
        new double3(28.43885f, -80.59561f, 0f),
        new double3(28.44231f, -80.59462f, 0f),
        new double3(28.44392f, -80.59412f, 0f),
        new double3(28.44536f, -80.59356f, 0f),
        new double3(28.44536f, -80.59356f, 0f),
        new double3(28.44549f, -80.5921f, 0f),
        new double3(28.44563f, -80.59083f, 0f),
        new double3(28.44579f, -80.58882f, 0f),
        new double3(28.44583f, -80.58842f, 0f),
        new double3(28.44605f, -80.58603f, 0f),
        new double3(28.44635f, -80.58277f, 0f),
        new double3(28.44645f, -80.58165f, 0f),
        new double3(28.4471f, -80.57427f, 0f),
        new double3(28.44717f, -80.57356f, 0f),
        new double3(28.44717f, -80.57356f, 0f)
    };
    private int currentWaypoint = 0;

    public MockDataService() {
        var jsonTextFile = Resources.Load<TextAsset>("fake_data");

        if (jsonTextFile != null)
        {
            String fakeJsonData = jsonTextFile.text;
            List<TrackedEntity> fakeEntities = HttpDataService.getTrackedEntityFromJsonText(fakeJsonData);

            if (fakeEntities.Count > 0)
                fakeLatLongs.Clear();
            
            foreach (var fakeEntity in fakeEntities)
            {
                fakeLatLongs.Add(new double3(fakeEntity.Lat, fakeEntity.Lon, fakeEntity.Alt));
            }
        }

        if (lastLatLongHeight.Equals(new double3(28.41878f, -80.60952f, 0f)))
            lastLatLongHeight = new double3(28.41878f, -80.60952f, 0f);
    }

    public MockDataService(string DatabaseName) : base() {
    }

    public IEnumerable<TrackedEntity> GetTrackedEntities(){
        currentWaypoint = Mathf.Min(currentWaypoint, fakeLatLongs.Count - 1);
        List<TrackedEntity> mockData = new List<TrackedEntity>();
        lastLatLongHeight.x = fakeLatLongs[currentWaypoint].x;
        lastLatLongHeight.y = fakeLatLongs[currentWaypoint].y;
        lastLatLongHeight.z = fakeLatLongs[currentWaypoint].z;
        mockData.Add(new TrackedEntity(1, "Test", lastLatLongHeight.x, lastLatLongHeight.y, lastLatLongHeight.z, DateTime.Now.ToString(),
            "Test First", "Test Last", "Test Company", "Test Phone Number"));
        int nextWaypoint = ++currentWaypoint;
        nextWaypoint = Mathf.Min(nextWaypoint, fakeLatLongs.Count - 1);
        lastLatLongHeight.x = fakeLatLongs[nextWaypoint].x;
        lastLatLongHeight.y = fakeLatLongs[nextWaypoint].y;
        lastLatLongHeight.z = fakeLatLongs[nextWaypoint].z;
        mockData.Add(new TrackedEntity(2, "Test1", lastLatLongHeight.x, lastLatLongHeight.y, lastLatLongHeight.z, DateTime.Now.ToString(),
            "Test First", "Test Last", "Test Company", "Test Phone Number"));
		return (IEnumerable<TrackedEntity>)mockData;
	}

    public IEnumerable<TrackedEntity> GetTrackedEntitiesByName(string name) { return (IEnumerable<TrackedEntity>)new List<TrackedEntity>(); }

	public TrackedEntity GetTrackedEntity(string name) { return new TrackedEntity(); }

	public TrackedEntity CreateTrackedEntity(string name, double latitude, double longitude, double height) { return new TrackedEntity(); }
}