using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    public class HSVLineAnimator : MonoBehaviour
    {
        public bool testing = true;

        [SerializeField]
        private Texture2DArray lineTexArray;
        [SerializeField]
        private int linePts;
        [SerializeField]
        private bool randomFrame; // Randomization of animation frames
        [SerializeField]
        private float lineFrameRate;
        [SerializeField]
        private float lineFragLength;
        [SerializeField]
        private float lineScale;
        [SerializeField]
        private float lineTotalLength;
        [SerializeField]
        private float lineDisplacement;
        [SerializeField]
        private bool animateLine;
        [SerializeField]
        private float animateRate;

        private LineRenderer line;
        private Material mat;

        private int animateTimerID = -1;
        private int frameTimerID = -1;
        private int frameNumber; // Frame counter
        private float propMult;
        private float elapsTime, lifetimePos;
        private float timeMark, uvTime;

        void Awake()
        {
            // Get line renderer component
            line = GetComponent<LineRenderer>();

            if(line != null)
            {
                mat = Application.isPlaying ? line.material : line.sharedMaterial;

                if (mat != null)
                {
                    mat.SetInt("_Index", 0);
                }
            }
        }

        // OnSpawned called by pool manager 
        void OnSpawn()
        {
            if (mat != null)
            {
                mat.SetInt("_Index", 0);
                if(lineTexArray.depth > 0)
                {
                    InitializeAnimation();
                }
            }

            if (animateLine)
            {
                animateTimerID = HSVMainTimer.time.AddTimer(1/animateRate, LineAnimation);
            }
        }

        // OnDespawned called by pool manager 
        void OnDespawn()
        {
            // Clear frame animation timer
            if (frameTimerID != -1)
            {
                HSVMainTimer.time.RemoveTimer(frameTimerID);
                frameTimerID = -1;
            }

            // Clear oscillation timer
            if (animateTimerID != -1)
            {
                HSVMainTimer.time.RemoveTimer(animateTimerID);
                animateTimerID = -1;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if(testing && Application.isPlaying)
            {
                if (mat != null)
                {
                    mat.SetInt("_Index", 0);
                    if (lineTexArray.depth > 0)
                    {
                        InitializeAnimation();
                    }
                }

                if (animateLine)
                {
                    animateTimerID = HSVMainTimer.time.AddTimer(1/animateRate, LineAnimation);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            timeMark = Time.deltaTime;
            uvTime = Time.time;

            UpdateLineStatus();
        }

        public void FadeInOut(bool fadeIn = true)
        {

        }

        private void LineAnimation()
        {
            var points = (int)((lineTotalLength / lineFragLength) * linePts);

            if (points < 2)
            {
                line.positionCount = 2;
                line.SetPosition(0, Vector3.zero);
                line.SetPosition(1, new Vector3(0, 0, lineTotalLength));
            }
            else
            {
                line.positionCount = points;
                line.SetPosition(0, Vector3.zero);

                for (int i = 1; i < points - 1; i++)
                    line.SetPosition(i,
                        new Vector3(RandomDisplacement(), RandomDisplacement(), (lineTotalLength / (points - 1)) * i));

                line.SetPosition(points - 1, new Vector3(0f, 0f, lineTotalLength));
            }
        }

        float RandomDisplacement()
        {
            return Random.Range(-lineDisplacement, lineDisplacement);
        }

        void InitializeAnimation()
        {
            frameNumber = 0;
            frameTimerID = HSVMainTimer.time.AddTimer(1/lineFrameRate, AdvanceFrame);
        }

        void AdvanceFrame()
        {
            if (randomFrame)
                frameNumber = Random.Range(0, lineTexArray.depth);

            line.material.SetInt("_Index", frameNumber);
            frameNumber++;

            if (frameNumber == lineTexArray.depth)
                frameNumber = 0;
        }

        private void UpdateLineStatus()
        {
            if (mat != null)
            {
                // Calculate default beam proportion multiplier based on default scale and maximum length
                propMult = lineTotalLength * (lineScale / lineFragLength);

                // Set beam scaling according to its length
                mat.SetTextureScale("_TextureArray", new Vector2(propMult, 1f));
            }
        }
    }
}