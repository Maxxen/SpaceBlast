﻿// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CharacterShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        
        _FlashAmount ("Flash Amount", Range(0,1)) = 0.0
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        
        _OutlineSize ("Outline Size", Range(0.00, 1)) = 0.1
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
        Pass // Render Outline
        {
            Name "OUTLINE"
            Tags {"LightMode" = "Always"}
            Cull Off
            ZWrite Off
            ZTest Always
            ColorMask RGB
            
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                
                float _OutlineSize;
                fixed4 _OutlineColor;
                 
                struct appdata {
                    float4 vertex : POSITION;
                    float4 tangent : TANGENT;
                    
                };
                 
                struct v2f {
                    float4 pos : POSITION;
                    float3 normal : NORMAL;
                };
                
                v2f vert (appdata v) {                
                    v2f o;
                    
                    o.pos = UnityObjectToClipPos(v.vertex);
                    
                    float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.tangent.xyz);
                    float2 offset = TransformViewToProjection(norm.xy);
                    
                    o.pos.xy += offset * o.pos.z * _OutlineSize;
                    return o;
                };
                
                half4 frag (v2f i) :COLOR {
                    return _OutlineColor;
                };               
            ENDCG
        } 
        
        // The actual surface shader
        
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows finalcolor:hit

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        
        
        float _FlashAmount;
        fixed4 _FlashColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

        
        void hit (Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            color = lerp(color, _FlashColor, _FlashAmount);
        }
        
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;        
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
