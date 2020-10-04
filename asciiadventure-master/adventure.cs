using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/*
 Cool ass stuff people could implement:
 > jumping
 > attacking
 > randomly moving monsters
 > smarter moving monsters
*/
namespace asciiadventure {
    public class Game {
        // private Random random = new Random();
        private static Boolean Eq(char c1, char c2){
            return c1.ToString().Equals(c2.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static string Menu() {
            return "Get the Treasure\nWASD to move\nIJKL to attack/interact\nWatch out for the traps and moving walls\nReach the pressure plate to stop the moving walls\nEnter command: ";
        }

        private static void PrintScreen(Screen screen, string message, string menu) {
            Console.Clear();
            Console.WriteLine(screen);
            Console.WriteLine($"\n{message}");
            Console.WriteLine($"\n{menu}");
        }

        public void Run() {
            Console.ForegroundColor = ConsoleColor.Green;

            Screen screen = new Screen(10, 10);
            // add a couple of walls
            for (int i=0; i < 3; i++){
                new Wall(1, 2 + i, screen);
            }
            for (int i=0; i < 4; i++){
                new Wall(3 + i, 4, screen);
            }
            
            // add a player
            Player player = new Player(0, 0, screen, "Zelda");
            
            // add a treasure
            Treasure treasure = new Treasure(6, 2, screen);

            // add a trap at a random location
            Tuple<int, int> loc = screen.GetLegalRandPlace(screen);
            Trap trap = new Trap(loc.Item1, loc.Item2, screen);
            

            // add some mobs
            List<MovingGameObject> mobs = new List<MovingGameObject>();
            mobs.Add(new Mob(9, 9, screen));

            //add some special walls
            List<MovingGameObject> movers = new List<MovingGameObject>();
            for (int i = 0; i< 3; i++){
                Tuple<int, int> locMW = screen.GetLegalRandPlace(screen);
                movers.Add(new MovingWall(locMW.Item1, locMW.Item2, screen, i));
            }

            // add the pressure plate
            Tuple<int, int> locPP = screen.GetLegalRandPlace(screen);
            PressurePlate pp = new PressurePlate(locPP.Item1, locPP.Item2, screen);
            
            // initially print the game board
            PrintScreen(screen, "Welcome!", Menu());
            
            Boolean gameOver = false;
            
            while(!gameOver) {
                char input = Console.ReadKey(true).KeyChar;

                String message = "";

                if (Eq(input, 'q')) {
                    break;
                } else if (Eq(input, 'w')) {
                    player.Move(-1, 0);
                } else if (Eq(input, 's')) {
                    player.Move(1, 0);
                } else if (Eq(input, 'a')) {
                    player.Move(0, -1);
                } else if (Eq(input, 'd')) {
                    player.Move(0, 1);
                } else if (Eq(input, 'i')) {
                    message += player.Action(-1, 0, ref gameOver) + "\n";
                } else if (Eq(input, 'k')) {
                    message += player.Action(1, 0, ref gameOver) + "\n";
                } else if (Eq(input, 'j')) {
                    message += player.Action(0, -1, ref gameOver) + "\n";
                } else if (Eq(input, 'l')) {
                    message += player.Action(0, 1, ref gameOver) + "\n";
                } else if (Eq(input, 'v')) {
                    // TODO: handle inventory
                    message = "You have nothing\n";
                } else {
                    message = $"Unknown command: {input}";
                }

                if (screen[trap.Row, trap.Col] is Player){
                    // The trap got the player!
                    player.Token = "*";
                    message += "You have fallen in to a trap and died\nTOO BAD SO SAD...\nGAME OVER\n";
                    gameOver = true;
                }

                else if (screen[pp.Row, pp.Col] is Player){
                    message = pp.Activate();
                }
                else if (screen[treasure.Row, treasure.Col] is Player){ 
                    message += "Oh no! You crushed the delicate Treasure\nYou must pick it up using IJKL\nGAME OVER!!!!!!!!\n";
                    gameOver = true;
                }

                // OK, now move the mobs
                if (pp.IsMoving){
                    message += screen.MoveManyRand(movers, ref gameOver);
                }
                message += screen.MoveManyRand(mobs, ref gameOver);
                // foreach (Mob mob in mobs){
                //     // TODO: Make mobs smarter, so they jump on the player, if it's possible to do so
                //     List<Tuple<int, int>> moves = screen.GetLegalMoves(mob.Row, mob.Col, mob.Speed);
                //     if (moves.Count == 0){
                //         continue;
                //     }
                //     // mobs move randomly
                //     // TBLUS:: For some reason I couldnt get tuple destructuring to work in 
                //     // vscode
                //     Tuple<int, int> t = moves[random.Next(moves.Count)];
                //     int deltaRow = t.Item1;
                //     int deltaCol = t.Item2;
            
                //     // var (deltaRow, deltaCol) = moves[random.Next(moves.Count)];
                    
                //     if (screen[mob.Row + deltaRow, mob.Col + deltaCol] is Player){
                //         // the mob got the player!
                //         mob.Token = "*";
                //         message += "A MOB GOT YOU! GAME OVER\n";
                //         gameOver = true;
                //     }
                //     mob.Move(deltaRow, deltaCol);
                // }
                PrintScreen(screen, message, Menu());
            }
        }

        public static void Main(string[] args){
            Game game = new Game();
            game.Run();
        }
    }
}