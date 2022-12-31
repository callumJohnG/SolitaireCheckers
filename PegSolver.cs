public class PegSolver{

    private PegSolitaire pegSolitaire;
    private List<SolverNode> tree;

    private long startTime;
    private long endTime;

    public PegSolver(PegSolitaire pegSolitaire){
        this.pegSolitaire = pegSolitaire;
        tree = new List<SolverNode>();
    }

    private List<char[,]> allSolutions = new List<char[,]>();

    public void Solve(bool findAll){
        startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        //Create the root node
        SolverNode? currentNode = CreateNextNode();
        tree.Add(currentNode);
        //Console.WriteLine(currentNode.ToString());


        //Start solving the board
        while(true){

            //Check if our current solver node has any moves to make
            if(currentNode == null)break;
            if(!(currentNode.HasMoves())){
                //Node has no moves, step back in the tree
                currentNode = StepBackInTree();
                if(currentNode == null){
                    if(!findAll){
                        //WE HAVE FAILED TO FIND A SOLUTION
                        Console.WriteLine("Failed to find any solutions");
                        break;
                    } else {
                        //Finished searching the tree
                        DisplayAllResults();
                        break;
                    }
                }
                continue;
            }


            //---Make the next move in the move list
            //Get the next move
            PegMove nextMove = currentNode.GetNextMove();

            //pegSolitaire.PrintBoard();
            //Console.WriteLine(nextMove.ToString());



            //Perform the move
            pegSolitaire.MakeMove(nextMove.startX, nextMove.startY, nextMove.endX, nextMove.endY);

            //Check to see if we have won
            if(pegSolitaire.CheckWin()){
                allSolutions.Add((char[,])pegSolitaire.GetGameBoard().Clone());
                if(!findAll){
                    DisplayWin();
                    return;
                }
                currentNode = StepBackInTree();
                continue;
            }

            //Create the new solver node for this board state
            currentNode = CreateNextNode();
            tree.Add(currentNode);
        }
    }

    private void DisplayWin(){
        endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long timeTaken = endTime - startTime;
        
        List<char[,]> boardHistory = new List<char[,]>();
        int currentIndex = 0;

        while(true){
            boardHistory.Add((char[,])pegSolitaire.GetGameBoard().Clone());
            if(!pegSolitaire.StepBackHistory()){
                break;
            }
        }

        boardHistory.Reverse();


        while(true){
            
            Console.Clear();

            Console.WriteLine("========SOLUTION FOUND===========");
            Console.WriteLine("(Took " + timeTaken + "ms to find solution)");

            char[,] currentBoard = boardHistory[currentIndex];

            Console.WriteLine("---------Move " + currentIndex + "----------");
            pegSolitaire.PrintBoard(currentBoard);

            Console.WriteLine("\n(N)ext, (P)revious, (Q)uit");
            string? result = Console.ReadLine()?.ToLower();
            if(result == "n"){
                currentIndex++;
            } else if(result == "p"){
                currentIndex--;
            } else if(result == "q"){
                break;
            }

            if(currentIndex < 0){
                currentIndex = 0;
            } else if (currentIndex >= boardHistory.Count){
                currentIndex = boardHistory.Count-1;
            }
        }
    }

    private void DisplayAllResults(){
        endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long timeTaken = endTime - startTime;
        Console.WriteLine("======FOUND " + allSolutions.Count + " SOLUTIONS=====");
        Console.WriteLine("(Took " + timeTaken + "ms to find all solutions)");
    }

    private SolverNode CreateNextNode(){
        //Using the current board position - generate a node for it
        SolverNode newNode = new SolverNode(
            pegSolitaire.GetGameBoard(),
            GenerateAllMoves()
        );

        return newNode;
    }

    private SolverNode? StepBackInTree(){
        //Try to take a step back in the tree
        if(tree.Count <= 1){
            //The tree is empty
            return null;
        }

        pegSolitaire.StepBackHistory();
        tree.Remove(tree.Last());
        return tree.Last();
    }

    private List<PegMove> GenerateAllMoves(){
        List<PegMove> allMoves = pegSolitaire.GetAllMoves();
        return allMoves;
    }

}

public class SolverNode{

    private List<PegMove> allMoves;
    private char[,] boardPosition;

    public SolverNode(char[,] boardPosition, List<PegMove> allMoves){
        this.boardPosition = boardPosition;
        this.allMoves = allMoves;
    }

    public bool HasMoves(){
        //Console.WriteLine("All moves left : " + allMoves.Count);
        return allMoves.Count > 0;
    }

    public PegMove GetNextMove(){
        PegMove nextMove = allMoves.Last();
        allMoves.Remove(nextMove);
        return nextMove;
    }

    public override string ToString()
    {
        string output = "";
        foreach(PegMove move in allMoves){
            output += move.startX + "," + move.startY + " " + move.endX + "," + move.endY + "\n";
        }
        return output;
    }

}

public struct PegMove{

    public int startX {get; private set;}
    public int startY {get; private set;}
    public int endX {get; private set;}
    public int endY {get; private set;}

    public PegMove(int startX, int startY, int endX, int endY){
        this.startX = startX;
        this.startY = startY;
        this.endX = endX;
        this.endY = endY;
    }

    public override string ToString()
    {
        return(startX + "," + startY + " " + endX + "," + endY);
    }

}