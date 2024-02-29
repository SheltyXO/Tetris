using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class BlockQueue
    {   // show the viable block types
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new OBlock(),
            new ZBlock(),
            new TBlock(),
            new SBlock(),
            new LBlock()

        };
        
        private readonly Random random = new Random();

        public Block NextBlock {  get; private set; }

        public BlockQueue()
        {
            NextBlock = RandomBlock();
        }

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }
        // gets the next block in queue and ensures that the next block is different from the current one
        public Block GetAndUpdate()
        {
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);
            
            return block;
        }
    }
}
