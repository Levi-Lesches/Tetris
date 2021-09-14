using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour {
	public Transform queue;
	public Queue hold;

	Queue[] nextPieces;

	// Start is called before the first frame update
	void Start() {
		nextPieces = queue.GetComponentsInChildren<Queue>();
	}

	public void RenderQueue(List<Piece> queue) {
		for (int index = 0; index < queue.Count; index++) {
			nextPieces [index].DisplayPiece(queue [index]);
		}
	}

	public void RenderHold(Piece held) {
		hold.DisplayPiece(held);
	}
}
