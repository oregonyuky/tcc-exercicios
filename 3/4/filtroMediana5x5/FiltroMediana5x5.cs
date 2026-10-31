using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class FiltroMediana5x5
{
    public static int Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            Console.Error.WriteLine("Uso: FiltroMediana5x5 <imagem-entrada> [imagem-saida.png]");
            return 1;
        }

        string caminhoEntrada = Path.GetFullPath(args[0]);
        if (!File.Exists(caminhoEntrada))
        {
            Console.Error.WriteLine("Imagem de entrada não encontrada: " + caminhoEntrada);
            return 1;
        }

        string caminhoSaida = args.Length == 2 ? Path.GetFullPath(args[1]) : Path.Combine( Path.GetDirectoryName(caminhoEntrada)!, Path.GetFileNameWithoutExtension(caminhoEntrada) + "_mediana5x5.png");

        using Bitmap original = new Bitmap(caminhoEntrada);
        using Bitmap imgS = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(imgS))
        {
            graphics.DrawImage(original, 0, 0, original.Width, original.Height);
        }

        using Bitmap imgD = new Bitmap(imgS);
        filtroMediana5x5(imgS, imgD);
        imgD.Save(caminhoSaida, ImageFormat.Png);

        Console.WriteLine("Imagem filtrada salva em: " + caminhoSaida);
        return 0;
    }

    public static unsafe void bubblesort(int[] med, int n)
    {
        for (int i = 0; i < n - 1; i++) {
            for (int j = 0; j < n - i - 1; j++) {
                if (med[j] > med[j + 1]) {
                    int aux = med[j];
                    med[j] = med[j + 1];
                    med[j + 1] = aux;
                }
            }
        }
    }

    public static unsafe void filtroMediana5x5(Bitmap imgS, Bitmap imgD)
    {
        int width = imgS.Width;
        int height = imgS.Height;
        BitmapData bmS = imgS.LockBits( new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData? bmD = null;
        try
        {
            bmD = imgD.LockBits( new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int sS = bmS.Stride;
            int sD = bmD.Stride;
            byte* src = (byte*)bmS.Scan0;
            byte* dst = (byte*)bmD.Scan0;
            for (int i = 2; i < height - 2; i++) {
                for (int j = 2; j < width - 2; j++) {
                    int[] med = new int[25];
                    int m = 0;
                    for (int k = -2; k <= 2; k++) {
                        for (int l = -2; l <= 2; l++) {
                            byte* aux = src + (sS * (i + k)) + ((j + l) * 3);
                            med[m++] = aux[0];
                        }
                    }
                    bubblesort(med, 25);
                    int valor = med[12];
                    byte* auxD = dst + (sD * i) + (j * 3);
                    auxD[0] = auxD[1] = auxD[2] = (byte)valor;
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
