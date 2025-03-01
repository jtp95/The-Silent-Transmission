using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe;
using TMPro;


public class SignRecognizer : MonoBehaviour
{
    private InferenceSession session;
    private WebCamTexture webcamTexture;
    public RawImage webcamDisplay;
    public RawImage handDisplay;
    private HandLandmarker handLandmarker; // Correct replacement for HandTrackingGraph
    private string modelPath;
    private int inputSize = 42; // 21 landmarks (x, y)
    public TextMeshProUGUI text;
    private LineRenderer handRenderer;

    void Start()
    {
        // Load ONNX Model
        modelPath = Application.dataPath + "/Models/signlang_model.onnx";
        session = new InferenceSession(modelPath);
        Debug.Log("ONNX model loaded!");

        // Start Webcam
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();

        if (webcamDisplay != null)
        {
            webcamDisplay.texture = webcamTexture;  // Display webcam on UI
        }
        else
        {
            Debug.LogWarning("RawImage (WebcamDisplay) not assigned! Please set it in the Inspector.");
        }

        handLandmarker = HandLandmarker.CreateFromModelPath(Application.dataPath + "/Models/hand_landmarker.task");

        // Create a GameObject for hand debugging
        InitializeHandRenderer();
    }

    void Update()
    {
        if (webcamTexture.didUpdateThisFrame)
        {
            Texture2D tex = new Texture2D(webcamTexture.width, webcamTexture.height);
            tex.SetPixels32(webcamTexture.GetPixels32());
            tex.Apply();

            float[] handLandmarks = GetHandLandmarks(tex);

            if (handLandmarks == null)
            {
                Debug.Log("No hand detected.");
                handRenderer.enabled = false;  // Hide visualization if no hand detected
                return;
            }

            // Enable hand visualization
            handRenderer.enabled = true;

            // Draw hand landmarks
            DrawHandKeypoints(handLandmarks);

            // Run ONNX Inference
            string predictedSign = PredictHandSign(handLandmarks);
            Debug.Log($"Predicted Sign: {predictedSign}");
            text.text = predictedSign;
        }
    }

    private void InitializeHandRenderer()
    {
        // Create a new GameObject for drawing if one doesn't exist.
        GameObject debugObj = new GameObject("HandDebug");
        debugObj.transform.SetParent(handDisplay.transform, false); // Make it a child of the RawImage
        handRenderer = debugObj.AddComponent<LineRenderer>();
        handRenderer.useWorldSpace = true; // ✅ Enable World Space
        handRenderer.material = new Material(Shader.Find("Sprites/Default")); // ✅ Ensure visible material
        handRenderer.startColor = UnityEngine.Color.red; // ✅ Make lines visible
        handRenderer.endColor = UnityEngine.Color.red;
        handRenderer.startWidth = 10f; // ✅ Increase line thickness
        handRenderer.endWidth = 10f;
        handRenderer.positionCount = 0;
        handRenderer.sortingOrder = 10;
        handRenderer.useWorldSpace = false; // So positions are relative to the RawImage
    }
    private void DrawHandKeypoints(float[] landmarkData)
    {
        // Check that landmarkData has 42 values (21 points * 2)
        if (landmarkData == null || landmarkData.Length < 42)
        {
            Debug.LogError("Invalid landmark data size.");
            return;
        }

        // Get the RawImage's RectTransform dimensions
        RectTransform rt = handDisplay.rectTransform;
        float width = rt.rect.width;
        float height = rt.rect.height;

        // Convert normalized landmark coordinates to pixel positions in the UI
        // Assuming landmarks are in [0,1] where (0,0) is the top-left.
        Vector3[] positions = new Vector3[21];
        for (int i = 0; i < 21; i++)
        {
            // Get normalized coordinates
            float normX = landmarkData[i * 2];
            float normY = landmarkData[i * 2 + 1];

            // Convert to pixel positions relative to RawImage:
            // Here, (0,0) is assumed at top-left. If your UI uses center as pivot, adjust accordingly.
            float pixelX = normX * width;
            float pixelY = (1 - normY) * height; // Flip Y axis if necessary

            // For UI, you may need to offset so that (0,0) is at the RawImage's bottom-left:
            Vector3 screenPos = new Vector3(pixelX, pixelY, 0);
            positions[i] = Camera.main.ScreenToWorldPoint(screenPos);
        }

        // Define connections (using typical hand skeleton connections)
        int[] connections = new int[]
        {
            0,1, 1,2, 2,3, 3,4,        // Thumb
            0,5, 5,6, 6,7, 7,8,        // Index finger
            5,9, 9,10, 10,11, 11,12,   // Middle finger
            9,13, 13,14, 14,15, 15,16,  // Ring finger
            13,17, 17,18, 18,19, 19,20, // Pinky finger
            0,17                      // Palm connection
        };

        // Build a list of positions to draw the lines
        List<Vector3> linePositions = new List<Vector3>();
        for (int i = 0; i < connections.Length; i += 2)
        {
            int startIdx = connections[i];
            int endIdx = connections[i + 1];
            if (startIdx < positions.Length && endIdx < positions.Length)
            {
                linePositions.Add(positions[startIdx]);
                linePositions.Add(positions[endIdx]);
            }
        }

        // Update the LineRenderer
        handRenderer.positionCount = linePositions.Count;
        handRenderer.SetPositions(linePositions.ToArray());
    }
    private float[] GetHandLandmarks(Texture2D frame)
    {
        if (handLandmarker == null) return null;

        var image = new Mediapipe.Image(frame);
        
        var result = handLandmarker.Detect(image);

        if (result.handLandmarks == null || result.handLandmarks.Count == 0) return null;

        float[] landmarkData = new float[inputSize];

        for (int i = 0; i < result.handLandmarks[0].landmarks.Count; i++)
        {
            landmarkData[i * 2] = result.handLandmarks[0].landmarks[i].x;
            landmarkData[i * 2 + 1] = result.handLandmarks[0].landmarks[i].y;
        }

        return landmarkData;
    }

    private string PredictHandSign(float[] inputData)
    {
        if (session == null)
        {
            Debug.LogError("ERROR: ONNX session not loaded.");
            return "Error: Model not loaded";
        }

        if (inputData == null || inputData.Length != 42)
        {
            Debug.LogError("ERROR: Invalid landmark data! inputData is null or has incorrect size.");
            return "Error: Invalid input data";
        }

        var inputTensor = new DenseTensor<float>(inputData, new int[] { 1, 42 });

        using (var results = session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", inputTensor) }))
        {
            if (results == null || results.Count == 0)
            {
                Debug.LogError("ERROR: ONNX inference returned null!");
                return "Error: ONNX inference failed";
            }

            var outputTensor = results[0].AsTensor<string>();

            return outputTensor.ToArray()[0];
        }
    }
}