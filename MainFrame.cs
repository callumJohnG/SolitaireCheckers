class MainFrame
{

    private static PegSolitaire pegSolitaire = new PegSolitaire();

    static void Main(string[] args)
    {

        Console.WriteLine("Welcome to chinese checkers!");
        Console.WriteLine("Do you want to play? (P)");
        Console.WriteLine("Or run the solver? (S)");
        string? result = Console.ReadLine();
        if(result?.ToLower() == "p"){
            GameLoop();
        } else if (result?.ToLower() == "s"){
            RunSolver();
        }
    }


    private static void GameLoop(){
        int moveCounter = 1;
        while(true){
            Console.Clear();

            Console.WriteLine("======== MOVE " + moveCounter + " ========\n\n");

            pegSolitaire.PrintBoard();


            int pegX, pegY, endX, endY;

            try{
                Console.WriteLine("\nEnter Coords\n");
                Console.WriteLine("(Enter 'B' to go back 1 move");
                Console.WriteLine("From  - To");
                Console.WriteLine("(x,y x,y)\n");
                string? result = Console.ReadLine();
                if(result?.ToLower() == "B"){
                    if(pegSolitaire.StepBackHistory()){
                        moveCounter--;
                    }
                    continue;
                }

                string[]? results = result?.Split(' ');
                pegX = Convert.ToInt32(results?[0].Split(',')[1]) - 1;
                pegY = Convert.ToInt32(results?[0].Split(',')[0]) - 1;
                endX = Convert.ToInt32(results?[1].Split(',')[1]) - 1;
                endY = Convert.ToInt32(results?[1].Split(',')[0]) - 1;
            } catch {
                Console.WriteLine("WRONG");
                continue;    
            }

            
            moveCounter++;

            pegSolitaire.MakeMove(pegX, pegY, endX, endY);
            if(pegSolitaire.CheckWin()){
                Console.WriteLine("Congradulations! You win!");
                break;
            } else if(pegSolitaire.CheckLoss()){
                Console.WriteLine("Sorry! You lost! No more valid moves");
                break;
            }
        }

    }


    private static void RunSolver(){

        PegSolver pegSolver = new PegSolver(pegSolitaire);
        pegSolver.Solve();

    }

}