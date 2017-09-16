Shader "Custom/White" {
   Properties {
        _Color ("Color", Color) = (1,1,1,1)
    }
  
    SubShader {
        Color [_Color]
        Pass {Color (1,0,0,0)}
    } 
    FallBack "Diffuse"
}