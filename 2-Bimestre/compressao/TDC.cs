public class TDC{
    private Bitmap imgS;
    private Bitmap imgD;
    private int N = 8;
    public unsafe byte* getP(int i, int j){
         return imgS.Scan0 + img.Stride * i + j*3;
    }
    public unsafe byte* getPA(int i, int j){
         return imgA.Scan0 + imgA.Stride * i + j*3;
    }
    public unsafe boolean ehPreto(int p){
        return p==0;
    }
    public double a(int x){
        if(x==0)return 1.0/Math.sqrt(N);
        return Math.sqrt(2.0/N);
    }
    public unsafe void tdc(Bitmap imgS, Bitmap imgD){
        int width = imgS.Width;
        int height = imgS.Height;
        Rectangle r = new Rectangle(0,0,width,height);
        BitmapData *bmS = imgS.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData *bmD = imgD.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        int N = 8;
        double[,] C = new double[N, N];
        for(int i=0;i<N;i++){
            for(int j=0;j<N;j++){
                double sum =0;
                for(int y=0;y<N;y++){
                    for(int x=0;x<N;x++){
                        byte* fxy = getP(y, x);
                        sum += fxy[0] * Math.cos((2*x+1)*i*Math.PI/(2*N)) * Math.cos((2*y+1)*j*Math.PI/(2*N));
                        C[i, j] = sum * a(i) * a(j);
                    }
                }

            }
        }
    }
    public unsafe void quantização(Bitmap imgS, Bitmap imgD){
        int width = imgS.Width;
        int height = imgS.Height;
        Rectangle r = new Rectangle(0,0,width,height);
        BitmapData *bmS = imgS.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData *bmD = imgD.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        int N = 8;
        int fator_de_qualidade = 2;
        int[,] Q = new int[N, N];
        double[,] Valor_quantizado = new double[N, N];
        for(int i=0;i<N;i++){
            for(int j=0;j<N;j++){
                Q[i, j] = 1 + (1 + i + j) * fator_de_qualidade;
            }
        }
        for(int i=0;i<N;i++){
            for(int j=0;j<N;j++){
                Valor_quantizado[i, j] = C[i, j] / Q[i, j]; 
            }
        }
    }
}