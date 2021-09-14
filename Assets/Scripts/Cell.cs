using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {	
	public SpriteRenderer sprite;

	public Color color {
		get { return sprite.color; }
		set { sprite.color = value; }
	}

	public void Clear() {
		this.color = Config.defaultColor;
		sprite.color = color;
	}
}
