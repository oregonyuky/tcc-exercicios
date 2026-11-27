public class No{
    public int valor;
    public int freq;
    public No esq;
    public No dir;
    public No(int valor, int freq){
        this.valor = valor;
        this.freq = freq;
        dir = null;
        esq = null;
    }
}
public class Descompressao{
    Bitmap imgS;
    BitmapData bmS;
    public int[] huffmanInversa(string codigo, No raiz){
        int[] vet = new int[N*N];
        No aux = raiz;
        int j=0;
        for(int i=0;i<codigo.Length;i++){
            if(codigo[i]=='0')aux = aux.esq;
            else aux = aux.dir;
            if(aux.esq==null && aux.dir==null){
                vet[j++] = aux.valor;
                aux = raiz;
            }
        }
        return vet;
    }
    public int[,] zigzagInversa(int[] vet){
        int[,] C = new int[N*N];
        int i=0;
        for(int s=0;s<=2*N+1;s++){
            if(s%2==0){
                for(int y=N-1;y>=0;y--){
                    int x = s-y;
                    if(x>=0 && x<N)C[y, x] = vet[i++];
                }
            } else {
                for(int y=0;y<N;y++){
                    int x = s-y;
                    if(x>=0 && x<N)C[y, x] = vet[i++];
                }
            }
        }
        return C;
    } 
    public double[,] quantizacaoInversa(int[,] C){
        int fq = 2;
        int[,] Q = new int[N,N];
        double[,] D = new double[N,N];
        for(int i=0;i<N;i++){
            for(int j=0;j<N;j++){
                Q[i, j] = 1 + (1+i+j)*fq;
                D[i, j] = C[i,j] * Q[i,j];
            }
        }
        return D;
    }
    public double a(int x){
        if(x==0)return 1.0/Math.Sqrt(N);
        return Math.Sqrt(2.0/N);
    }
    public void tdcInversa(double[,] D){
        int sum=0;
        Bitmap imgD = new Bitmap(imgS.Width, imgS.Height, PixelFormat.Format24bppRgb);
        BitmapData bmD = imgD.LockBits(new Rectangle(0,0,width,height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        for(int x=0;x<N;x++){
            for(int y=0;x<N;x++){
                double sum=0;
                for(int i=0;i<N;i++){
                    for(int j=0;j<N;j++){
                        sum += a(i) * a(j) * D[i, j] * Math.Cos((2*x+1)*i*Math.PI/(2*N)) * Math.Cos((2*y+1)*j*Math.PI/(2*N));
                    }
                }
                byte* auxD = (byte*)bmD.Scan0 + bmD.Stride*y + x*3;
                int p = Math.Round(sum);
                if(p<0)p=0;
                if(p>255)p=255;
                auxD[0] = auxD[1] = auxD[2] = (byte)p;
            }
        }
        imgD.UnlockBits(bmD);
        return imgD;
    }
} 