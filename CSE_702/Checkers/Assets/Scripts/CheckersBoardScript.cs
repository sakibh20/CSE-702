using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CheckersBoardScript : MonoBehaviour {
	public AIPlayerClass aiclass;
	public Pieces[,] pieces= new Pieces[8,8];
	public Pieces[,] Pieces_Copy;
	public GameObject whitePiecePrefabs;
	public GameObject BlackPiecePrefabs;
	public GameObject HighlightContainer;
	private Vector3 boardOffset=new Vector3(-3.5f, 0.125f, -3.5f);
	public Pieces SelectedPieces;
	public Pieces Bc;
	public Pieces BackupPiece;
	public Pieces TestingPiece;
	public Pieces TargetToMove;
	public Vector2 TargetToMovePos;
	public List<Vector2> BackupPositions;
	public List<Vector2> WillBePositions;
	public List<Pieces> EatenPiecesList;
	public List<Vector2> forceposList;
	public List<int> looplength;
	private Vector2 mouseOver;
	private Vector2 startDrag;
	private Vector2 destinationDrag;
	public List<Vector2> elligibleForceMOveList;
	public bool isWhiteTurn;
	public bool isWhite;
	public bool hasKilled;
	public bool hasKilled2;
	public List<Pieces> forcedPieces;
	public List<Pieces> forcedPiecesForParticular;
	public List<int> willbeKilledCount;
	public List<GameObject> PossibleMoveHighlightContainer;
	private int whiteScore;
	private int blackScore;
	public int minimumKill;
	private bool AgainKill;
	private int AgainKillX;
	private int AgainKillY;
	public TextMeshProUGUI whiteS;
	public TextMeshProUGUI blackS;
	public TextMeshProUGUI winText;
	public TextMeshProUGUI AIwinText;
	public TextMeshProUGUI congoText;

	public TextMeshProUGUI whiteTurnText;
	public TextMeshProUGUI BlackTurnText;
	public GameObject WhiteTurn;
	public GameObject BlackTurn;
	public GameObject winPanel;
	public GameObject DrawPanel;
	public GameObject BoardSet;
	public GameObject all;
	private int WhiteMoveCount;
	private int BlackMoveCount;
	public bool AIPlayer;
	public bool AIsMove;
	public bool check;
	public bool KillCount;
	public bool cancelThisMove;
	public int a1;
	public int b1;
	public int a2;
	public int b2;


	public int initialKillCount;
	public int FinalKillCount;
	public int BestKillSide;

	AudioSource audioSource;

	private void Start(){
		audioSource = GetComponent<AudioSource> ();

		initialKillCount = 0;
		FinalKillCount = -1;
		//StartCoroutine ("wait");
		aiclass=new AIPlayerClass() ;
		if(PlayerPrefs.GetInt("AI")==1){
			AIPlayer = true;
			BlackTurnText.text = "MY   TURN";
			whiteTurnText.text = "YOUR  TURN";
		}
		
		winPanel.SetActive (false);
		DrawPanel.SetActive (false);
		BlackTurn.SetActive (false);
		whiteS.text = whiteScore.ToString();
		blackS.text = blackScore.ToString();
		foreach(Transform t in HighlightContainer.transform){
			t.position = Vector3.down * 100;
		}
		isWhite = true;
		isWhiteTurn = true;
		AIsMove = false;
		forcedPieces= new List<Pieces>();
		GenerateBoard ();
		AvailableMove ();
	}

	private void Update(){
		
		if(AIsMove){
			Debug.Log ("calling DoAI");

			//AIsMove = false;
			aiclass.DoAI ();
		}
		//Debug.Log (mouseOver);
//		foreach(Transform t in HighlightContainer.transform){
//			t.Rotate(Vector3.up*90*Time.deltaTime);
//		}

		UpdateMouseOver ();
		//CheckVictory ();

		//if AIs Turn


		//if my turn
		if((isWhite)?isWhiteTurn:!isWhiteTurn)
		{
			int x=(int)(mouseOver.x);
			int y=(int)(mouseOver.y);

			if(SelectedPieces !=null){
				
				updatepieceDrag (SelectedPieces);
			}
			if(Input.GetMouseButtonDown(0)){
				clearHighlight ();
				if(AgainKill){
					if(AgainKillX !=x || AgainKillY !=y){
						//clearHighlight ();
						return;
					}
				}
				possibleMoveHighlight (x,y);
				//Debug.Log ("Working");
					
				Selectpiece (x,y);
			}
			if(Input.GetMouseButtonUp(0)){
				
				TryMove ((int)startDrag.x, (int)startDrag.y, x, y);
			}
		}
	}
		

	private void UpdateMouseOver(){
		if(!Camera.main){
			//Debug.Log ("No Main Camara");
			return;
		}

		RaycastHit hit;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 25.0f, LayerMask.GetMask ("Board"))) {
			mouseOver.x = (int)(hit.point.x+4.0f);
			mouseOver.y = (int)(hit.point.z+4.0f);
		}
		else{
			mouseOver.x = -1;
			mouseOver.y = -1;
		}
	}

	private void updatepieceDrag(Pieces p ){
		if(!Camera.main){
			Debug.Log ("No Main Camara");
			return;
		}

		RaycastHit hit;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 25.0f, LayerMask.GetMask ("Board"))) {
			p.transform.position = hit.point + Vector3.up;
		}
	}

	private void Selectpiece(int x, int y){
		//out of bounds
		if(x < 0 || x >= 8 || y < 0 || y >= 8){
			//clearHighlight ();
			return;
		}
		Pieces p=pieces[x,y];
		if (p != null && p.isWhite==isWhite) {
			if(forcedPieces.Count==0){
				SelectedPieces = p;
				startDrag = mouseOver;
			}
			else{
				if(forcedPieces.Find(fp => fp==p)==null){
					return;
				}
				SelectedPieces = p;
				startDrag = mouseOver;
			}
				
		}
	}

	public void TryMove(int x1,int y1,int x2, int y2){


		if(!AgainKill ){
			forcedPieces = ScanForPossibleMove ();
		}

		
		//multiplayer support
		startDrag=new Vector2(x1,y1);
		destinationDrag = new Vector2 (x2,y2);
		SelectedPieces=pieces[x1,y1];
		if(check || KillCount){
			Bc=pieces[x1,y1];
		}

		//Out of Bounds
		if(x2<0 || x2>=8 || y2<0 || y2 >=8){
			
			if (SelectedPieces != null) {
				MovePiece (SelectedPieces,x1,y1);
			}
			Highlight ();
			startDrag = Vector2.zero;
			SelectedPieces = null;
			return;
		}

		if(SelectedPieces !=null){
			
			
			//if it has not moved
			if(destinationDrag==startDrag){
				MovePiece (SelectedPieces,x1,y1);
				startDrag = Vector2.zero;
				//clearHighlight ();
				Highlight ();
				SelectedPieces = null;
				return;
			}

			// if its a valid move
			if (SelectedPieces.Validmove (pieces, x1, y1, x2, y2)) {
				//Debug.Log ("x1: "+x1+"y1: "+y1);
				if(AgainKill){
					if(AgainKillX !=x1 && AgainKillY !=y1 ){
						MovePiece (SelectedPieces,x1,y1);
						startDrag = Vector2.zero;
						SelectedPieces = null;
						return;
					}
				}
				
				//Did we kill anything
				//if this is a jump
				if (Mathf.Abs (x1 - x2) == 2) {
					
					Pieces p = pieces [(x1 + x2) / 2, (y1 + y2) / 2];

					if (p != null) {
						
						if(!KillCount){
							pieces [(x1 + x2) / 2, (y1 + y2) / 2] = null;
							Destroy (p.gameObject);
						}
							
						hasKilled = true;
						hasKilled2 = true;
						if(! KillCount){
							if(isWhiteTurn){
								WhiteMoveCount = -1;
								BlackMoveCount = -1;
								whiteScore += 1;
								whiteS.text =whiteScore.ToString();

								if(whiteScore==12){
									if (!AIPlayer) {
										winText.text = " WHITE PLAYER ";
									} 
									else {
										winText.text = "";
										AIwinText.text = "YOU WIN";
									}
									winPanel.SetActive (true);
									BoardSet.SetActive (false);
									all.SetActive (false);
									//Victory(true);
								}
							}
							if(!isWhiteTurn){
								BlackMoveCount = -1;
								WhiteMoveCount = -1;
								blackScore += 1;
								blackS.text =blackScore.ToString();
								if(blackScore==12){
									if (!AIPlayer) {
										winText.text = " BLACK PLAYER ";
									} 
									else {
										congoText.text="YOU LOOSE !!!!";
										winText.text="";
										AIwinText.text = "   BETTER LUCK NEXT TIME   ";
									}

									winPanel.SetActive (true);
									BoardSet.SetActive (false);
									all.SetActive (false);
									//Victory(false);
								}
							}
						}
					}
				}

				//were we supposed to kill anything
				if(forcedPieces.Count !=0 && !hasKilled){
					MovePiece (SelectedPieces,x1,y1);
					startDrag = Vector2.zero;
					Highlight ();
					clearHighlight ();
					possibleMoveHighlight (x1,y1);
					SelectedPieces = null;
					return;
				}
					

				if(check){
					a1=x1;
					b1=y1;
					a2=x2;
					b2=y2;

					pieces [x2, y2] = SelectedPieces;
					pieces [x1, y1] = null;

					//Debug.Log (pieces[a1,b1]);
					//Debug.Log (pieces[a2,b2]);
					//Debug.Log(pieces[6,4]+"aaaa");
					FindTurn ();
	
					//cancelThisMove = true;
					pieces [a2, b2] = null;
					pieces  [a1, b1] = Bc;
					//forcedPieces = new List<Pieces> ();



					//Debug.Log (pieces[a1,b1]);
					//Debug.Log (pieces[a2,b2]);
					//forcedPieces = new List<Pieces> ();
					return;
				}
					
				if(!check){
					pieces [x2, y2] = SelectedPieces;
					pieces [x1, y1] = null;

					if (AIsMove && !SelectedPieces.isWhite) {
						
						//PossibleMoveHighlightContainer [36].SetActive (true);
						DoWait (1.3f);

						//StartCoroutine (wait (x2, y2));
					} 
						
					MovePiece (SelectedPieces, x2, y2);
					audioSource.Play ();	
					FindTurn ();


				}

				//AvailableMove();
			} 
			else {
				
				MovePiece (SelectedPieces,x1,y1);
				startDrag = Vector2.zero;
				SelectedPieces = null;
				Highlight ();
				return;
			}
		}
	}

