using System.Collections.Generic;
using Configs;
using Core.Persistence;

namespace Core.Board
{
    public static class BoardModelFactory
    {
        public static BoardModel CreateFromSave(int width, int height, IReadOnlyList<BlockSaveData> blocks)
        {
            var boardModel = new BoardModel(width, height);
            foreach (var block in blocks)
            {
                var data = new BlockData(block.type);
                boardModel.Set(block.x, block.y, data);
            }
            
            return boardModel;
        }

        public static BoardModel CreateFromConfig(LevelConfig config)
        {
            var boardModel = new BoardModel(config.Width, config.Height);
            foreach (var block in config.Blocks)
            {
                var data = new BlockData(block.Type);
                boardModel.Set(block.Position.x, block.Position.y, data);
            }
            
            return boardModel;
        }
    }
}