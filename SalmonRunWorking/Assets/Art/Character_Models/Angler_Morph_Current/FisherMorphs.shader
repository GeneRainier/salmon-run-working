Shader "Landon/fisherPeople"{
	Properties{
		_MainTex	("Albedo RGB,  Occlusion A", 2D) = "grey" {}
		_Tint		("Paint Layers RGBA", 2D) = "black"{}
		_Bump		("Normal RGB,  Roughness A", 2D) = "bump"{}
		_PaintR 	("Color RGB,", Color) = (1,1,1,0)
		_PaintG 	("Color RGB,", Color) = (1,1,1,0)
		_PaintB 	("Color RGB,", Color) = (1,1,1,0)
		_PaintA 	("Color RGB,", Color) = (1,1,1,0)
		_SpecColor	("Specular RGB,  Gloss A", Color) = (0.5, 0.5, 0.5, 0.5)
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf BlinnPhong

		struct Input{
			half2 uv_MainTex, uv_Tint, uv_Bump;
		};
		sampler2D	_MainTex, _Tint, _Bump;
		fixed4		_PaintR, _PaintG, _PaintB , _PaintA;
		
		void surf(Input IN, inout SurfaceOutput o){
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 tint = tex2D(_Tint, IN.uv_Tint);
			fixed4 bump = tex2D(_Bump, IN.uv_Bump);
			fixed ao = tex.a;
			fixed shine = bump.a * bump.a * _SpecColor.a * ao;
			fixed3 t = tint.r * _PaintR + tint.g * _PaintG + tint.b * _PaintB + (1-tint.a) * _PaintA + 1 * (1- tint.r - tint.g - tint.b - (1-tint.a));
			tex.rgb *= t;
			
			o.Albedo = tex.rgb * ao;
			o.Normal = normalize(bump.xyz * 2 - 1);
			o.Gloss = shine * 5;
			o.Specular = 0.05 + shine;
		}
		ENDCG
	}
	FallBack "Legacy Shaders/Diffuse"
}