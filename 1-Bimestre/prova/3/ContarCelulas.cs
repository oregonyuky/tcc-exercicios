using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

public unsafe class ContarCelulas{
    private byte* src;
    private byte* dst;
    private int height;
    private int width;
    private int sS;
    private int sD;

    private int[] dy = {-1, 0, 1, 0};
    private int[] dx = {0, 1, 0, -1};

    private int[] dy8 = {-1, -1, -1, 0, 1, 1, 1, 0};
    private int[] dx8 = {-1, 0, 1, 1, 1, 0, -1, -1};

    class Objeto{
        public int id;
        public int minY;
        public int maxY;
        public int minX;
        public int maxX;
    };

    public byte* getP(int i, int j){
        return src + (sS*i) + j*3;
    }

    public byte* getD(int i, int j){
        return dst + (sD*i) + j*3;
    }

    public void luminancia(Bitmap imgS, Bitmap imgD){
        height = imgS.Height;
        width = imgS.Width;
        BitmapData bmS = imgS.LockBits( new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmD = imgD.LockBits( new Rectangle(0,0,width,height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        try{
            src = (byte*)bmS.Scan0;
            dst = (byte*)bmD.Scan0;
            sS = bmS.Stride;
            sD = bmD.Stride;
            for(int i=0;i<height;i++){
                for(int j=0;j<width;j++){
                    byte* aux = getP(i,j);
                    byte* auxD = getD(i,j);
                    byte cinza = (byte)( 0.114 * aux[0] + 0.587 * aux[1] + 0.299 * aux[2]);
                    auxD[0] = cinza;
                    auxD[1] = cinza;
                    auxD[2] = cinza;
                }
            }
        }
        finally{
            imgS.UnlockBits(bmS);
            imgD.UnlockBits(bmD);
        }
    }
    public void threshold(Bitmap imgS, Bitmap imgD){
        height = imgS.Height;
        width = imgS.Width;

        BitmapData bmS = imgS.LockBits( new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmD = imgD.LockBits( new Rectangle(0,0,width,height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        try{
            src = (byte*)bmS.Scan0;
            dst = (byte*)bmD.Scan0;
            sS = bmS.Stride;
            sD = bmD.Stride;
            for(int i=0;i<height;i++){
                for(int j=0;j<width;j++){
                    byte* aux = getP(i,j);
                    byte* auxD = getD(i,j);
                    auxD[0] = auxD[1] = auxD[2] = (byte)(aux[0] < 128 ? 0 : 255);
                }
            }
        }
        finally{
            imgS.UnlockBits(bmS);
            imgD.UnlockBits(bmD);
        }
    }

    public int retornarQtdCelulas(Bitmap imgS){
        height = imgS.Height;
        width = imgS.Width;
        Bitmap imgCinza = new Bitmap(width,height,PixelFormat.Format24bppRgb);
        Bitmap imgD = new Bitmap(width,height,PixelFormat.Format24bppRgb);

        luminancia(imgS,imgCinza);
        threshold(imgCinza,imgD);
        BitmapData bmD = imgD.LockBits( new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try{
            src = (byte*)bmD.Scan0;
            sS = bmD.Stride;
            bool[,] visitado = new bool[height,width];
            int conectividade = 8;
            int id = 1;
            List<Objeto> l = new List<Objeto>();
            for(int i=0;i<height;i++){
                for(int j=0;j<width;j++){
                    byte* aux = getP(i,j);
                    if(aux[0]==255 || visitado[i,j]) continue;
                    visitado[i,j] = true;
                    Objeto o = new Objeto();
                    o.id = id++;
                    o.minY = o.maxY = i;
                    o.minX = o.maxX = j;
                    Stack<Point> s = new Stack<Point>();
                    s.Push(new Point(j,i));
                    while(s.Count > 0){
                        Point p = s.Pop();
                        int x = p.X;
                        int y = p.Y;
                        for(int k=0;k<conectividade;k++){
                            int ny, nx;
                            if(conectividade==4){
                                ny = y + dy[k];
                                nx = x + dx[k];
                            }else{
                                ny = y + dy8[k];
                                nx = x + dx8[k];
                            }
                            if(nx>=0 && nx<width && ny>=0 && ny<height && !visitado[ny,nx]){
                                byte* aux2 = getP(ny,nx);
                                if(aux2[0]!=0) continue;
                                s.Push(new Point(nx,ny));
                                visitado[ny,nx] = true;
                                if(nx > o.maxX) o.maxX = nx;
                                if(nx < o.minX) o.minX = nx;
                                if(ny > o.maxY) o.maxY = ny;
                                if(ny < o.minY) o.minY = ny;
                            }
                        }
                    }
                    l.Add(o);
                }
            }
            return l.Count;
        }
        finally{
            imgD.UnlockBits(bmD);
            imgD.Dispose();
        }
    }
}