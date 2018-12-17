using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChessBoard : MonoBehaviour
{
    public static ChessBoard Instance { set; get; }

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject whiteBishopPrefab;
    public GameObject blackBishopPrefab;
    public GameObject whiteRookPrefab;
    public GameObject blackRookPrefab;
    public GameObject whiteKnightPrefab;
    public GameObject blackKnightPrefab;
    public GameObject whiteQueenPrefab;
    public GameObject blackQueenPrefab;
    public GameObject whiteKingPrefab;
    public GameObject blackKingPrefab;
    public GameObject victoryMenu;

    private bool gameIsOver;
    private float winTime;

    private Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0.125f, 0.5f);

    public bool isWhite;
    private bool isWhiteTurn;
    private bool hasKilled;

    private Piece selectedPiece;
    private List<Piece> forcedPieces;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private Client client;
    private Text text;

    private void Start()
    {
        victoryMenu.SetActive(false);
        Instance = this;
        client = FindObjectOfType<Client>();


        if (client)
        {
            isWhite = client.isHost;
        }
        else
        {
            Transform c = GameObject.Find("Canvas").transform;
            foreach (Transform t in c)
            {
                t.gameObject.SetActive(false);
            }

            c.GetChild(0).gameObject.SetActive(true);
        }

        isWhiteTurn = true;
        forcedPieces = new List<Piece>();
        GenerateBoard();
    }

    private void Update()
    {
        if (gameIsOver)
        {
            if (Time.time - winTime > 3.0f)
            {
                Server server = FindObjectOfType<Server>();
                Client client = FindObjectOfType<Client>();

                if (server)
                    Destroy(server.gameObject);

                if (client)
                    Destroy(client.gameObject);

                victoryMenu.SetActive(true);

                //SceneManager.LoadScene("MenuScene");
            }

            return;
        }

        UpdateMouseOver();

        if ((isWhite) ? isWhiteTurn : !isWhiteTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (selectedPiece != null)
                UpdatePieceDrag(selectedPiece);

            if (Input.GetMouseButtonDown(0))
                SelectPiece(x, y);

            if (Input.GetMouseButtonUp(0))
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
        }
    }
    public void OkButton()
    {
        SceneManager.LoadScene("MenuScene");
    }
    private void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }
    private void UpdatePieceDrag(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    private void SelectPiece(int x, int y)
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
            return;

        Piece p = pieces[x, y];
        if (p != null && p.isWhite == isWhite)
        {
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                if (forcedPieces.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }
    public void TryMove(int x1, int y1, int x2, int y2)
    {
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8)
        {
            if (selectedPiece != null)
                MovePiece(selectedPiece, x1, y1);

            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }

        if (selectedPiece != null)
        {
            if (endDrag == startDrag)
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                Debug.Log("valid?");
                    Piece p = pieces[x2,y2];
                    if (p != null)
                    {
                        Debug.Log("killed");
                        pieces[x2,y2] = null;
                        DestroyImmediate(p.gameObject);
                        hasKilled = true;
                    }
                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }
    private void EndTurn()
    {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;


        if (client)
        {
            string msg = "CMOV|";
            msg += startDrag.x.ToString() + "|";
            msg += startDrag.y.ToString() + "|";
            msg += endDrag.x.ToString() + "|";
            msg += endDrag.y.ToString();

            client.Send(msg);
        }

        selectedPiece = null;
        startDrag = Vector2.zero;


        isWhiteTurn = !isWhiteTurn;
        hasKilled = false;
        CheckVictory();

    }
    private void CheckVictory()
    {
        var ps = FindObjectsOfType<King>();
        bool hasWhite = false, hasBlack = false;
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].isWhite)
                hasWhite = true;
            else
                hasBlack = true;
        }
        Debug.Log(ps.Length);
        if (!hasWhite)
            Victory(false);
        if (!hasBlack)
            Victory(true);
    }
    private void Victory(bool isWhite)
    {
        winTime = Time.time;
    
         if (isWhite)
            Debug.Log("White player has won!");
         else
            Debug.Log("Black player has won!");

        gameIsOver = true;
    }


    private void GenerateBoard()
    {
        for (int x = 0; x < 8; x += 1)
            {
                GeneratePiece(x, 1, "Pawn");
            }
        GeneratePiece(2, 0, "Bishop");
        GeneratePiece(5, 0, "Bishop");
        GeneratePiece(0, 0, "Rook");
        GeneratePiece(7, 0, "Rook");
        GeneratePiece(1, 0, "Knight");
        GeneratePiece(6, 0, "Knight");
        GeneratePiece(3, 0, "Queen");
        GeneratePiece(4, 0, "King");

        for (int x = 0; x < 8; x += 1)
            {
                GeneratePiece(x, 6, "Pawn");
            }
        GeneratePiece(2, 7, "Bishop");
        GeneratePiece(5, 7, "Bishop");
        GeneratePiece(0, 7, "Rook");
        GeneratePiece(7, 7, "Rook");
        GeneratePiece(1, 7, "Knight");
        GeneratePiece(6, 7, "Knight");
        GeneratePiece(3, 7, "Queen");
        GeneratePiece(4, 7, "King");

    }
    private void GeneratePiece(int x, int y, string piece)
    {
        bool isPieceWhite = (y > 3) ? false : true;
        if(piece == "Pawn")
        {
            GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
            go.transform.SetParent(transform);
            Pawn p = go.GetComponent<Pawn>();
            pieces[x, y] = p;
            MovePiece(p, x, y);
        }
        if(piece == "Bishop")
        {
            GameObject go = Instantiate((isPieceWhite) ? whiteBishopPrefab : blackBishopPrefab) as GameObject;
            go.transform.SetParent(transform);
            Bishop p = go.GetComponent<Bishop>();
            pieces[x, y] = p;
            MovePiece(p, x, y);
        }
        if(piece == "Rook")
        {
            GameObject go = Instantiate((isPieceWhite) ? whiteRookPrefab : blackRookPrefab) as GameObject;
            go.transform.SetParent(transform);
            Rook p = go.GetComponent<Rook>();
            pieces[x, y] = p;
            MovePiece(p, x, y);
        }
        if (piece == "Queen")
        {
            GameObject go = Instantiate((isPieceWhite) ? whiteQueenPrefab : blackQueenPrefab) as GameObject;
            go.transform.SetParent(transform);
            Queen p = go.GetComponent<Queen>();
            pieces[x, y] = p;
            MovePiece(p, x, y);
        }
        if (piece == "King")
        {
            GameObject go = Instantiate((isPieceWhite) ? whiteKingPrefab : blackKingPrefab) as GameObject;
            go.transform.SetParent(transform);
            King p = go.GetComponent<King>();
            pieces[x, y] = p;
            MovePiece(p, x, y);
        }
        if (piece == "Knight")
        {
            GameObject go = Instantiate((isPieceWhite) ? whiteKnightPrefab : blackKnightPrefab) as GameObject;
            go.transform.SetParent(transform);
            Knight p = go.GetComponent<Knight>();
            pieces[x, y] = p;
            MovePiece(p, x, y);
        }
    }
    private void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
    
}
