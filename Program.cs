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

        private int columns = 15;
        private int rows = 15;

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

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    string c = MazeArr[i, j];

                    if (c == brick)
                        Console.Write(brick + brick);
                    else if (c == "A")
                        Console.Write(" A");
                    else if (c == "E")
                        Console.Write(" E");
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
                    MazeArr[0, rnd.Next(1, rows - 1)] = "A";
                    break;
                case 1:
                    EntrySide = 1;
                    MazeArr[rnd.Next(1, rows - 1), rows - 1] = "A";
                    break;
                case 2:
                    EntrySide = 2;
                    MazeArr[rows - 1, rnd.Next(1, rows - 1)] = "A";
                    break;
                case 3:
                    EntrySide = 3;
                    MazeArr[rnd.Next(1, rows - 1), 0] = "A";
                    break;
            }
        }
        private void PutBrick(int r, int c)
        {
        
            if (r >= 0 && r < rows && c >= 0 && c < columns)
                MazeArr[r, c] = brick;
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
                    MazeArr[0, rnd.Next(1, rows - 1)] = "E";
                    break;
                case 1:
                    MazeArr[rnd.Next(1, rows - 1), rows - 1] = "E";
                    break;
                case 2:
                    MazeArr[rows - 1, rnd.Next(1, rows - 1)] = "E";
                    break;
                case 3:
                    MazeArr[rnd.Next(1, rows - 1), 0] = "E";
                    break;
            }
        }

        public void GenerateMaze()
        {
            var rnd = new Random();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (i == 0 || i == rows - 1 || j == 0 || j == columns - 1)
                    {
                        MazeArr[i, j] = brick;
                    }
                }
            }

            MakeEntrance();
            MakeExit();

            if (plotnost == 1)
            {
                for (int i = 2; i < rows - 2; i+=2)
                {
                    for (int j = 2; j < columns - 2; j+=2)
                    {
                        int side = rnd.Next(4);

                        switch (side)
                        {
                            case 0:
                                for(int k = 0; k < 2; k++)
                                {
                                   PutBrick(i-k,j);
                                }
                                break;
                             case 1:
                                for (int k = 0; k < 2; k++)
                                {
                                    PutBrick(i, j+k);
                                }
                                break;
                            case 2:
                                for (int k = 0; k < 2; k++)
                                {
                                    PutBrick(i + k, j);
                                }
                                break;
                            case 3:
                                for (int k = 0; k < 2; k++)
                                {
                                    PutBrick(i, j-k);
                                }
                                break;

                        }
                    }
                }
            }
            else
            {
                for (int i = 2; i < rows - 2; i += 3)
                {
                    for (int j =2; j < columns - 2; j += 3)
                    {
                        int side = rnd.Next(4);

                        switch (side)
                        {
                            case 0:
                                for (int k = 0; k < 6; k++)
                                {
                                    if (MazeArr[i - k, j] == brick)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        PutBrick(i - k, j);
                                    }
                                }
                                break;
                            case 1:
                                for (int k = 0; k < 6; k++)
                                {
                                    if (MazeArr[i, j+k] == brick)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        MazeArr[i, j+k] = brick;
                                    }   
                                }
                                break;
                            case 2:
                                for (int k = 0; k < 6; k++)
                                {
                                    if (MazeArr[i + k, j] == brick)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        PutBrick(i + k, j);
                                    }
                                }
                                break;
                            case 3:
                                for (int k = 0; k < 6; k++)
                                {
                                    if (MazeArr[i, j-k] == brick)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        PutBrick(i, j - k);
                                    }
                                }
                                break;

                        }
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Maze maze = new Maze();
            maze.DataAsk();
            maze.GenerateMaze();
            maze.DrawMaze();
        }
    }
}
