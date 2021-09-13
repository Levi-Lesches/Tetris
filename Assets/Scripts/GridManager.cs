using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public HashSet<Vector2> occupiedSquares = new HashSet<Vector2>();

	List<List<Cell>> grid = new List<List<Cell>>();
	List<Piece> bag = new List<Piece>();
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

	void ShuffleBag() {
		int[] indices = new int[7] {0, 1, 2, 3, 4, 5, 6};
		for (int index = 0; index < indices.Length; index++) {
			int temp = indices [index];
			int randomIndex = Random.Range(index, indices.Length);
			indices [index] = indices [randomIndex];
			indices [randomIndex] = temp;
		}
		bag.Clear();
		foreach (int index in indices) {
			Vector2[] template = Pieces.pieces [index];
			Color color = Pieces.colors [index];
			Piece piece = new Piece(template, color);
			bag.Add(piece);
		}
	}

	void AddPiece() {
		if (bag.Count == 0) ShuffleBag();
		piece = bag[0];
		bag.RemoveAt(0);
	}

	bool MovePiece(Vector2 direction) {
		RenderPiece(Cell.defaultColor);
		bool didMove = piece.Move(this, direction);
		RenderPiece(piece.color);
		return didMove;
	}

	void RotatePiece() {
		RenderPiece(Cell.defaultColor);
		piece.Rotate(this);
		RenderPiece(piece.color);
	}

	public bool IsValid(Vector2 position) {
		return !occupiedSquares.Contains(position)
			&& position.y >= 0
			&& position.x >= 0 && position.x < Config.width;
	}

	// Update is called once per frame
	void Update() {
		// Speed up
		if (Input.GetKeyDown("down")) timeBetweenMoves = 0.1f;
		else if (Input.GetKeyUp("down")) timeBetweenMoves = 1f;

		// Move left or right
		if (Input.GetKeyDown("left")) MovePiece(Vector2.left); 
		else if (Input.GetKeyDown("right")) MovePiece(Vector2.right);

		// Rotate
		if (Input.GetKeyDown("up")) RotatePiece();

		// Check if the piece should move
		if (piece == null) return;
		timeSinceLastMove += Time.deltaTime;
		if (timeSinceLastMove < timeBetweenMoves) return;
		timeSinceLastMove = 0;

		bool didMove = MovePiece(Vector2.down);

		// Piece is blocked, add it to the board and send another piece
		if (!didMove) {
			foreach (Vector2 square in piece.GetSquares()) 
				occupiedSquares.Add(square);
			AddPiece();
		}
	}

	Cell GetCell(Vector2 pos) {
		return grid [(int) pos.y] [(int) pos.x];
	}

	void RenderPiece(Color color) {
		foreach (Vector2 square in piece.GetSquares()) {
			if (square.y >= Config.height) continue;
			GetCell(square).sprite.color = color;
		}
	}
}
