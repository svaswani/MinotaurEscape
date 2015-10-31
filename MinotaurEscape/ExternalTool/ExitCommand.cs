using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ExternalTool
{
    class ExitCommand : InvertableCommand
    {
        // Action for changing exit
        private Action<int, int> exitChange;

        // Exit's old and new position
        private int oldX, oldY, newX, newY;

        // Creates an exit command from the given value to the given value using the given method
        public ExitCommand(int oldX, int oldY, int newX, int newY, Action<int, int> exitChange)
        {
            // store given
                this.oldY = oldY;
                this.oldX = oldX;
                this.newX = newX;
                this.newY = newY;
                this.exitChange = exitChange;
        }

        public void Execute()
        {
            exitChange(newX, newY);
        }

        public void Undo()
        {
            exitChange(oldX, oldY);
        }
    }
}
