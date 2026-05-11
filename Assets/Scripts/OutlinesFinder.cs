using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;

/// <summary>
/// OpenCV getting cam image and get outlines for colliders
/// </summary>
public class OutlinesFinder : WebCamera
{
    [SerializeField] RectTransform _rctf;
    [SerializeField] PolygonCollider2D _polygonCollider;
    [SerializeField] FlipMode _imageFlip;
    [SerializeField] Vector2[] _vectorArray;
    [SerializeField] float _threshold = 96.4f;
    [SerializeField] float _curveAccuracy = 10;
    [SerializeField] float _minArea = 5000;
    [SerializeField] bool _showProcessingImage = true;
    [SerializeField] bool _showContours = true;

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
                if (_showContours)
                    if (_showProcessingImage)
                        DrawContour(_processImage, new Scalar(127, 127, 127), 2, points);
                    else
                        DrawContour(_image, new Scalar(127, 127, 127), 2, points);

                _polygonCollider.pathCount++;
                _polygonCollider.SetPath(_polygonCollider.pathCount - 1, PointsToVector2(points));
            }
        }

        if (output == null)
        {
            if (_showProcessingImage)
                output = OpenCvSharp.Unity.MatToTexture(_processImage);
            else
                output = OpenCvSharp.Unity.MatToTexture(_image);
        }
        else
        {
            if (_showProcessingImage)
                OpenCvSharp.Unity.MatToTexture(_processImage, output);
            else
                OpenCvSharp.Unity.MatToTexture(_image, output);
        }

        return true;
    }

    Vector2[] PointsToVector2(Point[] points)
    {
        _vectorArray = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            float scaleX = _rctf.rect.width / _image.Width;
            float scaleY = _rctf.rect.height / _image.Height;

            _vectorArray[i] = new Vector2((points[i].X * scaleX) - (_rctf.rect.width * 0.5f), -(points[i].Y * scaleY) + (_rctf.rect.height * 0.5f));
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
