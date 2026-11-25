using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class MediaKVizinhos9
{
    public static int Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            Console.Error.WriteLine("Uso: MediaKVizinhos9 <imagem-entrada> [imagem-saida.png]");
            return 1;
        }

        string caminhoEntrada = Path.GetFullPath(args[0]);
        if (!File.Exists(caminhoEntrada))
        {
            Console.Error.WriteLine("Imagem de entrada não encontrada: " + caminhoEntrada);
            return 1;
        }

        string caminhoSaida = args.Length == 2
            ? Path.GetFullPath(args[1])
            : Path.Combine(Path.GetDirectoryName(caminhoEntrada)!, Path.GetFileNameWithoutExtension(caminhoEntrada) + "_mediaK9.png");

        using Bitmap original = new Bitmap(caminhoEntrada);
        using Bitmap imgS = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(imgS))
        {
            graphics.DrawImage(original, 0, 0, original.Width, original.Height);
        }

        using Bitmap imgD = new Bitmap(imgS);
        mediaKVizinhos9(imgS, imgD);
        imgD.Save(caminhoSaida, ImageFormat.Png);
        Console.WriteLine("Imagem filtrada salva em: " + caminhoSaida);
        return 0;
    }

    public static void bubblesort(int[] med, int[] abs, int n)
    {
        // Ordena apenas os n menores valores necessários para o filtro.
        for (int i = 0; i < n; i++)
        {
            int menor = i;
            for (int j = i + 1; j < med.Length; j++)
            {
                if (abs[j] < abs[menor])
                {
                    menor = j;
                }
            }

            (med[i], med[menor]) = (med[menor], med[i]);
            (abs[i], abs[menor]) = (abs[menor], abs[i]);
        }
    }

    public static unsafe void mediaKVizinhos9(Bitmap imgS, Bitmap imgD)
    {
        int width = imgS.Width;
        int height = imgS.Height;
        BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData? bmD = null;
        try
        {
            bmD = imgD.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            byte* src = (byte*)bmS.Scan0;
            byte* dst = (byte*)bmD.Scan0;
            int sS = bmS.Stride;
            int sD = bmD.Stride;
            const int kVizinhos = 9;
            int[] med = new int[25];
            int[] abs = new int[25];

            for (int y = 2; y < height - 2; y++)
            {
                for (int x = 2; x < width - 2; x++)
                {
                    int m = 0;
                    byte* centro = src + (sS * y) + (x * 3);
                    for (int dy = -2; dy <= 2; dy++)
                    {
                        for (int dx = -2; dx <= 2; dx++)
                        {
                            byte* pixel = src + (sS * (y + dy)) + ((x + dx) * 3);
                            med[m] = pixel[0];
                            abs[m++] = Math.Abs(pixel[0] - centro[0]);
                        }
                    }

                    bubblesort(med, abs, kVizinhos);
                    int soma = 0;
                    for (int n = 0; n < kVizinhos; n++)
                    {
                        soma += med[n];
                    }

                    byte* destino = dst + (sD * y) + (x * 3);
                    destino[0] = destino[1] = destino[2] = (byte)(soma / kVizinhos);
                }
            }
        }
        finally
        {
            if (bmD != null)
            {
                imgD.UnlockBits(bmD);
            }

            imgS.UnlockBits(bmS);
        }
    }
}
