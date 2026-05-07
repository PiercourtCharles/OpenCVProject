using OpenCvSharp;
using OpenCvSharp.Demo;
using System.Collections.Generic;
using UnityEngine;

public class CounterFinder : WebCamera
{
    [SerializeField] FlipMode _imageFlip;
    [SerializeField] float _threshold = 96.4f;
    [SerializeField] bool _showProcessingImage = true;
    [SerializeField] float _curveAccuracy = 10;
    [SerializeField] float _minArea = 5000;
    [SerializeField] PolygonCollider2D _polygonCollider;
    [SerializeField] Vector2[] _vectorArray;

    Mat _image;
    Mat _processImage = new();
    Point[][] _contours;
    HierarchyIndex[] _hierarchy;

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        _image = OpenCvSharp.Unity.TextureToMat(input);


        Cv2.Flip(_image, _image, _imageFlip);
        Cv2.CvtColor(_image, _processImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(_processImage, _processImage, _threshold, 255, ThresholdTypes.BinaryInv);
        Cv2.FindContours(_processImage, out _contours, out _hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);

        _polygonCollider.pathCount = 0;

        foreach (Point[] contour in _contours)
        {
            Point[] points = Cv2.ApproxPolyDP(contour, _curveAccuracy, true);
            var area = Cv2.ContourArea(contour);

            if (area > _minArea)
            {
                DrawContour(_processImage, new Scalar(127, 127, 127), 2, points);

                _polygonCollider.pathCount++;
                _polygonCollider.SetPath(_polygonCollider.pathCount - 1, PointsToVector2(points));
            }
        }

        if (output == null)
            output = OpenCvSharp.Unity.MatToTexture(_showProcessingImage ? _processImage : _image);
        else
            OpenCvSharp.Unity.MatToTexture(_showProcessingImage ? _processImage : _image, output);

        return true;
    }

    Vector2[] PointsToVector2(Point[] points)
    {
        _vectorArray = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            _vectorArray[i] = new Vector2(points[i].X, points[i].Y);
        }

        return _vectorArray;
    }

    void DrawContour(Mat image, Scalar color, int thickness, Point[] points)
    {
        for (int i = 1; i < points.Length; i++)
        {
            Cv2.Line(image, points[i - 1], points[i], color, thickness);
        }

        Cv2.Line(image, points[points.Length - 1], points[0], color, thickness);
    }
}
