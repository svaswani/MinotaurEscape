using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExternalTool
{
    public static class MazeGeneration
    {
        // The different Algorithm's that can be used to generate a maze
        public enum Algorithm { Blank, Depth___first_search, Kruskal__algorithm, Eller__algorithm_by_row, Eller__algorithm_by_colmun, Hunt___and___kill_algorithm, Growing_tree_algorithm_75____25, Growing_tree_algorithm_50____50, Growing_tree_algorithm_25____75 }

        // Random used for maze generation
        private static Random rand = new Random();

        public static bool[][] generateMaze(Algorithm algorithm, int width, int height, out Point exit)
        {
            // Create a variable for holding the maze
                bool[][] maze;

            // Get inner height and width
                int innerWidth = width-2, innerHeight = height-2;

            // Call the method for the given algorithm
                switch (algorithm)
                {
                    case Algorithm.Depth___first_search:
                        maze = generateDepthFirstSearch(innerWidth, innerHeight);
                        break;
                    //case Algorithm.Recursive_division:
                    //    maze = generateRecursiveDivision(innerWidth, innerHeight);
                    //    break;
                    case Algorithm.Kruskal__algorithm:
                        maze = generateKruskalAlgorithm(innerWidth, innerHeight);
                        break;
                   // case Algorithm.Prim__algorithm:
                    //    maze = generatePrimAlgorithm(innerWidth, innerHeight);
                   //     break;
                    case Algorithm.Eller__algorithm_by_row:
                        maze = generateElleralgorithm(innerWidth, innerHeight, false);
                        break;
                    case Algorithm.Eller__algorithm_by_colmun:
                        maze = generateElleralgorithm(innerWidth, innerHeight, true);
                        break;
                    case Algorithm.Hunt___and___kill_algorithm:
                        maze = generateHuntAndKillAlgorithm(innerWidth, innerHeight);
                        break;
                    case Algorithm.Growing_tree_algorithm_75____25:
                        maze = generateGrowingTreeAlgorithm(innerWidth, innerHeight, 75, 25);
                        break;
                    case Algorithm.Growing_tree_algorithm_50____50:
                        maze = generateGrowingTreeAlgorithm(innerWidth, innerHeight, 50, 20);
                        break;
                    case Algorithm.Growing_tree_algorithm_25____75:
                        maze = generateGrowingTreeAlgorithm(innerWidth, innerHeight, 25, 75);
                        break;
                    default:
                        maze = setupMazeEmpty(innerWidth, innerHeight);
                        break;
                }

            // Add an outer wall to the maze
                bool[][] newMaze = new bool[width][];
                for(int x = 0; x < width; x++)
                {
                    newMaze[x] = new bool[height];
                    for(int y = 0; y < height; y++)
                    {
                        if (y == 0 || x == 0 || y == height - 1 || x == width - 1)
                            newMaze[x][y] = false;
                        else
                            newMaze[x][y] = maze[x - 1][y - 1];
                    }
                }

            // Set the exit to a random open slot in the maze
                Point[] posExits = newMaze.SelectMany((col, x) => col.Select((tile, y) => tile ? new Point(x, y) : new Point(-1, -1)).Where(cell => !cell.Equals(new Point(-1, -1)))).ToArray();
                exit = posExits[rand.Next(posExits.Length)];

            // Return the newley created maze
                return newMaze;
        }

        public static bool[][] generateDepthFirstSearch(int width, int height)
        {
            // Get an empty maze
                bool[][] maze = setupMazeEmpty(width, height);

            // Get the cell width and height and create stack for holding visited cells
                int cellWidth = width / 2 + 1, cellHeight = height / 2 + 1;
                List<Point> visitedCells = new List<Point>();
                Stack<Point> stack = new Stack<Point>();

            // Select an outer cell as the start and mark it
                Point curCell = selectOuterCell(cellWidth, cellHeight);
                visitedCells.Add(curCell);

            // Loop until all cells have been visited
                while (visitedCells.Count < cellHeight * cellWidth)
                {
                    // Check if any unvisted Neighbors
                        Point[] unvistedNeighbors = getNeighbours(curCell, cellWidth, cellHeight).Where(cell => !visitedCells.Contains(cell)).ToArray();
                        if (unvistedNeighbors.Length > 0)
                        {
                            // Pick a random unvisted neighbor
                                stack.Push(curCell);
                                Point newCell = unvistedNeighbors[rand.Next(unvistedNeighbors.Length)];

                            // Remove the wall, mark the new cell as visited and set it as the current cell
                                removeWallBetween(curCell, newCell, maze);
                                curCell = newCell;
                                visitedCells.Add(curCell);
                        
                        }
                        else // Pick another cell to start from
                            curCell = stack.Pop();
                }

            // Return the newly made maze boxed out
                return maze;
        }

        public static bool[][] generateRecursiveDivision(int width, int height)
        {
            // Create an empty maze
                bool[][] maze = new bool[width][];
                for (int x = 0; x < width; x++)
                {
                    maze[x] = new bool[height];
                    for (int y = 0; y < height; y++)
                        maze[x][y] = true;
                }

            // Split the maze
                split(0, 0, width, height, maze);

            // Get rid of any single walls
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        if (!maze[x][y] && !getNeighbours(new Point(x, y), width, height).Select(tile => maze[(int)tile.X][(int)tile.Y]).Contains(false))
                            maze[x][y] = true;

            // Return the maze
                return maze;
        }

        // Splits the given rectangle in the given maze
        private static void split(int x, int y, int width, int height, bool[][] maze)
        {

            // Make sure spliting is not done
                if(width>1 && height>1)
                {
                    // Check which way to split
                        if(width > height)
                        {
                            
                            // Place a vertical wall randomly in the rectangle
                                int wallX = x + rand.Next(width - 2) + 1;
                                int holeY = rand.Next(height)+y;
                                for (int wallY = y; wallY < y + height; wallY++)
                                    if (wallY != holeY)
                                        maze[wallX][wallY] = false;

                            // Split the two new rectangles
                                split(x, y, wallX - x, height, maze);
                                split(wallX + 1, y, width + x - wallX-1, height, maze);

                        }
                        else
                        {
                            // Place a horizontal wall randomly in the rectangle
                                int wallY = y + rand.Next(height - 2) + 1;
                                int holeX = rand.Next(width) + x;
                                for (int wallX = x; wallX < x + width; wallX++)
                                    if (wallX != holeX)
                                        maze[wallX][wallY] = false;

                            // Split the two new rectangles
                                split(x, y, width, wallY-y, maze);
                                split(x, wallY+1, width, height+y-wallY-1, maze);
                        }
                }

        }

        public static bool[][] generateKruskalAlgorithm(int width, int height)
        {
            // Get an empty maze
                bool[][] maze = setupMazeEmpty(width, height);

            // Create a lists of all the walls in the maze and the cells in the maze (placing each cell in its own set)
                List<Point> walls = new List<Point>();
                List<List<Point>> cellSets = new List<List<Point>>();
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (maze[x][y])
                            cellSets.Add(new Point[] { new Point(x, y) }.ToList());
                        else if ((x % 2 != 1 && y % 2 == 1 && y != 0 && y != height - 1) || (x % 2 == 1 && y % 2 != 1 && x != 0 && x != width - 1))
                            walls.Add(new Point(x, y));
                    }
                }

            // Loop until every wall has been removed from the list
                while(walls.Count > 0)
                {
                    // Pick a random wall from the list
                       Point wall = walls.ElementAt(rand.Next(walls.Count));

                    // Find the sets that contain the cells on each side of the current wall
                        List<Point> curSet1 = null, curSet2 = null;
                        if(wall.X % 2 != 1 && wall.Y % 2 == 1)
                        {
                            curSet1 = cellSets.First(set => set.Contains(new Point(wall.X, wall.Y-1)));
                            curSet2 = cellSets.First(set => set.Contains(new Point(wall.X, wall.Y + 1)));
                        }
                        else if(wall.X % 2 == 1 && wall.Y % 2 != 1)
                        {
                            curSet1 = cellSets.First(set => set.Contains(new Point(wall.X - 1, wall.Y)));
                            curSet2 = cellSets.First(set => set.Contains(new Point(wall.X + 1, wall.Y)));
                        }
                        

                    // If the sets are not the same remove the wall and merge the sets
                        if (curSet1!=curSet2)
                        {
                            maze[(int)wall.X][(int)wall.Y] = true;
                            curSet1.AddRange(curSet2);
                            cellSets.Remove(curSet2);
                        }

                    // Remove the wall from the list since done
                        walls.Remove(wall);
                        
                }

            // Return the newly made maze
                return maze;
        }

        public static bool[][] generatePrimAlgorithm(int width, int height)
        {
            // Get an empty maze
                bool[][] maze = setupMazeEmpty(width, height);

            // Create a list for holding unused walls and a list for holding used cells
                List<Point> walls = new List<Point>();
                List<Point> cells = new List<Point>();

            // Select a random starting cell, mark it as part of the maze, and add it's walls to the wall list
                Point cell = selectOuterCell(width / 2 + 1, height / 2 + 1);
                cells.Add(cell);
                walls.AddRange(getNeighbours(cell, width, height));

            // Loop until there are no longer any unused walls
                while(walls.Count > 0)
                {
                    // Pick a random wall and neighbor and check if it's a cell yet
                        Point wall = walls.ElementAt(rand.Next(walls.Count));
                        Point[] neighbors = getNeighbours(wall, width, height).Where(tile => maze[(int)tile.X][(int)tile.Y]).ToArray();
                        cell = neighbors[rand.Next(neighbors.Length)];
                        if (!cells.Contains(cell))
                        {
                            maze[(int)wall.X][(int)wall.Y] = true;
                            cells.Add(cell);
                            walls.AddRange(getNeighbours(cell, width, height).Where(tile => !maze[(int)tile.X][(int)tile.Y]));
                        }
                        

                    // Remove the wall from the list
                        if(neighbors.Where(tile => !cells.Contains(tile)).Count()==0)
                            walls.Remove(wall);
                }

            // Return the newly made maze
                return maze;
        }


        public static bool[][] generateElleralgorithm(int width, int height, bool dir)
        {

            // Get an empty maze
                bool[][] maze = setupMazeEmpty(width, height);

            // Create a for holding the cells in the maze
                List<List<Point>> cellSets = new List<List<Point>>();

            // Loop through every col or row depeding on the direction given in the maze
                if (dir)
                {
                    for (int x = 0; x < width / 2 + 1; x++)
                    {
                        // Place each cell in this row into a set if it's not in own already
                            for (int y = 0; y < height / 2 + 1; y++)
                                if (cellSets.Where(set => set.Contains(new Point(x, y))).Count() == 0)
                                    cellSets.Add(new Point[] { new Point(x, y) }.ToList());

                        // Randomly join disjointed cells
                            for (int y = 0; y < height / 2; y++)
                            {
                                List<Point> curSet = cellSets.First(set => set.Contains(new Point(x, y)));
                                if (rand.Next(2) == 0 && !curSet.Contains(new Point(x, y + 1)))
                                {
                                    List<Point> oldSet = cellSets.First(set => set.Contains(new Point(x, y + 1)));
                                    curSet.AddRange(oldSet);
                                    cellSets.Remove(oldSet);
                                    removeWallBetween(new Point(x, y), new Point(x, y + 1), maze);
                                }
                            }

                        // If not the last row make connections to the next row
                            if (x != width / 2)
                            {
                                // loop through each set and make at least one connection
                                    foreach (List<Point> set in cellSets)
                                    {
                                        // Create a random number of connections with at least one
                                            List<Point> pos = set.Where(cell => cell.X == x).ToList();
                                            int connections = rand.Next(pos.Count) + 1;
                                            for (int con = 0; con < connections; con++)
                                            {
                                                Point conPoint = pos[rand.Next(pos.Count)];
                                                pos.Remove(conPoint);
                                                removeWallBetween(conPoint, new Point(conPoint.X + 1, conPoint.Y), maze);
                                                set.Add(new Point(conPoint.X + 1, conPoint.Y));
                                            }

                                    }

                            }
                            else
                            {
                                // Connect all disjonted cells
                                    for (int y = 0; y < height / 2; y++)
                                    {
                                        List<Point> curSet = cellSets.First(set => set.Contains(new Point(x, y)));
                                        if (!curSet.Contains(new Point(x, y + 1)))
                                        {
                                            List<Point> oldSet = cellSets.First(set => set.Contains(new Point(x, y + 1)));
                                            curSet.AddRange(oldSet);
                                            cellSets.Remove(oldSet);
                                            removeWallBetween(new Point(x, y), new Point(x, y + 1), maze);
                                        }
                                    }

                            }
                    }
                }
                else
                {
                    for (int y = 0; y < height / 2 + 1; y++)
                    {
                        // Place each cell in this col into a set if it's not in own already
                            for (int x = 0; x < width / 2 + 1; x++)
                                if (cellSets.Where(set => set.Contains(new Point(x, y))).Count() == 0)
                                    cellSets.Add(new Point[] { new Point(x, y) }.ToList());

                        // Randomly join disjointed cells
                            for (int x = 0; x < width / 2; x++)
                            {
                                List<Point> curSet = cellSets.First(set => set.Contains(new Point(x, y)));
                                if (rand.Next(2) == 0 && !curSet.Contains(new Point(x + 1, y)))
                                {
                                    List<Point> oldSet = cellSets.First(set => set.Contains(new Point(x + 1, y)));
                                    curSet.AddRange(oldSet);
                                    cellSets.Remove(oldSet);
                                    removeWallBetween(new Point(x, y), new Point(x + 1, y), maze);
                                }
                            }

                        // If not the last col make connections to the next col
                            if (y != height / 2)
                            {
                                // loop through each set and make at least one connection
                                    foreach (List<Point> set in cellSets)
                                    {
                                        // Create a random number of connections with at least one
                                            List<Point> pos = set.Where(cell => cell.Y == y).ToList();
                                            int connections = rand.Next(pos.Count) + 1;
                                            for (int con = 0; con < connections; con++)
                                            {
                                                Point conPoint = pos[rand.Next(pos.Count)];
                                                pos.Remove(conPoint);
                                                removeWallBetween(conPoint, new Point(conPoint.X, conPoint.Y + 1), maze);
                                                set.Add(new Point(conPoint.X, conPoint.Y+1));
                                            }

                                    }

                        }
                        else
                        {
                            // Connect all disjonted cells
                                for (int x = 0; x < width / 2; x++)
                                {
                                    List<Point> curSet = cellSets.First(set => set.Contains(new Point(x, y)));
                                    if (!curSet.Contains(new Point(x + 1, y)))
                                    {
                                        List<Point> oldSet = cellSets.First(set => set.Contains(new Point(x + 1, y)));
                                        curSet.AddRange(oldSet);
                                        cellSets.Remove(oldSet);
                                        removeWallBetween(new Point(x, y), new Point(x + 1, y), maze);
                                    }
                                }

                        }
                    }
                }

            // Return the newly made maze
                return maze;
        }

        public static bool[][] generateHuntAndKillAlgorithm(int width, int height)
        {
            // Get an empty maze
                bool[][] maze = setupMazeEmpty(width, height);

            // Select a random starting cell
                Point curCell = selectOuterCell(width / 2 + 1, height / 2 + 1);

            // Create a list for visted cells
                List<Point> visted = new List<Point>();

            // Loop until there are no longer any unvisted cells
                do
                {
                    // Mark the current cell as visted
                        visted.Add(curCell);

                    // Loop while there is an unvisted neighbor
                        Point[] unvisitedNeighbours;
                        while((unvisitedNeighbours = getNeighbours(curCell, width / 2 + 1, height / 2 + 1).Where(cell => !visted.Contains(cell)).ToArray()).Length > 0)
                        {
                            // pick a random unvisted neighbor
                                Point neighbour = unvisitedNeighbours[rand.Next(unvisitedNeighbours.Length)];

                            // Remove the wall between the current and the neighbor and set the neighbor as the current (and mark it as visited)
                                removeWallBetween(curCell, neighbour, maze);
                                curCell = neighbour;
                                visted.Add(curCell);
                        }


            } while ((curCell = getUnvistedCell(maze, visted)) != new Point(-1, -1));

            // Return the newly made maze
                return maze;
        }

        // Gets an unvisted cell with a visted neighbor (creating a passsage between the two)
        private static Point getUnvistedCell(bool[][] maze, List<Point> visted)
        {
            // Search throught the maze for a cell that is not visted and has a visted neighbor
                int cellWidth = maze.Length / 2 + 1,
                    cellHeigth = maze[0].Length / 2 + 1;
                for (int x = 0; x < cellWidth; x++)
                {
                    for (int y = 0; y < cellHeigth; y++)
                    {
                        Point curCell = new Point(x, y);
                        if (!visted.Contains(curCell))
                        {
                            Point[] vistedNeighbours = getNeighbours(curCell, cellWidth, cellHeigth).Where(cell => visted.Contains(cell)).ToArray();
                            if (vistedNeighbours.Length > 0)
                            {
                                removeWallBetween(curCell, vistedNeighbours[rand.Next(vistedNeighbours.Length)], maze);
                                return curCell;
                            }
                        }
                    }
                }

            // No unvisted cells left
                return new Point(-1, -1);
        }

        public static bool[][] generateGrowingTreeAlgorithm(int width, int height, int newest, int random)
        {
            // Get an empty maze
                bool[][] maze = setupMazeEmpty(width, height);

            // Get the cell width and height and create list for holding visited cells and list fo current cells
                int cellWidth = width / 2 + 1, cellHeight = height / 2 + 1;
                List<Point> visitedCells = new List<Point>();
                List<Point> currentCells = new List<Point>();

            // Select an outer cell as the start, mark it, and add it to the curren cells
                Point curCell = selectOuterCell(cellWidth, cellHeight);
                currentCells.Add(curCell);
                visitedCells.Add(curCell);

            // Loop until all cells have been visited
                while (currentCells.Count > 0)
                {
                    // Choose the next cell depending on the percentages given
                        if (rand.Next(100) < newest)
                            curCell = currentCells.ElementAt(currentCells.Count-1);
                        else
                            curCell = currentCells.ElementAt(rand.Next(currentCells.Count));

                    // Check if any unvisted Neighbors
                        Point[] unvistedNeighbors = getNeighbours(curCell, cellWidth, cellHeight).Where(cell => !visitedCells.Contains(cell)).ToArray();
                        if (unvistedNeighbors.Length > 0)
                        {
                            // Pick a random unvisted neighbor and add it to the current cells
                                Point newCell = unvistedNeighbors[rand.Next(unvistedNeighbors.Length)];
                                currentCells.Add(newCell);

                            // Get the wall between the cells
                                Point wall = new Point(-1, -1);
                                if (curCell.X == newCell.X && Math.Abs(curCell.Y - newCell.Y) == 1)
                                    wall = new Point(curCell.X * 2, curCell.Y + newCell.Y);
                                else if (curCell.Y == newCell.Y && Math.Abs(curCell.X - newCell.X) == 1)
                                    wall = new Point(curCell.X + newCell.X, curCell.Y * 2);

                            // Remove the wall, mark the new cell as visited
                                maze[(int)wall.X][(int)wall.Y] = true;
                                visitedCells.Add(newCell);

                        }
                        else // Remove the cell since it no longer has unvisted neighbors
                            currentCells.Remove(curCell);
                }

            // Return the newly made maze boxed out
                return maze;
        }

        // Removes the wall between the two given cells
        private static void removeWallBetween(Point cell1, Point cell2, bool[][] maze)
        {
            // Get the wall between the cells
                Point wall = new Point(-1, -1);
                if (cell1.X == cell2.X && Math.Abs(cell1.Y - cell2.Y) == 1)
                    wall = new Point(cell1.X * 2, cell1.Y + cell2.Y);
                else if (cell1.Y == cell2.Y && Math.Abs(cell1.X - cell2.X) == 1)
                    wall = new Point(cell1.X + cell2.X, cell1.Y * 2);

            // Remove the wall
                maze[(int)wall.X][(int)wall.Y] = true;
        }

        // Setups up the maze with empty cells
        private static bool[][] setupMazeEmpty(int width, int height)
        {
            // Create the maze as a bunch of cells
                bool[][] maze = new bool[width][];
                for (int x = 0; x < width; x++)
                {
                    maze[x] = new bool[height];
                    for (int y = 0; y < height; y++)
                        maze[x][y] = x % 2 == 0 && y % 2 == 0;
                        
                }
                return maze;
        }

        // selects a random outer cell
        private static Point selectOuterCell(int cellWidth, int cellHeight)
        {
            if (rand.Next(2) == 0)
            {
                if (rand.Next(2) == 0)
                    return new Point(cellWidth - 1, rand.Next(cellHeight));
                else
                    return new Point(0, rand.Next(cellHeight));
            }
            else
            {
                if (rand.Next(2) == 0)
                    return new Point(rand.Next(cellWidth), cellHeight - 1);
                else
                    return new Point(rand.Next(cellWidth), 0);
            }
        }

        // Gets the neighbor's of the given tile
        private static Point[] getNeighbours(Point tile, int width, int height)
        {

            List<Point> neighbours = new List<Point>();
            if (tile.X - 1 >= 0)
                neighbours.Add(new Point(tile.X-1, tile.Y));
            if (tile.X + 1 < width)
                neighbours.Add(new Point(tile.X + 1, tile.Y));
            if (tile.Y - 1 >= 0)
                neighbours.Add(new Point(tile.X, tile.Y - 1));
            if (tile.Y + 1 < height)
                neighbours.Add(new Point(tile.X, tile.Y + 1));

            return neighbours.ToArray();
        }
    }
}
