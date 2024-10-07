using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Zork
{
    enum Commands
    {
        QUIT,
        LOOK,
        NORTH,
        SOUTH,
        EAST,
        WEST,
        UNKNOWN
    }

    enum Fields
    {
        Name = 0,
        Description
    }

    enum CommandLineArguments
    {
        RoomsFileName = 0
    }

    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void IsTrue(bool expression, string message = null)
        {
            if(expression == false)
            {
                throw new Exception(message);
            }
        }
    }

    internal class Program
    {
        private static readonly Dictionary<string, Room> roomMap;

        private static Room CurrentRoom
        {
            get
            {
                return Rooms[Location.Row, Location.Column];
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");

            //string roomsFileName = "Rooms.txt";
            
            const string defaultRoomsFileName = "Rooms.txt";
            string roomsFileName = (args.Length > 0 ? args[(int)
                CommandLineArguments.RoomsFileName] : defaultRoomsFileName);
            InitializeRooms(roomsFileName);

            Room previousRoom = null;
            Commands command = Commands.UNKNOWN;
            while (command != Commands.QUIT)
            {
                Console.WriteLine(CurrentRoom);

                if (previousRoom != CurrentRoom)
                {
                    Console.WriteLine(CurrentRoom.Description);
                    previousRoom = CurrentRoom;
                }

                Console.Write("> ");
                command = ToCommand(Console.ReadLine().Trim());

                
                switch (command)
                {
                    case Commands.QUIT:
                        Console.WriteLine("Thank you for playing!");
                        break;

                    case Commands.LOOK:
                        Console.WriteLine(CurrentRoom.Description);
                        break;

                    case Commands.NORTH:
                    case Commands.SOUTH:
                    case Commands.EAST:
                    case Commands.WEST:
                        if (Move(command) == false)
                        {
                            Console.WriteLine("The way is shut.");
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }



        private static void InitializeRooms(string roomsFileName) =>
            Rooms = JsonConvert.DeserializeObject<Room[,]>(File.ReadAllText(roomsFileName));
        

        private static bool Move(Commands command)
        {
            Assert.IsTrue(IsDirection(command), "Invalid direction.");

            bool isValidMove = true;
            switch (command)
            {
                case Commands.NORTH when Location.Row < Rooms.GetLength(0) - 1:
                    Location.Row++;
                    break;

                case Commands.SOUTH when Location.Row > 0:
                    Location.Row--;
                    break;

                case Commands.EAST when Location.Column < Rooms.GetLength(1) - 1:
                    Location.Column++;
                    break;

                case Commands.WEST when Location.Column > 0:
                    Location.Column--;
                    break;

                default:
                    isValidMove = false;
                    break;
            }
            return isValidMove;
        }

        private static Commands ToCommand(string commandString) => (Enum.TryParse(commandString, true, out Commands result) ? result : Commands.UNKNOWN);

        private static bool IsDirection(Commands command) => Directions.Contains(command);

        private static Room[,] Rooms;

        private static readonly List<Commands> Directions = new List<Commands>
        {
            Commands.NORTH,
            Commands.SOUTH,
            Commands.EAST,
            Commands.WEST
        };

        private static (int Row, int Column) Location = (1, 1);
    }
}