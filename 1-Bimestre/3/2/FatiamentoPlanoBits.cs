using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class FatiamentoPlanoBits
{
    public static void Main(string[] args)
    {
        string pathImage = args.Length > 0
            ? Path.GetFullPath(args[0])
            : Path.Combine(AppContext.BaseDirectory, "imagem.jpg");

        if (!File.Exists(pathImage))
        {
            throw new FileNotFoundException("Imagem de entrada não encontrada.", pathImage);
        }

        using Bitmap imgS = new Bitmap(pathImage);
        fatiamentoplanobits(imgS, pathImage);
    }

    public static void fatiamentoplanobits(Bitmap imgS, string pathImage)
    {
        string pasta = Path.GetDirectoryName(Path.GetFullPath(pathImage))!;
        string nome = Path.GetFileNameWithoutExtension(pathImage);

        for (int plano = 0; plano < 8; plano++)
        {
            using Bitmap res = new Bitmap(imgS.Width, imgS.Height, PixelFormat.Format24bppRgb);

            for (int y = 0; y < imgS.Height; y++)
            {
                for (int x = 0; x < imgS.Width; x++)
                {
                    Color pixel = imgS.GetPixel(x, y);
                    int cinza = (pixel.R + pixel.G + pixel.B) / 3;
                    int valor = ((cinza >> plano) & 1) * 255;
                    res.SetPixel(x, y, Color.FromArgb(valor, valor, valor));
                }
            }

            string pathSaida = Path.Combine(pasta, nome + "_plano_" + plano + ".png");
            res.Save(pathSaida, ImageFormat.Png);
        }

        Console.WriteLine("Planos de bits salvos em: " + pasta);
    }
}
