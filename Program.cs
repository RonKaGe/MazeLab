using System.Security.Cryptography.X509Certificates;

namespace GenerationMaze
{


    public class Maze
    {
        private int plotnost;
        private int ItemsAmount;
        private string brick = "\u2588";
        private string[,] MazeArr = new string[15, 15];
        private int EntrySide;
        public void DataAsk()
        {
            Console.WriteLine("Введите плотность лабиринта (1(плотно) или 2(не очень плотно)): ");
            if (int.TryParse(Console.ReadLine(), out plotnost) && plotnost == 1 || plotnost == 2) { }
            else { plotnost = 1; Console.WriteLine("Ошибка ввода, плотность взята 1"); }
            Console.WriteLine("Введите количество предметов, которые нужно собрать(до 10): ");
            if (int.TryParse(Console.ReadLine(), out ItemsAmount) && ItemsAmount >= 1 && ItemsAmount <= 10) { }
            else { Console.WriteLine("Ошибка ввода, количество предметов взято 5"); ItemsAmount = 5; }
            Console.Clear();
        }
        public void DrawMaze()
        {
            Console.WriteLine("\t\t\t Генерируем лабиринт....\n\n\n");

            for (int i = 0; i < MazeArr.GetLength(0); i++)
            {
                for (int j = 0; j < MazeArr.GetLength(1); j++)
                {
                    string c = MazeArr[i, j];

                    if (c == brick)
                        Console.Write(brick + brick);
                    else if (c == "A")
                    {
                        Console.Write(" A");
                    }
                    else if (c == "E")
                    {
                        Console.Write(" E");
                    }
                    else
                        Console.Write("  ");
                }
                Console.WriteLine();
            }

        }
        private void MakeEntrance()
        {
            var rnd = new Random();
            int side = rnd.Next(4);

            switch (side)
            {
                case 0:
                    EntrySide = 0;
                    MazeArr[0, rnd.Next(1, MazeArr.GetLength(0) - 1)] = "A";
                    break;

                case 1:
                    EntrySide = 1;
                    MazeArr[rnd.Next(1, MazeArr.GetLength(0) - 1), MazeArr.GetLength(0) - 1] = "A";
                    break;
                case 2:
                    EntrySide = 2;
                    MazeArr[MazeArr.GetLength(1) - 1, rnd.Next(1, MazeArr.GetLength(0) - 1)] = "A";
                    break;
                case 3:
                    EntrySide = 3;
                    MazeArr[rnd.Next(1, MazeArr.GetLength(1) - 1), 0] = "A";
                    break;


            }
        }
        private void MakeExit()
        {
            var rnd = new Random();
            int side = rnd.Next(4);
            while (side == EntrySide)
            {
                side = rnd.Next(4);
            }
            switch (side)
            {
                case 0:
                    MazeArr[0, rnd.Next(1, MazeArr.GetLength(0) - 1)] = "E";
                    break;
                case 1:
                    MazeArr[rnd.Next(1, MazeArr.GetLength(0) - 1), MazeArr.GetLength(0) - 1] = "E";
                    break;
                case 2:
                    MazeArr[MazeArr.GetLength(1) - 1, rnd.Next(1, MazeArr.GetLength(0) - 1)] = "E";
                    break;
                case 3:
                    MazeArr[rnd.Next(1, MazeArr.GetLength(1) - 1), 0] = "E";
                    break;


            }
        }


        public void GenerateMaze()
        {
            var rnd = new Random();

            for (int i = 0; i < MazeArr.GetLength(0); i++)
            {

                for (int j = 0; j < MazeArr.GetLength(1); j++)
                {
                    MazeArr[i, j] = brick;

                }
            }

            MakeEntrance();
            MakeExit();
            for (int i = 2; i < MazeArr.GetLength(0) - 2; i++)
            {
                for (int j = 2; j < MazeArr.GetLength(1) - 2; j++)
                {
                    int side = rnd.Next(4);



                }

            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Maze maze = new Maze();


            maze.GenerateMaze();
            maze.DrawMaze();


        }
    }
}



