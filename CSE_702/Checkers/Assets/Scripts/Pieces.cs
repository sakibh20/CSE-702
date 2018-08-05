using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour {

	public bool isWhite;
	public bool isKing;
	CheckersBoardScript cbs;
	private void Start(){
		cbs = GameObject.FindObjectOfType<CheckersBoardScript> ();
	}

	public bool IsForceToMove(Pieces[,] board,int x, int y){
		bool forcemove= false;
		cbs.forceposList = new List<Vector2> ();
		
		if (isWhite || isKing)
		{
			
			// Top left
			if (x >= 2 && y <= 5)
			{
				
				Pieces p = board[x - 1, y + 1];
				if(cbs.check ){
					//Debug.Log (board[x , y]+"at : "+x+y);
				}
				// If there is a piece, and it is not the same color as ours
				if (p != null && p.isWhite != isWhite)
				{
					
					// Check if its possible to land after the jump
					if (board[x - 2, y + 2] == null){
						cbs.forceposList.Add (new Vector2(x-2,y+2));
						//Debug.Log ("Top Left True");
						int d=((x-1)*10)+(y+1);
						if(cbs.check && !cbs.willbeKilledCount.Contains(d)){
							cbs.willbeKilledCount.Add (d);
						}
						if(cbs.KillCount){
							cbs.elligibleForceMOveList.Add (new Vector2(x-2,y+2));
						}
						forcemove= true;
					}
						
				}
			}

			// Top Right
			if (x <= 5 && y <= 5)
			{
				Pieces p = board[x + 1, y + 1];
				// If there is a piece, and it is not the same color as ours
				if (p != null && p.isWhite != isWhite)
				{
					//Debug.Log ("Top Right True");
					// Check if its possible to land after the jump
					if (board[x + 2, y + 2] == null){
						cbs.forceposList.Add (new Vector2(x+2,y+2));
						int d=((x+1)*10)+(y+1);
						if(cbs.check && !cbs.willbeKilledCount.Contains(d)){
							cbs.willbeKilledCount.Add (d);
						}
						if(cbs.KillCount){
							cbs.elligibleForceMOveList.Add (new Vector2(x+2,y+2));
						}
						forcemove= true;
					}
						
				}
			}
		} 

		if(!isWhite || isKing)
		{            
			// Bot left
			if (x >= 2 && y >= 2)
			{
				Pieces p = board[x - 1, y - 1];
				// If there is a piece, and it is not the same color as ours
				if (p != null && p.isWhite != isWhite)
				{
					//Debug.Log ("Down left True");
					// Check if its possible to land after the jump
					if (board [x - 2, y - 2] == null) {
						cbs.forceposList.Add (new Vector2(x-2,y-2));
						int d=((x-1)*10)+(y-1);
						if(cbs.check && !cbs.willbeKilledCount.Contains(d)){
							cbs.willbeKilledCount.Add (d);
						}
						if(cbs.KillCount){
							cbs.elligibleForceMOveList.Add (new Vector2(x-2,y-2));
						}
						forcemove= true;

					}
				}
			}

			// Bot Right
			if (x <= 5 && y >= 2)
			{
				Pieces p = board[x + 1, y - 1];
				// If there is a piece, and it is not the same color as ours
				if (p != null && p.isWhite != isWhite)
				{
					//Debug.Log ("Down Right True");
					// Check if its possible to land after the jump
					if (board [x + 2, y - 2] == null) {
						cbs.forceposList.Add (new Vector2(x+2,y-2));
						int d=((x+1)*10)+(y-1);
						if(cbs.check && !cbs.willbeKilledCount.Contains(d)){
							cbs.willbeKilledCount.Add (d);
						}
						if(cbs.KillCount){
							cbs.elligibleForceMOveList.Add (new Vector2(x+2,y-2));
						}
						forcemove= true;
					}
				}
			}
		}
		if(forcemove){
			return true;
		}
		return false;
	}

	public bool Validmove(Pieces[,] board, int x1, int y1, int x2, int y2){
		// if on top on another piece
		if(board[x2,y2] !=null ){
			return false;
		}

		int deltaMove = Mathf.Abs (x1-x2);
		int deltaMoveY = y1-y2;
		if(isWhite || isKing){
			
			if(deltaMove==1){
				
				if (deltaMoveY == -1) {
					return true;
				}
			}
			else if (deltaMove ==2){
				if(deltaMoveY==-2){
					Pieces p=board[(x1+x2)/2,(y1+y2)/2];
					if (p!=null && p.isWhite != isWhite){
						return true;
					}
				}
			}

		}


		if(!isWhite || isKing){
			if(deltaMove==1){
				if (deltaMoveY == 1) {
					return true;
				}
			}
			else if (deltaMove ==2){
				if(deltaMoveY==2){
					Pieces p=board[(x1+x2)/2,(y1+y2)/2];
					if (p!=null && p.isWhite !=isWhite){
						return true;
					}
				}
			}

		}
		return false;

	}
}
