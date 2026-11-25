using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class RobertsFilter
{
    public static int Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            Console.Error.WriteLine("Uso: Roberts <imagem-entrada> [imagem-saida.png]");
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
            : Path.Combine(Path.GetDirectoryName(caminhoEntrada)!, Path.GetFileNameWithoutExtension(caminhoEntrada) + "_roberts.png");

        using Bitmap original = new Bitmap(caminhoEntrada);
        using Bitmap imgS = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(imgS))
        {
            graphics.DrawImage(original, 0, 0, original.Width, original.Height);
        }

        using Bitmap imgD = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
        Roberts(imgS, imgD);
        imgD.Save(caminhoSaida, ImageFormat.Png);
        Console.WriteLine("Imagem processada salva em: " + caminhoSaida);
        return 0;
    }

    public static unsafe void Roberts(Bitmap imgS, Bitmap imgD)
    {
        int width = imgS.Width;
        int height = imgS.Height;
        BitmapData bmS = imgS.LockBits(new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmD = imgD.LockBits(new Rectangle(0,0,width,height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        byte* src = (byte*)bmS.Scan0;
        byte* dst = (byte*)bmD.Scan0;
        int sS = bmS.Stride;
        int sD = bmD.Stride;
        int[] h1 = {1, 0, 0, -1};
        int[] h2 = {0, 1, -1, 0};
        for(int i=0;i<height-1;i++){
            for(int j=0;j<width-1;j++){
                int fh1=0, fh2=0;
                for(int k=0;k<4;k++){
                    byte *aux = src + (sS*(i+(k/2))) + ((j+(k%2))*3);
                    fh1 += h1[k] * aux[0];
                    fh2 += h2[k] * aux[0];
                }
                byte* auxD = dst + (sD*i) + (j*3);
                auxD[0] = auxD[1] = auxD[2] = (byte)Math.Sqrt((fh1*fh1) + (fh2*fh2));
            }
        }
        imgS.UnlockBits(bmS);
        imgD.UnlockBits(bmD);
    }
}
