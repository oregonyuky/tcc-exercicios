using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class MediaVizinhanca5x5
{
    public static int Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            Console.Error.WriteLine("Uso: MediaVizinhanca5x5 <imagem-entrada> [imagem-saida.png]");
            return 1;
        }

        string caminhoEntrada = Path.GetFullPath(args[0]);
        if (!File.Exists(caminhoEntrada))
        {
            Console.Error.WriteLine("Imagem de entrada não encontrada: " + caminhoEntrada);
            return 1;
        }

        string caminhoSaida = args.Length == 2 ? Path.GetFullPath(args[1]) : Path.Combine( Path.GetDirectoryName(caminhoEntrada)!, Path.GetFileNameWithoutExtension(caminhoEntrada) + "_media5x5.png");
        using Bitmap original = new Bitmap(caminhoEntrada);
        using Bitmap imgS = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(imgS))
        {
            graphics.DrawImage(original, 0, 0, original.Width, original.Height);
        }

        using Bitmap imgD = new Bitmap(imgS);
        mediaVizinhanca5x5(imgS, imgD);
        imgD.Save(caminhoSaida, ImageFormat.Png);

        Console.WriteLine("Imagem filtrada salva em: " + caminhoSaida);
        return 0;
    }

    public static unsafe void mediaVizinhanca5x5(Bitmap imgS, Bitmap imgD)
    {
        int width = imgS.Width;
        int height = imgS.Height;
        BitmapData bmS = imgS.LockBits( new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData? bmD = null;
        try
        {
            bmD = imgD.LockBits( new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            byte* src = (byte*)bmS.Scan0;
            byte* dst = (byte*)bmD.Scan0;
            int sS = bmS.Stride;
            int sD = bmD.Stride;
            for (int i = 2; i < height - 2; i++) {
                for (int j = 2; j < width - 2; j++) {
                    int sum = 0;
                    for (int k = -2; k <= 2; k++) {
                        for (int l = -2; l <= 2; l++) {
                            byte* aux = src + (sS * (i + k)) + ((j + l) * 3);
                            sum += aux[0];
                        }
                    }

                    byte* auxD = dst + (sD * i) + (j * 3);
                    auxD[0] = auxD[1] = auxD[2] = (byte)(sum / 25);
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
