using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece {
    public King()
    {
        isKing = true;
    }
   
    public override bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        int deltaMove = Mathf.Abs(x1 - x2);
        int deltaMoveY = Mathf.Abs(y1 - y2);

        if(deltaMove==1 || deltaMoveY == 1)
        {
            return this.CanKill(board, x2, y2);
        }
        return false;
    }
}
