using UnityEngine;
using System.Collections;

public enum MColor {White, Blue, Green, Cyan, Red, Magenta, Yellow, Black}; 


public class MinimalColor : Singleton<MinimalColor> {
	public  Color[] colors;
	
	public Color Get(MColor color) {
		return colors[(int)color];
	}
	
	public MColor Combine(MColor colorA, MColor colorB) {
		return (MColor)((int)colorA ^ (int)colorB);
	}
}
	