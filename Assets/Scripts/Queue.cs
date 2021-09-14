using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queue : MonoBehaviour {
	static Vector2 center = new Vector2(2, 2);
	List<Cell> cells;

	// Start is called before the first frame update
	void Awake() {
		cells = new List<Cell> (transform.GetComponentsInChildren<Cell>());
	}

	public void DisplayPiece(Piece piece) {
		foreach (Cell tile in cells) tile.Clear();
		foreach (Vector2 square in piece.GetSquares(center)) {
			int index = (int) square.y * 4 + (int) square.x;
			cells [index].color = piece.color;
		}
	}
}
