using System;
using System.Collections.Generic;
using Enums;

namespace Core.Persistence
{
    [Serializable]
    public struct BlockSaveData
    {
        public int x;
        public int y;
        public BlockType type;
        
        public BlockSaveData(int x, int y, BlockType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
    
    [Serializable]
    public class BoardSaveData
    {
        public int width;
        public int height;
        public List<BlockSaveData> blocks;

        public BoardSaveData(int width, int height, List<BlockSaveData> blocks)
        {
            this.width = width;
            this.height = height;
            this.blocks = blocks;
        }
    }
}