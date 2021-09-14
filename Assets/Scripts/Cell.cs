using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
	static public Color defaultColor = new Color(0.47f, 0.47f, 0.47f, 1f);
	
	public SpriteRenderer sprite;

	public void Clear() {
		sprite.color = defaultColor;
	}

	public void SetColor(Color color) {
		sprite.color = color;
	}
}
