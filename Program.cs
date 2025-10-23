using System;
using System.Collections.Generic;
using System.Linq;

namespace GenerationMaze
{
    // Новый класс для поиска кратчайшего пути с сбором предметов
    public class ShortestPathFinder
    {
        private string[,] maze;
        private int rows;
        private int columns;
        private string brick = "\u2588";

        public ShortestPathFinder(string[,] maze, int rows, int columns)
        {
            this.maze = maze;
            this.rows = rows;
            this.columns = columns;
        }

        // Структура для позиции
        private struct Position
        {
            public int Row;
            public int Col;
            public Position(int row, int col) { Row = row; Col = col; }
            public override bool Equals(object obj) => obj is Position pos && Row == pos.Row && Col == pos.Col;
            public override int GetHashCode() => HashCode.Combine(Row, Col);
        }

        // Класс для узла пути
        private class PathNode
        {
            public Position Pos;
            public PathNode Parent;
            public int Steps;
            public HashSet<Position> CollectedItems;
            public string Direction; // Для визуализации направления

            public PathNode(Position pos, PathNode parent, int steps, HashSet<Position> collectedItems, string direction = "")
            {
                Pos = pos;
                Parent = parent;
                Steps = steps;
                CollectedItems = new HashSet<Position>(collectedItems);
                Direction = direction;
            }
        }

        // Поиск входа и выхода
        private (Position start, Position end) FindEntryAndExit()
        {
            Position start = new Position();
            Position end = new Position();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (maze[i, j] == "A")
                    {
                        start = new Position(i, j);
                    }
                    else if (maze[i, j] == "E")
                    {
                        end = new Position(i, j);
                    }
                }
            }

