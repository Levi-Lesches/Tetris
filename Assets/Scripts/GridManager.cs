using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public HashSet<Vector2> occupiedSquares = new HashSet<Vector2>();

	List<Square> squares = new List<Square>();
	List<List<Cell>> grid = new List<List<Cell>>();
	Piece piece;

	float timeBetweenMoves = 1;
	float timeSinceLastMove = 0;

	// Start is called before the first frame update
	void Start() {
		foreach (Transform child in transform) {
			List<Cell> row = new List<Cell>();
			foreach (Cell cell in child.GetComponentsInChildren<Cell>()) {
				row.Add(cell);
			}
			grid.Add(row);
		}
		AddPiece();
	}

	void AddPiece() {
		Vector2[] template = Pieces.pieces [Random.Range(0, Pieces.pieces.Length)];
		this.piece = new Piece(template);
		squares.AddRange(piece.squares);
	}

	// Update is called once per frame
	void Update() {
		if (piece == null) return;
		timeSinceLastMove += Time.deltaTime;
		if (timeSinceLastMove < timeBetweenMoves) return;
		timeSinceLastMove = 0;

		RenderPiece(Cell.defaultColor);
		bool didMove = piece.Move(this);
		RenderPiece(piece.color);

		if (!didMove) AddPiece();
	}

	Cell GetCell(Vector2 pos) {
		return grid [(int) pos.y] [(int) pos.x];
	}

	void RenderPiece(Color color) {
		foreach (Square square in piece.squares) {
			GetCell(square.position).sprite.color = color;
		}
	}
}
