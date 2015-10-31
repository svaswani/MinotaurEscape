using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalTool
{
    class ResizeCommand : InvertableCommand
    {

        // Action for resizing the maze
        private Action<int, int> resizeMaze;

        // maze size before and after
        private int newWidth, newHeight, oldWidth, oldHeight;

        // Creates a rize tile command for the given tile to change to the given value using the given method
        public ResizeCommand(int newWidth, int newHeight, int oldWidth, int oldHeight, Action<int, int> resizeMaze)
        {
            // store all the given values
                this.resizeMaze = resizeMaze;
                this.newWidth = newWidth;
                this.newHeight = newHeight;
                this.oldWidth = oldWidth;
                this.oldHeight = oldHeight;
        }

        public void Execute()
        {
            resizeMaze(newWidth, newHeight);
        }

        public void Undo()
        {
            resizeMaze(oldWidth, oldWidth);
        }

    }
}
