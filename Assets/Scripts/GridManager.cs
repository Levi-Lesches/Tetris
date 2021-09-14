using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public HUDController hud;

	HashSet<Vector2> occupiedSquares = new HashSet<Vector2>();
	List<List<Cell>> grid = new List<List<Cell>>();
	List<Piece> bag = new List<Piece>();
	List<Piece> nextBag = new List<Piece>();
	Piece piece;
	Piece hold;
	
	float timeBetweenMoves = Config.defaultFallSpeed;
	float timeSinceLastMove = 0;
	float? timeUntilLock;
	int rotations = 0;

	// Start is called before the first frame update
	void Start() {
		foreach (Transform child in transform) {
			grid.Add(new List<Cell>(child.GetComponentsInChildren<Cell>()));
		}
		ShuffleBag();   // initialize nextBat
		bag = nextBag.ToList();  // initialize bag
		ShuffleBag();   // initialize nextBag again
		AddPiece();  // pull from bag and nextBag
	}

	void ShuffleBag() {
		// Shuffle indices from 0..7
		int[] indices = new int[7] {0, 1, 2, 3, 4, 5, 6};
		for (int index = 0; index < indices.Length; index++) {
			int temp = indices [index];
			int randomIndex = Random.Range(index, indices.Length);
			indices [index] = indices [randomIndex];
			indices [randomIndex] = temp;
		}
		// Assign pieces based on the shuffled indices
		nextBag.Clear();
		foreach (int index in indices) {
			List<Vector2> template = Pieces.pieces [index];
			Color color = Pieces.colors [index];
			Piece piece = new Piece(template, color);
			nextBag.Add(piece);
		}
	}

	void AddPiece() {
		/* Pop from [bag], transfer from [nextBag] to [bag] */
		piece = bag[0];  // take
		bag.RemoveAt(0);  // remove
		bag.Add(nextBag[0]);  // replace
		nextBag.RemoveAt(0);
		if (nextBag.Count == 0) {
			ShuffleBag();
		}
		hud.RenderQueue(bag);
		rotations = 0;
	}

	bool MovePiece(Vector2 offset) {
		RenderPiece(Config.defaultColor);
		bool didMove = piece.Move(this, offset);
		RenderPiece(piece.color);
		return didMove;
	}

	void RotatePiece() {
		/* Rotates the current piece clockwise */
		rotations++;
		RenderPiece(Config.defaultColor);
		piece.Rotate(this);
		RenderPiece(piece.color);
	}

	public bool IsValid(Vector2 position) {
		/* Returns whether the given position is unblocked */
		return !occupiedSquares.Contains(position)
			&& position.y >= 0
			&& position.x >= 0 && position.x < Config.width;
	}

	List<int> GetClearedRows() {
		/* Returns a list of cleared rows from the bottom up */
		List<int> cleared = piece.GetSquares()
			.Select(square => (int) square.y)  // convert to rows
			.Where(row => Enumerable.Range(0, Config.width).All(  // full rows
				column => occupiedSquares.Contains(new Vector2(column, row))
			))
			.Distinct().ToList();  // remove duplicates
		cleared.Sort();  // clear from bottom to top
		return cleared;
	}

	void ClearRows() {
		HashSet<Vector2> newBoard = new HashSet<Vector2>();
		List<int> clearedRows = GetClearedRows();
		if (clearedRows.Count == 0) return;

		foreach (Vector2 oldSquare in occupiedSquares) {
			// Clear the cell, in case it hasn't been already
			Cell oldCell = GetCell(oldSquare);
			Color color = oldCell.color;
			if (!newBoard.Contains(oldSquare)) oldCell.Clear();

			// Find out how many rows to drop down
			int oldRow = (int) oldSquare.y;
			if (clearedRows.Contains(oldRow)) continue;
			int rows = clearedRows.Where(row => oldRow > row).Count();
			Vector2 newSquare = oldSquare + Vector2.down * rows;

			// Remove the square, or move it down
			GetCell(newSquare).color = color;
			newBoard.Add(newSquare);
		}
		occupiedSquares = newBoard;  // swap the new board
	}

	void Lock() {
		// yield return new WaitForSeconds(0.5f);  // lock delay
		occupiedSquares.UnionWith(piece.GetSquares());
		ClearRows();
		AddPiece();
	}

	void Hold() {
		// Rotate the block so it won't overflow the holding cell
		int rotations = this.rotations;  // will be modified
		for (int _ = 0; _ < 4 - rotations % 4; _++) RotatePiece();
		RenderPiece(Config.defaultColor);

		if (hold == null) {  // add to hold, draw new piece
			hold = piece;
			AddPiece();
		} else {  // swap piece and hold
			Piece temp = piece;
			piece = hold;
			hold = temp;
			piece.MoveToTop();  // piece was already falling when it was held
		}
		hud.RenderHold(hold);
	}

	// Update is called once per frame
	void Update() {
		// Speed up
		if (Input.GetKeyDown("down")) timeBetweenMoves = Config.fastFallSpeed;
		else if (Input.GetKeyUp("down")) timeBetweenMoves = Config.defaultFallSpeed;

		// Move left or right
		if (Input.GetKeyDown("left")) MovePiece(Vector2.left); 
		else if (Input.GetKeyDown("right")) MovePiece(Vector2.right);

		// Rotate
		if (Input.GetKeyDown("up")) RotatePiece();

		// Hold piece
		if (Input.GetKeyDown("right shift") || Input.GetKeyDown("left shift")) Hold();

		// Check if piece is locked
		if (timeUntilLock != null) {
			timeUntilLock -= Time.deltaTime;
			if (timeUntilLock <= 0) {
				// The piece may have been moved since it locked
				bool canMove = piece.Move(this, Vector2.down);
				if (canMove) piece.Move(this, Vector2.up);  // undo
				timeUntilLock = null;
				if (!canMove) Lock();
			}
			return;
		}

		// Check if the piece should move
		timeSinceLastMove += Time.deltaTime;
		if (timeSinceLastMove < timeBetweenMoves) return;
		timeSinceLastMove = 0;

		// Check if piece is blocked and queue it for locking
		bool didMove = MovePiece(Vector2.down);
		if (!didMove) {
			timeUntilLock = 0.5f;
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
