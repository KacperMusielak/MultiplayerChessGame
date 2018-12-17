using UnityEngine;
using System.Collections;

public class Pawn : Piece
{

    public bool hasMoved = false;


    public override bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {


        int deltaMove = Mathf.Abs(x1 - x2);
        int deltaMoveY = y2 - y1;

        if (isWhite)
        {
            if (deltaMove == 0)
            {
                if (deltaMoveY == 1)
                {
                    if (board[x2, y2] == null)
                    {
                        hasMoved = true;
                        return true;
                    }
                    else return false;
                        
                }                   
                if (deltaMoveY == 2 && !hasMoved)
                {
                    if ((board[x2, y2] == null)&&(board[x2, y2-1]==null))
                    {
                        hasMoved = true;
                        return true;
                    }
                    else return false;
                }
                    
            }
            else if (deltaMove == 1)
            {
                if (deltaMoveY == 1)
                {
                    hasMoved = true;
                    Piece p = board[x2,y2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }

        if (!isWhite)
        {
            if (deltaMove == 0)
            {
                if (deltaMoveY == -1)
                {
                    if (board[x2, y2] == null)
                    {
                        hasMoved = true;
                        return true;
                    }
                    else return false;
                }
                if (deltaMoveY == -2 && !hasMoved)
                {
                    if ((board[x2, y2] == null) && (board[x2, y2 + 1] == null))
                    {
                        hasMoved = true;
                        return true;
                    }
                    else return false;
                }
            }
            else if (deltaMove == 1)
            {
                if (deltaMoveY == -1)
                {
                    hasMoved = true;
                    Piece p = board[x2, y2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }

        return false;
    }
}
