using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

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
        private int CollectionItems;


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

        public void DrawMaze(string[,] MazeArr)   //передаем массив чтобы могли вызывать метод для двух лабиринтов 
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

        public string[,] UnknownMaze = new string[15, 15]; // второй массив, отображающий неизвестный компьютеру,
                                                           // но уже сгенирированный нами ранее лабиринт
        private void GenerationUnknownMaze()               // генерируем заготовку 
        {
            UnknownMaze[EntryRow, EntryCol] = "A";
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (UnknownMaze[i, j] != "A")
                    {
                        UnknownMaze[i, j] = "?";
                    }
                }
            }
        }

        private bool Inside(int r, int c) =>                //проверка выхода за границы лабиринта(массива)
          r >= 0 && r < rows && c >= 0 && c < columns;


        private bool[,] visited = new bool[15, 15];             // будем хранить тут значения известных проходов
        private int[] R = new int[4] { -1, 0, 1, 0 };          // это массивы, при комбинации которых будет получаться направление,
        private int[] C = new int[4] { 0, 1, 0, -1 };                                                   // что то типо векторов -1 0 наверх; 0 1 вправо и т.д. 

        private void dfc(int r, int c)                  //dfc или поиск в глубину
        {
            if (MazeArr[r, c] == "*" && !visited[r, c])
            { // Если у нас комп наступает на предмет
              // то счётчик собранных предметов обновляется

                CollectionItems++;
                Console.WriteLine($"Собран предмет! Всего: {CollectionItems} ");
            }

            visited[r, c] = true;
            UnknownMaze[r, c] = MazeArr[r, c];


            for (int i = 0; i < 4; i++)
            {
                int nr = r + R[i];                                  // nr = nextrow nc = nextcolumn
                int nc = c + C[i];

                if (!Inside(nr, nc)) continue;

                if (MazeArr[nr, nc] == brick)
                {
                    UnknownMaze[nr, nc] = brick;
                    continue;

                }
                if (!visited[nr, nc]) { dfc(nr, nc); }              // рекурсивный шаг

            }
        }



        public void GoingThrowMaze()
        {
            CollectionItems = 0;
            // метод прохода компьютером лабиринта 
            GenerationUnknownMaze();

            Console.WriteLine("Начальное состояние лабиринта: ");
            DrawMaze(UnknownMaze);
            Console.WriteLine("\n Начинаем прохождение . . . ");
            dfc(EntryRow, EntryCol);
            Console.WriteLine("\n Результат прохождения: ");
            DrawMaze(UnknownMaze);
            Console.WriteLine($"\n === РЕЗУЛЬТТЫ ПРОХОЖДЕНИЯ === ");
            Console.WriteLine($"Собрано предметов: {CollectionItems} ");
            Console.WriteLine($"Всего предметов в лабиринте: {ItemsAmount}");
            int counter1 = 0;
            int counter2 = 0;
            for (int i = 0; i < rows; i++)                                  //сравнение числа вкусняшек в двух лабиринтах
            {
                for (int j = 0; j < columns; j++)
                {
                    if (MazeArr[i, j] == "*") { counter1++; }
                }
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (UnknownMaze[i, j] == "*") { counter2++; }
                }
            }
            if (counter1 == counter2) { return; }
            else
            {
                for (int i = 1; i < rows - 1; i++)
                {
                    for (int j = 1; j < columns - 1; j++)
                    {
                        if (MazeArr[i, j] == "*" && !visited[i, j]) { ItemsAmount--; }  //если несостыковка в количестве ищем в оригинальном лабиринте вкусняху
                                                                                        //и проверяем чтобы в массиве прохода эта клетка была непройденной
                    }
                }
            }
        }
    }

    // НОВЫЙ КЛАСС С ПРОВЕРКОЙ УНИКАЛЬНОСТИ ПРЕДМЕТОВ
    public class SmartItemCollector
    {
        private string[,] maze;
        private int rows;
        private int columns;
        private string brick = "\u2588";

        public SmartItemCollector(string[,] maze, int rows, int columns)
        {
            this.maze = maze;
            this.rows = rows;
            this.columns = columns;
        }

        // Структура для позиции с сравнением
        private struct Position
        {
            public int Row;
            public int Col;
            public Position(int row, int col) { Row = row; Col = col; }

            public override bool Equals(object obj)
            {
                return obj is Position position &&
                       Row == position.Row &&
                       Col == position.Col;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Row, Col);
            }
        }

        // Находим вход и выход
        private (Position start, Position end) FindStartAndEnd()
        {
            Position start = new Position();
            Position end = new Position();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (maze[i, j] == "A")
                        start = new Position(i, j);
                    else if (maze[i, j] == "E")
                        end = new Position(i, j);
                }
            }
            return (start, end);
        }

        // Проверка границ
        private bool Inside(int r, int c) => r >= 0 && r < rows && c >= 0 && c < columns;

        // Находим все доступные предметы с проверкой достижимости
        private HashSet<Position> FindAllReachableItems(Position start)
        {
            HashSet<Position> items = new HashSet<Position>();
            bool[,] visited = new bool[rows, columns];
            Queue<Position> queue = new Queue<Position>();

            queue.Enqueue(start);
            visited[start.Row, start.Col] = true;

            int[] dr = { -1, 0, 1, 0 };
            int[] dc = { 0, 1, 0, -1 };

            while (queue.Count > 0)
            {
                Position current = queue.Dequeue();

                // Если нашли предмет, добавляем в HashSet (автоматически уникальные)
                if (maze[current.Row, current.Col] == "*")
                {
                    items.Add(current);
                }

                for (int i = 0; i < 4; i++)
                {
                    int newRow = current.Row + dr[i];
                    int newCol = current.Col + dc[i];

                    if (Inside(newRow, newCol) && !visited[newRow, newCol] && maze[newRow, newCol] != brick)
                    {
                        visited[newRow, newCol] = true;
                        queue.Enqueue(new Position(newRow, newCol));
                    }
                }
            }

            return items;
        }

        // Поиск пути между двумя точками с учетом собранных предметов
        private List<Position> FindPath(Position from, Position to, HashSet<Position> collectedItems)
        {
            int[,] dist = new int[rows, columns];
            Position[,] parent = new Position[rows, columns];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    dist[i, j] = -1;

            Queue<Position> queue = new Queue<Position>();
            queue.Enqueue(from);
            dist[from.Row, from.Col] = 0;

            int[] dr = { -1, 0, 1, 0 };
            int[] dc = { 0, 1, 0, -1 };

            while (queue.Count > 0)
            {
                Position current = queue.Dequeue();

                if (current.Row == to.Row && current.Col == to.Col)
                    break;

                for (int i = 0; i < 4; i++)
                {
                    int newRow = current.Row + dr[i];
                    int newCol = current.Col + dc[i];

                    if (Inside(newRow, newCol) && dist[newRow, newCol] == -1 && maze[newRow, newCol] != brick)
                    {
                        dist[newRow, newCol] = dist[current.Row, current.Col] + 1;
                        parent[newRow, newCol] = current;
                        queue.Enqueue(new Position(newRow, newCol));
                    }
                }
            }

            // Восстанавливаем путь
            List<Position> path = new List<Position>();
            if (dist[to.Row, to.Col] != -1)
            {
                Position current = to;
                while (!(current.Row == from.Row && current.Col == from.Col))
                {
                    path.Add(current);
                    current = parent[current.Row, current.Col];
                }
                path.Add(from);
                path.Reverse();
            }

            return path;
        }

        // Построение маршрута со сбором ВСЕХ доступных предметов
        private (List<Position> path, int collectedCount) BuildOptimalRoute(Position start, Position end)
        {
            List<Position> fullPath = new List<Position>();
            HashSet<Position> collectedItems = new HashSet<Position>();
            Position currentPosition = start;
            fullPath.Add(start);

            // Находим все доступные предметы
            HashSet<Position> allReachableItems = FindAllReachableItems(start);
            Console.WriteLine($"Найдено доступных предметов: {allReachableItems.Count}");

            // Пока есть не собранные предметы
            while (allReachableItems.Count > 0)
            {
                // Ищем ближайший не собранный предмет
                Position nearestItem = new Position();
                int minDistance = int.MaxValue;
                List<Position> bestPathToItem = new List<Position>();

                foreach (Position item in allReachableItems)
                {
                    if (!collectedItems.Contains(item)) // Проверяем, что предмет еще не собран
                    {
                        List<Position> pathToItem = FindPath(currentPosition, item, collectedItems);
                        if (pathToItem.Count > 0 && pathToItem.Count < minDistance)
                        {
                            minDistance = pathToItem.Count;
                            nearestItem = item;
                            bestPathToItem = pathToItem;
                        }
                    }
                }

                // Если нашли путь к предмету
                if (bestPathToItem.Count > 0)
                {
                    // Добавляем путь к предмету (исключая первую позицию)
                    for (int i = 1; i < bestPathToItem.Count; i++)
                    {
                        fullPath.Add(bestPathToItem[i]);

                        // Проверяем и собираем предмет если находимся на клетке с предметом
                        Position currentPos = bestPathToItem[i];
                        if (maze[currentPos.Row, currentPos.Col] == "*" && !collectedItems.Contains(currentPos))
                        {
                            collectedItems.Add(currentPos);
                            Console.WriteLine($"✅ Собран предмет на позиции ({currentPos.Row}, {currentPos.Col})");
                        }
                    }

                    currentPosition = nearestItem;
                    allReachableItems.Remove(nearestItem); // Удаляем из списка доступных
                }
                else
                {
                    break; // Не можем добраться до оставшихся предметов
                }
            }

            // Путь к выходу
            List<Position> pathToExit = FindPath(currentPosition, end, collectedItems);
            if (pathToExit.Count > 0)
            {
                for (int i = 1; i < pathToExit.Count; i++)
                {
                    fullPath.Add(pathToExit[i]);
                }
            }

            return (fullPath, collectedItems.Count);
        }

        // Визуализация пути
        private void VisualizePath(List<Position> path, HashSet<Position> collectedItems)
        {
            string[,] pathMap = new string[rows, columns];

            // Копируем лабиринт
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    pathMap[i, j] = maze[i, j];
                }
            }

            // Отмечаем путь и собранные предметы
            foreach (Position pos in path)
            {
                if (pathMap[pos.Row, pos.Col] != "A" && pathMap[pos.Row, pos.Col] != "E")
                {
                    if (collectedItems.Contains(pos) && maze[pos.Row, pos.Col] == "*")
                    {
                        pathMap[pos.Row, pos.Col] = "✓"; // Помечаем собранные предметы
                    }
                    else
                    {
                        pathMap[pos.Row, pos.Col] = ".";
                    }
                }
            }

            Console.WriteLine("\nМаршрут со сбором всех предметов (✓ - собранные предметы):");
            DrawMap(pathMap);
        }

        // Отрисовка карты
        private void DrawMap(string[,] map)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    string c = map[i, j];

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
                    else if (c == "✓")
                    {
                        Console.Write(" ✓");
                    }
                    else if (c == ".")
                    {
                        Console.Write(" .");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
        }

        // Подсчет всех предметов в лабиринте
        private int CountAllItems()
        {
            int count = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (maze[i, j] == "*")
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        // Основной метод
        public void CollectAllItemsSafely()
        {
            var (start, end) = FindStartAndEnd();

            Console.WriteLine("\n\n=== УМНЫЙ СБОР ВСЕХ ПРЕДМЕТОВ С ПРОВЕРКОЙ УНИКАЛЬНОСТИ ===");

            int totalItems = CountAllItems();
            Console.WriteLine($"Всего предметов в лабиринте: {totalItems}");

            var (path, collectedCount) = BuildOptimalRoute(start, end);

            if (path.Count > 0)
            {
                // Пересчитываем для проверки
                HashSet<Position> actuallyCollected = new HashSet<Position>();
                foreach (Position pos in path)
                {
                    if (maze[pos.Row, pos.Col] == "*")
                    {
                        actuallyCollected.Add(pos);
                    }
                }

                VisualizePath(path, actuallyCollected);

                Console.WriteLine($"\n=== РЕЗУЛЬТАТЫ С ПРОВЕРКОЙ ===");
                Console.WriteLine($"Длина пути: {path.Count - 1} шагов");
                Console.WriteLine($"Уникально собрано предметов: {actuallyCollected.Count}");
                Console.WriteLine($"Всего доступных предметов: {FindAllReachableItems(start).Count}");

                if (actuallyCollected.Count == FindAllReachableItems(start).Count)
                {
                    Console.WriteLine(" Собраны ВСЕ доступные предметы без дубликатов!");
                }
                else
                {
                    Console.WriteLine($"  Не собрано предметов: {FindAllReachableItems(start).Count - actuallyCollected.Count}");
                }

                // Дополнительная проверка
                Console.WriteLine($"\n Проверка уникальности: {actuallyCollected.Count} уникальных предметов");
            }
            else
            {
                Console.WriteLine("Путь до выхода не найден!");
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

            // ДОБАВЛЕНО: умный сбор всех предметов с проверкой уникальности
            SmartItemCollector smartCollector = new SmartItemCollector(maze.MazeArr, 15, 15);
            smartCollector.CollectAllItemsSafely();
        }
    }
}