Shader "Unlit/MatcapWorm"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FarTex ("Texture", 2D) = "white" {}
        _RampTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 worldNormal : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float amount : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _FarTex;
            float4 _FarTex_ST;
            sampler2D _RampTex;
            float4 _RampTex_ST;

           
            v2f vert (appdata v)
            {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                float amount = world.z/1000;
                o.amount = amount;
               
                half4 rampuv;
                rampuv.x = amount + _Time[0];
                rampuv.y = 0.5;
                rampuv.z = 0;
                rampuv.w = 0;
                fixed4 a = tex2Dlod(_RampTex, rampuv);
                rampuv.x = amount + (_Time[1]*0.5);
                fixed4 b = tex2Dlod(_RampTex, rampuv);

                o.vertex.y += (a.r*amount)*30;
                o.vertex.x += (b.r*amount)*30;
                o.worldNormal = mul((float3x3)UNITY_MATRIX_V, UnityObjectToWorldNormal(v.normal)).xy*0.45 + 0.5;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.worldNormal);
                fixed4 col2 = tex2D(_FarTex, i.worldNormal);
                
                return lerp(col,col2,i.amount);
            }
            ENDCG
        }
    }
}