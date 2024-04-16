using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio
{
    public static class HSVUtility
    {
        /// <summary>
        /// Utility method to calculate rect intercept point
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 CalculateEdgeIntercept(Vector2 pivot, Rect rect)
        {
            var dir = pivot - rect.center;
            var dirBase = rect.max - rect.center;
            var dirRatio = new Vector2(dirBase.x / Mathf.Max(0.1f, Mathf.Abs(dir.x)), dirBase.y / Mathf.Max(0.1f, Mathf.Abs(dir.y)));
            Vector2 edgePt = Vector2.zero;
            if(dirRatio.x < dirRatio.y)
            {
                //point is on the vertical side of the rect
                edgePt.x = dir.x > 0 ? rect.xMax : rect.xMin;
                edgePt.y = dir.y * dirRatio.x + rect.center.y;
            }
            else
            {
                edgePt.x = dir.x * dirRatio.y + rect.center.x;
                edgePt.y = dir.y > 0 ? rect.yMax : rect.yMin;
            }

            return edgePt;
        }

        /// <summary>
        /// Utility method to calculate UI offset point 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="interceptPt"></param>
        /// <param name="displaySize"></param>
        /// <returns></returns>
        public static Vector2 CalculateOffsetPoint(Vector2 center, Vector2 interceptPt, Vector2 displaySize)
        {
            var dir = interceptPt - center;
            var radius = new Vector2(displaySize.x / 2, displaySize.y / 2).magnitude;
            return dir.normalized * (dir.magnitude + radius) + center;
        }

        /// <summary>
        /// Utility method to calculate UI on screen point when target is off screen
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cam"></param>
        /// <param name="canvas"></param>
        /// <param name="targetLocation"></param>
        /// <param name="screenOffset"></param>
        public static void CalculateOutOfScreenPos(GameObject obj, Camera cam, Canvas canvas, ref Vector3 targetLocation, float screenOffset)
        {
            if (cam == null || canvas == null)
                return;

            float posAngle, slope, cosine, sine, clampX, clampY;
            Vector3 viewPortCenter = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2);
            Vector2 mappedPos;

            var curScreenPos = cam.WorldToScreenPoint(obj.transform.position);
            if (curScreenPos.z < 0)
            {
                curScreenPos.x = cam.pixelWidth - curScreenPos.x;
                curScreenPos.y = cam.pixelHeight - curScreenPos.y;
            }

            //Adjust the center to the viewport center
            curScreenPos -= viewPortCenter;

            //Get the Angle and adjust it to viewport angle
            posAngle = Mathf.Atan2(curScreenPos.y, curScreenPos.x);
            posAngle -= 90 * Mathf.Deg2Rad;

            cosine = Mathf.Cos(posAngle);
            sine = -Mathf.Sin(posAngle);

            slope = cosine / sine;

            var clampScreenPos = viewPortCenter * screenOffset;

            //check the position of the intercept point
            if (cosine > 0)
            {
                //up
                clampY = clampScreenPos.y;
            }
            else
            {
                //down
                clampY = -clampScreenPos.y;
            }

            clampX = clampY / slope;

            //check out of bound situation
            if (clampX > clampScreenPos.x)
            {
                //out of bound right
                clampX = clampScreenPos.x;
                clampY = clampX * slope;
            }
            else if (clampX < -clampScreenPos.x)
            {
                //out of bound let
                clampX = -clampScreenPos.x;
                clampY = clampX * slope;
            }

            //map back to viewport
            curScreenPos = new Vector3(clampX, clampY) + viewPortCenter;
            Camera screenCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, curScreenPos, screenCam, out mappedPos))
            {
                targetLocation = mappedPos;
            }
            else
            {
                targetLocation = Vector3.zero;
            }
        }

        /// <summary>
        /// Utility method to calculate UI on screen point when target is off screen
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="cam"></param>
        /// <param name="canvas"></param>
        /// <param name="targetLocation"></param>
        /// <param name="screenOffset"></param>
        /// <param name="outOfScreen"></param>
        /// <returns></returns>
        public static bool CalculateOutOfScreenPos(Vector3 pos, Camera cam, Canvas canvas, ref Vector3 targetLocation, float screenOffset, out bool outOfScreen)
        {
            if (cam == null || canvas == null)
            {
                outOfScreen = false;
                return false;
            }

            float posAngle, slope, cosine, sine, clampX, clampY;
            Vector3 viewPortCenter = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2);
            var clampScreenPos = viewPortCenter * screenOffset;
            Vector2 mappedPos;

            var curScreenPos = CalculateScreenPos(pos, cam);

            //Adjust the center to the viewport center
            curScreenPos -= viewPortCenter;

            if (TargetInBoundCheck(curScreenPos, clampScreenPos))
            {
                outOfScreen = false;
                curScreenPos += viewPortCenter;
            }
            else
            {
                outOfScreen = true;
                //Get the Angle and adjust it to viewport angle
                posAngle = Mathf.Atan2(curScreenPos.y, curScreenPos.x);
                posAngle -= 90 * Mathf.Deg2Rad;

                cosine = Mathf.Cos(posAngle);
                sine = -Mathf.Sin(posAngle);

                slope = cosine / sine;

                //check the position of the intercept point
                if (cosine > 0)
                {
                    //up
                    clampY = clampScreenPos.y;
                }
                else
                {
                    //down
                    clampY = -clampScreenPos.y;
                }

                clampX = clampY / slope;

                //check out of bound situation
                if (clampX > clampScreenPos.x)
                {
                    //out of bound right
                    clampX = clampScreenPos.x;
                    clampY = clampX * slope;
                }
                else if (clampX < -clampScreenPos.x)
                {
                    //out of bound let
                    clampX = -clampScreenPos.x;
                    clampY = clampX * slope;
                }

                //map back to viewport
                curScreenPos = new Vector3(clampX, clampY) + viewPortCenter;
            }

            Camera screenCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, curScreenPos, screenCam, out mappedPos))
            {
                targetLocation = mappedPos;
                return true;
            }
            else
            {
                targetLocation = Vector3.zero;
            }

            return false;
        }

        public static bool TargetInBoundCheck(Vector3 pos, Vector3 screenBorder)
        {
            if(pos.z > 0 && Mathf.Abs(pos.x) < screenBorder.x && Mathf.Abs(pos.y) < screenBorder.y)
            {
                return true;
            }

            return false;
        }

        public static Vector3 CalculateScreenPos(Vector3 pos, Camera cam)
        {
            var curScreenPos = cam.WorldToScreenPoint(pos);
            if (curScreenPos.z < 0)
            {
                curScreenPos.x = cam.pixelWidth - curScreenPos.x;
                curScreenPos.y = cam.pixelHeight - curScreenPos.y;
            }
            return curScreenPos;
        }

        public static bool CalculateLocalRectPoint(Vector3 worldPos, Camera cam, Canvas canvas, out Vector3 screenPos)
        {
            var curScreenPos = CalculateScreenPos(worldPos, cam);
            Camera screenCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            Vector2 mappedPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, curScreenPos, screenCam, out mappedPos))
            {
                screenPos = mappedPos;
                screenPos.z = curScreenPos.z;
                return true;
            }
            screenPos = Vector3.zero;
            return false;
        }
    }
}