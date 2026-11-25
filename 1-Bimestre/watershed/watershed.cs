
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public unsafe class Watershed
{

    private static readonly int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
    private static readonly int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

    public static int Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.Error.WriteLine("Uso: watershed [caminho-da-imagem]");
            return 1;
        }

        string caminhoImagem = Path.GetFullPath(args.Length == 1
            ? args[0]
            : Path.Combine(AppContext.BaseDirectory, "cells.png"));

        if (!File.Exists(caminhoImagem))
        {
            Console.Error.WriteLine("Imagem nao encontrada: " + caminhoImagem);
            return 1;
        }

        try
        {
            using (Bitmap original = new Bitmap(caminhoImagem))
            using (Bitmap imagem = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb))
            using (Bitmap resultado = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics graphics = Graphics.FromImage(imagem))
                {
                    graphics.DrawImage(original, 0, 0, original.Width, original.Height);
                }

                int quantidade = Aplicar(imagem, resultado);
                Console.WriteLine("Quantidade de celulas: " + quantidade);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Falha ao processar imagem: " + ex.Message);
            return 1;
        }
    }

    public static int Aplicar(Bitmap imgS, Bitmap imgD, int limiar = 128, int distanciaMinima = 8)
    {
        int width = imgS.Width;
        int height = imgS.Height;

        Rectangle r = new Rectangle(0, 0, width, height);

        BitmapData dadosS = imgS.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData dadosD = imgD.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        byte* src = (byte*)dadosS.Scan0;
        byte* dst = (byte*)dadosD.Scan0;
        int sS = dadosS.Stride;
        int sD = dadosD.Stride;
        bool[,] mascara = new bool[height, width];
        float[,] distancia = new float[height, width];

        // 1. THRESHOLD
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                byte* p = src + y * sS + x * 3;
                int cinza = (p[0] + p[1] + p[2]) / 3;
                mascara[y, x] = cinza < limiar;
            }
        }

        // 2. DISTANCE TRANSFORM
        float diagonal = 1.41421356f;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                distancia[y, x] = mascara[y, x] ? 999999 : 0;
            }
        }
        // Passagem de cima para baixo
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (!mascara[y, x]) continue;

                if (x > 0)
                    distancia[y, x] = Math.Min(distancia[y, x], distancia[y, x - 1] + 1);

                if (y > 0)
                    distancia[y, x] = Math.Min(distancia[y, x], distancia[y - 1, x] + 1);

                if (x > 0 && y > 0)
                    distancia[y, x] = Math.Min(distancia[y, x], distancia[y - 1, x - 1] + diagonal);

                if (x < width - 1 && y > 0)
                    distancia[y, x] = Math.Min(distancia[y, x], distancia[y - 1, x + 1] + diagonal);
            }
        }

        // Passagem de baixo para cima
        for (int y = height - 1; y >= 0; y--) {
            for (int x = width - 1; x >= 0; x--) {
                if (!mascara[y, x]) continue;
                if (x < width - 1) distancia[y, x] = Math.Min(distancia[y, x], distancia[y, x + 1] + 1);
                if (y < height - 1) distancia[y, x] = Math.Min(distancia[y, x], distancia[y + 1, x] + 1);
                if (x < width - 1 && y < height - 1) distancia[y, x] = Math.Min(distancia[y, x], distancia[y + 1, x + 1] + diagonal);
                if (x > 0 && y < height - 1) distancia[y, x] = Math.Min(distancia[y, x], distancia[y + 1, x - 1] + diagonal);
            }
        }

        // 3. ENCONTRAR MAXIMOS LOCAIS
        List<Point> candidatos = new List<Point>();
        for (int y = 1; y < height - 1; y++) {
            for (int x = 1; x < width - 1; x++) {
                if (!mascara[y,x]) continue;
                bool maximo = true;
                for (int k = 0; k < 8; k++) {
                    int sx = x + dx[k];
                    int sy = y + dy[k];
                    if (distancia[sy,sx] > distancia[y,x]) {
                        maximo = false;
                        break;
                    }
                }
                if (maximo) candidatos.Add(new Point(x, y));
            }
        }
        // Ordenar 
        for (int i = 0; i < candidatos.Count - 1; i++) {
            for (int j = 0; j < candidatos.Count - i - 1; j++) {

                Point a = candidatos[j];
                Point b = candidatos[j + 1];

                if (distancia[a.Y, a.X] < distancia[b.Y, b.X]) {
                    Point aux = candidatos[j];
                    candidatos[j] = candidatos[j + 1];
                    candidatos[j + 1] = aux;
                }
            }
        }
        // 4. SUPRESSAO DE MAXIMOS PROXIMOS
        List<Point> sementes = new List<Point>();
        int d2 = distanciaMinima * distanciaMinima;
        foreach (Point candidato in candidatos) {
            int cx = candidato.X;
            int cy = candidato.Y;
            bool muitoProximo = false;
            foreach (Point s in sementes) {
                int ddx = cx - s.X;
                int ddy = cy - s.Y;
                if (ddx * ddx + ddy * ddy < d2) {
                    muitoProximo = true;
                    break;
                }
            }
            if (!muitoProximo)
                sementes.Add(candidato);
        }

        // 5. CRIAR MARCADORES

        imgS.UnlockBits(dadosS);
        imgD.UnlockBits(dadosD);
        return sementes.Count;
    }
}
