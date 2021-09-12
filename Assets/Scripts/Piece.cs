using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces {
	static public Vector2[] L = new Vector2[] {
		new Vector2(0, 13),
		new Vector2(0, 14),
		new Vector2(1, 14),
		new Vector2(2, 14),
	};

	static public Vector2[][] pieces = new Vector2[][] {L};
}

public class Piece {
	static public Color[] colors = new Color[] {Color.blue, Color.red, Color.green};

	public Square[] squares;
	public Color color;

	public Piece(Vector2[] other) {  // Use a template from Pieces
		squares = new Square[other.Length];
		color = colors [Random.Range(0, colors.Length)];

		for (int index = 0; index < squares.Length; index++) {
			Vector2 position = other [index];
			Square copy = new Square(position);
			copy.color = color;
			squares [index] = copy;
		}
	}

	public bool Move(GridManager grid) {  // returns whether the piece moved
		/* If a single square is blocked, so is the whole piece */
		List<Vector2> newPositions = new List<Vector2>();

		// Remove all squares from the board to avoid interference
		foreach (Square square in squares) {
			grid.occupiedSquares.Remove(square.position);
		}

		// Calculate each square one at a time
		foreach (Square square in squares) {
			Vector2 currentPosition = square.position;
			Vector2 newPosition = currentPosition + Vector2.down;
			if (!grid.occupiedSquares.Contains(newPosition) && currentPosition.y > 0)
				newPositions.Add(newPosition);  // not blocked
		}

		if (newPositions.Count != squares.Length) {  // blocked
			// Restore all the squares back to the board
			foreach (Square square in squares) {
				grid.occupiedSquares.Add(square.position);
			}
			return false;  // didn't move
		} else {  // not blocked
			// move each square to its new position all at once
			for (int index = 0; index < squares.Length; index++) {
				Square square = squares [index];
				Vector2 newPosition = newPositions [index];
				grid.occupiedSquares.Add(newPosition);
				square.position = newPosition;
			}
			return true;  // moved
		}
	}
}
