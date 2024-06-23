using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class LineSolver
    {
        private float _slope;
        private float _intercept;

        public LineSolver(float slope, float intercept)
        {
            _slope = slope;
            _intercept = intercept;
        }

        public float SolveUnclamped(float x)
        {
            return _slope * x + _intercept;
        }

        public float Solve(float x, float min, float max)
        {
            return Mathf.Clamp(SolveUnclamped(x), min, max);
        }

    }
}
