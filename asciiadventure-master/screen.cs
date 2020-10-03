
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace asciiadventure {
    public class Screen {
        private Random scrnRand = new Random();
        private GameObject[,] grid;

        public int NumRows {
            get;
            private set;
        }
        public int NumCols {
            get;
            private set;
        }

        public Screen(int numRows, int numCols){
            NumRows = numRows;
            NumCols = numCols;
            this.grid = new GameObject[NumRows, NumCols];
        }

        private Boolean IsLegalMove(int row, int col, int deltaRow, int deltaCol) {
            int newRow = row + deltaRow;
            int newCol = col + deltaCol;
            if (!IsInBounds(newRow, newCol)){
                return false;
            }
            GameObject other = this[newRow, newCol];
            if (other == null){
                return true;
            }
            return other.IsPassable() || other is Player;
        }

        public Tuple<int, int> GetLegalRandPlacement(Screen screen){
            int x, y;
            do{
                y = scrnRand.Next(1, screen.NumRows);
                x = scrnRand.Next(1, screen.NumCols);
            }
            while(screen.IsOtherObject(y, x));
            return new Tuple<int,int>(y, x);
        }
        public List<Tuple<int, int>> GetLegalMoves(int row, int col, int speed) {
            List<Tuple<int, int>> moves = new List<Tuple<int, int>>();
            if (IsLegalMove(row, col, -speed, 0)) {
                moves.Add(Tuple.Create(-speed, 0));
            }
            if (IsLegalMove(row, col, speed, 0)){
                moves.Add(Tuple.Create(speed, 0));
            }
            if (IsLegalMove(row, col, 0, -speed)){
                moves.Add(Tuple.Create(0, -speed));
            }
            if (IsLegalMove(row, col, 0, speed)) {
                moves.Add(Tuple.Create(0, speed));
            }
            return moves;
        }

        public GameObject this[int row, int col]
        {
            get { 
                UseRowAndCol(row, col);
                return grid[row, col];
            }
            set {
                UseRowAndCol(row, col);
                grid[row, col] = value;
            }
        }
        public string MoveManyRand(List<MovingGameObject> objects, string message, ref Boolean gameOver){
            foreach(MovingGameObject obj in objects){
                // TODO: Make mobs smarter, so they jump on the player, if it's possible to do so
                List<Tuple<int, int>> moves = this.GetLegalMoves(obj.Row, obj.Col, obj.Speed);
                if (moves.Count == 0){
                    continue;
                }
                // mobs move randomly
                // TBLUS:: For some reason I couldnt get tuple destructuring to work in 
                // vscode
                Tuple<int, int> t = moves[scrnRand.Next(moves.Count)];
                int deltaRow = t.Item1;
                int deltaCol = t.Item2;
        
                // var (deltaRow, deltaCol) = moves[random.Next(moves.Count)];
                if (obj is Mob){
                    if (this[obj.Row + deltaRow, obj.Col + deltaCol] is Player){
                        // the mob got the player!
                        obj.Token = "*";
                        message += "A MOB GOT YOU! GAME OVER\n";
                        gameOver = true;
                    }
                    obj.Move(deltaRow, deltaCol);
                }
                if (obj is MovingWall){
                    if (this[obj.Row + deltaRow, obj.Col + deltaCol] is Player){
                        // the mob got the player!
                        obj.Token = "0";
                        message += "YOU WERE CRUSHED BY A WALL\n";
                        gameOver = true;
                    }
                    obj.Move(deltaRow, deltaCol);
                }
            }
            return message;
        }
        protected Boolean CheckRow(int row){
            return row >= 0 && row < NumRows;
        }

        protected Boolean CheckCol(int col){
            return col >= 0 && col < NumCols;
        }

        internal Boolean IsInBounds(int row, int col){
            // TODO: Check for obstacles
            return CheckRow(row) && CheckCol(col);
        }

        protected void UseRowAndCol(int row, int col){
            if (!CheckRow(row)){
                throw new ArgumentOutOfRangeException("row", $"{row} is out of range");
            }
            if (!CheckCol(col)){
                throw new ArgumentOutOfRangeException("col", $"{col} is out of range");
            }
        }

        public Boolean IsOtherObject(int row, int col){
            return grid[row, col] != null;
        }

        public override String ToString() {
            // create walls if needed
            StringBuilder result = new StringBuilder();
            result.Append("+");
            result.Append(String.Concat(Enumerable.Repeat("-", NumCols)));
            result.Append("+\n");
            for (int r=0; r < NumRows; r++){
                result.Append('|');
                for (int c=0; c < NumCols; c++){
                    GameObject gameObject = this[r, c];
                    if (gameObject == null){
                        result.Append(' ');
                    } else {
                        result.Append(gameObject.Token);
                    }
                }
                //Console.WriteLine($"newline for {r}");
                result.Append("|\n");
            }
            result.Append('+');
            result.Append(String.Concat(Enumerable.Repeat("-", NumRows)));
            result.Append('+');
            return result.ToString();
        }
    }
}