//	IEnumerator wait(int x2,int y2){
//		Debug.Log ("before");
//		yield return new WaitForSeconds (15.0f);
//		Debug.Log ("after");
//		MovePiece (SelectedPieces, x2, y2);
//		audioSource.Play ();	
//		FindTurn ();
//
//		
//	}

	public  void DoWait( float seconds )
	{
		DateTime  ts = DateTime.Now + TimeSpan.FromSeconds( seconds );

		do {} while ( DateTime.Now < ts );
	}


	private void FindTurn(){

		int x = (int)destinationDrag.x;
		int y = (int)destinationDrag.y;

		//promotions

		if(SelectedPieces !=null && !check){
			
			if(SelectedPieces.isWhite && !SelectedPieces.isKing && y==7){
				SelectedPieces.isKing = true;
				SelectedPieces.transform.Rotate (Vector3.right*180);
			}
			else if(! SelectedPieces.isWhite && !SelectedPieces.isKing && y==0){
				SelectedPieces.isKing = true;
				SelectedPieces.transform.Rotate (Vector3.right*180);
			}
		}

		SelectedPieces = null;
		startDrag = Vector2.zero;


		if(!check){
			
			if(ScanForPossibleMove(SelectedPieces,x,y).Count !=0 && hasKilled){
				//Debug.Log ("Returning for 2nd Kill");
				AgainKillX=x;
				AgainKillY = y;
				hasKilled = false;
				Highlight ();
				clearHighlight ();
				AgainKill = true;
				//Debug.Log ("x: "+x+"y: "+y);
				return;
			}
			AgainKill = false;
			if(isWhiteTurn){
				WhiteMoveCount += 1;
				if(WhiteMoveCount==40){
					all.SetActive (false);
					DrawPanel.SetActive (true);
				}
			}
			if(!isWhiteTurn){
				BlackMoveCount += 1;
				if(BlackMoveCount==40){
					all.SetActive (false);
					DrawPanel.SetActive (true);
				}
			}
		}

		isWhiteTurn = !isWhiteTurn;

		if (!AIPlayer) {
			isWhite = !isWhite;
		}
		if(AIPlayer){
			hasKilled2 = false;
			AIsMove = !AIsMove;
		}

		clearHighlight ();
		AvailableMove ();

		if(!check){
			if(isWhiteTurn){
				WhiteTurn.SetActive (true);
				BlackTurn.SetActive (false);
			}
			if(!isWhiteTurn){
				WhiteTurn.SetActive (false);
				BlackTurn.SetActive (true);
			}


		}
		hasKilled = false;


		ScanForPossibleMove ();
		if(check){
//			if(cancelThisMove){
//				//Debug.Log ("force not 0");
//				//cancelThisMove = true;
//				isWhiteTurn = !isWhiteTurn;
//				AIsMove = !AIsMove;
//				//hasKilled = false;
//				return;
//			}
			isWhiteTurn = !isWhiteTurn;
			AIsMove = !AIsMove;
			return;
		}

	}
		
	public void GenerateBoard(){
		//generate White Team
		for(int y=0;y<3;y++){
			bool oddrow = (y % 2 == 0);
			for(int x=0;x<8;x+=2){
				//Generate our piece
				GeneratePiece((oddrow) ? x: x+1 ,y);
			}
		}

		//Generate Black Team
		for(int y=7;y>4;y--){
			bool oddrow = (y % 2 == 0);
			for(int x=0;x<8;x+=2){
				//Generate our piece
				GeneratePiece((oddrow) ? x: x+1 ,y);
			}
		}
	}


	private List<Pieces> ScanForPossibleMove(Pieces p,int x,int y){
		forcedPieces = new List<Pieces> ();

		if(pieces[x,y].IsForceToMove(pieces,x,y)){
			forcedPieces.Add(pieces[x,y]);
		}
	    Highlight ();


		return forcedPieces;
	}



	public List<Pieces> ScanForPossibleMove(){
		forcedPieces = new List<Pieces> ();
		//willbeKilledCount=new List<int>();
		if(check){
			willbeKilledCount=new List<int>();
			//Debug.Log (a1+" "+b1+"To"+a2+" "+b2);
		}

		//check all pieces
		for(int i=0;i<8;i++){
			for(int j=0;j<8;j++){
				if(pieces[i,j]!=null && pieces[i,j].isWhite==isWhiteTurn){
//					if(check){
//						Debug.Log (pieces[i,j]+"at : "+i+j);
//					}
					if(pieces[i,j].IsForceToMove(pieces,i,j)){
						forcedPieces.Add(pieces[i,j]);
//						if(check){
//							cancelThisMove = true;
//							//Debug.Log (forcedPieces.Count);
//							return forcedPieces;
//						}
						cancelThisMove = true;

					}
					
				}
			}
		}
//		if(check && minimumKill>willbeKilledCount.Count){
//			minimumKill = willbeKilledCount.Count;
//		}
//		if(check){
//			Debug.Log (willbeKilledCount.Count+" possible pieces will be killed for "+a1+" "+b1+" to "+a2+" "+b2);
//		}
		if(!check){
			Highlight ();
		}

		return forcedPieces;
	}



	public void possibleMoveHighlight(int i, int j){

		if(AIPlayer && AIsMove){
			PossibleMoveHighlightContainer [i + j * 8].SetActive (true);
			return;
		}

		if (i<0 || i>7 || j<0 || j>7) {
			return;
		}

		bool kill = false;
		//if it is a kill
		if (pieces [i, j] != null && pieces [i, j].isWhite == isWhiteTurn) {
			for (int m = 0; m < 8; m++) {
				for (int n = 0; n < 8; n++) {
					if (pieces [i, j].Validmove (pieces, i, j, m, n)) {
						if (Mathf.Abs (i - m) == 2){
							kill = true;
						}

					}
				}
			}
		}


		if (pieces [i, j] != null && pieces [i, j].isWhite == isWhiteTurn) {
			for (int m = 0; m < 8; m++) {
				for (int n = 0; n < 8; n++) {
					if (kill) {
						if (Mathf.Abs (i - m) == 2) {
							if (pieces [i, j].Validmove (pieces, i, j, m, n)) {
							    PossibleMoveHighlightContainer [m + n * 8].SetActive (true);

							}
						}
					} 
					else {
						if (pieces [i, j].Validmove (pieces, i, j, m, n)) {
							PossibleMoveHighlightContainer [m + n * 8].SetActive (true);

						}
					}

				}
			}
		}


	}




	public void clearHighlight(){
		for (int m = 0; m < 8; m++) {
			for (int n = 0; n < 8; n++) {
				PossibleMoveHighlightContainer[m+n*8].SetActive(false);
			}
		}
	}


	private void AvailableMove(){
		bool gameOver=true;
		for(int i=0;i<8;i++){
			for(int j=0;j<8;j++){
				if(pieces[i,j]!=null && pieces[i,j].isWhite==isWhiteTurn){
					for(int m=0;m<8;m++){
						for(int n=0;n<8;n++){
							if(pieces[i,j].Validmove(pieces,i,j,m,n)){
								gameOver = false;
							}
						}
					}
				}
			}
		}

		if(gameOver){
			if(isWhiteTurn){
				all.SetActive(false);
				winPanel.SetActive (true);
				if (!AIPlayer) {
					winText.text = " BLACK PLAYER ";
				} 
				else {
					congoText.text="YOU LOOSE !!!!";
					winText.text="";
					AIwinText.text = "   BETTER LUCK NEXT TIME   ";
				}

			}
			else{
				all.SetActive(false);
				winPanel.SetActive (true);
				if (!AIPlayer) {
					winText.text = " WHITE PLAYER ";
				} 
				else {
					winText.text = "YOU WIN";
					AIwinText.text = "";
				}
			}
		}
	}

	private void Highlight(){

		foreach(Transform t in HighlightContainer.transform){
			t.position = Vector3.down * 100;
		}

		if(forcedPieces.Count>0){
			HighlightContainer.transform.GetChild (0).transform.position = forcedPieces [0].transform.position + Vector3.down * 0.1f;
		}
		if(forcedPieces.Count>1){
			HighlightContainer.transform.GetChild (1).transform.position = forcedPieces [1].transform.position + Vector3.down * 0.1f;
		}
		if(forcedPieces.Count>2){
			HighlightContainer.transform.GetChild (2).transform.position = forcedPieces [2].transform.position + Vector3.down * 0.1f;
		}
		if(forcedPieces.Count>3){
			HighlightContainer.transform.GetChild (3).transform.position = forcedPieces [3].transform.position + Vector3.down * 0.1f;
		}
	}


	private void GeneratePiece(int x,int y){
		bool isPieceWhite = (y > 3) ? false : true;
		GameObject go = Instantiate ((isPieceWhite) ? whitePiecePrefabs:BlackPiecePrefabs) as GameObject;
		go.transform.SetParent (transform);
		Pieces p = go.GetComponent<Pieces> ();
		pieces [x, y] = p;
		MovePiece (p,x,y);
	}

	public void MovePiece(Pieces p,int x,int y){
		p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset ;
		//Debug.Log (pieces[x,y]+"Moved To :"+x+" "+y);
	}


	private List<Pieces> ScanForParticularPiece(Pieces p,int x,int y){
		forcedPiecesForParticular = new List<Pieces> ();

		if(pieces[x,y].IsForceToMove(pieces,x,y)){
			forcedPiecesForParticular.Add(pieces[x,y]);
		}
		//Highlight ();


		return forcedPiecesForParticular;
	}


	public void killCalcutation(int x1,int y1,int x2,int y2){
		
		BackupPositions.Add (new Vector2 (x1, y1));
		WillBePositions.Add (new Vector2 (x2, y2));
		EatenPiecesList.Add (pieces[(x1+x2)/2,(y1+y2)/2]);
		TestingPiece=pieces[x1,y1];
		pieces [x1, y1] = null;
		pieces [x2, y2] = TestingPiece;
		for(int m=0;m<WillBePositions.Count;m++){
			Debug.Log("WillBePositions: "+WillBePositions[m]);
		}
		pieces [(x1 + x2) / 2, (y1 + y2) / 2] = null;
		Debug.Log ("Null at: "+(x1 + x2) / 2+" "+(y1 + y2) / 2);
		if (ScanForParticularPiece (TestingPiece, x2, y2).Count != 0) {


//			if(forceposList.Contains(new Vector2(x1,y1))){
//				forceposList.Remove (new Vector2(x1,y1));
//				if(forceposList.Count ==0){
//					pieces [(int)WillBePositions [WillBePositions.Count - 1].x, (int)WillBePositions [WillBePositions.Count - 1].y] = null;
//					WillBePositions.RemoveAt (WillBePositions.Count - 1);
//					int c = BackupPositions.Count;
//					pieces [(int)BackupPositions [c - 1].x, (int)BackupPositions [c - 1].y] = TestingPiece;
//					BackupPositions.RemoveAt (c - 1);
//					return;
//				}
//			}


			initialKillCount += 1;
			BestKillSide += 1;
			//Debug.Log ("ForcePiecesCount: " + forcedPieces.Count);
			int x = forceposList.Count;
			looplength.Add (forceposList.Count);
			//Debug.Log ("forceposList.Count: " + forceposList.Count);
			Debug.Log ("x: " + x);
			for (int i = 0; i < looplength [looplength.Count - 1]; i++) {
				Debug.Log ("looplength [looplength.Count - 1]: " + looplength [looplength.Count - 1]);
				Debug.Log ("x2 y2 " + x2 + " " + y2);
				int p = WillBePositions.Count;
				try{
					Debug.Log("TestingPiece: "+TestingPiece);
					for(int m=0;m<WillBePositions.Count;m++){
						Debug.Log("WillBePositions: "+WillBePositions[m]);
					}
					ScanForParticularPiece (TestingPiece,(int)WillBePositions [p - 1].x,(int)WillBePositions [p - 1].y);
					//Debug.Log("WillBePositions: "+(int)WillBePositions [p - 1].x +" "+(int)WillBePositions [p - 1].y );
					Debug.Log ("Inside Calling For: " + (int)WillBePositions [p - 1].x + " " + (int)WillBePositions [p - 1].y + " " + (int)forceposList [i].x + " " + (int)forceposList [i].y);
					killCalcutation ((int)WillBePositions [p - 1].x, (int)WillBePositions [p - 1].y, (int)forceposList [i].x, (int)forceposList [i].y);

				}
				catch(ArgumentOutOfRangeException){
					Debug.Log ("Heeee Heeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
				}
				Debug.Log ("looplength [looplength.Count - 1]: " + looplength [looplength.Count - 1]);
				pieces [(int)WillBePositions [WillBePositions.Count - 1].x, (int)WillBePositions [WillBePositions.Count - 1].y] = null;
				WillBePositions.RemoveAt (WillBePositions.Count - 1);
				for(int m=0;m<WillBePositions.Count;m++){
					Debug.Log("WillBePositions: "+WillBePositions[m]);
				}
				int c = BackupPositions.Count;
				pieces [(int)BackupPositions [c - 1].x, (int)BackupPositions [c - 1].y] = TestingPiece;
				BackupPositions.RemoveAt (c - 1);
				for(int m=0;m<WillBePositions.Count;m++){
					Debug.Log("WillBePositions: "+WillBePositions[m]);
				}
				//Debug.Log ("ForcePiecesCount: " + forcedPieces.Count);

			}


		

			//Debug.Log ("Atleast Two kill for " + x1 + " " + y1);
		} 
		else {
			looplength.Add (0);

		}
		looplength.RemoveAt (looplength.Count-1);
		int bp = BackupPositions.Count;
		int wp = WillBePositions.Count;
		pieces [(int)(BackupPositions [bp - 1].x + WillBePositions [wp - 1].x) / 2, (int)(BackupPositions [bp - 1].y + WillBePositions [wp - 1].y) / 2] = EatenPiecesList [EatenPiecesList.Count - 1];
		Debug.Log ("Inserting "+pieces[(int)(BackupPositions [bp - 1].x + WillBePositions [wp - 1].x) / 2, (int)(BackupPositions [bp - 1].y + WillBePositions [wp - 1].y) / 2]+" at "+(x1 + x2) / 2+" "+(y1 + y2) / 2);
		EatenPiecesList.RemoveAt (EatenPiecesList.Count-1);
		pieces[x2,y2]=null;
		//Debug.Log("ForcePiecesCount: "+forcedPieces.Count);
//		else {
//			//Debug.Log ("Onely One Kill For " + x1 + " " + y1);
//			int c = BackupPositions.Count;
//			pieces [x2, y2] = null;
//			pieces [(int)BackupPositions [c - 1].x, (int)BackupPositions [c- 1].y]=TestingPiece;
//			BackupPositions.RemoveAt (c-1);
//		}
//
//		pieces [(int)WillBePositions [WillBePositions.Count - 1].x, (int)WillBePositions [WillBePositions.Count - 1].y]=null;
//		pieces [(int)BackupPositions [BackupPositions.Count - 1].x, (int)BackupPositions [WillBePositions.Count - 1].y]=TestingPiece;
//
//		BackupPositions.RemoveAt (BackupPositions.Count-1);
//		WillBePositions.RemoveAt (WillBePositions.Count-1);
	}
}
