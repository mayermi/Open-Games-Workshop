Shader "Custom/Additive" {
  Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
  }
  SubShader {
    Tags { "Queue" = "Transparent" } 
    // draw after all opaque geometry has been drawn
    Pass { 
      Cull Off // Draw front and back faces
      ZWrite Off // Don't write to depth buffer 
      // in order not to occlude other objects

      Blend SrcAlpha One // Additive blending

      CGPROGRAM 
        #pragma vertex vert 
        #pragma fragment frag

        fixed4 _Color;

        float4 vert(float4 vertexPos : POSITION) : SV_POSITION {
          // Standard transform
          return mul(UNITY_MATRIX_MVP, vertexPos);
        }

        float4 frag(void) : COLOR {
          return _Color;
        }
      ENDCG  
    }
  }
}