using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces {
	/* 
		Pieces are drawn on a grid to assist with rotations.
		The piece is tracked by its center cell, and the rest 
		of the squares are tracked by their positions in the grid.

		(0, 0) is the center of the grid. These declarations are
		aligned to approximate the shape they define.
	*/
	static Vector2[] L = new Vector2[] {
			new Vector2(1, 1),
		Vector2.left, Vector2.zero, Vector2.right,
	};

	static Vector2[] J = new Vector2[] {
		new Vector2(-1, 1),
			Vector2.left, Vector2.zero, Vector2.right,
	};

	static Vector2[] I = new Vector2[] {
		new Vector2(0, -2),
		new Vector2(0, -1),
		new Vector2(0, 0),
		new Vector2(0, 1),
	};

	static Vector2[] O = new Vector2[] {
		new Vector2(1, 0), new Vector2(1, 1),
		new Vector2(0, 0), new Vector2(0, 1),
	};

	static Vector2[] S = new Vector2[] {
			Vector2.up, new Vector2(1, 1),
		Vector2.left, Vector2.zero, 
	};

	static Vector2[] T = new Vector2[] {
			Vector2.up, 
		Vector2.left, Vector2.zero, Vector2.right, 
	};

	static Vector2[] Z = new Vector2[] {
		new Vector2(-1, 1), Vector2.up,
			Vector2.zero, Vector2.right,
	};

	static public Vector2[][] pieces = new Vector2[][] 
		{L, J, I, O, S, T, Z};

	static public Color[] colors = new Color[] {  // same order as above
		new Color(1f, 0.5f, 0),  // orange. C'mon Unity :|
		Color.blue,
		Color.cyan, 
		Color.yellow, 
		Color.green, 
		Color.magenta, 
		Color.red,
	};
}

public class Piece {
	public Color color;  // the color of all the squares

	Vector2 center;  // position of the center piece
	Vector2[] relativeSquares;  // the squares relative to the center

	public Piece(Vector2[] positions, Color color) {  // Use a template from Pieces
		this.color = color;
		center = new Vector2(Config.width / 2, Config.height + 1);
		relativeSquares = new Vector2[positions.Length];
		positions.CopyTo(relativeSquares, 0);
	}

	public Vector2[] GetSquares() {
		Vector2[] result = new Vector2[relativeSquares.Length];
		for (int index = 0; index < result.Length; index++) {
			Vector2 relative = relativeSquares [index];
			result [index] = center + relative;
		}
		return result;
	}

	/* Moves the piece down a tile. Returns whether the piece moved. */
	public bool Move(GridManager grid, Vector2 direction) {
		center += direction;  // preemptively move the piece
		foreach (Vector2 square in GetSquares()) {
			if (!grid.IsValid(square)) {
				center -= direction;  // move the piece back
				return false;
			}
		}
		return true;
	}

	/* Rotates the piece, if it can */
	public void Rotate(GridManager grid) {
		foreach (Vector2 square in relativeSquares) {
			Vector2 newPosition = new Vector2(square.y, -square.x);
			Vector2 global = newPosition + center;
			if (!grid.IsValid(global)) return;
		}

		for (int index = 0; index < relativeSquares.Length; index++) {
			Vector2 square = relativeSquares [index];
			relativeSquares [index] = new Vector2(square.y, -square.x);
		}
	}
}
