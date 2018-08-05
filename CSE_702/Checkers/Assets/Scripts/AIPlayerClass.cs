using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class AIPlayerClass
{
	public Pieces[,] BackupPieces= new Pieces[8,8];
	public List<Pieces> AiPieces;
	public List<Vector2> piecePos;
	public List<Vector2> elligibleMovePoslist;
	CheckersBoardScript cbs;
	private bool working;
	private bool moved;
//	private bool ListEmpty;
	private bool startWaiting;
	public int waitTime;
	private int MinimumWillBeKilled;
	private int x1;
	private int y1;
	private int x2;
	private int y2;
	private int T;
	private int TargetedPos;
	private int a;
	private int b;

	public AIPlayerClass ()
	{
		AiPieces = new List<Pieces> ();
		cbs = GameObject.FindObjectOfType<CheckersBoardScript>();
		cbs.AIsMove = false;
		working = false;


	}
		
	private void checkPieces (){
		///elligibleMovePoslist = new List<Vector2> ();

		for (int m = 0; m < 8; m++) {
			for (int n = 0; n < 8; n++) {
				Debug.Log (cbs.pieces[m,n]+"at :"+m+n);
			}
		}
	}

	public void DoAI (){
		
		if(!working){
			working = true;
			cbs.ScanForPossibleMove();
			if(cbs.forcedPieces.Count !=0){
				//Debug.Log ("AIs ForceMove");
				Debug.Log ("Called Force Move");
				MovableForcedPieces ();
				moveForcedPiece ();
				working = false;
				//Debug.Log ("Working is false");
			}
			else{
				moved = false;
				//Debug.Log ("AIs RandomMove");
				MovablePieces ();
				//Debug.Log ("MoveablePieces: "+AiPieces.Count+" "+piecePos.Count);
				if(AiPieces.Count!=0){
					MinimumWillBeKilled = 12;
					moveRandomPiece ();
				}

				cbs.AIsMove = false;
				working = false;
				cbs.isWhiteTurn=true;
				cbs.hasKilled = false;
				//Debug.Log ("Working is false");
			}
			cbs.isWhite = true;
		}

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
								if(cbs.pieces[i,j].isKing){
									AiPieces.Add (cbs.pieces[i,j]);
									piecePos.Add(new Vector2 (i,j));

									AiPieces.Add (cbs.pieces[i,j]);
									piecePos.Add(new Vector2 (i,j));
								}
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
		if (cbs.hasKilled2) {
			Debug.Log ("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
			Debug.Log ("a and b: "+a+" "+b);
			AiPieces = new List<Pieces> ();
			piecePos = new List<Vector2> ();
			AiPieces.Add (cbs.pieces [a, b]);
			piecePos.Add (new Vector2 (a, b));
		} 
		else {
			
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


		Debug.Log ("Aipieces: "+ AiPieces.Count);
	}



	private void moveRandomPiece (){
		
		if(AiPieces.Count==0){
			//Debug.Log ("List Empty");
			cbs.TryMove(x1,y1,x2,y2);
			moved = true;
			cbs.clearHighlight ();
			return;
			//ListEmpty = true;
		    //MovablePieces ();
			//return;
		}
		//Debug.Log ("AiPieces:"+AiPieces.Count);
		int targetedPiece =UnityEngine.Random.Range (0,AiPieces.Count);
		int targetedPos;
		//Debug.Log ("target"+" "+targetedPiece);
		elligibleMovePos (targetedPiece);
		if(elligibleMovePoslist.Count==0){
			//Debug.Log ("NULLLLLLLLLLLLL");
//			cbs.AIsMove = false;
//			cbs.isWhiteTurn=true;
			return;
		}



		//Debug.Log (cbs.hasKilled);
		if (!moved) {
			targetedPos = UnityEngine.Random.Range (0, elligibleMovePoslist.Count);
			cbs.clearHighlight ();
			cbs.PossibleMoveHighlightContainer [(int)elligibleMovePoslist [targetedPos].x + (int)elligibleMovePoslist [targetedPos].y * 8].SetActive (true);
			cbs.possibleMoveHighlight ((int)piecePos [targetedPiece].x, (int)piecePos [targetedPiece].y);
			cbs.TryMove ((int)piecePos [targetedPiece].x, (int)piecePos [targetedPiece].y, (int)elligibleMovePoslist [targetedPos].x, (int)elligibleMovePoslist [targetedPos].y);
			Debug.Log ("From: "+(int)piecePos [targetedPiece].x +" "+ (int)piecePos [targetedPiece].y+" Moved To :"+(int)elligibleMovePoslist [targetedPos].x+" "+ (int)elligibleMovePoslist [targetedPos].y);
			moved = true;
			cbs.clearHighlight ();
			//return;
		} 
		else {
			//ListEmpty = false;
			return;
		}
			
	}

	private void moveForcedPiece (){
		T = 0;
		TargetedPos = 0;
		int tp=0;
		cbs.FinalKillCount = 0;
		cbs.BackupPositions = new List<Vector2> ();
		cbs.WillBePositions = new List<Vector2> ();
		cbs.EatenPiecesList = new List<Pieces> ();
		cbs.looplength = new List<int> ();
		createBackup ();
//		int target;
//		int targetedPos;
//		Debug.Log("ForcePiecesCount: "+cbs.forcedPieces.Count);
		int forceCount=AiPieces.Count;
		for(int i=0; i<forceCount; i++){
			elligibleForcedMovePos (i);
			cbs.initialKillCount = 1;
			int bestkillforj = -1;

			for(int j=0; j<elligibleMovePoslist.Count; j++){
				
				cbs.BackupPiece = cbs.pieces[(int)piecePos [i].x, (int)piecePos [i].y];
				//cbs.KillCount = true;
				//cbs.TryMove ((int)piecePos [i].x, (int)piecePos [i].y, (int)elligibleMovePoslist [j].x, (int)elligibleMovePoslist [j].y);
				Debug.Log ("Calling for "+(int)piecePos [i].x+" "+(int)piecePos [i].y);
				cbs.killCalcutation((int)piecePos [i].x, (int)piecePos [i].y, (int)elligibleMovePoslist [j].x, (int)elligibleMovePoslist [j].y);
				cbs.pieces[(int)piecePos [i].x, (int)piecePos [i].y]=cbs.BackupPiece;
				cbs.pieces[(int)elligibleMovePoslist [j].x, (int)elligibleMovePoslist [j].y]=null;
				if(cbs.BestKillSide>bestkillforj){
					bestkillforj = cbs.BestKillSide;
					tp = j;
				}
				cbs.BestKillSide = 0;
			}
			Debug.Log (cbs.initialKillCount+" Total kill for: "+(int)piecePos [i].x+" "+(int)piecePos [i].y);
			if(cbs.initialKillCount>cbs.FinalKillCount){
				
				T = i;
				TargetedPos = tp;
				Debug.Log ("T: "+T);
				Debug.Log ("TargetedPos: "+TargetedPos);
				cbs.FinalKillCount = cbs.initialKillCount;
				cbs.TargetToMove=cbs.pieces[(int)piecePos [i].x, (int)piecePos [i].y];
				cbs.TargetToMovePos = new Vector2 ((int)piecePos [i].x, (int)piecePos [i].y);
			}
			//cbs.initialKillCount = 0;
		}

		cbs.pieces = cbs.Pieces_Copy;

		Debug.Log("TargetToMovePos: "+(int)cbs.TargetToMovePos.x+" "+(int)cbs.TargetToMovePos.y);
//		Debug.Log("ForcePiecesCount: "+cbs.forcedPieces.Count);
		//int target = UnityEngine.Random.Range (0,cbs.forcedPieces.Count);
		int target = T;
		//Debug.Log ("target"+" "+target);
		elligibleForcedMovePos (target);
		Debug.Log ("target"+" "+target);


		//int targetedPos = UnityEngine.Random.Range (0, elligibleMovePoslist.Count);
		int targetedPos=TargetedPos;
		Debug.Log ("targetedPos"+" "+targetedPos);

		try{
			int p= elligibleMovePoslist.Count;
			Debug.Log("p: "+p);
		}
		catch(ArgumentOutOfRangeException){
			Debug.Log ("Another");
		}


		try{
			cbs.clearHighlight ();
			cbs.PossibleMoveHighlightContainer [(int)elligibleMovePoslist [targetedPos].x + (int)elligibleMovePoslist [targetedPos].y * 8].SetActive (true);

			cbs.possibleMoveHighlight ((int)piecePos [target].x, (int)piecePos [target].y);
			cbs.possibleMoveHighlight ((int)elligibleMovePoslist [targetedPos].x, (int)elligibleMovePoslist [targetedPos].y);

			cbs.TryMove ((int)piecePos [target].x, (int)piecePos [target].y, (int)elligibleMovePoslist [targetedPos].x, (int)elligibleMovePoslist [targetedPos].y);
			Debug.Log (cbs.hasKilled);
			a = (int)elligibleMovePoslist [targetedPos].x;
			b = (int)elligibleMovePoslist [targetedPos].y;
			Debug.Log ("Forcely Moved To "+a+" "+b);

		}
		catch(ArgumentOutOfRangeException){
			Debug.Log ("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
		}



	}



	private void elligibleMovePos (int target){
		int a = (int)piecePos [target].x;
		int b = (int)piecePos [target].y;
		//checkPieces ();

		elligibleMovePoslist = new List<Vector2> ();

		for (int m = 0; m < 8; m++) {
			for (int n = 0; n < 8; n++) {
				//Debug.Log (cbs.pieces[a,b].Validmove(cbs.pieces,a,b,m,n));
				if(cbs.pieces[a,b].Validmove(cbs.pieces,a,b,m,n)){
					//Debug.Log ("True For: "+m+n);
					//checkPieces ();
//					if( ListEmpty ){
//						elligibleMovePoslist.Add (new Vector2(m,n));
//						//Debug.Log ("No Danger in: "+m+"  "+n);
//					}
					if(WillbeTaken(target,m,n)){
						elligibleMovePoslist.Add (new Vector2(m,n));
						//Debug.Log ("Inner for: "+m+n);
						//checkPieces ();
					}
				}
			}
		}
		//Debug.Log ("elligibleMovePoslist.Count: "+elligibleMovePoslist.Count);

		if(elligibleMovePoslist.Count==0){
//			Debug.Log ("AiPieces:"+AiPieces.Count);
//			Debug.Log ("PiecesPos:"+piecePos.Count);
			AiPieces.RemoveAt (target);
			piecePos.RemoveAt (target);
//			Debug.Log ("AiPieces:"+AiPieces.Count);
//			Debug.Log ("PiecesPos:"+piecePos.Count);
//			Debug.Log ("No Smooth Move For This Piece getting another target");
			moveRandomPiece ();
//			if(ListEmpty){
//				moveRandomPiece ();
//			}

		}
	}

	private void elligibleForcedMovePos (int target){
		int a = (int)piecePos [target].x;
		int b = (int)piecePos [target].y;
//		Debug.Log ("(int)piecePos [target].x: "+(int)piecePos [target].x);
//		Debug.Log ("(int)piecePos [target].y: "+(int)piecePos [target].y);
		elligibleMovePoslist = new List<Vector2> ();

		for (int m = 0; m < 8; m++) {
			for (int n = 0; n < 8; n++) {
				if (cbs.pieces[a,b].Validmove(cbs.pieces,a,b,m,n) && Mathf.Abs (a- m) == 2) {
					elligibleMovePoslist.Add (new Vector2(m,n));
					//Debug.Log (m+"  "+n);
				}
			}
		}
	}


	public void createBackup(){
		cbs.Pieces_Copy= new Pieces[8,8];
		cbs.Pieces_Copy = cbs.pieces;

	}
		

	private bool WillbeTaken(int targetedPiece,int m, int n){
		cbs.check = true;
		//Debug.Log ((int)piecePos[targetedPiece].x+" "+(int)piecePos[targetedPiece].y+"To"+m+" "+n);
		cbs.TryMove((int)piecePos[targetedPiece].x,(int)piecePos[targetedPiece].y,m,n);
		if (cbs.cancelThisMove) {
			//Debug.Log ("cbs.cancelThisMove:"+cbs.cancelThisMove);
			if(cbs.willbeKilledCount.Count<MinimumWillBeKilled){
				MinimumWillBeKilled = cbs.willbeKilledCount.Count;
				x1=(int)piecePos[targetedPiece].x;
				y1=(int)piecePos[targetedPiece].y;
				x2=m;
				y2=n;
			}
			cbs.cancelThisMove = false;
			cbs.check = false;
			return false;
		} 
		else {
			
			cbs.check = false;
			return true;
		}
		   
	}
		
}
