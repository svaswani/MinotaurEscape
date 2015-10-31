using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalTool
{
    public interface InvertableCommand
    {

        void Undo();

        void Execute();

    }
}
