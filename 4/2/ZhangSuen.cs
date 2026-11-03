using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public static class ZhangSuenFilter {
    private static void Luminancia(Bitmap imgS, Bitmap imgD) {
        int width = imgS.Width;
        int height = imgS.Height;
        BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmD = imgD.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        try {
            unsafe {
                byte* src = (byte*)bmS.Scan0;
                byte* dst = (byte*)bmD.Scan0;
                int sS = bmS.Stride;
                int sD = bmD.Stride;
                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++) {
                        byte* aux = src + (sS * i) + (j * 3);
                        byte* auxD = dst + (sD * i) + (j * 3);
                        int cinza = (int)(0.114 * aux[0] + 0.587 * aux[1] + 0.299 * aux[2]);
                        auxD[0] = auxD[1] = auxD[2] = (byte)cinza;
                    }
                }
            }
        }
        finally
        {
            imgS.UnlockBits(bmS);
            imgD.UnlockBits(bmD);
        }
    }

    private static void Threshold(Bitmap imgS, bool[,] img)
    {
        int width = imgS.Width;
        int height = imgS.Height;
        BitmapData bmS = imgS.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try {
            unsafe {
                byte* src = (byte*)bmS.Scan0;
                int stride = bmS.Stride;
                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++) {
                        byte* pixel = src + (stride * i) + (j * 3);
                        img[i, j] = pixel[0] <= 128;
                    }
                }
            }
        }
        finally
        {
            imgS.UnlockBits(bmS);
        }
    }

    private static int ContarVizinho(bool pixel) {
        return pixel ? 1 : 0;
    }

    public static Bitmap ZhangSuen(Bitmap imgS) {
        int width = imgS.Width;
        int height = imgS.Height;
        bool[,] img = new bool[height, width];
        Bitmap imgD = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        Luminancia(imgS, imgD);
        Threshold(imgD, img);
        bool mudou;
        do {
            mudou = false;
            for (int s = 0; s < 2; s++) {
                List<Point> list = new List<Point>();
                for (int i = 1; i < height - 1; i++) {
                    for (int j = 1; j < width - 1; j++) {
                        if (!img[i, j]) continue;
                        bool p2 = img[i - 1, j];
                        bool p3 = img[i - 1, j + 1];
                        bool p4 = img[i, j + 1];
                        bool p5 = img[i + 1, j + 1];
                        bool p6 = img[i + 1, j];
                        bool p7 = img[i + 1, j - 1];
                        bool p8 = img[i, j - 1];
                        bool p9 = img[i - 1, j - 1];
                        int b = ContarVizinho(p2) + ContarVizinho(p3) + ContarVizinho(p4) + ContarVizinho(p5) + ContarVizinho(p6) + ContarVizinho(p7) + ContarVizinho(p8) + ContarVizinho(p9);
                        if (b < 2 || b > 6) continue;
                        int a = 0;
                        if (!p2 && p3) a++;
                        if (!p3 && p4) a++;
                        if (!p4 && p5) a++;
                        if (!p5 && p6) a++;
                        if (!p6 && p7) a++;
                        if (!p7 && p8) a++;
                        if (!p8 && p9) a++;
                        if (!p9 && p2) a++;

                        if (a == 1) {
                            bool ok = s == 0 ? !(p2 && p4 && p6) && !(p4 && p6 && p8) : !(p2 && p4 && p8) && !(p2 && p6 && p8);
                            if (ok) list.Add(new Point(j, i));
                        }
                    }
                }
                for (int i = 0; i < list.Count; i++) {
                    img[list[i].Y, list[i].X] = false;
                    mudou = true;
                }
            }
        } while (mudou);

        BitmapData bmD = imgD.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        try {
            unsafe {
                byte* dst = (byte*)bmD.Scan0;
                int stride = bmD.Stride;
                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++) {
                        byte* pixel = dst + (stride * i) + (j * 3);
                        pixel[0] = pixel[1] = pixel[2] = (byte)(img[i, j] ? 0 : 255);
                    }
                }
            }
        }
        finally
        {
            imgD.UnlockBits(bmD);
        }

        return imgD;
    }

    public static int Main(string[] args)
    {
        if (args.Length > 2)
        {
            Console.Error.WriteLine("Uso: ZhangSuen [imagem-entrada] [imagem-saida.png]");
            return 1;
        }

        string diretorio = AppDomain.CurrentDomain.BaseDirectory;
        string caminhoEntrada = Path.GetFullPath(args.Length >= 1
            ? args[0]
            : Path.Combine(diretorio, "Imagempb.png"));
        string caminhoSaida = Path.GetFullPath(args.Length == 2
            ? args[1]
            : Path.Combine(diretorio, "Imagempb_zhangsuen.png"));

        if (!File.Exists(caminhoEntrada))
        {
            Console.Error.WriteLine("Imagem de entrada nao encontrada: " + caminhoEntrada);
            return 1;
        }

        try
        {
            using (Bitmap original = new Bitmap(caminhoEntrada))
            using (Bitmap imgS = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb))
            using (Graphics graphics = Graphics.FromImage(imgS))
            {
                graphics.DrawImage(original, 0, 0, original.Width, original.Height);
                using (Bitmap resultado = ZhangSuen(imgS))
                    resultado.Save(caminhoSaida, ImageFormat.Png);
            }

            Console.WriteLine("Imagem processada salva em: " + caminhoSaida);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Falha ao processar a imagem: " + ex.Message);
            return 1;
        }
    }
}
