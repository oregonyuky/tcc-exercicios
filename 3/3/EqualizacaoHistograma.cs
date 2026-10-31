using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class EqualizacaoHistograma
{
    public static int Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            Console.Error.WriteLine("Uso: EqualizacaoHistograma <imagem-entrada> [imagem-saida.png]");
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
            : Path.Combine(
                Path.GetDirectoryName(caminhoEntrada)!,
                Path.GetFileNameWithoutExtension(caminhoEntrada) + "_equalizada.png");

        using Bitmap original = new Bitmap(caminhoEntrada);
        using Bitmap imgS = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(imgS))
        {
            graphics.DrawImage(original, 0, 0, original.Width, original.Height);
        }

        using Bitmap imgD = new Bitmap(imgS.Width, imgS.Height, PixelFormat.Format24bppRgb);
        equalizacaoHistograma(imgS, imgD);
        imgD.Save(caminhoSaida, ImageFormat.Png);

        Console.WriteLine("Imagem equalizada salva em: " + caminhoSaida);
        return 0;
    }

    public static unsafe void equalizacaoHistograma(Bitmap imgS, Bitmap imgD)
    {
        int width = imgS.Width;
        int height = imgS.Height;
        BitmapData bmS = imgS.LockBits( new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData? bmD = null;

        try
        {
            bmD = imgD.LockBits( new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int sS = bmS.Stride;
            int sD = bmD.Stride;
            byte* src = (byte*)bmS.Scan0;
            byte* dst = (byte*)bmD.Scan0;
            int[] n = new int[256];
            int[] sn = new int[256];
            int[] q = new int[256];
            double intensidadeMedia = (double)width * height / 256;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte* pixel = src + (i * sS) + (j * 3);
                    n[pixel[0]]++;
                }
            }

            sn[0] = n[0];
            for (int i = 1; i < 256; i++) { sn[i] = sn[i - 1] + n[i]; }
            for (int i = 0; i < 256; i++) {
                q[i] = Math.Clamp((int)Math.Round(sn[i] / intensidadeMedia) - 1, 0, 255);
            }
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    byte* pixel = src + (sS * i) + (j * 3);
                    byte* pixelDestino = dst + (sD * i) + (j * 3);
                    byte valor = (byte)q[pixel[0]];
                    pixelDestino[0] = pixelDestino[1] = pixelDestino[2] = valor;
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
