using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static ExternalTool.MazeGeneration;

namespace ExternalTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The images of the tiles
        private BitmapImage floorImage, selectImage;
        private Dictionary<string, BitmapImage> wallImages;

        // The size of the maze
        private int mazeWidth = 81, mazeHeight = 61;

        // The image the maze is in
        private Grid mazeGrid;

        // All the tiles in the maze (true == floor, false == wall)
        private bool[][] tiles;

        // Size of the tiles
        private int tileSize = 16;

        // If the current maze is saved
        private bool saved = true;

        // The current file open if any
        private string curFile;

        // Mode for selecting area to fill
        private bool fillMode = false;
        private Image fillSelectedImage;

        // Used for undo and redo
        private Stack<InvertableCommand[]> undoCommands = new Stack<InvertableCommand[]>(),
            redoCommands = new Stack<InvertableCommand[]>();
        private Stack<InvertableCommand> currentCommands = new Stack<InvertableCommand>();

        // The Exit's Image
        private Image exitImage;
        private bool exitMode;

        // The Entrance's Image
        private Image entranceImage;
        private bool entranceMode;

        // The worker for background tasks, a timer, and loading screen
        private BackgroundWorker worker;
        private DateTime lastTime = DateTime.Now;
        private Loading loading;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Called when the window is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Setup the keybindings and images
                setupKeybindings();
                setupImages();

            // Perform the setup on another thread since it will take a while
                runTaskWithLoadingScreen("Seting up a basic grid", "Loading Start...", () =>
                    {
                        DateTime now = DateTime.Now;
                        // Setup the maze
                            setupMaze();
                            generateMaze(MazeGeneration.Algorithm.Blank);
                        Console.WriteLine(DateTime.Now - now);
                    });
        }

        // Setup the images for this window
        private void setupImages()
        {
            floorImage = new BitmapImage(new Uri("Assets/floor.png", UriKind.Relative));
            selectImage = new BitmapImage(new Uri("Assets/selected.png", UriKind.Relative));
            wallImages = new Dictionary<string, BitmapImage>();
            wallImages.Add("FFFF", new BitmapImage(new Uri("Assets/wall-FFFF.png", UriKind.Relative)));
            wallImages.Add("FFFT", new BitmapImage(new Uri("Assets/wall-FFFT.png", UriKind.Relative)));
            wallImages.Add("FFTF", new BitmapImage(new Uri("Assets/wall-FFTF.png", UriKind.Relative)));
            wallImages.Add("FFTT", new BitmapImage(new Uri("Assets/wall-FFTT.png", UriKind.Relative)));
            wallImages.Add("FTFF", new BitmapImage(new Uri("Assets/wall-FTFF.png", UriKind.Relative)));
            wallImages.Add("FTFT", new BitmapImage(new Uri("Assets/wall-FTFT.png", UriKind.Relative)));
            wallImages.Add("FTTF", new BitmapImage(new Uri("Assets/wall-FTTF.png", UriKind.Relative)));
            wallImages.Add("FTTT", new BitmapImage(new Uri("Assets/wall-FTTT.png", UriKind.Relative)));
            wallImages.Add("TFFF", new BitmapImage(new Uri("Assets/wall-TFFF.png", UriKind.Relative)));
            wallImages.Add("TFFT", new BitmapImage(new Uri("Assets/wall-TFFT.png", UriKind.Relative)));
            wallImages.Add("TFTF", new BitmapImage(new Uri("Assets/wall-TFTF.png", UriKind.Relative)));
            wallImages.Add("TFTT", new BitmapImage(new Uri("Assets/wall-TFTT.png", UriKind.Relative)));
            wallImages.Add("TTFF", new BitmapImage(new Uri("Assets/wall-TTFF.png", UriKind.Relative)));
            wallImages.Add("TTFT", new BitmapImage(new Uri("Assets/wall-TTFT.png", UriKind.Relative)));
            wallImages.Add("TTTF", new BitmapImage(new Uri("Assets/wall-TTTF.png", UriKind.Relative)));
            wallImages.Add("TTTT", new BitmapImage(new Uri("Assets/wall-TTTT.png", UriKind.Relative)));
        }

        // Setup the commands for keyboard shortcuts and buttons
        private void setupKeybindings()
        {
            RoutedCommand command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(command, saveCommand));

            command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Shift | ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(command, saveAsCommand));

            command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(command, settingsCommand));

            command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(command, generateCommand));

            command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(command, helpCommand));

            command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(command, loadCommand));

            command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(command, undoCommand));

            command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
            command.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift));
            CommandBindings.Add(new CommandBinding(command, redoCommand));

        }

        // Sets the maze as "saved"
        private void setSaved(bool saved)
        {
            // Update the title's * to indicate the current save state
            if(saved!=this.saved)
                Dispatcher.Invoke(() =>
                {
                    if (this.saved && !saved)
                        Title += "*";
                    else if (!this.saved && saved)
                        Title = Title.Substring(0, Title.Length - 1);
                });

            // Update the saved variable
                this.saved = saved;
        }

        // Sets up the maze with all walls
        private void setupMaze()
        {
            // Turn fill mode off just incase
                fillMode = false;

            // Set the tile's size to the maze's size
                tiles = new bool[mazeWidth][];

                Dispatcher.Invoke(() =>
                {
                    // Setup the new grid and replace the old one in the scrollViewer
                    mazeGrid = new Grid();
                        mazeGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        mazeGrid.VerticalAlignment = VerticalAlignment.Top;
                        scrollViewer.Content = mazeGrid;

                    // Setup the grid's sizes to the current maze sizes
                        mazeGrid.Height = mazeHeight * tileSize;
                        mazeGrid.Width = mazeWidth * tileSize;
                });

            // Setup the grid and set all the tiles in both the grid and array to walls
                for (int x = 0; x < mazeWidth; x++)
                {
                    // Set up the current col
                        tiles[x] = new bool[mazeHeight];
                        Dispatcher.Invoke(() =>
                        {
                            ColumnDefinition col = new ColumnDefinition();
                            col.Width = new GridLength(tileSize, GridUnitType.Pixel);
                            mazeGrid.ColumnDefinitions.Add(col);
                        });

                    for (int y = 0; y < mazeHeight; y++)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            // Setup the current row
                                RowDefinition row = new RowDefinition();
                                row.Height = new GridLength(tileSize, GridUnitType.Pixel);
                                mazeGrid.RowDefinitions.Add(row);

                            // Create the image of the tile and set its mouse event to the tile clicked event
                                Image tileImage = new Image();
                                tileImage.Width = tileSize;
                                tileImage.Height = tileSize;
                                tileImage.MouseDown += tilePressed;
                                tileImage.MouseEnter += tilePressed;
                                MouseUp += tileReleased;

                            // Add the image to the grid
                                Grid.SetRow(tileImage, y);
                                Grid.SetColumn(tileImage, x);
                                mazeGrid.Children.Add(tileImage);
                        });

                        // Update loading screen and Make sure updates aren't right next to each other
                            worker.ReportProgress(50);
                            if ((y+x*mazeHeight) % 50 == 0)
                                Thread.Sleep(1);
                    }

                }

            // Setup the exit and entrance images
                Dispatcher.Invoke(() =>
                {
                    if (entranceImage == null)
                    {
                        entranceImage = new Image();
                        entranceImage.Source = new BitmapImage(new Uri("Assets/entrance.png", UriKind.Relative));
                    }
                    if (entranceImage.Parent != null)
                        ((Grid)entranceImage.Parent).Children.Remove(entranceImage);
                    mazeGrid.Children.Add(entranceImage);
                    entranceImage.Width = tileSize;
                    entranceImage.Height = tileSize;

                    if (exitImage == null)
                    {
                        exitImage = new Image();
                        exitImage.Source = new BitmapImage(new Uri("Assets/exit.png", UriKind.Relative));
                    }
                    if (exitImage.Parent != null)
                        ((Grid)exitImage.Parent).Children.Remove(exitImage);
                    mazeGrid.Children.Add(exitImage);
                    exitImage.Width = tileSize;
                    exitImage.Height = tileSize;
                });

        }

        // Generates the maze procedurally
        private void generateMaze(MazeGeneration.Algorithm algorithm)
        {
            // Generate a maze
                Point exit, entrance;
                tiles = MazeGeneration.generateMaze(algorithm, mazeWidth, mazeHeight, out entrance, out exit);

            // Set the exit and entrance image
                Dispatcher.Invoke(() =>
                {
                    Grid.SetColumn(entranceImage, (int)entrance.X);
                    Grid.SetRow(entranceImage, (int)entrance.Y);
                    Grid.SetColumn(exitImage, (int)exit.X);
                    Grid.SetRow(exitImage, (int)exit.Y);
                });

            // Update the maze grid
                updateGrid();
        }

        // Updates the maze's grid to match the tile array
        private void updateGrid()
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    // Update the current tile and the loading screen
                        Dispatcher.Invoke(() => updateTile(x, y));
                        worker.ReportProgress(50);

                    // Make sure updates aren't right next to each other
                        if ((y + x * mazeHeight) % 50 == 0)
                            Thread.Sleep(1);
                }
            }
        }

        // Sets the tile at the given position to the given type
        private void setTile(int x, int y, bool floor)
        {

            // Make sure it's a vaild tile to set
                if (x % 2 == y % 2 || x==0 || y==0 || x==mazeWidth-1 || y==mazeHeight)
                    return;

            // Since a tile is being changed set saved to false
                setSaved(false);

            // Set the tile in the tile array
                tiles[x][y] = floor;

            // Update this tile and any of the tiles next to it that are walls
                updateTile(x, y);
                if (x + 1 < mazeWidth && !tiles[x + 1][y])
                    updateTile(x + 1, y);
                if (x - 1 >= 0 && !tiles[x - 1][y])
                    updateTile(x - 1, y);
                if (y + 1 < mazeHeight && !tiles[x][y + 1])
                    updateTile(x, y + 1);
                if (y - 1 >= 0 && !tiles[x][y - 1])
                    updateTile(x, y - 1);
        }

        //  Updates the image of the given tile
        private void updateTile(int x, int y)
        {
            // Get the tile the tile
                Image tileImage = mazeGrid.Children.Cast<Image>().First(image => Grid.GetRow(image) == y && Grid.GetColumn(image) == x);

            // Set the tile's image
                if (tiles[x][y])
                    tileImage.Source = floorImage;
                else
                    tileImage.Source = getWallImage(x, y);
        }

        // Gets the wall type that should be in the given position
        private BitmapImage getWallImage(int x, int y)
        {
            return wallImages[(isWall(x - 1, y) ? "T" : "F") + (isWall(x, y - 1) ? "T" : "F") + (isWall(x, y + 1) ? "T" : "F") + (isWall(x + 1, y) ? "T" : "F")];
        }

        // Checks if the given tile is a wall
        private bool isWall(int x, int y)
        {
            return x >= 0 && x < mazeWidth && y >= 0 && y < mazeHeight && !tiles[x][y];
        }

        // Saves the game to a user choosen file
        private bool saveToFile()
        {
            // Create a stream for saving the file
                Stream saveStream;

            // Create a dialog for asking for save location
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "maze files (*.maz)|*.maz";
                dialog.DefaultExt = ".maz";
                dialog.AddExtension = true;
                if (curFile != null)
                {
                    dialog.FileName = curFile.Substring(dialog.FileName.LastIndexOf('\\') + 1);
                    dialog.InitialDirectory = curFile.Substring(0, dialog.FileName.LastIndexOf('\\') + 1);
                }else
                    dialog.RestoreDirectory = true;

            // Ask the user for a place to save at
                if (dialog.ShowDialog(this).Value && (saveStream = dialog.OpenFile()) != null)
                {

                    // Save the maze to the selected stream
                        saveToStream(saveStream);

                    // Set saved to true since it is saved now and set the current file to the save
                        setSaved(true);
                        curFile = dialog.FileName;

                    // Return true since the file was saved
                    return true;
                }
                else
                    return false;
        }

        // Saves the maze to the given stream
        private void saveToStream(Stream saveStream)
        {
            // Save the tiles array with the maze's size
                BinaryWriter writer = new BinaryWriter(saveStream);
                writer.Write(mazeWidth);
                writer.Write(mazeHeight);
                writer.Write((int)Grid.GetColumn(entranceImage));
                writer.Write((int)Grid.GetRow(entranceImage));
                writer.Write((int)Grid.GetColumn(exitImage));
                writer.Write((int)Grid.GetRow(exitImage));
                foreach (bool[] col in tiles)
                    foreach (bool tile in col)
                        writer.Write(tile);
                writer.Close();
        }

        // Called when user hits the save button
        private void saveCommand(object sender, RoutedEventArgs e)
        {
            clearMode();

            if (curFile != null)
            {
                saveToStream(File.Open(curFile, FileMode.Create));
                setSaved(true);
            }
            else
                saveAsCommand(sender, e);
        }

        // Called when user hits the save as button
        private void saveAsCommand(object sender, RoutedEventArgs e)
        {
            clearMode();
            saveToFile();
        }

        // Called when user hits the load button
        private void loadCommand(object sender, RoutedEventArgs e)
        {
            clearMode();

            // Check the current maze is not saved and if not prompt for confirmation
                if (!saved)
                {
                    MessageBoxResult save = MessageBox.Show(this, "Warning! The current maze has not been saved! Would you like to save your changes before loading a new maze? (If you don't, any unsaved progress will be lost when you load a new maze!)", "Unsaved Progress", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (save == MessageBoxResult.Yes)
                    {
                        if (!saveToFile())
                            return;
                    }
                    else if (save != MessageBoxResult.No)
                        return;
                }

            // Create a stream for loading the file
                Stream loadStream;

            // Create a dialog for asking for save location
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "maze files (*.maz)|*.maz";
                if (curFile != null)
                {
                    dialog.FileName = curFile.Substring(dialog.FileName.LastIndexOf('\\') + 1);
                    dialog.InitialDirectory = curFile.Substring(0, dialog.FileName.LastIndexOf('\\') + 1);
                }
                else
                    dialog.RestoreDirectory = true;

            // Ask the user for a file to load
                if (dialog.ShowDialog(this).Value && (loadStream = dialog.OpenFile()) != null)
                {
                    // load the maze's size
                        BinaryReader reader = new BinaryReader(loadStream);
                        mazeWidth = reader.ReadInt32();
                        mazeHeight = reader.ReadInt32();

                    // setup the maze first to fix the grid's size
                        runTaskWithLoadingScreen("Loading the maze", "Loading Maze...", () =>
                        {
                            setupMaze();

                            // load the exit and entrance
                                Dispatcher.Invoke(() =>
                                {
                                    Grid.SetColumn(entranceImage, reader.ReadInt32());
                                    Grid.SetRow(entranceImage, reader.ReadInt32());
                                    Grid.SetColumn(exitImage, reader.ReadInt32());
                                    Grid.SetRow(exitImage, reader.ReadInt32());
                                });

                            // load the tile array
                                for (int x = 0; x < mazeWidth; x++)
                                    for (int y = 0; y < mazeHeight; y++)
                                        tiles[x][y] = reader.ReadBoolean();
                                reader.Close();

                            // Update the grid
                                updateGrid();

                            // Set saved to true since it is just loaded and set the current file to the load
                                setSaved(true);
                                curFile = dialog.FileName;
                        });
                }
        }

        // Called when user hits the generate button
        private void generateCommand(object sender, RoutedEventArgs e)
        {
            
            clearMode();

            // Check the current maze is not saved and if not prompt for confirmation
            if (!saved)
                {
                    MessageBoxResult save = MessageBox.Show(this, "Warning! The current maze has not been saved! Would you like to save your changes before generating a new maze? (If you don't, any unsaved progress will be lost when you generate a new maze!)", "Unsaved Progress", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (save == MessageBoxResult.Yes)
                    {
                        if (!saveToFile())
                            return;
                    }
                    else if (save != MessageBoxResult.No)
                        return;
                }

            // Prompt for size of the maze to generate
                GenerateMaze generate = new GenerateMaze();
                if (generate.ShowDialog(this).Value)
                {
                    // Make sure valid height and width were inputed
                        if (generate.MazeHeight <= 0 || generate.MaxWidth <= 0)
                        {
                            MessageBox.Show(this, "Invalid Width or Height! Both most be positive integers!", "Invaild Dimensions", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                    // Set the current file to nothing
                        curFile = null;

                    // Store the height and width
                        mazeWidth = generate.MazeWidth;
                        mazeHeight = generate.MazeHeight;

                    // Perform the rest on another thread since it will take a while
                        Algorithm algo = generate.algorithm;
                        runTaskWithLoadingScreen("Generating new maze", "Generating Maze...", () =>
                        {
                            DateTime now = DateTime.Now;
                            // Setup the empty maze
                            setupMaze();

                                // Generate a maze
                                    generateMaze(algo);

                                // Set as not saved since new maze created
                                    setSaved(false);
                            Console.WriteLine(DateTime.Now - now);
                        });
                }
        }

        // Called when the user clicks the settings button
        private void settingsCommand(object sender, RoutedEventArgs e)
        {

            clearMode();

            // Display Settings using the generate menu for changing
                Settings settings = new Settings();
                if (settings.ShowDialog(this, mazeWidth, mazeHeight, tileSize).Value)
                {
                    // Make sure valid height and width were inputed
                        if (settings.MazeHeight <= 0 || settings.MazeWidth <= 0 || settings.TileSize <= 0)
                        {
                            MessageBox.Show(this, "Invalid Width, Height, or Tile Size! All most be positive integers! Width and Height must be Odd as well!", "Invaild Dimensions", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (settings.TileSize < 6 && settings.TileSize % 2 == 0)
                        {
                            MessageBox.Show(this, "Invalid Tile Size! If Tile Size is even it must be 6 or greater! Width and Height must be Odd as well!", "Invaild Dimensions", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                    // Check if height or width are less than before and display a warning
                        if (mazeHeight > settings.MazeHeight || mazeWidth > settings.MazeWidth)
                            if (MessageBox.Show(this, "Warning! Changing the maze's size to a smaller value will get remove part of the maze! There is no way to get that part of the maze back! Are you sure you want to reduce the size of the maze?", "Smaller Size", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                                return;

                    // Perform the rest on another thread since it will take a while
                        int newTileSize = settings.TileSize, newWidth = settings.MazeWidth, newHeight = settings.MazeHeight;
                        runTaskWithLoadingScreen("Resizing Maze", "Resizing Maze...", () =>
                        {
                            // Update the tile size and then resize the maze with the new size (adding the command to the stack)
                                tileSize = newTileSize;
                                InvertableCommand resize = new ResizeCommand(newWidth, newHeight, mazeWidth, mazeHeight, resizeMaze);
                                currentCommands.Push(resize);
                                storeCurrentCommands();
                                resize.Execute();
                        });
                }
        }

        // Resizes the current maze
        private void resizeMaze(int newWidth, int newHeight)
        {
            // Set as not saved if width or height were changed
                if(mazeWidth!=newWidth || mazeHeight!=newHeight)
                    setSaved(false);

            // Store the new height, width, and tile size inputed
                mazeWidth = newWidth;
                mazeHeight = newHeight;

            // Create a copy of the tile array to be recalled after the grid is resized
                bool[][] tempTiles = new bool[tiles.Length][];
                for (int x = 0; x < tiles.Length; x++)
                {
                    tempTiles[x] = new bool[tiles[x].Length];
                    Array.Copy(tiles[x], tempTiles[x], tiles[x].Length);
                }

            // Setup the grid with the new values
                setupMaze();

            // Copy the tile array back into the new one (cutting out any extra) and update the grid to the tiles
                for (int x = 0; x < (tiles.Length < tempTiles.Length ? tiles.Length : tempTiles.Length); x++)
                    Array.Copy(tempTiles[x], tiles[x], (tiles[x].Length < tempTiles[x].Length ? tiles[x].Length : tempTiles[x].Length));
                updateGrid();

        }

        // Detects when the user presses on the grid of the maze
        private void tilePressed(object sender, MouseEventArgs e)
        {
            // If Shift click is pressed start fill mode
                if (e is MouseButtonEventArgs && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                {
                    Mouse.OverrideCursor = Cursors.Cross;
                    fillMode = true;
                }

            // If Alt click is pressed set exit using a command
                if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                {
                    if (e is MouseButtonEventArgs)
                    {
                        InvertableCommand command = new EntranceCommand(Grid.GetColumn(entranceImage), Grid.GetRow(entranceImage), Grid.GetColumn((Image)sender), Grid.GetRow((Image)sender), changeEntrance);
                        currentCommands.Push(command);
                        command.Execute();
                    }
                }

            // If Crtl click is pressed set exit using a command
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (e is MouseButtonEventArgs)
                    {
                        InvertableCommand command = new EntranceCommand(Grid.GetColumn(exitImage), Grid.GetRow(exitImage), Grid.GetColumn((Image)sender), Grid.GetRow((Image)sender), changeExit);
                        currentCommands.Push(command);
                        command.Execute();
                    }
                }

            // Check if fill mode or exit mode is on and mouse button was clicked
                else if (fillMode && e is MouseButtonEventArgs)
                    fillTilePressed((Image)sender, (MouseButtonEventArgs)e);
                else if (entranceMode && e is MouseButtonEventArgs)
                {
                    InvertableCommand command = new EntranceCommand(Grid.GetColumn(entranceImage), Grid.GetRow(entranceImage), Grid.GetColumn((Image)sender), Grid.GetRow((Image)sender), changeEntrance);
                    currentCommands.Push(command);
                    command.Execute();
                }
                else if (exitMode && e is MouseButtonEventArgs)
                {
                    InvertableCommand command = new EntranceCommand(Grid.GetColumn(exitImage), Grid.GetRow(exitImage), Grid.GetColumn((Image)sender), Grid.GetRow((Image)sender), changeExit);
                    currentCommands.Push(command);
                    command.Execute();
                }
                else
                {
                    // Change the tile of the currently selected tile
                    if (e.RightButton == MouseButtonState.Pressed && tiles[Grid.GetColumn((Image)sender)][Grid.GetRow((Image)sender)] != true)
                    {
                        InvertableCommand command = new ChangeTileCommand(Grid.GetColumn((Image)sender), Grid.GetRow((Image)sender), true, setTile);
                        currentCommands.Push(command);
                        command.Execute();
                    }
                    else if (e.LeftButton == MouseButtonState.Pressed && tiles[Grid.GetColumn((Image)sender)][Grid.GetRow((Image)sender)] != false)
                    {
                        InvertableCommand command = new ChangeTileCommand(Grid.GetColumn((Image)sender), Grid.GetRow((Image)sender), false, setTile);
                        currentCommands.Push(command);
                        command.Execute();
                    }
                }
        }

        // Clears the mode of the application
        private void clearMode()
        {
            fillMode = false;
            if(fillSelectedImage!=null)
                fillSelectedImage.Visibility = Visibility.Hidden;
            exitMode = false;
            entranceMode = false;
            Mouse.OverrideCursor = null;
        }

        // Detects when the user releases on the grid of the maze
        private void tileReleased(object sender, MouseEventArgs e)
        {
            // Make sure fill mode is off and there are currentCommands and then add the current commands to the undo stack
                if (!fillMode && currentCommands.Count > 0)
                    storeCurrentCommands();
        }

        // Called to change the position of the entrance
        private void changeEntrance(int x, int y)
        {
            if (tiles[x][y])
            {
                Grid.SetColumn(entranceImage, x);
                Grid.SetRow(entranceImage, y);
            }
            clearMode();
        }

        // Called to change the position of the exit
        private void changeExit(int x, int y)
        {
            if(tiles[x][y])
            {
                Grid.SetColumn(exitImage, x);
                Grid.SetRow(exitImage, y);
            }
            clearMode();
        }

        // Called when a tile is left clicked in fill mode
        private void fillTilePressed(Image tile, MouseButtonEventArgs e)
        {
            // If the selected image has not been created yet create it
                if (fillSelectedImage == null)
                {
                    // Draw a box around the current selection
                        fillSelectedImage = new Image();
                        fillSelectedImage.Source = selectImage;
                        Grid.SetColumn(fillSelectedImage, Grid.GetColumn(tile));
                        Grid.SetRow(fillSelectedImage, Grid.GetRow(tile));
                        fillSelectedImage.Width = tileSize;
                        fillSelectedImage.Height = tileSize;
                        mazeGrid.Children.Add(fillSelectedImage);
                        fillSelectedImage.Visibility = Visibility.Hidden;
                }

                // Check which point is being selected
                    if (fillSelectedImage.Visibility == Visibility.Hidden)
                    {
                        // Draw the box around the first selection
                            Grid.SetColumn(fillSelectedImage, Grid.GetColumn(tile));
                            Grid.SetRow(fillSelectedImage, Grid.GetRow(tile));
                            fillSelectedImage.Width = tileSize;
                            fillSelectedImage.Height = tileSize;
                            fillSelectedImage.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        // Get the parameters of the whole selection, hide the old image, and set fill mode off
                            int x1 = Math.Min(Grid.GetColumn(fillSelectedImage), Grid.GetColumn(tile)),
                                y1 = Math.Min(Grid.GetRow(fillSelectedImage), Grid.GetRow(tile)),
                                x2 = Math.Max(Grid.GetColumn(fillSelectedImage), Grid.GetColumn(tile)) + 1,
                                y2 = Math.Max(Grid.GetRow(fillSelectedImage), Grid.GetRow(tile)) + 1;
                            clearMode();

                        // Fill the selection with walls or floors depending on the mouse button clicked
                            for (int x = x1; x < x2; x++)
                            {
                                for (int y = y1; y < y2; y++)
                                {
                                    // Create commands only of the tiles that are going to be changed
                                        if (tiles[x][y] != (e.RightButton == MouseButtonState.Pressed))
                                        {
                                            InvertableCommand command = new ChangeTileCommand(x, y, e.RightButton == MouseButtonState.Pressed, setTile);
                                            currentCommands.Push(command);
                                            command.Execute();
                                        }
                                }
                            }

                        // Add the commands to the undo stack
                            storeCurrentCommands();
                    }
        }

        // starts fill mode
        private void fillCommand(object sender, RoutedEventArgs e)
        {
            clearMode();
            fillMode = true;
            Mouse.OverrideCursor = Cursors.Cross;
        }

        // Called when the window is going to close
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Make sure the worker is not working
                if (worker.IsBusy)
                    e.Cancel = true;
                else
                {
                    // Check if the maze is saved and if not prompt for confirmation
                        if (!saved)
                        {
                            MessageBoxResult save = MessageBox.Show(this, "Warning! The current maze has not been saved! Do you want to save your unsaved changes before closing? (If not, all unsaved changes will be lost on close!)", "Unsaved Progress", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                            if (save == MessageBoxResult.Yes)
                            {
                                if (curFile != null)
                                    saveToStream(File.Open(curFile, FileMode.Create));
                                else if (!saveToFile())
                                    e.Cancel = true;
                            }
                            else if (save != MessageBoxResult.No)
                                e.Cancel = true;
                        }
                }
        }

        // Undo the top command on the undo stack
        private void undoCommand(object sender, RoutedEventArgs e)
        {
            // Make sure there are commands to undo
                if (undoCommands.Count > 0)
                {
                    // Get the commands to undo
                        InvertableCommand[] commands = undoCommands.Pop();

                    // Add the commands to the redo stack and then undo the commands
                        redoCommands.Push(commands);
                        foreach (InvertableCommand command in commands)
                            command.Undo();
                }
        }

        // Redo the top command on the redo stack
        private void redoCommand(object sender, RoutedEventArgs e)
        {
            // Make sure there are commands to redo
                if (redoCommands.Count > 0)
                {
                    // Get the commands to redo
                        InvertableCommand[] commands = redoCommands.Pop();

                    // Add the commands to the undo stack and then redo the commands
                        undoCommands.Push(commands);
                        foreach (InvertableCommand command in commands)
                            command.Execute();
                }
        }

        // Stores the current commands to the undo stack
        private void storeCurrentCommands()
        {
            // Add the current commands to the undo stack and clear both the current commands and the redo commands
                undoCommands.Push(currentCommands.ToArray());
                currentCommands.Clear();
                redoCommands.Clear();
        }

        // Displays the help menu
        private void helpCommand(object sender, RoutedEventArgs e)
        {
            clearMode();
            Help help = new Help();
            help.Show(this);
        }

        // Starts entrance mode to set the entrance tile
        private void entranceCommand(object sender, RoutedEventArgs e)
        {
            clearMode();
            entranceMode = true;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        // Starts exit mode to set the exit tile
        private void exitCommand(object sender, RoutedEventArgs e)
        {
            clearMode();
            exitMode = true;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        // Closes the window
        private void quitCommand(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Runs the given Action on the background worker
        private void runTaskWithLoadingScreen(string loadingMessage, string loadingTitle, Action work)
        {

            // Create variables used in loading (maz load value, background worker, and loading screen)
                int maxLoadValue = mazeWidth * mazeHeight;
                worker = new BackgroundWorker();
                loading = new Loading();

            // Set the background worker to run the given method
                worker.DoWork += (sender, e) => work();

            // Se the background worker to return the cursor and close the loading screen when it's done
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    loading.Close();
                    Mouse.OverrideCursor = null;
                };

            // Set the background worker to increment the loading value and update time remaining on update
                    worker.ProgressChanged += (sender, e) =>
                    {
                        loading.LoadingValue+=e.ProgressPercentage/100.0;
                        if ((int)loading.LoadingValue % 200 == 0)
                        {
                            if ((int)loading.LoadingValue / 200 % 3 == 0)
                                loading.LoadingMessage = loadingMessage + "..";
                            else if ((int)loading.LoadingValue / 200 % 3 == 1)
                                loading.LoadingMessage = loadingMessage + "...";
                            else if ((int)loading.LoadingValue / 200 % 3 == 2)
                                loading.LoadingMessage = loadingMessage + ".";
                        }
                    };
                        worker.WorkerReportsProgress = true;


                        // Show the loading screen, change the cursour, and run the background worker
                        loading.Show(this, loadingMessage+"...", loadingTitle, maxLoadValue);
                        Mouse.OverrideCursor = Cursors.Wait;
                        worker.RunWorkerAsync();

                    }



    }
}