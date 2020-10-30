Shader "Unlit/Spooky"
{
     Properties
     {
         _MainTex ("Texture", 2D) = "white" {}
         _ColourA ("ColourA", Color) = (0,0,0,0)
         _ColourB ("ColourB", Color) = (0,0,0,0)
         _Density ("Fog Density", float) = 0
     }
     SubShader
     {
         // No culling or depth
         Cull Off ZWrite Off ZTest Always
 
         Pass
         {
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
 
             #include "UnityCG.cginc"
             
             // vertex input: position, UV
             struct appdata {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
             };
 
             struct v2f {
                 float4 pos : SV_POSITION;
                 float2 uv : TEXCOORD0;
                 float3 viewVector : TEXCOORD1;
             };
             
             v2f vert (appdata v) {
                 v2f output;
                 output.pos = UnityObjectToClipPos(v.vertex);
                 output.uv = v.uv;
                 // Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
                 // (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                 float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                 output.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                 return output;
             }
 
             sampler2D _MainTex;
             sampler2D _CameraDepthTexture;
 
             float4 _ColourA;
             float4 _ColourB;

             float _Density;
             float startDst;

 
             float remap(float v, float minOld, float maxOld, float minNew, float maxNew) {
                 return minNew + (v-minOld) * (maxNew - minNew) / (maxOld-minOld);
             }
 
             fixed4 frag (v2f i) : SV_Target
             {
                 //create ray
                float3 rayPos = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                // get depth
                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = Linear01Depth(nonlin_depth) * viewLength;

                //calculate fog density
                float fogDensity = 1 - saturate(exp(-(depth ) * _Density * 0.01));
                float4 fogCol = lerp(_ColourA, _ColourB, rayDir.y * 0.5);

                // apply fog
                float4 originalCol = tex2D(_MainTex, i.uv);
                return fogCol * fogDensity + originalCol * (1 - fogDensity);
                
                /*

                 //create ray
                float3 rayPos = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                // get depth
                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = Linear01Depth(nonlin_depth);// * viewLength;

                //calculate fog density
                float fogDensity = 1 - saturate(exp(-(depth - startDst) * density * 0.01));
                float4 fogCol = lerp(_ColourA, _ColourB, rayDir.y * 0.5);

                // apply fog
                float4 originalCol = tex2D(_MainTex, i.uv);
                return fogCol * fogDensity + originalCol * (1 - fogDensity);*/
             }
             ENDCG
         }
     }
 }
 