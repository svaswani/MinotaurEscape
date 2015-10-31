using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalTool
{
    class ChangeTileCommand : InvertableCommand
    {
        // Action for changing tiles
        private Action<int, int, bool> changeTile;

        // Tile going to be changed
        private int tileX, tileY;
        private bool floor;

        // Creates a change tile command for the given tile to change to the given value using the given method
        public ChangeTileCommand(int tileX, int tileY, bool floor, Action<int, int, bool> changeTile)
        {
            // store all the given values
                this.changeTile = changeTile;
                this.tileX = tileX;
                this.tileY = tileY;
                this.floor = floor;
        }

        public void Execute()
        {
            changeTile(tileX, tileY, floor);
        }

        public void Undo()
        {
            changeTile(tileX, tileY, !floor);
        }
    }
}
