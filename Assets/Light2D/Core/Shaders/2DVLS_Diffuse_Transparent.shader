Shader "2DVLS/Cutout" 
{
    Properties
    {
        _MainTex ("Base (RGB) Trans. (Alpha)", 2D) = "white" { }
    }

    Category
    {
        ZWrite On
        Cull Back
        Lighting Off
        SubShader
        {
            // extra pass that renders to depth buffer only
		    Pass {
				ColorMask 0
				AlphaTest GEqual 1
				SetTexture [_MainTex] { 
				  combine texture
				}            
			}

            Pass
            {
           		Blend DstColor DstAlpha
           		AlphaTest GEqual 1
                SetTexture [_MainTex] { 
                	combine texture
                }             
            }
        } 
    }

	Fallback "2DVLS/2DVLS_Light_Fallback"
}