            return (start, end);
        }

        // Поиск всех доступных предметов
        private List<Position> FindAllItems()
        {
            var items = new List<Position>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (maze[i, j] == "*")
                    {
                        items.Add(new Position(i, j));
                    }
                }
            }
            return items;
        }

        // Проверка находится ли позиция внутри лабиринта
        private bool IsInside(int r, int c) => r >= 0 && r < rows && c >= 0 && c < columns;

        // Получение направления движения
        private string GetDirection(Position from, Position to)
        {
            if (to.Row == from.Row)
                return to.Col > from.Col ? "→" : "←";
            else
                return to.Row > from.Row ? "↓" : "↑";
        }

        // Поиск кратчайшего пути с максимальным сбором предметов
        public void FindOptimalPath()
        {
            var (start, end) = FindEntryAndExit();
            var allItems = FindAllItems();

            Console.WriteLine($"\n=== ПОИСК КРАТЧАЙШЕГО ПУТИ ===");
            Console.WriteLine($"Старт: ({start.Row}, {start.Col})");
            Console.WriteLine($"Финиш: ({end.Row}, {end.Col})");
            Console.WriteLine($"Всего предметов: {allItems.Count}");

            // Используем BFS для поиска оптимального пути
            var queue = new Queue<PathNode>();
            var visited = new Dictionary<string, (int steps, int items)>();

            var initialItems = new HashSet<Position>();
            queue.Enqueue(new PathNode(start, null, 0, initialItems));

            PathNode bestSolution = null;
            int maxItemsCollected = -1;
            int minSteps = int.MaxValue;

            int[] dr = { -1, 0, 1, 0 };
            int[] dc = { 0, 1, 0, -1 };
            string[] directions = { "↑", "→", "↓", "←" };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                // Если достигли выхода, проверяем лучшее решение
                if (current.Pos.Equals(end))
                {
                    if (current.CollectedItems.Count > maxItemsCollected ||
                        (current.CollectedItems.Count == maxItemsCollected && current.Steps < minSteps))
                    {
                        bestSolution = current;
                        maxItemsCollected = current.CollectedItems.Count;
                        minSteps = current.Steps;
                    }
                    continue;
                }

                // Проверяем все направления
                for (int i = 0; i < 4; i++)
                {
                    int newRow = current.Pos.Row + dr[i];
                    int newCol = current.Pos.Col + dc[i];

                    if (!IsInside(newRow, newCol)) continue;
                    if (maze[newRow, newCol] == brick) continue;

                    var newPos = new Position(newRow, newCol);
                    var newCollected = new HashSet<Position>(current.CollectedItems);

                    // Проверяем, есть ли предмет
                    if (maze[newRow, newCol] == "*" && !newCollected.Contains(newPos))
                    {
                        newCollected.Add(newPos);
                    }

                    // Создаем ключ состояния
                    string stateKey = $"{newPos.Row},{newPos.Col},{string.Join(",", newCollected.OrderBy(p => $"{p.Row},{p.Col}"))}";

                    // Проверяем, не посещали ли это состояние с лучшими параметрами
                    if (visited.TryGetValue(stateKey, out var existing))
                    {
                        if (existing.items > newCollected.Count ||
                            (existing.items == newCollected.Count && existing.steps <= current.Steps + 1))
                        {
                            continue;
                        }
                    }

                    var direction = GetDirection(current.Pos, newPos);
                    var newNode = new PathNode(newPos, current, current.Steps + 1, newCollected, direction);
                    queue.Enqueue(newNode);
                    visited[stateKey] = (current.Steps + 1, newCollected.Count);
                }
            }

            // Вывод результатов
            if (bestSolution != null)
            {
                Console.WriteLine($"\n=== РЕЗУЛЬТАТЫ ПОИСКА ===");
                Console.WriteLine($"Собрано предметов: {bestSolution.CollectedItems.Count}/{allItems.Count}");
                Console.WriteLine($"Длина пути: {bestSolution.Steps} шагов");
                Console.WriteLine($"Процент сбора: {(double)bestSolution.CollectedItems.Count / allItems.Count * 100:F1}%");

                // Визуализация пути
                VisualizePath(bestSolution, allItems);

                // Вывод пропущенных предметов
                var missedItems = allItems.Where(item => !bestSolution.CollectedItems.Contains(item)).ToList();
                if (missedItems.Count > 0)
                {
                    Console.WriteLine($"\nПропущенные предметы (недоступны):");
                    foreach (var item in missedItems)
                    {
                        Console.WriteLine($"  - Позиция: ({item.Row}, {item.Col})");
                    }
                }
            }
            else
            {
                Console.WriteLine("Путь до выхода не найден!");
            }
        }

        // Визуализация пути
        private void VisualizePath(PathNode solution, List<Position> allItems)
        {
            var pathMap = new string[rows, columns];

            // Копируем оригинальный лабиринт
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    pathMap[i, j] = maze[i, j];
                }
            }

            // Восстанавливаем путь и отмечаем его
            var path = new List<PathNode>();
            var current = solution;
            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }
            path.Reverse();

            // Отмечаем путь (кроме старта и финиша)
            foreach (var node in path)
            {
                if (!node.Pos.Equals(path[0].Pos) && !node.Pos.Equals(path[path.Count - 1].Pos))
                {
                    pathMap[node.Pos.Row, node.Pos.Col] = node.Direction;
                }
            }

            Console.WriteLine("\nКарта с оптимальным путем:");
            DrawPathMap(pathMap);
        }

        // Отрисовка карты с путем
        private void DrawPathMap(string[,] pathMap)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    string c = pathMap[i, j];

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
                    else if (c == "→" || c == "←" || c == "↑" || c == "↓")
                    {
                        Console.Write(c + " ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
        }
    }

    // Существующий класс Maze остается без изменений
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

        private static int EntryRow, EntryCol;
        private int ExitRow, ExitCol;

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

        public void DrawMaze(string[,] MazeArr)
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
            while (side == EntrySide)
            {
                side = rnd.Next(4);
            }
            ExitSide = side;

            switch (side)
            {
                case 0:
                    ExitSide = 0;
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

        private void EnsurePathClear()
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

        public string[,] UnknownMaze = new string[15, 15];

        private void GenerationUnknownMaze()
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

        private bool Inside(int r, int c) => r >= 0 && r < rows && c >= 0 && c < columns;

        private bool[,] visited = new bool[15, 15];
        private int[] R = new int[4] { -1, 0, 1, 0 };
        private int[] C = new int[4] { 0, 1, 0, -1 };

        private void dfc(int r, int c)
        {
            if (MazeArr[r, c] == "*" && !visited[r, c])
            {
                CollectionItems++;
                Console.WriteLine($"Собран предмет! Всего: {CollectionItems} ");
            }

            visited[r, c] = true;
            UnknownMaze[r, c] = MazeArr[r, c];

            for (int i = 0; i < 4; i++)
            {
                int nr = r + R[i];
                int nc = c + C[i];

                if (!Inside(nr, nc)) continue;

                if (MazeArr[nr, nc] == brick)
                {
                    UnknownMaze[nr, nc] = brick;
                    continue;
                }
                if (!visited[nr, nc]) { dfc(nr, nc); }
            }
        }

        public void GoingThrowMaze()
        {
            CollectionItems = 0;

            GenerationUnknownMaze();

            Console.WriteLine("Начальное состояние лабиринта: ");
            DrawMaze(UnknownMaze);
            Console.WriteLine("\n Начинаем прохождение . . . ");
            dfc(EntryRow, EntryCol);
            Console.WriteLine("\n Результат прохождения: ");
            DrawMaze(UnknownMaze);
            Console.WriteLine($"\n === РЕЗУЛЬТАТЫ ПРОХОЖДЕНИЯ === ");
            Console.WriteLine($"Собрано предметов: {CollectionItems} ");
            Console.WriteLine($"Всего предметов в лабиринте: {ItemsAmount}");

            int counter1 = 0;
            int counter2 = 0;
            for (int i = 0; i < rows; i++)
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
                        if (MazeArr[i, j] == "*" && !visited[i, j]) { ItemsAmount--; }
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
            maze.DrawMaze(maze.MazeArr);

            // Добавляем вызов нового функционала
            ShortestPathFinder pathFinder = new ShortestPathFinder(maze.MazeArr, 15, 15);
            pathFinder.FindOptimalPath();

            // Оригинальный вызов
            maze.GoingThrowMaze();
        }
    }
}