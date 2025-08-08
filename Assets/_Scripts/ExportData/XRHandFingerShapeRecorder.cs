using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace UnityEngine.XR.Hands.Samples.Gestures.DebugTools
{
    public class XRHandFingerShapeRecorder : MonoBehaviour
    {
        public static XRHandFingerShapeRecorder Instance { get; private set; }

        XRFingerShape[] m_LeftHandFingerShapes;
        XRFingerShape[] m_RightHandFingerShapes;

        static List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();

        List<string> csvData = new List<string>();

        string csvFilePath;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            m_LeftHandFingerShapes = new XRFingerShape[5];
            m_RightHandFingerShapes = new XRFingerShape[5];

            var header = new StringBuilder("Timestamp,Handedness,Finger,FullCurl,BaseCurl,TipCurl,Pinch,Spread");
            csvData.Add(header.ToString());
        }

        public void SetUp(string path)
        {
            csvFilePath = path;
            Debug.Log($"CSV will be saved to: {csvFilePath}");
        }

        void Update()
        {
            var subsystem = TryGetSubsystem();
            if (subsystem == null)
                return;

            var leftHand = subsystem.leftHand;
            var rightHand = subsystem.rightHand;

            for (var fingerIndex = (int)XRHandFingerID.Thumb; fingerIndex <= (int)XRHandFingerID.Little; ++fingerIndex)
            {
                m_LeftHandFingerShapes[fingerIndex] = leftHand.CalculateFingerShape((XRHandFingerID)fingerIndex, XRFingerShapeTypes.All);
                m_RightHandFingerShapes[fingerIndex] = rightHand.CalculateFingerShape((XRHandFingerID)fingerIndex, XRFingerShapeTypes.All);
            }
        }

        public void RecordCurrentFingerShapes()
        {
            var timestamp = Time.time;

            RecordHandFingerShapes(timestamp, Handedness.Left, m_LeftHandFingerShapes);
            RecordHandFingerShapes(timestamp, Handedness.Right, m_RightHandFingerShapes);

            SaveCSV();
        }

        void RecordHandFingerShapes(float timestamp, Handedness handedness, XRFingerShape[] fingerShapes)
        {
            for (var fingerIndex = (int)XRHandFingerID.Thumb; fingerIndex <= (int)XRHandFingerID.Little; ++fingerIndex)
            {
                var shapes = fingerShapes[fingerIndex];
                var fingerName = ((XRHandFingerID)fingerIndex).ToString();

                shapes.TryGetFullCurl(out var fullCurl);
                shapes.TryGetBaseCurl(out var baseCurl);
                shapes.TryGetTipCurl(out var tipCurl);
                shapes.TryGetPinch(out var pinch);
                shapes.TryGetSpread(out var spread);

                var line = $"{timestamp},{handedness},{fingerName},{fullCurl},{baseCurl},{tipCurl},{pinch},{spread}";
                csvData.Add(line);
            }
        }

        void SaveCSV()
        {
            if (string.IsNullOrEmpty(csvFilePath))
            {
                return;
            }
            File.WriteAllLines(csvFilePath, csvData);
        }

        static XRHandSubsystem TryGetSubsystem()
        {
            SubsystemManager.GetSubsystems(s_SubsystemsReuse);
            return s_SubsystemsReuse.Count > 0 ? s_SubsystemsReuse[0] : null;
        }
    }
}
