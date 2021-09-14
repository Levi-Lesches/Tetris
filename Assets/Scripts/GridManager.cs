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
		// pop from bag, transfer from nextBag
		piece = bag[0];  // take
		bag.RemoveAt(0);  // remove
		bag.Add(nextBag[0]);  // replace
		nextBag.RemoveAt(0);
		if (nextBag.Count == 0) {
			ShuffleBag();
		}
		hud.RenderQueue(bag);
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

	void ClearRow(int row) {
		/* Move everything down and delete the cleared row */
		HashSet<Vector2> newBoard = new HashSet<Vector2>();
		foreach(Vector2 oldSquare in occupiedSquares) {
			Color color = GetCell(oldSquare).sprite.color;  // record old color
			if (!newBoard.Contains(oldSquare))  // only clear if this is an old square
				GetCell(oldSquare).sprite.color = Cell.defaultColor;
			if (oldSquare.y == row) continue;
			Vector2 newPosition = oldSquare.y < row
				? oldSquare : oldSquare + Vector2.down;
			newBoard.Add(newPosition);
			GetCell(newPosition).sprite.color = color;
		}
		occupiedSquares = newBoard;
	}

	IEnumerator Lock(Piece piece) {  // use instead of this.piece since it changes
		yield return new WaitForSeconds(0.5f);  // lock delay

		// Add the squares to the board
		foreach (Vector2 square in piece.GetSquares()) 
			occupiedSquares.Add(square);

		// Check which of the affected rows are cleared
		List<int> cleared = piece.GetSquares()
			.Select(square => (int) square.y)  // convert to rows
			.Where(row => Enumerable.Range(0, Config.width).All(  // full rows
				column => occupiedSquares.Contains(new Vector2(column, row))
			))
			.Distinct().ToList();  // remove duplicates
		cleared.Sort();  // clear from bottom to top

		// Every row that's cleared means the next rows move down one
		int numRows = 0;
		for (int index = 0; index < cleared.Count; index++) {
			int row = cleared [index];
			row -= numRows;  // everything moves down one
			numRows++;
			ClearRow(row);
		}
	}

	void Hold() {
		RenderPiece(Cell.defaultColor);
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

		// Check if the piece should move
		if (piece == null) return;
		timeSinceLastMove += Time.deltaTime;
		if (timeSinceLastMove < timeBetweenMoves) return;
		timeSinceLastMove = 0;

		bool didMove = MovePiece(Vector2.down);

		// Piece locked, add it to the board and send another piece
		if (!didMove) {
			StartCoroutine(Lock(piece));
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
