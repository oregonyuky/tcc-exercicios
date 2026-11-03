using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using imageLockMode = System.Drawing.Imaging.ImageLockMode;

public unsafe class Contorno
{
    private byte* src;
    private byte* dst;
    private int sS;
    private int sD;
    private int[] dy = {-1, -1, 0, 1, 1, 1, 0, -1};
    private int[] dx = {0, 1, 1, 1, 0, -1, -1, -1};

    public unsafe byte* getP(int i, int j){
        return src + (sS*i) + (j*3);
    }
    public unsafe void contornoDMA(Bitmap imgS, Bitmap imgD){
        int height = imgS.Height;
        int width = imgS.Width;
        BitmapData bmS = imgS.LockBits(new Rectangle(0,0,width,height), imageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmD = imgD.LockBits(new Rectangle(0,0,width,height), imageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        bool[,] img = new bool[height, width];
        src = (byte*)bmS.Scan0;
        dst = (byte*)bmD.Scan0;
        sS = bmS.Stride;
        sD = bmD.Stride;
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                byte* aux = getP(i, j);
                if (aux[0] == 255) continue;
                for (int k = 0; k < 8; k++) {
                    int ny = i + dy[k];
                    int nx = j + dx[k];
                    if (ny < 0 || ny >= height || nx < 0 || nx >= width) {
                        img[i, j] = true;
                        break;
                    }
                    byte* vizinho = getP(ny, nx);
                    if (vizinho[0] == 255) {
                        img[i, j] = true;
                        break;
                    }
                }
            }
        }
        for(int i=0;i<height;i++){
            for(int j=0;j<width;j++){
                byte* aux = dst + (sD*i) + j*3;
                aux[0] = aux[1] = aux[2] = (byte)(img[i, j] ? 0 : 255);
            }
        }
        imgS.UnlockBits(bmS);
        imgD.UnlockBits(bmD);
    }

    
}

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length > 2)
        {
            Console.Error.WriteLine("Uso: contorno [imagem-entrada] [imagem-saida.png]");
            return 1;
        }

        string diretorio = AppDomain.CurrentDomain.BaseDirectory;
        string caminhoEntrada = Path.GetFullPath(args.Length >= 1
            ? args[0]
            : Path.Combine(diretorio, "Imagempb.png"));
        string caminhoSaida = Path.GetFullPath(args.Length == 2
            ? args[1]
            : Path.Combine(diretorio, "Imagempb_contorno.png"));

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
            using (Bitmap imgD = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb))
            {
                graphics.DrawImage(original, 0, 0, original.Width, original.Height);
                new Contorno().contornoDMA(imgS, imgD);
                imgD.Save(caminhoSaida, ImageFormat.Png);
            }

            Console.WriteLine("Contorno salvo em: " + caminhoSaida);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Falha ao processar a imagem: " + ex.Message);
            return 1;
        }
    }
}
