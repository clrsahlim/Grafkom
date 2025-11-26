Shader "Custom/CustomSpriteShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _CustomColor ("Custom Color", Color) = (1,1,1,1)
        _Brightness ("Brightness", Float) = 1.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _CustomColor;
            float _Brightness;
            float4x4 _CustomTransform;

            v2f vert (appdata v)
            {
                v2f o;
    
                float4 worldVertex = mul(_CustomTransform, v.vertex);
                
                float4 viewVertex = mul(UNITY_MATRIX_V, worldVertex);
                o.vertex = mul(UNITY_MATRIX_P, viewVertex);
                
                o.uv = v.uv;
                o.worldPos = worldVertex.xyz;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                col *= _CustomColor * _Brightness;

                float wave = sin(i.worldPos.x * 3.0 + _Time.y * 2.0) * 0.1;
                col.rgb *= (1.0 - wave);
                
                float2 center = i.uv - 0.5;
                float dist = length(center);
                float glow = 1.0 + (1.0 - dist) * 0.2;
                col.rgb *= glow;
                
                return col;
            }
            ENDCG
        }
    }
}