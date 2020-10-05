using System;

namespace asciiadventure {
    class Player : MovingGameObject {
        public Player(int row, int col, Screen screen, string name) : base(row, col, "@", screen, 1) {
            Name = name;
        }
        public string Name {
            get;
            protected set;
        }
        public override Boolean IsPassable(){
            return true;
        }

        public String Action(int deltaRow, int deltaCol, ref Boolean gameOver, ref Boolean DW){
            int newRow = Row + deltaRow;
            int newCol = Col + deltaCol;
            if (!Screen.IsInBounds(newRow, newCol)){
                return "nope";
            }
            GameObject other = Screen[newRow, newCol];
            if (other == null){
                return "negative";
            }

            // TODO: Interact with the object
            if (other is Treasure){
                other.Delete();
                gameOver = true;
                return "Yay, we got the treasure!";
            }
            if (other is AcidTrigger){
                DW = true;
                return "Run! You Triggered the acid wave!";
            }
            return "ouch";
        }
    }
}