Shader "Stencils/StencilMask"
{
	Properties
	{
		_StencilMask("Stencil Mask", Int) = 0
	}

	SubShader
	{
		Tags {
			"RenderType" = "Opaque"
			"Queue" = "Geometry-100"		// even if in front of the geometry, is essential that this mask is drawn before the content of the cube. so we change the Queue value to Geometry - 100
		}
		ColorMask 0							// turns off rendering to all color channels. if this wasn't here, we'd still see the geometry we want, but every pixel other than that would render color
		ZWrite off
		Stencil {							// write into the stencil buffer
			Ref[_StencilMask]				// indicates that the value we want to write is provided in the property _StencilMask
			Comp always						// indicates that the value has to be written regardless of what was on the stencil buffer already
			Pass replace					// MOST IMPORTANT LINE. If Comp passes (which it always will, since it's set to always), replace the pixels of the mask with the geometry occupying the same pixel space
		}

		/////////////// everything below this line is standard stuff ///////////////

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR {
				return half4(1,1,0,1);
			}
			ENDCG
		}
	}
}