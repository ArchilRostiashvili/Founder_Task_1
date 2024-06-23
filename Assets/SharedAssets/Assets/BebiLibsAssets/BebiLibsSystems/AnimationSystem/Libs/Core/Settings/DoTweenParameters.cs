using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

namespace BebiAnimations.Libs.Core.Settings
{
    public class DoTweenParameters
    {
        public float EndValue;
        public float Duration;
        public Color EndColorValue;
        public string MaterialPropertyName;
        public int MaterialPropertyID;

        public Vector3 IncrementByValue;
        public bool Snapping = false;

        public RotateMode TargetRotationMode = RotateMode.Fast;

        public Vector3 Punch;
        public int Vibrato;
        public float Elasticity;

        public bool WithCallbacks;
        public bool Complete;

        public Vector3 EndVector3Value;
        public Vector2 EndVector2Value;
        public float To;
        public bool AndPlay;
        public float JumpPower;

        public PathMode PathModeValue;
        public Path PathValue;

        public Vector3[] PathArray;
        public PathType PathTypeValue;
        public Quaternion EndRotationValue;
        public Vector3 Towards;
        public int Resolution;
        public Rect EndRectValue;

        public float ToStartWidth;
        public float ToEndWidth;
        public bool IncludeDelay;
        public bool Randomness;
        public bool FadeOut;
        public Vector3 StrengthVector3;
        public float Strength;
        public Vector4 EndVector4Value;
    }
}
