using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace GenerationMaze
{
    public class Maze
    {
        Random rnd = new Random();
        private int plotnost;
        private int ItemsAmount;
        private string brick = "\u2588";
        public string[,] MazeArr = new string[15, 15];
        private int EntrySide;
        private int ExitSide;
        private int PersRow;
        private int PersCol;

        private int columns = 15;
        private int rows = 15;

        private static int EntryRow, EntryCol; // Позиция входа потому что у нас он
                                        // может быть с 4 сторон, поэтому
                                        // нужно учитывать столбцы и строки в
        private int ExitRow, ExitCol; // Позиция выхода

        private void PlaceItems()
        {

            int predmet = 0;
            int attempts = 0;
            int maxAttempts = rows * columns * 10;

            while (predmet < ItemsAmount && predmet < maxAttempts)
            {
                attempts++;
                int r = rnd.Next(1, rows - 1);
                int c = rnd.Next(1, columns - 1);

                if (MazeArr[r, c] != brick && MazeArr[r, c] != "A" && MazeArr[r, c] != "E" && MazeArr[r, c] != "*")
                {
                    MazeArr[r, c] = "*";
                    predmet++;
                }

            }
            
            if (predmet < ItemsAmount)
            {
                Console.WriteLine("Все предметы разместить невозможно!");
            }
            Console.WriteLine($"Размещено: {predmet}/{ItemsAmount}");
        }

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

        public void DrawMaze(string [,] MazeArr)   //передаем массив чтобы могли вызывать метод для двух лабиринтов 
        {
            Console.WriteLine("\t\t\t Генерируем лабиринт....\n\n\n");

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    string c = MazeArr[i, j];

                    if (c == brick)
                    {
                        Console.Write(brick + brick);
                    }
                    else if (c == "A")
                    {
                        Console.Write(" A");
                    }
                    else if (c == "E")
                    {
                        Console.Write(" E");
                    }
                    else if (c == "*")
                    {
                        Console.Write(" *");
                    }
                    else if (c == "?")
                    {
                        Console.Write(" ?");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
        }

        private void MakeEntrance()
        {
            int side = rnd.Next(4);

            switch (side)
            {
                case 0:
                    EntrySide = 0;
                    EntryCol = rnd.Next(1, columns - 1);
                    EntryRow = 0;
                    MazeArr[0, EntryCol] = "A";
                    break;
                case 1:
                    EntrySide = 1;
                    EntryRow = rnd.Next(1, rows - 1);
                    EntryCol = columns - 1;
                    MazeArr[EntryRow, columns - 1] = "A";
                    break;
                case 2:
                    EntrySide = 2;
                    EntryCol = rnd.Next(1, columns - 1);
                    EntryRow = rows - 1;
                    MazeArr[rows - 1, EntryCol] = "A";
                    break;
                case 3:
                    EntrySide = 3;
                    EntryRow = rnd.Next(1, rows - 1);
                    EntryCol = 0;
                    MazeArr[EntryRow, 0] = "A";
                    break;
            }
                PersRow = EntryRow;
                PersCol = EntryCol;
        }
        private void PutBrick(int r, int c)
        {

            if (r >= 0 && r < rows && c >= 0 && c < columns)
                MazeArr[r, c] = brick;
        }
        private void MakeExit()
        {
            int side = rnd.Next(4);
            while (side == EntrySide)   // изменил параметр на корректный
            {
                side = rnd.Next(4);
            }
            ExitSide = side;


            switch (side)
            {
                case 0:
                    ExitSide = 0;           // переменная для сохранения стороны выхода
                    ExitCol = rnd.Next(1, columns - 1);
                    ExitRow = 0;
                    MazeArr[0, ExitCol] = "E";
                    break;
                case 1:
                    ExitSide = 1;
                    ExitRow = rnd.Next(1, rows - 1);
                    ExitCol = columns - 1;
                    MazeArr[ExitRow, columns - 1] = "E";
                    break;
                case 2:
                    ExitSide = 2;
                    ExitCol = rnd.Next(1, columns - 1);
                    ExitRow = rows - 1;
                    MazeArr[rows - 1, ExitCol] = "E";
                    break;
                case 3:
                    ExitSide = 3;
                    ExitRow = rnd.Next(1, rows - 1);
                    ExitCol = 0;
                    MazeArr[ExitRow, 0] = "E";
                    break;
            }
        }

        private void EnsurePathClear() // отчистка стен после входа и перед выходом,
                                       // чтобы дать пользователю возможность войти и выйти без проблем

        {
            switch (EntrySide)
            {
                case 0:
                    MazeArr[EntryRow + 1, EntryCol] = " ";
                    break;
                case 1:
                    MazeArr[EntryRow, EntryCol - 1] = " ";
                    break;
                case 2:
                    MazeArr[EntryRow - 1, EntryCol] = " ";
                    break;
                case 3:
                    MazeArr[EntryRow, EntryCol + 1] = " ";
                    break;
            }
            switch (ExitSide)
            {
                case 0:
                    MazeArr[ExitRow + 1, ExitCol] = " ";
                    break;
                case 1:
                    MazeArr[ExitRow, ExitCol - 1] = " ";
                    break;
                case 2:
                    MazeArr[ExitRow - 1, ExitCol] = " ";
                    break;
                case 3:
                    MazeArr[ExitRow, ExitCol + 1] = " ";
                    break;
            }
        }

        public void GenerateMaze()
        {

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
                for (int i = 2; i < rows - 2; i += 2)
                {
                    for (int j = 2; j < columns - 2; j += 2)
                    {
                        int side = rnd.Next(4);

                        switch (side)
                        {
                            case 0:
                                for (int k = 0; k < 2; k++)
                                {
                                    PutBrick(i - k, j);
                                }
                                break;
                            case 1:
                                for (int k = 0; k < 2; k++)
                                {
                                    PutBrick(i, j + k);
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
                                    PutBrick(i, j - k);
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
                    for (int j = 2; j < columns - 2; j += 3)
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
                                    if (MazeArr[i, j + k] == brick)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        MazeArr[i, j + k] = brick;
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
                                    if (MazeArr[i, j - k] == brick)
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
            EnsurePathClear();
            PlaceItems();
        }
        
        public string[,] UnknownMaze = new string[15,15]; // второй массив, отображающий неизвестный компьютеру,
                                                          // но уже сгенирированный нами ранее лабиринт
        private void GenerationUnknownMaze()               // генерируем заготовку 
        {
            UnknownMaze[EntryRow, EntryCol] = "A";
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++) 
                {
                    if(UnknownMaze[i, j] != "A")
                    {
                        UnknownMaze[i,j] = "?";
                    }
                }
            }
        }

        private bool Inside(int r, int c) =>
          r >= 0 && r < rows && c >= 0 && c < columns;


        private List<int> visited = new List<int>();
        private bool[,,] tried = new bool[15, 15, 4]; 

        private bool TryStep(int side)
        {
            int nextRow = PersRow;   // куда собираемся
            int nextCol = PersCol;

            switch (side)
            {
                case 0: // вверх
                    if (tried[PersRow,PersCol,0]==true) return false; 
                    nextRow--;
                    break;

                case 1: // вправо
                    if (tried[PersRow, PersCol, 1] == true) return false; 
                    nextCol++;
                    break;

                case 2: // вниз
                    if (tried[PersRow, PersCol, 2] == true) return false; 
                    nextRow++;
                    break;

                case 3: // влево
                    if (tried[PersRow, PersCol, 3] == true) return false; 
                    nextCol--;
                    break;

                default:
                    return false;

            }
            if (!Inside(nextRow, nextCol)) return false;
            UnknownMaze[nextRow, nextCol] = MazeArr[nextRow,nextCol];
            if (MazeArr[nextRow, nextCol] != brick)
            {
                visited.Add(side);
                PersRow = nextRow;
                PersCol = nextCol;
                return true;
            }
            else 
            {
                tried[PersRow, PersCol, side] = true;
                return false; 
            }
        }
       
        public void GoingThrowMaze()
        {                                                                          // метод прохода компьютером лабиринта 
            GenerationUnknownMaze();
            while (true)
            {
                int side = rnd.Next(4);
                TryStep(side);
                DrawMaze(UnknownMaze);
                Console.WriteLine(visited.Count);



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
            maze.DrawMaze(maze.MazeArr);                                    // сделал два массива публичными, чтобы могли в мейне обращаться к двум сразу
            maze.GoingThrowMaze();
            maze.DrawMaze(maze.UnknownMaze);
        }
    }
}