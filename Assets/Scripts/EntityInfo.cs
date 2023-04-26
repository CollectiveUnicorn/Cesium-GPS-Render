using System;
using Unity.Mathematics;
using UnityEngine;

namespace SQLite4Unity3d
{
    public class EntityInfo : MonoBehaviour
    {
        public double3 targetPosition = new double3();
        public bool hasReachedPosition = false;
        public DateTime curTime = DateTime.MinValue;
        public String truckId;
    }
}