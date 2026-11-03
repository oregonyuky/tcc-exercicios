using System;
using System.Collections.Generic;

public static class ZhangSuen
{
    /// <summary>
    /// Afinamento (thinning/esqueletização) de Zhang-Suen.
    /// image[linha, coluna] = true para pixel de objeto (primeiro plano).
    /// Retorna uma nova matriz com o esqueleto (a original não é alterada).
    /// </summary>
    public static bool[,] Thin(bool[,] image)
    {
        int rows = image.GetLength(0);
        int cols = image.GetLength(1);
        var img = (bool[,])image.Clone();

        bool changed;
        do
        {
            changed = false;
            for (int step = 0; step < 2; step++)
            {
                var toRemove = new List<(int r, int c)>();

                // Bordas da imagem são ignoradas (precisam de 8 vizinhos)
                for (int r = 1; r < rows - 1; r++)
                {
                    for (int c = 1; c < cols - 1; c++)
                    {
                        if (!img[r, c]) continue;

                        // Vizinhos em sentido horário, começando pelo Norte:
                        // P9 P2 P3
                        // P8 P1 P4
                        // P7 P6 P5
                        bool p2 = img[r - 1, c];
                        bool p3 = img[r - 1, c + 1];
                        bool p4 = img[r, c + 1];
                        bool p5 = img[r + 1, c + 1];
                        bool p6 = img[r + 1, c];
                        bool p7 = img[r + 1, c - 1];
                        bool p8 = img[r, c - 1];
                        bool p9 = img[r - 1, c - 1];

                        // B = número de vizinhos de objeto
                        int b = Count(p2) + Count(p3) + Count(p4) + Count(p5)
                              + Count(p6) + Count(p7) + Count(p8) + Count(p9);
                        if (b < 2 || b > 6) continue;

                        // A = transições 0 -> 1 na sequência P2,P3,...,P9,P2
                        int a = 0;
                        if (!p2 && p3) a++;
                        if (!p3 && p4) a++;
                        if (!p4 && p5) a++;
                        if (!p5 && p6) a++;
                        if (!p6 && p7) a++;
                        if (!p7 && p8) a++;
                        if (!p8 && p9) a++;
                        if (!p9 && p2) a++;
                        if (a != 1) continue;

                        bool ok = step == 0
                            ? !(p2 && p4 && p6) && !(p4 && p6 && p8)   // sub-iteração 1
                            : !(p2 && p4 && p8) && !(p2 && p6 && p8);  // sub-iteração 2

                        if (ok) toRemove.Add((r, c));
                    }
                }

                // Remoção só ao final da sub-iteração (processamento paralelo)
                foreach (var (r, c) in toRemove)
                    img[r, c] = false;

                if (toRemove.Count > 0) changed = true;
            }
        } while (changed);

        return img;
    }

    private static int Count(bool v) => v ? 1 : 0;
}

public static class Program
{
    public static void Main()
    {
        // Exemplo: retângulo cheio + um "braço" vertical (formato de T)
        string[] shape =
        {
            "..............................",
            "..##########################..",
            "..##########################..",
            "..##########################..",
            "..##########################..",
            "..............####............",
            "..............####............",
            "..............####............",
            "..............####............",
            "..............####............",
            "..............####............",
            "..............................",
        };

        int rows = shape.Length, cols = shape[0].Length;
        var img = new bool[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                img[r, c] = shape[r][c] == '#';

        var skeleton = ZhangSuen.Thin(img);

        Print("Original:", img);
        Print("Esqueleto (Zhang-Suen):", skeleton);
    }

    private static void Print(string title, bool[,] m)
    {
        Console.WriteLine(title);
        for (int r = 0; r < m.GetLength(0); r++)
        {
            for (int c = 0; c < m.GetLength(1); c++)
                Console.Write(m[r, c] ? '#' : '.');
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}