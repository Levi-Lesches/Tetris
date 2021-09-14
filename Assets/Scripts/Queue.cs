using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queue : MonoBehaviour {
	static Vector2 center = new Vector2(2, 2);
	List<Cell> cells;

	// Start is called before the first frame update
	void Start() {
		cells = new List<Cell> (transform.GetComponentsInChildren<Cell>());
	}

	public void DisplayPiece(Piece piece) {
		Piece blank = new Piece(piece);
		foreach (Cell tile in cells) tile.Clear();
		foreach (Vector2 relative in blank.GetSquares()) {
			Vector2 square = relative + center;
			if (relative == new Vector2(-2, 0))
				Debug.Log("Offsetting " + relative + " to " + square);
			Render(square, blank.color);
		}
	}

	void Render(Vector2 cell, Color color) {
		int index = (int) cell.y * 4 + (int) cell.x;
		if (cell == new Vector2(0, 2)) 
			Debug.Log("Mapped " + cell + " to " + index);
		cells [index].SetColor(color);
	}
}
