using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public bool isWhite;
    public bool isKing;

    public virtual bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        return false;
    }
    public bool CanKill(Piece[,] board, int x, int y)
    {
        Piece p = board[x, y];
        if ((p != null && p.isWhite == isWhite))
        {
            return false;
        }
        else return true;

    }
}
