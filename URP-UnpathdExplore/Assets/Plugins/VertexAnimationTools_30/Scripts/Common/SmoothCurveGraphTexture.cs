using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VertexAnimationTools_30{

	public class SmoothCurveGraphTexture {

        Color32[] clearPixels;
        Color32[] pixels;
        Color32 redPixel = new Color32((byte)255, (byte)0, (byte)0, (byte)80);
        Color32 backgroundPixel = new Color32((byte)255, (byte)255, (byte)255, (byte)120);
        public Texture2D GraphTexture;

        public SmoothCurveGraphTexture(float minAmount, float maxAmount, float easeOffset, float easeLength, bool isDarkTheme){
            if (isDarkTheme) {
                redPixel = new Color32((byte)255, (byte)100, (byte)100, (byte)255);
                backgroundPixel = new Color32((byte)100, (byte)100, (byte)100, (byte)255);
            }

			GraphTexture = new Texture2D(200,100, TextureFormat.ARGB32, false);
			clearPixels = new Color32[20000];
			for(int i = 0; i<clearPixels.Length; i++){
				clearPixels[i] = backgroundPixel;
			}
			pixels = new Color32[20000];
			RepaintTexture(minAmount, maxAmount, easeOffset, easeLength);
		}



		public void RepaintTexture( float minAmount, float maxAmount, float easeOffset, float easeLength){
			clearPixels.CopyTo( pixels, 0);
			for(int x = 0; x<GraphTexture.width; x++){
				float fx = x*0.005f;
				float val = Extension.SmoothLoopCurve(fx, minAmount, maxAmount, easeOffset, easeLength);
				int columnHeight = (int)(val * 100f);
				columnHeight = Mathf.Clamp( columnHeight,0,99);
				for(int y = 0; y<columnHeight; y++){
					int pixelIdx = y*200+x;
	 				pixels[pixelIdx] = redPixel;
				}
	 		}
			GraphTexture.SetPixels32( pixels );
			GraphTexture.Apply();
		}
 
	}
}
