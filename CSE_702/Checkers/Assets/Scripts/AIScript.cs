using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour {

	public List<Pieces> AiPieces;
	public List<Vector2> piecePos;
	public List<Vector2> elligibleMovePoslist;
	CheckersBoardScript cbs;

	// Use this for initialization
	void Start () {
		AiPieces = new List<Pieces> ();
		cbs = GameObject.FindObjectOfType<CheckersBoardScript>();
		cbs.AIsMove = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DoAI (){
		cbs.isWhite = true;
		cbs.ScanForPossibleMove();

		Debug.Log ("f="+cbs.forcedPieces.Count);
		if(cbs.forcedPieces.Count !=0){

			Debug.Log ("Called Force Move");
			MovableForcedPieces ();
			moveForcedPiece ();
		}
		else{
			MovablePieces ();
			moveRandomPiece ();
		}

		//if any force move


		//Debug.Log (AiPieces.Count);






	}


	private void MovablePieces () {
		
		bool go = false;
		AiPieces = new List<Pieces> ();
		piecePos = new List<Vector2> ();
		for(int i=0;i<8;i++){
			for(int j=0;j<8;j++){
				if(cbs.pieces[i,j]!=null && !cbs.pieces[i,j].isWhite && !go){
					for(int m=0;m<8;m++){
						for(int n=0;n<8;n++){
							if(cbs.pieces[i,j].Validmove(cbs.pieces,i,j,m,n)){
								//Debug.Log (i+" "+j);
								AiPieces.Add (cbs.pieces[i,j]);
								piecePos.Add(new Vector2 (i,j));
								go = true;
								break;
							}

						}
						if (go) 
						{
							break;
						}
					}
				}
				if (go) 
				{
					go = false;
					continue;
				}
			}

		}
	}


	private void MovableForcedPieces () {
		bool go = false;
		AiPieces = new List<Pieces> ();
		piecePos = new List<Vector2> ();
		for(int i=0;i<8;i++){
			for(int j=0;j<8;j++){
				if(cbs.pieces[i,j]!=null && !cbs.pieces[i,j].isWhite && !go){
					for(int m=0;m<8;m++){
						for(int n=0;n<8;n++){
							if(cbs.pieces[i,j].Validmove(cbs.pieces,i,j,m,n) ){
								if (cbs.pieces [i, j].IsForceToMove (cbs.pieces, i, j)) {
									//Debug.Log (i+" "+j);
									AiPieces.Add (cbs.pieces[i,j]);
									piecePos.Add(new Vector2 (i,j));
									go = true;
									break;
								}

							}

						}
						if (go) 
						{
							break;
						}
					}
				}
				if (go) 
				{
					go = false;
					continue;
				}
			}

		}
	}



	private void moveRandomPiece (){
		int targetedPiece = Random.Range (0,AiPieces.Count);
		Debug.Log ("target"+" "+targetedPiece);
		elligibleMovePos (targetedPiece);
		int targetedPos = Random.Range (0,elligibleMovePoslist.Count);
		//StartCoroutine ("wait");
		cbs.TryMove((int)piecePos[targetedPiece].x,(int)piecePos[targetedPiece].y,(int)elligibleMovePoslist[targetedPos].x,(int)elligibleMovePoslist[targetedPos].y);
		Debug.Log ("Moved");
	}


	private void moveForcedPiece (){
		int target = Random.Range (0,cbs.forcedPieces.Count);
		Debug.Log ("target"+" "+target);
		elligibleForcedMovePos (target);
		int targetedPos = Random.Range (0,elligibleMovePoslist.Count);
		//StartCoroutine ("wait");
		//cbs.TryMove((int)piecePos[target].x,(int)piecePos[target].y,(int)elligibleMovePoslist[targetedPos].x,(int)elligibleMovePoslist[targetedPos].y);
		cbs.TryMove((int)piecePos[target].x,(int)piecePos[target].y,(int)elligibleMovePoslist[targetedPos].x,(int)elligibleMovePoslist[targetedPos].y);
		Debug.Log ("Moved");
	}


	private void elligibleMovePos (int target){


		elligibleMovePoslist = new List<Vector2> ();

		for (int m = 0; m < 8; m++) {
			for (int n = 0; n < 8; n++) {
				if (cbs.pieces[(int)piecePos[target].x,(int)piecePos[target].y].Validmove (cbs.pieces,(int)piecePos[target].x,(int)piecePos[target].y, m, n)) {
					elligibleMovePoslist.Add (new Vector2(m,n));
					Debug.Log (m+"  "+n);
				}
			}
		}
	}

	private void elligibleForcedMovePos (int target){


		elligibleMovePoslist = new List<Vector2> ();

		for (int m = 0; m < 8; m++) {
			for (int n = 0; n < 8; n++) {
				if (cbs.pieces[(int)piecePos[target].x,(int)piecePos[target].y].Validmove (cbs.pieces,(int)piecePos[target].x,(int)piecePos[target].y, m, n) &&Mathf.Abs ((int)piecePos[target].x - m) == 2) {
					elligibleMovePoslist.Add (new Vector2(m,n));
					Debug.Log (m+"  "+n);
				}
			}
		}
	}


	public void wait(){
		float t = Time.timeScale;
		Debug.Log (t);
	}
}
