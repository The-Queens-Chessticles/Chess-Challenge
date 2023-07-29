using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    // max depth for the search algorithm
    private static int maxDepth = 3;

    // value maps for the pieces
    private static readonly short[] PawnTable = new short[]
    {
        0,   0,   0,   0,   0,   0,   0,   0,
    50,  50,  50,  50,  50,  50,  50,  50,
    10,  10,  20,  30,  30,  20,  10,  10,
        5,   5,  10,  27,  27,  10,   5,   5,
        0,   0,   0,  25,  25,   0,   0,   0,
        5,  -5, -10,   0,   0, -10,  -5,   5,
        5,  10,  10, -25, -25,  10,  10,   5,
        0,   0,   0,   0,   0,   0,   0,   0
    };
    private static readonly short[] KnightTable = new short[]
    {
    -50, -40, -30, -30, -30, -30, -40, -50,
    -40, -20,   0,   0,   0,   0, -20, -40,
    -30,   0,  10,  15,  15,  10,   0, -30,
    -30,   5,  15,  20,  20,  15,   5, -30,
    -30,   0,  15,  20,  20,  15,   0, -30,
    -30,   5,  10,  15,  15,  10,   5, -30,
    -40, -20,   0,   5,   5,   0, -20, -40,
    -50, -40, -20, -30, -30, -20, -40, -50
    };
    private static readonly short[] BishopTable = new short[]
    {
    -20, -10, -10, -10, -10, -10, -10, -20,
    -10,   0,   0,   0,   0,   0,   0, -10,
    -10,   0,   5,  10,  10,   5,   0, -10,
    -10,   5,   5,  10,  10,   5,   5, -10,
    -10,   0,  10,  10,  10,  10,   0, -10,
    -10,  10,  10,  10,  10,  10,  10, -10,
    -10,   5,   0,   0,   0,   0,   5, -10,
    -20, -10, -40, -10, -10, -40, -10, -20,
    };
    private static readonly short[] KingTable = new short[]
    {
    -30, -40, -40, -50, -50, -40, -40, -30,
    -30, -40, -40, -50, -50, -40, -40, -30,
    -30, -40, -40, -50, -50, -40, -40, -30,
    -30, -40, -40, -50, -50, -40, -40, -30,
    -20, -30, -30, -40, -40, -30, -30, -20,
    -10, -20, -20, -20, -20, -20, -20, -10,
        20,  20,   0,   0,   0,   0,  20,  20,
        20,  30,  10,   0,   0,  10,  30,  20
    };

    public Move Think(Board board, Timer timer)
    {
        // no evaluation at the moment - should return the last move in the list
        Console.WriteLine(Evaluation(board));
        Move bestMove = NegaMaxRoot(board, maxDepth);
        return bestMove;
    }

    // The most basic of chess evaulation search trees - very inefficient but effective to begin
    private static int NegaMax(Board board, int depth)
    {
        bool sideToMove = board.IsWhiteToMove;
        int bestScore = -32767;
        int positionScore;
        if (depth == 0) { if (sideToMove) return Evaluation(board); else return -Evaluation(board); }

        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            positionScore = -NegaMax(board, depth - 1);
            if (positionScore > bestScore)
            {
                bestScore = positionScore;
            }
            board.UndoMove(move);
        }
        return bestScore;
    }

    // Beginning of search for a move - first iteration and will be slow however will transition to an AlphaBeta Implementation
    private static Move NegaMaxRoot(Board board, int depth)
    {
        
        int bestScore = -32767;
        int positionScore = 0;
        Move bestMove = Move.NullMove;
        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            positionScore = -NegaMax(board, depth);
            if (positionScore > bestScore)
            {
                bestScore = positionScore;
                bestMove = move;
            }
            board.UndoMove(move);
        }
        

        return bestMove;


    }
    private static int Evaluation(Board board)
    {
        int boardIndex;
        int whiteScore = 0;
        int blackScore = 0;
        int pieceValue = 0;
        PieceList[] pieceLists = board.GetAllPieceLists();
        foreach (PieceList pieceList in pieceLists)
        {
            foreach (Piece piece in pieceList)
            {
                if (piece.IsWhite) boardIndex = piece.Square.Index;
                else boardIndex = 63 - piece.Square.Index;

                switch (piece.PieceType)
                {
                    // Pawn
                    case PieceType.Pawn:
                        pieceValue = 100 + 2 * PawnTable[boardIndex];
                        break;
                    case PieceType.Knight:
                        pieceValue = 320 + KnightTable[boardIndex];
                        break;
                    case PieceType.Bishop:
                        pieceValue = 325 + BishopTable[boardIndex];
                        break;
                    case PieceType.Rook:
                        pieceValue = 500;
                        break;
                    case PieceType.Queen:
                        pieceValue = 925;
                        break;
                    case PieceType.King:
                        pieceValue = 10000 + KingTable[boardIndex];
                        break;
                    default:
                        pieceValue = 0;
                        break;

                }

                if (piece.IsWhite) whiteScore += pieceValue;
                else blackScore += pieceValue;

            }
        }
        //Console.WriteLine(whiteScore);
        //Console.WriteLine(blackScore);
        return whiteScore - blackScore;
    }
}
