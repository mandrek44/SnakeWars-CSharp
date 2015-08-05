using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeWars.SampleBot
{
    internal class GameBoardState
    {
        public readonly GameStateDTO gameState;

        public GameBoardState(GameStateDTO gameState)
        {
            this.gameState = gameState;
        }

        public PointDTO GetSnakeNewHeadPosition(string snakeId, Move move)
        {
            var snake = GetSnake(snakeId);
            var newHead = move.GetSnakeNewHead(snake, gameState.BoardSize);
            return newHead;
        }
        
        public HashSet<PointDTO> GetOccupiedCells()
        {
            return new HashSet<PointDTO>(gameState.Walls.Concat(gameState.Snakes.SelectMany(snake => snake.Cells)));
        }

        public SnakeDTO GetSnake(string snakeId)
        {
            return gameState.Snakes.First(s => s.Id == snakeId);
        }
    }
}