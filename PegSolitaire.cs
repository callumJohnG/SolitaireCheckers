using System.Numerics;

public class PegSolitaire{

    //The class containing the logic for the peg game

    private char[,] gameBoard;

    public char[,] GetGameBoard(){return gameBoard;}

    private int xMin = 1;
    private int xMax = 5;
    private int yMin = 1;
    private int yMax = 5;
    private const int BOARD_SIZE = 7;
    private const int BOARD_CENTER = 3;

    private const char PEG = 'o';
    private const char GAP = '.';
    private const char WALL = ' ';


    public PegSolitaire(){
        InitialiseGameBoard(out gameBoard);
    }

    private void InitialiseGameBoard(out char[,] gameBoard){
        gameBoard = new char[BOARD_SIZE,BOARD_SIZE];

        for(int x = 0; x < BOARD_SIZE; x++){
            for(int y = 0; y < BOARD_SIZE; y++){
                char nextChar;

                if( (x <= xMin || x >= xMax) && (y <= yMin || y >= yMax) ){
                    nextChar = WALL;
                } else if (x == BOARD_CENTER && y == BOARD_CENTER){
                    nextChar = '.';
                } else {
                    nextChar = 'o';
                }

                gameBoard[x,y] = nextChar;
            }
        }
    }

    public void PrintBoard(char[,] board){

        for(int x = 0; x < BOARD_SIZE + 1; x++){
            if(x > 0){
                Console.Write(x + "|");
            }

            for(int y = 0; y < BOARD_SIZE; y++){
                if(x == 0){
                    if(y == 0){
                        Console.Write("  ");
                    }
                    Console.Write(y + 1 + " ");
                    continue;
                }

                Console.Write(board[x - 1,y] + " ");
            }
            Console.WriteLine("");
        }
    }

    public void PrintBoard(){
        PrintBoard(gameBoard);
    }

    #region Making Moves

    public void MakeMove(int pegX, int pegY, int endX, int endY){
        if(!CheckMoveIsValid(pegX, pegY, endX, endY)){
            Console.WriteLine("MOVE INVALID");
            return;
        }

        //Add the previous board to the history
        AddToHistory(gameBoard);

        //Remove peg at start point and mid points and place a peg at the end point
        int midX = (pegX + endX) / 2;
        int midY = (pegY + endY) / 2;

        SetPeg(pegX, pegY, GAP);
        SetPeg(midX, midY, GAP);
        SetPeg(endX, endY, PEG);

        //Console.WriteLine("MADE THE MOVE");


    }

    private bool CheckMoveIsValid(int pegX, int pegY, int endX, int endY){
        //Check that the points are on the grid
        if(!IsPointOnGrid(pegX, pegY))return false;
        if(!IsPointOnGrid(endX, endY))return false;

        //Check there is a peg on our point
        if(GetPoint(pegX, pegY) != PEG) return false;

        //Check there is no peg on the end point
        if(GetPoint(endX, endY) != GAP) return false;

        //Check the start point is exactly 2 spots away from the move point
        if(GetDistance(pegX, pegY, endX, endY) != 2) return false;
        if(!ArePointsInLine(pegX, pegY, endX, endY)) return false;

        //Check that there is a peg between the 2 spots
        int midX = (pegX + endX) / 2;
        int midY = (pegY + endY) / 2;
        
        if(GetPoint(midX, midY) != PEG) return false;

        return true;
    }

    #endregion

    private bool IsPointOnGrid(int x, int y){
        if(x < 0 || x >= BOARD_SIZE || y < 0 || y >= BOARD_SIZE)return false;
        return true;
    }

    private bool ArePointsInLine(int startX, int startY, int endX, int endY){
        return (startX == endX || startY == endY);
    }

    private int GetDistance(int startX, int startY, int endX, int endY){
        return Math.Abs(startX - endX) + Math.Abs(startY - endY);
    }

    private char GetPoint(int x, int y){
        return gameBoard[x, y];
    }

    private void SetPeg(int x, int y, char state){
        gameBoard[x, y] = state;
    }

    public bool CheckWin(){
        bool singlePeg = false;
        bool inCenter = false;

        for(int x = 0; x < BOARD_SIZE; x++){
            for(int y = 0; y < BOARD_SIZE; y++){
                if(GetPoint(x, y) == PEG){
                    if(singlePeg) return false;
                    singlePeg = true;
                    if(x == 3 && y == 3){
                        inCenter = true;
                    }
                }
            }
        }
        
        return inCenter;
    }

    public bool CheckLoss(){
        for(int x = 0; x < BOARD_SIZE; x++){
            for(int y = 0; y < BOARD_SIZE; y++){
                //Check all directions
                if(GetPoint(x, y) != PEG) continue;

                //Get the 4 directions it could go
                if(CheckMoveIsValid(x, y, x-2, y))return false;
                if(CheckMoveIsValid(x, y, x, y-2))return false;
                if(CheckMoveIsValid(x, y, x+2, y))return false;
                if(CheckMoveIsValid(x, y, x, y+2))return false;
            }
        }

        //There are no valid moves
        return true;
    }

    public List<PegMove> GetAllMoves(){
        List<PegMove> allMoves = new List<PegMove>();

        for(int x = 0; x < BOARD_SIZE; x++){
            for(int y = 0; y < BOARD_SIZE; y++){
                //Check all directions
                if(GetPoint(x, y) != PEG) continue;

                //Get the 4 directions it could go
                if(CheckMoveIsValid(x, y, x-2, y))allMoves.Add(new PegMove(x, y, x-2, y));
                if(CheckMoveIsValid(x, y, x, y-2))allMoves.Add(new PegMove(x, y, x, y-2));
                if(CheckMoveIsValid(x, y, x+2, y))allMoves.Add(new PegMove(x, y, x+2, y));
                if(CheckMoveIsValid(x, y, x, y+2))allMoves.Add(new PegMove(x, y, x, y+2));
            }
        }

        return allMoves;
    }

    #region History

    private List<char[,]> history = new List<char[,]>();

    private void AddToHistory(char[,] historyBoard){
        char[,] copyBoard = new char[BOARD_SIZE, BOARD_SIZE];
        copyBoard = (char[,])historyBoard.Clone();

        history.Add(copyBoard);
    }

    public bool StepBackHistory(){
        char[,] previousBoard = PopHistory();
        if(previousBoard.Length == 0)return false;

        gameBoard = previousBoard;
        return true;
    }

    private char[,] PopHistory(){
        if(history.Count == 0){
            return new char[0,0];
        }
        char[,] lastBoardState = history.Last();
        history.Remove(lastBoardState);
        return lastBoardState;
    }

    #endregion
}