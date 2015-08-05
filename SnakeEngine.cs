using System;
using System.Collections.Generic;
using System.Linq;
using SnakeWars.SampleBot.Properties;

namespace SnakeWars.SampleBot
{
    internal class SnakeEngine
    {
        private const int DANGER_DIST = 3;
        private readonly string _mySnakeId;
        private readonly Random _random = new Random();
        private readonly Queue<PointDTO> _lastPoints = new Queue<PointDTO>(); 

        public SnakeEngine(string mySnakeId)
        {
            _mySnakeId = mySnakeId;
            for (int i = 0; i < DANGER_DIST*2; i++)
            {
                _lastPoints.Enqueue(new PointDTO());
            }
        }

        public Move GetNextMove(GameBoardState gameBoardState)
        {
            
            //===========================
            // Your snake logic goes here
            //===========================
            var i = gameBoardState.GetSnake("F");
            //i.Head.X
            Move myMove = Move.Straight;
            var nearestFood = gameBoardState.gameState.Food.OrderBy(f => Dist(gameBoardState.gameState.BoardSize, f, i.Head)).FirstOrDefault();

            if (gameBoardState.gameState.Food.Any())
            {

                var start = i.Head;
                var end = nearestFood;

                if (i.Direction == SnakeDirection.Right)
                {
                    if (end.Y > start.Y)
                        myMove = Move.Left;
                    else if (end.Y < start.Y)
                        myMove = Move.Right;
                }
                else if (i.Direction == SnakeDirection.Left)
                {
                    if (end.Y > start.Y)
                        myMove = Move.Right;
                    else if (end.Y < start.Y)
                        myMove = Move.Left;
                }
                else if (i.Direction == SnakeDirection.Up)
                {
                    if (end.X > start.X)
                        myMove = Move.Right;
                    else if (end.X < start.X)
                        myMove = Move.Left;
                }
                else if (i.Direction == SnakeDirection.Down)
                {
                    if (end.X > start.X)
                        myMove = Move.Left;
                    else if (end.X < start.X)
                        myMove = Move.Right;
                }
            }


            var mySnake = gameBoardState.GetSnake(_mySnakeId);
            if (mySnake.IsAlive)
            {
                var occupiedCells = gameBoardState.GetOccupiedCells();
                var otherSnakes = occupiedCells
                    .Where(c => !c.Equals(i.Head))
                    .Except(_lastPoints)
                    .Except(gameBoardState.gameState.Walls)
                    .Except(gameBoardState.gameState.Snakes.Where(s => s.IsAlive).Select(s => s.Head))
                    .ToList();

                // Check possible moves in random order.
                var possibleMoves = new []
                {
                    Move.Straight,
                    Move.Right,
                    Move.Left,
                };

                var orderedMoves = possibleMoves.OrderBy(m =>
                {
                    var possibleNewPosition = gameBoardState.GetSnakeNewHeadPosition(_mySnakeId, m);

                    return gameBoardState.gameState.Snakes.Sum(s => Dist(gameBoardState.gameState.BoardSize, s.Head, possibleNewPosition));
                }).ToList();

                //if (!gameBoardState.gameState.Snakes.Any(s => s.Id != _mySnakeId && Dist(gameBoardState.gameState.BoardSize, i.Head, s.Head) < DANGER_DIST))
                //{
                //    orderedMoves.Add(myMove);
                //}

                if (!otherSnakes.Any(s => Dist(gameBoardState.gameState.BoardSize, i.Head, s) < DANGER_DIST))
                {
                    orderedMoves.Add(myMove);
                }

                var moves = new Stack<Move>(orderedMoves);

                while (moves.Any())
                {
                    // Select random move.
                    var move = moves.Pop();

                    var newHead = gameBoardState.GetSnakeNewHeadPosition(_mySnakeId, move);
                    if (!occupiedCells.Contains(newHead))
                    {
                        _lastPoints.Enqueue(newHead);
                        _lastPoints.Dequeue();
                        return move;
                    }
                }
            }
            return Move.None;
        }

        private static int Dist(SizeDTO boardSize, PointDTO f, PointDTO i)
        {

            var dx = Math.Abs(f.X - i.X);
            if (dx > boardSize.Width / 2)
                dx = boardSize.Width - dx;


            var dy = Math.Abs(f.Y - i.Y);
            if (dy > boardSize.Height / 2)
                dy = boardSize.Height - dy;

            return dx + dy;
        }
    }
}