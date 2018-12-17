using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece {

    public override bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        int deltaMove = Mathf.Abs(x1 - x2);
        int deltaMoveY = Mathf.Abs(y2 - y1);

        int currentX = x1 + 1;
        int currentY = y1 + 1;
        if (deltaMove == deltaMoveY)
        {
            if (((x1 - x2) < 0) && ((y1 - y2) < 0))
            {
                Debug.Log("Gora, prawo");
                for (int i = 0; i < deltaMove-1; i++)
                {
                    if (board[currentX, currentY] != null)
                    {
                        Debug.Log("Cant move");
                        return false;
                    }
                    currentX++;
                    currentY++;
                }
                return this.CanKill(board, x2, y2);

            }
            if(((x1 - x2) > 0) && ((y1 - y2) < 0))
            {
                Debug.Log("Gora, lewo");
                currentX = x1 - 1;
                currentY = y1 + 1;
                for (int i = 0; i < deltaMove - 1; i++)
                {
                    if (board[currentX, currentY] != null)
                    {
                        Debug.Log("Cant move");
                        return false;
                    }
                    currentX--;
                    currentY++;
                }
                return this.CanKill(board, x2, y2);
            }
            if (((x1 - x2) < 0) && ((y1 - y2) > 0))
            {    
                Debug.Log("Dol, prawo");
                currentX = x1 + 1;
                currentY = y1 - 1;

                for (int i = 0; i < deltaMove - 1; i++)
                {
                    if (board[currentX, currentY] != null)
                    {
                        Debug.Log("Cant move");
                        return false;
                    }
                    currentX++;
                    currentY--;
                }
                return this.CanKill(board, x2, y2);
            }
            if(((x1 - x2) > 0) && ((y1 - y2) > 0))
            {
                Debug.Log("Dol, lewo");
                currentX = x1 - 1;
                currentY = y1 - 1;

                for (int i = 0; i < deltaMove - 1; i++)
                {
                    if (board[currentX, currentY] != null)
                    {
                        Debug.Log("Cant move");
                        return false;
                    }
                    currentX--;
                    currentY--;
                }
                return this.CanKill(board, x2, y2);
            }
            return true;
        }
        return false;
    }
}
