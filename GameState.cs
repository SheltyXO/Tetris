using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;
        
        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();
                // moves the block down to check if it fits within the grid
                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }
        // shows the game grid where the blocks are placed
        public GameGrid GameGrid { get; }
        // show the queue of upcoming blocks
        public BlockQueue BlockQueue { get; }
        // tells if the game is over
        public bool GameOver { get; private set; }
        // shows the players score
        public int Score { get; private set; }
        // show the block that is currently being hold
        public Block HeldBlock { get; private set; }
        // indicates whether a block can be hold or not
        public bool CanHold { get; private set; }
        // constructor to initialize the game state
        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
        }
        // checks if the current block fits within the game grid
        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if(!GameGrid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }
            return true;
        }
        // holds the block
        public void HoldBlock()
        {
            if (!CanHold)
            {
                return;
            }

            if(HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;
        }
        // moves the block clockwise and handles collision
        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }
        // moves the block counterclockwise and handles collision
        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }
        // moves the current block to the left and handles the collision for it
        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }
        // moves the current block to the right and handles the collision for it
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }
        // checks the 2 top rows to know if the game is over
        private bool isGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        // place the current block onto the game grid and handles scoring
        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            Score += GameGrid.ClearFullRows();

            if (isGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CanHold = true;
            }
        }
        // moves the current block down and if its not able to go further, its going to be placed
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        // calculates the distance a tile can drop
        private int TileDropDistance(Position p)
        {
            int drop = 0;
            while(GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        } 
        // calculates the minimum drop distance for the entire block
        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }
            return drop;
        }
        // drops the current block to the lowest possible poistion available
        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

    }
}
