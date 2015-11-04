using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ExternalTool
{
    class EntranceCommand : InvertableCommand
    {
        // Action for changing exit
        private Action<int, int> entranceChange;

        // Exit's old and new position
        private int oldX, oldY, newX, newY;

        // Creates an entrance command from the given value to the given value using the given method
        public EntranceCommand(int oldX, int oldY, int newX, int newY, Action<int, int> entranceChange)
        {
            // store given
                this.oldY = oldY;
                this.oldX = oldX;
                this.newX = newX;
                this.newY = newY;
                this.entranceChange = entranceChange;
        }

        public void Execute()
        {
            entranceChange(newX, newY);
        }

        public void Undo()
        {
            entranceChange(oldX, oldY);
        }
    }
}
