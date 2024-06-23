Shader "Unlit/TileParalex"
{
    Properties
    {
       [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
       [PerRendererData] _MinMax ("Min Max Texture", vector) = (0, 1, 0, 0)
        _AnimationTime("Time", float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
		Lighting Off
		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color: COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MinMax;
            float _AnimationTime;
        
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

           
            // float DecodeFloatRG(float2 enc)
            // {
            //     float2 kDecodeDot = float2 (1.0, 1 / 255.0);
            //     return dot (enc, kDecodeDot);
            // }
            float Repeat(float t, float length)
            {
                return t - floor(t / length) * length;
            }

            fixed4 frag(v2f i) : SV_Target
            {   
                float x = i.uv.x + (_AnimationTime * _MinMax.z);
                float n = _MinMax.x + fmod(x, _MinMax.y - _MinMax.x); 
                float2 uv = float2(n, i.uv.y);
                float4 element = tex2D(_MainTex, uv);
                return element;
            }
            ENDCG
        }
    }
}
