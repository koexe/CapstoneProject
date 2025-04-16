Shader "Singular Bear/Grass Alpha"
{
    Properties
    {
       
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _TransparencyStart ("Transparency Start", Range(0,1)) = 0      
        _TransparencyEnd ("Transparency End", Range(0,1)) = 1        
        _GradientColor ("Gradient Color", Color) = (1,1,1,1)       
        _GradientStartPos ("Gradient Start Position", Range(0,1)) = 0        
        _GradientEndPos ("Gradient End Position", Range(0,1)) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _WaveAmplitude ("Wave Amplitude", Range(0,5)) = 0.1   
        _WaveFrequency ("Wave Frequency", Range(0,10)) = 1.0
        _WaveMask ("Wave Mask", 2D) = "white" {}  
        _PositionColor ("Position Color", Color) = (1,1,1,1)
        _UsePositionColor ("Use Position Color", Float) = 0.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float2 waveUV : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TransparencyStart;
            float _TransparencyEnd;
            fixed4 _GradientColor;
            float _GradientStartPos;
            float _GradientEndPos;
            fixed4 _Color;
            float _WaveAmplitude;
            float _WaveFrequency;
            sampler2D _WaveMask;
            float4 _WaveMask_ST;
            fixed4 _PositionColor;
            float _UsePositionColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.waveUV = TRANSFORM_TEX(v.uv, _WaveMask); 
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                float mask = tex2D(_WaveMask, i.waveUV).r;
                float wave = sin(_WaveFrequency * i.worldPos.y + _Time.y) * _WaveAmplitude * mask;
                float2 uv = i.uv;
                uv.x += wave * 2.0;
                uv = clamp(uv, 0.0, 1.0);           
                c = tex2D(_MainTex, uv);
                float height = (i.uv.y - _GradientStartPos) / (_GradientEndPos - _GradientStartPos);
                float transparency = lerp(_TransparencyStart, _TransparencyEnd, height);

                c.a *= transparency;
                c.rgb *= _GradientColor.rgb * transparency;

                c.rgb *= _Color.rgb;

                if (_UsePositionColor > 0.5)
                {
                    c.rgb *= _PositionColor.rgb * i.worldPos.x * i.worldPos.y;
                }

                return c;
            }
            ENDCG
        }
    }
}
