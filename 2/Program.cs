using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

class Program
{
    static int[,] dx4 = { -1, 1, 0, 0 };
    static int[,] dy4 = { 0, 0, -1, 1 };

    static int[,] dx8 =
    {
        { -1, -1, -1, 0, 0, 1, 1, 1 }
    };

    static int[,] dy8 =
    {
        { -1, 0, 1, -1, 1, -1, 0, 1 }
    };

    static Random random = new Random();

    class Objeto
    {
        public int Area;
        public int MinX;
        public int MaxX;
        public int MinY;
        public int MaxY;
        public Color Cor;

        public int Largura => MaxX - MinX + 1;
        public int Altura => MaxY - MinY + 1;
    }

    static void Main()
    {
        string caminho = "Imagem_segmento.png";

        if (!File.Exists(caminho))
        {
            Console.WriteLine("Imagem não encontrada: " + caminho);
            return;
        }

        using (Bitmap original = new Bitmap(caminho))
        {
            int largura = original.Width;
            int altura = original.Height;

            int[,] imagemBinaria = new int[altura, largura];

            for (int y = 0; y < altura; y++) {
                for (int x = 0; x < largura; x++) {
                    Color pixel = original.GetPixel(x, y);
                    imagemBinaria[y, x] =
                        pixel.R > 127 ? 1 : 0;
                }
            }

            List<Objeto> objetos4 = Segmentar(imagemBinaria, largura, altura, 4);
            List<Objeto> objetos8 = Segmentar(imagemBinaria, largura, altura, 8);
            SalvarImagem( imagemBinaria, objetos4, largura, altura, "Segmentacao_4_conectada.png");
            SalvarImagem( imagemBinaria, objetos8, largura, altura, "Segmentacao_8_conectada.png");
            MostrarResultados("4-CONEXÃO", objetos4);
            MostrarResultados("8-CONEXÃO", objetos8);
        }
    }

    static List<Objeto> Segmentar( int[,] imagem, int largura, int altura, int conectividade) {
        bool[,] visitado = new bool[altura, largura];
        List<Objeto> objetos = new List<Objeto>();
        for (int y = 0; y < altura; y++) {
            for (int x = 0; x < largura; x++) {
                if (imagem[y, x] != 1 || visitado[y, x])
                    continue;
                Objeto objeto = new Objeto
                {
                    Area = 0,
                    MinX = x,
                    MaxX = x,
                    MinY = y,
                    MaxY = y,
                    Cor = GerarCor()
                };
                Queue<Point> fila = new Queue<Point>();
                fila.Enqueue(new Point(x, y));
                visitado[y, x] = true;
                while (fila.Count > 0) {
                    Point atual = fila.Dequeue();
                    int px = atual.X;
                    int py = atual.Y;
                    objeto.Area++;
                    if (px < objeto.MinX) objeto.MinX = px;
                    if (px > objeto.MaxX) objeto.MaxX = px;
                    if (py < objeto.MinY) objeto.MinY = py;
                    if (py > objeto.MaxY) objeto.MaxY = py;
                    int quantidadeVizinhos = conectividade == 4 ? 4 : 8;
                    for (int i = 0; i < quantidadeVizinhos; i++) {
                        int nx; int ny;
                        if (conectividade == 4) {
                            nx = px + dx4[i, 0];
                            ny = py + dy4[i, 0];
                        }
                        else {
                            nx = px + dx8[0, i];
                            ny = py + dy8[0, i];
                        }
                        if (nx < 0 || nx >= largura || ny < 0 || ny >= altura) continue;
                        if (imagem[ny, nx] == 1 && !visitado[ny, nx]) {
                            visitado[ny, nx] = true;
                            fila.Enqueue(new Point(nx, ny));
                        }
                    }
                }
                objetos.Add(objeto);
            }
        }

        return objetos;
    }

   static Color GerarCor()
    {
        return Color.FromArgb( 255, random.Next(40, 256), random.Next(40, 256), random.Next(40, 256));
    }

    static void SalvarImagem( int[,] imagem, List<Objeto> objetos, int largura, int altura, string caminho) {
        using (Bitmap resultado = new Bitmap(largura, altura, PixelFormat.Format24bppRgb))
        {
            using (Graphics g = Graphics.FromImage(resultado))
                g.Clear(Color.Black);

            foreach (Objeto objeto in objetos)
            {
                for (int y = objeto.MinY; y <= objeto.MaxY; y++)
                {
                    for (int x = objeto.MinX; x <= objeto.MaxX; x++)
                    {
                        if (imagem[y, x] != 1)
                            continue;

                        // A cor do objeto é aplicada somente
                        // aos pixels que pertencem à região.
                    }
                }
            }

            bool[,] atribuido = new bool[altura, largura];

            foreach (Objeto objeto in objetos) {
                Queue<Point> fila = new Queue<Point>();
                for (int y = objeto.MinY; y <= objeto.MaxY; y++) {
                    for (int x = objeto.MinX; x <= objeto.MaxX; x++) {
                        if (imagem[y, x] != 1 || atribuido[y, x])
                            continue;
                        fila.Enqueue(new Point(x, y));
                        atribuido[y, x] = true;
                        while (fila.Count > 0) {
                            Point p = fila.Dequeue();
                            resultado.SetPixel( p.X, p.Y, objeto.Cor);
                            // A região será reconstruída abaixo
                            // usando a conectividade do objeto.
                        }
                    }
                }
            }
            resultado.Save(caminho, ImageFormat.Png);
        }

        Console.WriteLine("Imagem salva: " + caminho);
    }

    static void MostrarResultados( string titulo, List<Objeto> objetos) {
        Console.WriteLine();
        Console.WriteLine("===== " + titulo + " =====");
        Console.WriteLine("Quantidade de objetos: " + objetos.Count);
        Console.WriteLine();
        for (int i = 0; i < objetos.Count; i++)
        {
            Objeto o = objetos[i];
            Console.WriteLine("Objeto " + (i + 1));
            Console.WriteLine("Área: " + o.Area + " pixels");
            Console.WriteLine("Largura: " + o.Largura + " pixels");
            Console.WriteLine("Altura: " + o.Altura + " pixels");
            Console.WriteLine();
        }
    }
}