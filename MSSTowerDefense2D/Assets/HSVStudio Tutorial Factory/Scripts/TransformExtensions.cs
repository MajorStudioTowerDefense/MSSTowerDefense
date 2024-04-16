using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace HSVStudio.Tutorial
{
    public static class TransformExtensions
    {
        private static Vector3[] m_Corners = new Vector3[4];
        /// <summary>
        /// transform extension to calculate recttransform bounds in local space
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Bounds CalculateRelativeLocalBounds(this Transform trans)
        {
            var component = trans as RectTransform;
            if (component == null)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            var v1 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var v2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            component.GetLocalCorners(m_Corners);
            for (int i = 0; i < 4; ++i)
            {
                v1 = Vector3.Min(m_Corners[i], v1);
                v2 = Vector3.Max(m_Corners[i], v2);
            }

            var bounds = new Bounds(v1, Vector3.zero);
            bounds.Encapsulate(v2);
            return bounds;
        }

        /// <summary>
        /// Translate bounds using translation matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Bounds TranslateBounds(Matrix4x4 matrix, Bounds bounds)
        {
            return TransformExtensionUtility.TranslateBounds(ref matrix, ref bounds);
        }
    }

    public static class TransformExtensionUtility
    {
        private class CalculatePointResult
        {
            public Vector3 point;
            public CalculatePointResult[] edpts;
            public bool check;
        }
        //this is specified for the purpose of the bounds
        private static CalculatePointResult[] s_ptResults;
        private static List<Vector3> boundPoints;
        static TransformExtensionUtility()
        {
            //Initialize the ptResult array
            s_ptResults = new CalculatePointResult[8];
            for (int i = 0; i < s_ptResults.Length; i++)
            {
                s_ptResults[i] = new CalculatePointResult();
                s_ptResults[i].edpts = new CalculatePointResult[3];
            }
            InitializedEdgePoint(s_ptResults[0], s_ptResults[1], s_ptResults[2], s_ptResults[4]);
            InitializedEdgePoint(s_ptResults[1], s_ptResults[0], s_ptResults[3], s_ptResults[5]);
            InitializedEdgePoint(s_ptResults[2], s_ptResults[0], s_ptResults[3], s_ptResults[6]);
            InitializedEdgePoint(s_ptResults[3], s_ptResults[1], s_ptResults[2], s_ptResults[7]);
            InitializedEdgePoint(s_ptResults[4], s_ptResults[0], s_ptResults[5], s_ptResults[6]);
            InitializedEdgePoint(s_ptResults[5], s_ptResults[1], s_ptResults[4], s_ptResults[7]);
            InitializedEdgePoint(s_ptResults[6], s_ptResults[2], s_ptResults[4], s_ptResults[7]);
            InitializedEdgePoint(s_ptResults[7], s_ptResults[3], s_ptResults[6], s_ptResults[5]);

            boundPoints = new List<Vector3>();
        }

        /// <summary>
        /// Initialize edge point array
        /// </summary>
        /// <param name="source"></param>
        /// <param name="edge1"></param>
        /// <param name="edge2"></param>
        /// <param name="edge3"></param>
        private static void InitializedEdgePoint(CalculatePointResult source, CalculatePointResult edge1, CalculatePointResult edge2, CalculatePointResult edge3)
        {
            source.edpts[0] = edge1;
            source.edpts[1] = edge2;
            source.edpts[2] = edge3;
            source.check = false;
        }

        /// <summary>
        /// Convert screen rect to local rect
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="screenRect"></param>
        /// <param name="cam"></param>
        /// <param name="localRect"></param>
        /// <returns></returns>
        public static bool ScreenRectToLocalRectInRectangle(RectTransform rt, Rect screenRect, Camera cam, out Rect localRect)
        {
            Vector2 min, max;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenRect.min, cam, out min) &&
               RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenRect.max, cam, out max))
            {
                localRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
                return true;
            }

            localRect = Rect.zero;
            return false;
        }

        /// <summary>
        /// Convert world space bound to screen rect
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="bounds"></param>
        /// <param name="isInFrustumCheck"></param>
        /// <returns></returns>
        public static Rect BoundsToScreenRect(Camera cam, Bounds[] bounds, bool isInFrustumCheck)
        {
            if (!isInFrustumCheck)
            {
                return BoundsToScreenRect(cam, bounds);
            }

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;

            int i;
            for (i = 0; i < bounds.Length; ++i)
            {
                if (GeometryUtility.TestPlanesAABB(planes, bounds[i]))
                {
                    Rect rect = BoundsToScreenRect(cam, bounds[i]);
                    min = rect.min;
                    max = rect.max;
                    break;
                }
            }

            for (i = 0; i < bounds.Length; ++i)
            {
                if (!GeometryUtility.TestPlanesAABB(planes, bounds[i]))
                {
                    continue;
                }

                Rect rect = BoundsToScreenRect(cam, bounds[i]);
                min.x = Mathf.Min(rect.min.x, min.x);
                min.y = Mathf.Min(rect.min.y, min.y);
                max.x = Mathf.Max(rect.max.x, max.x);
                max.y = Mathf.Max(rect.max.y, max.y);
            }

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        /// <summary>
        /// Convert world space bound to screen rect
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Rect BoundsToScreenRect(Camera cam, Bounds[] bounds)
        {
            if (bounds.Length == 0)
            {
                return Rect.zero;
            }

            Rect rect = BoundsToScreenRect(cam, bounds[0]);
            Vector2 min = rect.min;
            Vector2 max = rect.max;

            for (int i = 1; i < bounds.Length; ++i)
            {
                rect = BoundsToScreenRect(cam, bounds[i]);
                min.x = Mathf.Min(rect.min.x, min.x);
                min.y = Mathf.Min(rect.min.y, min.y);
                max.x = Mathf.Max(rect.max.x, max.x);
                max.y = Mathf.Max(rect.max.y, max.y);
            }

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        /// <summary>
        /// Convert world space bound to screen rect
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Rect BoundsToScreenRect(Camera cam, Bounds bounds)
        {
            Vector3 cen = bounds.center;
            Vector3 ext = bounds.extents;

            Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z));
            Vector2 max = min;

            Vector2 point = min;
            GetMinMax(point, ref min, ref max);

            point = RectTransformUtility.WorldToScreenPoint(cam, new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z));
            GetMinMax(point, ref min, ref max);

            point = RectTransformUtility.WorldToScreenPoint(cam, new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z));
            GetMinMax(point, ref min, ref max);

            point = RectTransformUtility.WorldToScreenPoint(cam, new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z));
            GetMinMax(point, ref min, ref max);

            point = RectTransformUtility.WorldToScreenPoint(cam, new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z));
            GetMinMax(point, ref min, ref max);

            point = RectTransformUtility.WorldToScreenPoint(cam, new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z));
            GetMinMax(point, ref min, ref max);

            point = RectTransformUtility.WorldToScreenPoint(cam, new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z));
            GetMinMax(point, ref min, ref max);

            point = RectTransformUtility.WorldToScreenPoint(cam, new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z));
            GetMinMax(point, ref min, ref max);

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        /// <summary>
        /// Convert world space bound to screen rect
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Rect BoundsToScreenRectMinMax(Camera cam, Bounds bounds)
        {
            Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, bounds.min);
            Vector2 max = min;
            Vector2 point = min;
            GetMinMax(point, ref min, ref max);
            point = RectTransformUtility.WorldToScreenPoint(cam, bounds.max);
            GetMinMax(point, ref min, ref max);
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        /// <summary>
        /// Gets min max for the point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private static void GetMinMax(Vector2 point, ref Vector2 min, ref Vector2 max)
        {
            min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
            max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);
        }

        /// <summary>
        /// Gets center of the target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 GetCenter(this Transform target)
        {
            MeshFilter filter = target.GetComponent<MeshFilter>();
            if (filter != null && filter.sharedMesh != null)
            {
                return target.TransformPoint(filter.sharedMesh.bounds.center);
            }

            SkinnedMeshRenderer smr = target.GetComponent<SkinnedMeshRenderer>();
            if (smr != null && smr.sharedMesh != null)
            {
                return target.TransformPoint(smr.sharedMesh.bounds.center);
            }

            return target.position;
        }

        /// <summary>
        /// Gets common center for all transform
        /// </summary>
        /// <param name="transforms"></param>
        /// <returns></returns>
        public static Vector3 GetCommonCenter(IList<Transform> transforms)
        {
            Vector3 centerPosition = GetCenter(transforms[0]);
            for (int i = 1; i < transforms.Count; ++i)
            {
                Transform target = transforms[i];
                centerPosition += GetCenter(target);
            }

            centerPosition = centerPosition / transforms.Count;
            return centerPosition;
        }

        /// <summary>
        /// Gets center point for all the vector
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static Vector3 CenterPoint(Vector3[] vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }

            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }
            return sum / vectors.Length;
        }

        /// <summary>
        /// Calculate regluar bounds for all the transforms
        /// </summary>
        /// <param name="transforms"></param>
        /// <returns></returns>
        public static Bounds CalculateBounds(Transform[] transforms)
        {
            CalculateBoundsResult result = new CalculateBoundsResult();
            for (int i = 0; i < transforms.Length; ++i)
            {
                Transform t = transforms[i];
                CalculateBounds(t, result);
            }

            if (result.Initialized)
            {
                return result.Bounds;
            }

            Vector3 center = CenterPoint(transforms.Select(t => t.position).ToArray());
            return new Bounds(center, Vector3.zero);
        }

        private static CalculateBoundsResult s_result = new CalculateBoundsResult();
        /// <summary>
        /// Calculate regluar bounds for the transform
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Bounds CalculateBounds(Transform transform)
        {
            s_result.Initialized = false;

            CalculateBounds(transform, s_result);

            if (s_result.Initialized)
            {
                return s_result.Bounds;
            }

            return new Bounds(transform.position, Vector3.zero);
        }

        private class CalculateBoundsResult
        {
            public Bounds Bounds;
            public bool Initialized;
        }

        /// <summary>
        /// Calculate regluar bounds for the transform
        /// </summary>
        /// <param name="t"></param>
        /// <param name="result"></param>
        private static void CalculateBounds(Transform t, CalculateBoundsResult result)
        {
            if (t is RectTransform)
            {
                CalculateBounds((RectTransform)t, result);
            }
            else
            {
                Renderer renderer = t.GetComponent<Renderer>();
                if (renderer != null)
                {
                    CalculateBounds(renderer, result);
                }
            }

            foreach (Transform child in t)
            {
                CalculateBounds(child, result);
            }
        }

        /// <summary>
        /// Calculate regluar bounds for recttransform
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="result"></param>
        private static void CalculateBounds(RectTransform rt, CalculateBoundsResult result)
        {
            var relativeBounds = rt.CalculateRelativeLocalBounds();
            var localToWorldMatrix = rt.localToWorldMatrix;
            var bounds = TranslateBounds(ref localToWorldMatrix, ref relativeBounds);
            if (!result.Initialized)
            {
                result.Bounds = bounds;
                result.Initialized = true;
            }
            else
            {
                result.Bounds.Encapsulate(bounds.min);
                result.Bounds.Encapsulate(bounds.max);
            }
        }

        /// <summary>
        /// Calculate regluar bounds for the renderer
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="result"></param>
        private static void CalculateBounds(Renderer renderer, CalculateBoundsResult result)
        {
            if (renderer is ParticleSystemRenderer)
            {
                return;
            }

            var bounds = renderer.bounds;
            if (bounds.size == Vector3.zero && bounds.center != renderer.transform.position)
            {
                var localToWorldMatrix = renderer.transform.localToWorldMatrix;
                bounds = TranslateBounds(ref localToWorldMatrix, ref bounds);
            }

            if (!result.Initialized)
            {
                result.Bounds = bounds;
                result.Initialized = true;
            }
            else
            {
                result.Bounds.Encapsulate(bounds.min);
                result.Bounds.Encapsulate(bounds.max);
            }
        }

        /// <summary>
        /// Translate bounds in different space
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Bounds TranslateBounds(ref Matrix4x4 matrix, ref Bounds bounds)
        {
            var center = matrix.MultiplyPoint(bounds.center);

            // transform the local extents' axes
            var extents = bounds.extents;
            var axisX = matrix.MultiplyVector(new Vector3(extents.x, 0, 0));
            var axisY = matrix.MultiplyVector(new Vector3(0, extents.y, 0));
            var axisZ = matrix.MultiplyVector(new Vector3(0, 0, extents.z));

            // sum their absolute value to get the world extents
            extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
            extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
            extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

            return new Bounds { center = center, extents = extents };
        }


        private static Vector3[] sourcePoints = new Vector3[8];
        private static Vector3[] points = new Vector3[8];

        private static List<CalculatePointResult> pointResults = new List<CalculatePointResult>();
        /// <summary>
        /// Expensive way of realtime calculation of the renderer's bound
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static Rect CalculateScreenRect(Transform trans, Camera cam)
        {
            var rect = new Rect();
            var renderer = trans.GetComponent<Renderer>();
            if (renderer != null)
            {
                Quaternion originalRotation = trans.rotation;
                // Reset rotation
                trans.rotation = Quaternion.identity;
                // Get object bounds from unrotated object
                Bounds bounds = renderer.bounds;

                // Get the unrotated points
                sourcePoints[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z) - trans.position; // Bot left near
                sourcePoints[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z) - trans.position; // Bot right near
                sourcePoints[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z) - trans.position; // Top left near
                sourcePoints[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z) - trans.position; // Top right near
                sourcePoints[4] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z) - trans.position; // Bot left far
                sourcePoints[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z) - trans.position; // Bot right far
                sourcePoints[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z) - trans.position; // Top left far
                sourcePoints[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z) - trans.position; // Top right far

                // Restore rotation
                trans.rotation = originalRotation;
                // Apply scaling
                for (int s = 0; s < sourcePoints.Length; s++)
                {
                    sourcePoints[s] = new Vector3(sourcePoints[s].x / trans.localScale.x,
                                                  sourcePoints[s].y / trans.localScale.y,
                                                  sourcePoints[s].z / trans.localScale.z);
                }

                Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, trans.TransformPoint(sourcePoints[0]));
                Vector2 max = min;
                Vector2 point = min;
                GetMinMax(point, ref min, ref max);
                // Transform points from local to world space
                for (int t = 1; t < sourcePoints.Length; t++)
                {
                    point = RectTransformUtility.WorldToScreenPoint(cam, trans.TransformPoint(sourcePoints[t]));
                    GetMinMax(point, ref min, ref max);
                }

                return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
            }

            return rect;
        }

        /// <summary>
        /// Calculate screen rect from unscaled bounds
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cam"></param>
        /// <param name="bounds"></param>
        /// <param name="outOfScreen"></param>
        /// <returns></returns>
        public static Rect CalculateScreenRectFromBound(Transform trans, Camera cam, Bounds bounds, out bool outOfScreen)
        {
            outOfScreen = false;

            if (cam == null)
                return new Rect();

            sourcePoints[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z); // Bot left near
            sourcePoints[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z); // Bot right near
            sourcePoints[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z); // Top left near
            sourcePoints[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z); // Top right near
            sourcePoints[4] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z); // Bot left far
            sourcePoints[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z); // Bot right far
            sourcePoints[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z); // Top left far
            sourcePoints[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z); // Top right far

            // Apply scaling
            bool behindScreen = true;
            for (int s = 0; s < sourcePoints.Length; s++)
            {
                sourcePoints[s] = new Vector3(sourcePoints[s].x / trans.localScale.x,
                                              sourcePoints[s].y / trans.localScale.y,
                                              sourcePoints[s].z / trans.localScale.z);

                s_ptResults[s].point = cam.WorldToScreenPoint(trans.TransformPoint(sourcePoints[s]));
                s_ptResults[s].check = false;
                if(s_ptResults[s].point.z >= 0)
                {
                    //point is out of camera
                    behindScreen = false;
                }
                else
                {
                    s_ptResults[s].point.x = cam.pixelWidth - s_ptResults[s].point.x;
                    s_ptResults[s].point.y = cam.pixelHeight - s_ptResults[s].point.y;
                }
            }

            if (behindScreen)
            {
                outOfScreen = true;
                return new Rect();
            }
            else
            {
                pointResults.Clear();
                boundPoints.Clear();
                int counter = 0;
                for(int t = 0; t < s_ptResults.Length; t++)
                {
                    if(CheckPointOutOfScreen(s_ptResults[t], cam))
                    {
                        counter++;
                        if(counter >= 4)
                        {
                            outOfScreen = true;
                            return new Rect();
                        }

                        for(int m = 0; m < s_ptResults[t].edpts.Length; m++)
                        {
                            //Check the interception points on screen for the edges that hasn't been flagged check
                            if(!s_ptResults[t].edpts[m].check)
                            {
                                GetInterceptionPoint(s_ptResults[t], s_ptResults[t].edpts[m], ref boundPoints, cam);
                            }
                        }
                        s_ptResults[t].check = true;
                    }
                    else
                    {
                        boundPoints.Add(s_ptResults[t].point);
                    }
                }

                Rect screenRect = new Rect();
                if (boundPoints.Count > 0)
                {
                    Vector2 min = boundPoints[0];
                    Vector2 max = min;
                    Vector2 refPoint;
                    for (int i = 0; i < boundPoints.Count; i++)
                    {
                        refPoint = boundPoints[i];
                        GetMinMax(refPoint, ref min, ref max);
                    }

                    screenRect.min = min;
                    screenRect.max = max;
                }
                return screenRect;
            }
        }

        /// <summary>
        /// Check if point is out of screen
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        private static bool CheckPointOutOfScreen(CalculatePointResult pt, Camera cam)
        {
            return pt.point.x < 0 || pt.point.x > cam.pixelWidth || pt.point.y < 0 || pt.point.y > cam.pixelHeight;
        }

        /// <summary>
        /// Get the interception point on the edge of the screen
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="points"></param>
        /// <param name="cam"></param>
        private static void GetInterceptionPoint(CalculatePointResult pt1, CalculatePointResult pt2, ref List<Vector3> points, Camera cam)
        {
            if ((pt1.point.z < 0 || pt2.point.z < 0) || (pt1.point.x < 0 && pt2.point.x < 0) || (pt1.point.x > cam.pixelWidth && pt2.point.x > cam.pixelWidth) || (pt1.point.y < 0 && pt2.point.y < 0) || (pt1.point.y > cam.pixelHeight && pt2.point.y > cam.pixelHeight))
                return;

            var slope = (pt2.point.y - pt1.point.y) / (pt2.point.x - pt1.point.x);
            var offset = pt1.point.y - pt1.point.x * slope;
            var refPt = offset;
            if(((pt1.point.x <= 0 && pt2.point.x >= 0) || (pt1.point.x >= 0 && pt2.point.x <=0)) && (refPt >= 0 && refPt <= cam.pixelHeight))
            {
                //Intercepts left edge of the screen
                points.Add(new Vector3(0, refPt, 0));
            }

            if ((pt1.point.x <= cam.pixelWidth && pt2.point.x >= cam.pixelWidth) || (pt1.point.x >= cam.pixelWidth && pt2.point.x <= cam.pixelWidth))
            {
                refPt = offset + cam.pixelWidth * slope;
                if (refPt >= 0 && refPt <= cam.pixelHeight)
                {
                    //intercept right edge of screen
                    points.Add(new Vector3(cam.pixelWidth, refPt, 0));
                }
            }

            if ((pt1.point.y <= 0 && pt2.point.y >= 0) || (pt1.point.y >= 0 && pt2.point.y <= 0))
            {
                refPt = -offset / slope;
                if (refPt >= 0 && refPt <= cam.pixelWidth)
                {
                    //intercept bottom edge of screen
                    points.Add(new Vector3(refPt, 0, 0));
                }
            }

            if ((pt1.point.y <= cam.pixelHeight && pt2.point.y >= cam.pixelHeight) || (pt1.point.y >= cam.pixelHeight && pt2.point.y <= cam.pixelHeight))
            {
                refPt = (cam.pixelHeight - offset) / slope;
                if (refPt >= 0 && refPt <= cam.pixelWidth)
                {
                    //intercept top edge of screen
                    points.Add(new Vector3(refPt, cam.pixelHeight, 0));
                }
            }
        }

        /// <summary>
        /// Calculate unscale bounds of the transform
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="includeChildrens"></param>
        /// <returns></returns>
        public static Bounds CalculateUnscaledBounds(Transform trans, bool includeChildrens = false)
        {
            s_result.Initialized = false;

            CalculateUnscaledBounds(trans, s_result, includeChildrens);

            if (s_result.Initialized)
            {
                return s_result.Bounds;
            }

            return new Bounds(trans.position, Vector3.zero);
        }

        /// <summary>
        /// Calculate unscale bounds of the transform
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="result"></param>
        /// <param name="includeChildrens"></param>
        private static void CalculateUnscaledBounds(Transform trans, CalculateBoundsResult result, bool includeChildrens = false)
        {
            if (trans is RectTransform)
            {
                CalculateBounds((RectTransform)trans, result);
            }
            else
            {
                Renderer renderer = trans.GetComponent<Renderer>();
                if (renderer != null)
                {
                    CalculateUnscaledBounds(renderer, result);
                }
            }

            if (includeChildrens)
            {
                foreach (Transform child in trans)
                {
                    CalculateUnscaledBounds(child, result);
                }
            }
        }

        /// <summary>
        /// Calculate unscale bounds of the renderer
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="result"></param>
        private static void CalculateUnscaledBounds(Renderer renderer, CalculateBoundsResult result)
        {
            if (renderer is ParticleSystemRenderer)
            {
                return;
            }

            Bounds bounds;

            if (renderer is SkinnedMeshRenderer)
            {
                bounds = ((SkinnedMeshRenderer)renderer).sharedMesh.bounds;
            }
            else
            {
                var originalPosition = renderer.transform.position;
                var originalRotation = renderer.transform.rotation;

                //reset target position and rotation
                renderer.transform.position = Vector3.zero;
                renderer.transform.rotation = Quaternion.identity;

                bounds = renderer.bounds;

                renderer.transform.position = originalPosition;
                renderer.transform.rotation = originalRotation;
            }

            if (!result.Initialized)
            {
                result.Bounds = bounds;
                result.Initialized = true;
            }
            else
            {
                result.Bounds.Encapsulate(bounds.min);
                result.Bounds.Encapsulate(bounds.max);
            }
        }

        /// <summary>
        /// Calculate all unscale bounds of the transform
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static Dictionary<Transform, Bounds> CalculateAllTransBound(Transform trans, bool includeInactive = false)
        {
            var dict = new Dictionary<Transform, Bounds>();
            var allTrans = trans.GetComponentsInChildren<Transform>(includeInactive);
            for(int i = 0; i< allTrans.Length; i++)
            {
                var bounds = CalculateUnscaledBounds(allTrans[i], false);
                if (bounds.size != Vector3.zero)
                {
                    dict.Add(allTrans[i], bounds);
                }
            }

            return dict;
        }

        /// <summary>
        /// Calculate combined screen rect of all the unscaled bounds
        /// </summary>
        /// <param name="unscaledBounds"></param>
        /// <param name="cam"></param>
        /// <param name="outOfScreen"></param>
        /// <returns></returns>
        public static Rect CalculateCombineScreenRect(Dictionary<Transform, Bounds> unscaledBounds, Camera cam, out bool outOfScreen)
        {
            var rect = new Rect();
            outOfScreen = false;
            bool outScreen;
            foreach (var pair in unscaledBounds)
            {
                CalculateCombineRect(ref rect, CalculateScreenRectFromBound(pair.Key, cam, pair.Value, out outScreen));
                if(outScreen)
                {
                    outOfScreen = true;
                }
            }
           
            return rect;
        }

        public static Rect CalculateCombineScreenRect(Transform trans, Bounds unscaledBound, Camera cam, out bool outOfScreen)
        {
            var rect = new Rect();
            outOfScreen = false;
            bool outScreen;
            CalculateCombineRect(ref rect, CalculateScreenRectFromBound(trans, cam, unscaledBound, out outScreen));
            if (outScreen)
            {
                outOfScreen = true;
            }

            return rect;
        }

        /// <summary>
        /// Calculate combining rect
        /// </summary>
        /// <param name="combineRect"></param>
        /// <param name="rect"></param>
        private static void CalculateCombineRect(ref Rect combineRect, Rect rect)
        {
            if (rect.size.magnitude != 0)
            {
                if (combineRect.size.magnitude != 0)
                {
                    combineRect.min = new Vector2(Mathf.Min(combineRect.min.x, rect.min.x), Mathf.Min(combineRect.min.y, rect.min.y));
                    combineRect.max = new Vector2(Mathf.Max(combineRect.max.x, rect.max.x), Mathf.Max(combineRect.max.y, rect.max.y));
                }
                else
                {
                    combineRect = rect;
                }
            }
        }

        /// <summary>
        /// Convert recttransform from local to screen space
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static Rect RectTransformToScreenSpace(List<RectTransform> trans, Camera cam)
        {
            var rect = new Rect();

            for (int i = 0; i < trans.Count; i++)
            {
                CalculateCombineRect(ref rect, CalculateScreenRect(trans[i], cam));
            }

            return rect;
        }

        /// <summary>
        /// Calculate screen rect for the recttransform
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="cam"></param>
        /// <param name="cutDecimals"></param>
        /// <returns></returns>
        public static Rect CalculateScreenRect(RectTransform transform, Camera cam, bool cutDecimals = false)
        {
            var worldCorners = new Vector3[4];
            var screenCorners = new Vector3[4];

            transform.GetWorldCorners(worldCorners);

            for (int i = 0; i < 4; i++)
            {
                screenCorners[i] = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[i]);
                if (cutDecimals)
                {
                    screenCorners[i].x = (int)screenCorners[i].x;
                    screenCorners[i].y = (int)screenCorners[i].y;
                }
            }

            return new Rect(screenCorners[0].x,
                            screenCorners[0].y,
                            screenCorners[2].x - screenCorners[0].x,
                            screenCorners[2].y - screenCorners[0].y);
        }
    }
}