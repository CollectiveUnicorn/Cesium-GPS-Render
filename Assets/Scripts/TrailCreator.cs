using System;
using System.Collections.Generic;
using RTS_Cam;
using UnityEngine;

namespace SQLite4Unity3d
{
    public class TrailCreator : MonoBehaviour
    {
        public Material lineMaterial;
        public Color lineColor = Color.white;
        public float lineWidth = 1.0f;
        public float maxLineWidth = 50.0f;
        public float minLineWidth = 1.0f;
        private RTS_Camera camera;
        private GameObject lineParent;
        private GameObject lineInstance;
        public bool track = false;

        public void Start()
        {
            camera = GameObject.FindWithTag("MainCamera").GetComponent<RTS_Camera>();
            lineParent = GameObject.Find("Lines");
            CreateTrail();
            InvokeRepeating(nameof(UpdateLineInfo), 0f, 0.5f);
        }
        
        void UpdateLineInfo()
        {
            if (track)
            {
                float predictedLineWidth = Mathf.Lerp(maxLineWidth, minLineWidth, camera.zoomPos);
                if (!Mathf.Approximately(predictedLineWidth, lineWidth))
                {
                    lineWidth = predictedLineWidth;
                    lineInstance.GetComponent<LineRenderer>().startWidth = lineWidth;
                    lineInstance.GetComponent<LineRenderer>().endWidth = lineWidth;
                }

                UpdateTrail();
            }
        }
        
        private GameObject CreateTrail()
        {
            lineInstance = new GameObject("Line");
            lineInstance.tag = "Line";
            lineInstance.transform.SetParent(lineParent.transform);

            LineRenderer lineRenderer = lineInstance.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.SetPositions(Array.Empty<Vector3>());
            lineRenderer.positionCount = 0;

            return lineInstance;
        }
        
        public void UpdateTrail()
        {
            // Update the line renderer to include the current position
            LineRenderer lineRenderer = lineInstance.GetComponent<LineRenderer>();
            Vector3[] curPos = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(curPos);
            List<Vector3> allPos = new List<Vector3>(curPos);
            allPos.Add(transform.position);
            lineRenderer.positionCount++;
            lineRenderer.SetPositions(allPos.ToArray());
        }
    }
}