using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class SegmentarObjeto{
    static int[] dx4 = {-1, 1, 0, 0};
    static int[] dy4 = {0, 0, -1, 1};
    static int[] dx8 = {-1, -1, -1, 0, 1, 1, 1, 0};
    static int[] dy8 = {-1, 0, 1, -1, -1, 0, 1, 1};

    class Objeto{
        public int area;
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;
        public Color cor;
        public List<Point> pixels = new List<Point>();
        public int largura;
        public int altura;
    }

    static void Main(string[] args){
        string caminho = args.Length > 0 ? args[0] : "Imagem_segmento.png";
        if (!File.Exists(caminho)){
            Console.WriteLine("Imagem não encontrada: " + caminho);
            Console.WriteLine("Informe o caminho da imagem como argumento ou coloque Imagem_segmento.png na pasta do programa.");
            return;
        }

        using (Bitmap imgS = new Bitmap(caminho)){
            ProcessarImagem(imgS, Path.GetDirectoryName(Path.GetFullPath(caminho)));
        }
    }

    static unsafe void ProcessarImagem(Bitmap imgS, string pastaSaida){
        int width = imgS.Width;
        int height = imgS.Height;
        Rectangle area = new Rectangle(0, 0, width, height);
        BitmapData bmS = imgS.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        int sS = bmS.Stride;
        int[,] imagemB = new int[height, width];
        try{
            byte* src = (byte*)bmS.Scan0.ToPointer();
            //limiarizacao
            for(int i=0;i<height;i++){
                for(int j=0;j<width;j++){
                    byte* auxS = src + (i * sS) + (j * 3);
                    int pixel = (auxS[0]+auxS[1]+auxS[2])/3;
                    imagemB[i, j] = pixel > 127 ? 0 : 1;
                }
            }
        }
        finally{
            imgS.UnlockBits(bmS);
        }

        List<Objeto> objetos4 = segmentar(imagemB, width, height, 4);
        List<Objeto> objetos8 = segmentar(imagemB, width, height, 8);
        salvarImagem(imagemB, objetos4, width, height, Path.Combine(pastaSaida, "segmento4.png"));
        salvarImagem(imagemB, objetos8, width, height, Path.Combine(pastaSaida, "segmento8.png"));
        mostrarResultado("segmento4", objetos4);
        mostrarResultado("segmento8", objetos8);
    }

    static unsafe List<Objeto> segmentar(int[,] imagem, int width, int heigth, int conectividade){
        bool[,] visitado = new bool[heigth, width];
        List<Objeto> objetos = new List<Objeto>();
        for(int y=0;y<heigth;y++){
            for(int x=0;x<width;x++){
                if(imagem[y, x]!=1 || visitado[y, x])continue;
                Queue<Point> f = new Queue<Point>();
                Objeto o = new Objeto{
                    area = 0,
                    minX = x,
                    maxX = x,
                    minY = y,
                    maxY = y,
                    cor = GerarCor()
                };
                f.Enqueue(new Point(x, y));
                visitado[y,x] = true;
                while(f.Count > 0){
                    Point no = f.Dequeue();
                    int px = no.X;
                    int py = no.Y;
                    o.pixels.Add(new Point(px, py));
                    o.area++;
                    if(px < o.minX)o.minX = px;
                    if(px > o.maxX)o.maxX = px;
                    if(py < o.minY)o.minY = py;
                    if(py > o.maxY)o.maxY = py;
                    for(int i=0;i<conectividade;i++){
                        int nx, ny;
                        if(conectividade == 4){
                            nx = px + dx4[i];
                            ny = py + dy4[i];
                        } else {
                            nx = px + dx8[i];
                            ny = py + dy8[i];
                        }
                        if(nx<0 || nx>=width || ny<0 || ny>=heigth)continue;
                        if(imagem[ny, nx]==1 && !visitado[ny,nx]){
                            visitado[ny,nx]=true;
                            f.Enqueue(new Point(nx, ny));
                        }
                    }
                }
                o.largura = o.maxX - o.minX + 1;
                o.altura = o.maxY - o.minY + 1;
                objetos.Add(o);
            }
        }
        return objetos;
    }

    static void mostrarResultado(
        string nome,
        List<Objeto> objetos)
    {
        Console.WriteLine($"Resultado {nome}:");

        foreach (Objeto o in objetos)
        {
            Console.WriteLine(
                $"Objeto: Area={o.area}, " +
                $"MinX={o.minX}, MaxX={o.maxX}, " +
                $"MinY={o.minY}, MaxY={o.maxY}, " +
                $"Largura={o.largura}, Altura={o.altura}");
        }
    }
    
    static Random random = new Random();

    static Color GerarCor()
    {
        return Color.FromArgb(
            255,
            random.Next(40, 256),
            random.Next(40, 256),
            random.Next(40, 256));
    }

    static void salvarImagem(int[,] imagemB, List<Objeto> objetos, int width, int heigth, string caminho){
        using (Bitmap resultado = new Bitmap(width, heigth, PixelFormat.Format24bppRgb))
        {
            using (Graphics graphics = Graphics.FromImage(resultado))
            {
                graphics.Clear(Color.Black);
            }

            foreach (Objeto objeto in objetos)
            {
                foreach (Point pixel in objeto.pixels)
                {
                    if (pixel.X >= 0 && pixel.X < width && pixel.Y >= 0 && pixel.Y < heigth)
                    {
                        resultado.SetPixel(pixel.X, pixel.Y, objeto.cor);
                    }
                }
            }

            resultado.Save(caminho, ImageFormat.Png);
        }
    }

    public static double distanciaEuclidiana (int x1 , int y1 , int x2 , int y2 )
    {
    return Math . Sqrt ( Math . Pow ( x2 - x1 , 2) + Math . Pow ( y2 - y1 , 2) ) ;
    }

}