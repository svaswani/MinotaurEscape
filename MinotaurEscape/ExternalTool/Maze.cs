using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/*namespace ExternalTool
{
    public class Maze
    {
        // The bool of the tiles
        private bool[][] tiles;
        public bool[][] Tiles { get { return tiles; } set { tiles = value; } }

        // The types of walls
        public enum WallTypes { FFFF, FFFT, FFTF, FFTT, FTFF, FTFT, FTTF, FTTT, TFFF, TFFT, TFTF, TFTT, TTFF, TTFT, TTTF, TTTT };


        // The image of the maze
        private WriteableBitmap mazeImage;
        public WriteableBitmap MazeImage { get { return mazeImage; } }

        // The images of the tiles
        private static byte[] floorImage, exitImage, fillSelectedImage;
        private static Dictionary<WallTypes, byte[]> wallImages;

        // The attributes of the images
        private static double dpiX, dpiY;
        private static PixelFormat format;
        private static int stride;
        public int TileSize { get; }
        public int Width { get { return (int)mazeImage.PixelWidth/TileSize; } }
        public int Height { get { return (int)mazeImage.PixelHeight/TileSize; } }

        // The position of the exit and fill select (And if the fill select is visible
        private Point exit, fillPoint;
        private bool fill;
        public Point ExitPoint { get { return exit; }
            set
            {
                exit = value;
                mazeImage.WritePixels(new Int32Rect(0, 0, TileSize, TileSize), exitImage, stride, exit.X * TileSize, exit.Y * TileSize);
            }
        }
        public Point FillPoint { get { return fillPoint; }
            set
            {
                fillPoint = value;
                if(fill)
                    mazeImage.WritePixels(new Int32Rect(0, 0, TileSize, TileSize), fillSelectedImage, stride, fillPoint.X * TileSize, fillPoint.Y * TileSize);
            }
        } 
        public bool Fill { get { return fill; }
            set
            {
                fill = value;
                updateTile(fillPoint);
            }
        }

        // Creates an empty maze of the given size
        public Maze(int width, int height, int tileSize)
        {
            TileSize = tileSize;
            mazeImage = new WriteableBitmap(width * TileSize+TileSize, height * TileSize + TileSize, dpiX, dpiY, format, null);
            tiles = new bool[width][];
            for (int x = 0; x < width; x++)
                tiles[x] = new bool[height];
        }

        // Setup the images for mazes
        public static void setupImages()
        {
            // Load the tile's attributes from the floor image
                BitmapImage image = new BitmapImage(new Uri("Assets/floor.png", UriKind.Relative));
                stride = image.PixelWidth * (image.Format.BitsPerPixel / 8);
                dpiX = image.DpiX;
                dpiY = image.DpiY;
                format = image.Format;



            // Load the floor image's pixels
                floorImage = new byte[stride * image.PixelHeight];
                image.CopyPixels(floorImage, stride, 0);

            // Load the select image's pixels
                image = new BitmapImage(new Uri("Assets/selected.png", UriKind.Relative));
                fillSelectedImage = new byte[stride * image.PixelHeight];
                image.CopyPixels(fillSelectedImage, stride, 0);

            // Load the exit image's pixels
                image = new BitmapImage(new Uri("Assets/exit.png", UriKind.Relative));
                exitImage = new byte[stride * image.PixelHeight];
                image.CopyPixels(exitImage, stride, 0);

            // Set up the dictonary for holding the wall images
                wallImages = new Dictionary<WallTypes, byte[]>();

            // Load the wall images' pixels
                foreach(WallTypes wallType in Enum.GetValues(typeof(WallTypes)))
                {
                    image = new BitmapImage(new Uri("Assets/wall-"+wallType+".png", UriKind.Relative));
                    byte[] wallImage = new byte[stride * image.PixelHeight];
                    image.CopyPixels(wallImage, stride, 0);
                    wallImages.Add(wallType, wallImage);
                }

        }

        // Sets the given tile to a floor (true) or wall (false)
        public void setTile(Point tile, bool floor)
        {
            // Set the tile in the tile array
                tiles[tile.X][tile.Y] = floor;

            // Update this tile and any of the tiles next to it that are walls
                updateTile(tile);
                if (tile.X + 1 < Width && !tiles[tile.X + 1][tile.Y])
                    updateTile(tile.offset(1, 0));
                if (tile.X - 1 >= 0 && !tiles[tile.X - 1][tile.Y])
                    updateTile(tile.offset(-1, 0));
                if (tile.Y + 1 < Height && !tiles[tile.X][tile.Y + 1])
                    updateTile(tile.offset(0, 1));
                if (tile.Y - 1 >= 0 && !tiles[tile.X][tile.Y - 1])
                    updateTile(tile.offset(0, -1));

            
        }

        // Updates all the tiles in the maze
        public void updateAll()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    updateTile(new Point(x, y));
        }

        // Updates the given tile based on the tile array
        public void updateTile(Point tile)
        {
            if (tiles[tile.X][tile.Y])
                mazeImage.WritePixels(new Int32Rect(0, 0, TileSize, TileSize), floorImage, stride, tile.X * TileSize, tile.Y * TileSize);
            else
                mazeImage.WritePixels(new Int32Rect(0, 0, TileSize, TileSize), wallImages[getWallType(tile)], stride, tile.X* TileSize, tile.Y* TileSize);
            if (exit == tile)
                mazeImage.WritePixels(new Int32Rect(0, 0, TileSize, TileSize), exitImage, stride, exit.X * TileSize, exit.Y * TileSize);
            if(fill && fillPoint==tile)
                mazeImage.WritePixels(new Int32Rect(0, 0, TileSize, TileSize), fillSelectedImage, stride, fillPoint.X * TileSize, fillPoint.Y * TileSize);
        }

        // Gets the wall type that should be in the given position
        private WallTypes getWallType(Point tile)
        {
            return (WallTypes)Enum.Parse(typeof(WallTypes), (isWall(tile.offset(-1, 0)) ? "T" : "F") + (isWall(tile.offset(0, -1)) ? "T" : "F") + (isWall(tile.offset(0, 1)) ? "T" : "F") + (isWall(tile.offset(1, 0)) ? "T" : "F"));
        }
        // Checks if the given tile is a wall
        private bool isWall(Point tile)
        {
            return tile.X >= 0 && tile.X < Width && tile.Y >= 0 && tile.Y < Height && !tiles[tile.X][tile.Y];
        }

        // Gets the tile at the real position
        public Point getTileAt(Point position)
        {
            return new Point(position.X / TileSize, position.Y / TileSize);
        }

        // Gets the value of the tile at the real position
        public bool getTileValueAt(Point position)
        {
            Point tile = getTileAt(position);
            return tiles[tile.X][tile.Y];
        }
    }
}
*/