using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pieces {
	/* 
		Pieces are drawn on a grid to assist with rotations.
		The piece is tracked by its center cell, and the rest 
		of the squares are tracked by their positions in the grid.

		(0, 0) is the center of the grid. These declarations are
		aligned to approximate the shape they define.
	*/
	static List<Vector2> L = new List<Vector2> {
			new Vector2(1, 1),
		Vector2.left, Vector2.zero, Vector2.right,
	};

	static List<Vector2> J = new List<Vector2> {
		new Vector2(-1, 1),
			Vector2.left, Vector2.zero, Vector2.right,
	};

	static List<Vector2> I = new List<Vector2> {
		new Vector2(-2, 0),
		new Vector2(-1, 0),
		new Vector2(0, 0),
		new Vector2(1, 0),
	};

	static List<Vector2> O = new List<Vector2> {
		new Vector2(-1, 0), new Vector2(0, 0),
		new Vector2(-1, -1), new Vector2(0, -1),
	};

	static List<Vector2> S = new List<Vector2> {
			Vector2.up, new Vector2(1, 1),
		Vector2.left, Vector2.zero, 
	};

	static List<Vector2> T = new List<Vector2> {
			Vector2.up, 
		Vector2.left, Vector2.zero, Vector2.right, 
	};

	static List<Vector2> Z = new List<Vector2> {
		new Vector2(-1, 1), Vector2.up,
			Vector2.zero, Vector2.right,
	};

	static public List<Vector2>[] pieces = new List<Vector2>[] 
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
	List<Vector2> relativeSquares;  // the squares relative to the center

	public Piece(Piece other) {  // copy other Piece
		color = other.color;
		relativeSquares = other.relativeSquares.ToList();
		center = Vector2.zero;
	}

	public Piece(List<Vector2> positions, Color color) {  // Use a template from Pieces
		this.color = color;
		MoveToTop();
		relativeSquares = positions.ToList();
	}

	public List<Vector2> GetSquares() {
		return relativeSquares.Select(relative => center + relative).ToList();
	}

	public void MoveToTop() {
		center = new Vector2(Config.width / 2, Config.height + 1);
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
		if (relativeSquares.Any(  // check in advance
			square => !grid.IsValid(new Vector2(square.y, -square.x) + center)
		)) return;

		relativeSquares = relativeSquares.Select(  // then make the change
			square => new Vector2(square.y, -square.x)
		).ToList();
	}
}
