﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Snake
{
	public class GameState
	{
		public int Rows {  get; }
		public int Columns { get; }
		public GridValue[,] Grids { get;}
		public Direction Dir { get;private set; }
		public int Score {  get; private set; }
		public bool GameOver{get; private set; }

		private readonly LinkedList<Direction> dirChanges=new LinkedList<Direction>();
		private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
		private readonly Random random = new Random();

		public GameState(int rows, int columns)
		{
			Rows = rows; Columns = columns;
			Grids = new GridValue[Rows, Columns];
			Dir=Direction.Right;
			AddSnake();
			AddFood();
		}

		private void AddSnake()
		{
			int r = Rows / 2;
			for(int c=1;c<=3;c++)
			{
				Grids[r, c] = GridValue.Snake;
				snakePositions.AddFirst(new Position(r, c));
			}
		}

		private IEnumerable<Position> EmptyPositions()
		{
			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Columns; c++)
				{
					if (Grids[r, c] == GridValue.Empty)
					{
						yield return new Position(r, c);
					}
				}
			}
		}

		private void AddFood()
		{
			List<Position> empty = new List<Position>(EmptyPositions());
			if (empty.Count == 0)
			{
				return;
			}

			Position pos = empty[random.Next(empty.Count)];
			Grids[pos.Row,pos.Column] = GridValue.Food;
		}

		public Position HeadPosition()
		{
			return snakePositions.First.Value;
		}
		public Position TailPosition()
		{
			return snakePositions.Last.Value;
		}

		public IEnumerable<Position> SnakePositions()
		{
			return snakePositions;
		}
		private void AddHead(Position pos)
		{
			snakePositions.AddFirst(pos);
			Grids[pos.Row, pos.Column] = GridValue.Snake;
		}

		private void RemoveTail()
		{
			Position tails = snakePositions.Last.Value;
			Grids[tails.Row, tails.Column] = GridValue.Empty;
			snakePositions.RemoveLast();
		}

		private Direction GetLastDir()
		{
			if (dirChanges.Count == 0)
			{
				return Dir;
			}

			return dirChanges.Last.Value;
		}

		private bool CanChangeDirection(Direction newDir)
		{
			if(dirChanges.Count == 2)
			{
				return false;
			}

			Direction lastDir=GetLastDir();
			return newDir != lastDir && newDir != lastDir.Opposite();
		}

		public void ChangeDirection(Direction direction)
		{
			if (CanChangeDirection(direction))
			{
				dirChanges.AddLast(direction);
			}
					}

		private bool OutsideGrid(Position pos)
		{
			return pos.Row<0||pos.Row>=Rows|| pos.Column < 0 || pos.Column >= Columns;
		}

		private GridValue WillHit(Position newHead)
		{
			if (OutsideGrid(newHead))
			{
				return GridValue.Outside;
			}

			if (newHead == TailPosition())
			{
				return GridValue.Empty;
			}
			return Grids[newHead.Row, newHead.Column];
		}

		public void Move()
		{
			if(dirChanges.Count > 0)
			{
				Dir=dirChanges.First.Value;
				dirChanges.RemoveFirst();
			}
			Position newHead = HeadPosition().Translate(Dir);
			GridValue hit = WillHit(newHead);
			if (hit == GridValue.Outside || hit == GridValue.Snake)
			{
				GameOver = true;
			}
			else if(hit==GridValue.Empty)
			{
				RemoveTail();
				AddHead(newHead);
			}
			else if(hit==GridValue.Food)
			{
				AddHead(newHead);
				Score++;
				AddFood();
			}
		}
	}
}