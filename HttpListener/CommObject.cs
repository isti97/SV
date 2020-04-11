using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualizationTool
{
    public class CommObject
    {
        public CommObject(CommandBase command)
        {
            Command = command;
        }

        public CommandBase Command
        {
            get;
            private set;
        }
    }

    public class CommandBase
    {
    }

    public class AccesCommand : CommandBase
    {
        public string CustomerID
        {
            get;
            set;
        }
    }

    public class UpdateCommand : CommandBase
    {
        public string CustomerID
        {
            get;
            set;
        }

        public double NewMoney
        {
            get;
            set;
        }
    }
}