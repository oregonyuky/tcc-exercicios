using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
public class No{
    public int valor;
    public int freq;
    public No esq;
    public No dir;
    public No(int valor, int freq){
        this.valor = valor;
        this.freq = freq;
        this.esq = null;
        this.dir = null;
    }
}
public class TDC{
    private Bitmap imgS;
    private Bitmap imgD;
    private BitmapData bmS;
    private BitmapData bmD;
    private int N = 8;
    private int[] set;
    public TDC(Bitmap imgS){
        this.imgS = imgS;
        this.imgD = new Bitmap(imgS.Width, imgS.Height);
    }
    public unsafe byte* getP(int i, int j){
        return (byte*)bmS.Scan0 + bmS.Stride * i + j*3;
    }
    public unsafe byte* getPA(int i, int j){
        return (byte*)bmD.Scan0 + bmD.Stride * i + j*3;
    }
    public bool ehPreto(int p){
        return p==0;
    }
    public double a(int x){
        if(x==0)return 1.0/Math.Sqrt(N);
        return Math.Sqrt(2.0/N);
    }
    public unsafe double[,] tdc(){
        int width = imgS.Width;
        int height = imgS.Height;
        Rectangle r = new Rectangle(0,0,width,height);
        bmS = imgS.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        double[,] C = new double[N,N];
        for(int i=0;i<N;i++){
            for(int j=0;j<N;j++){
                double sum = 0;
                for(int y=0;y<N;y++){
                    for(int x=0;x<N;x++){
                        byte* fxy = getP(y,x);
                        sum += fxy[0] * Math.Cos((2*x+1)*i*Math.PI/(2*N)) * Math.Cos((2*y+1)*j*Math.PI/(2*N));
                    }
                }
                C[i,j] = sum * a(i) * a(j);
            }
        }
        imgS.UnlockBits(bmS);
        return C;
    }
    public int[,] quantização(){
        int fator_de_qualidade = 2;
        double[,] C = tdc();
        int[,] Q = new int[N,N];
        int[,] Valor_quantizado = new int[N,N];
        for(int i=0;i<N;i++){
            for(int j=0;j<N;j++){
                Q[i,j] = 1 + (1+i+j) * fator_de_qualidade;
            }
        }
        for(int i=0;i<N;i++){
            for(int j=0;j<N;j++){
                Valor_quantizado[i,j] = (int)Math.Round(C[i,j]/Q[i,j]);
            }
        }
        return Valor_quantizado;
    }
    public int[] zigzag(){
        int[,] C = quantização();
        int[] vet = new int[N*N];
        int i=0;
        for(int s=0;s<2*N-1;s++){
            if(s%2==0){
                for(int y=N-1;y>=0;y--){
                    int x = s-y;
                    if(x>=0 && x<N)vet[i++] = C[y,x];
                }
            }else{
                for(int y=0;y<N;y++){
                    int x = s-y;
                    if(x>=0 && x<N)vet[i++] = C[y,x];
                }
            }
        }
        return vet;
    }
    public int getCount(int[] vet, int n){
        int c=0;
        for(int i=0;i<vet.Length;i++){
            if(vet[i]==n)c++;
        }
        return c;
    }
    public bool busca(int[] vet, int n, int tl){
        int pos=0;
        while(pos<tl && vet[pos]!=n)pos++;
        return pos<tl;
    }
    public int[] contarFreq(int[] vet){
        set = new int[N*N];
        int j=0;
        for(int i=0;i<vet.Length;i++){
            if(!busca(set,vet[i],j))set[j++] = vet[i];
        }
        int[] freq = new int[j];
        for(int i=0;i<j;i++){
            freq[i] = getCount(vet,set[i]);
        }
        return freq;
    }
    public void bubblesort(List<No> lista){
        int n = lista.Count;
        for(int i=0;i<n-1;i++){
            for(int j=0;j<n-i-1;j++){
                if(lista[j].freq > lista[j+1].freq){
                    No aux = lista[j];
                    lista[j] = lista[j+1];
                    lista[j+1] = aux;
                }
            }
        }
    }
    public No huffman(int[] vet){
        List<No> lista = new List<No>();
        int[] freq = contarFreq(vet);
        for(int i=0;i<freq.Length;i++){
            lista.Add(new No(set[i],freq[i]));
        }
        while(lista.Count>1){
            bubblesort(lista);
            No esq = lista[0];
            No dir = lista[1];
            lista.RemoveAt(0);
            lista.RemoveAt(0);
            No pai = new No(0,dir.freq+esq.freq);
            pai.esq = esq;
            pai.dir = dir;
            lista.Add(pai);
        }
        return lista.Count>0 ? lista[0] : null;
    }
    public Dictionary<int,string> gerarCodigo(No raiz){
        Dictionary<int,string> tabela = new Dictionary<int,string>();
        Stack<No> s = new Stack<No>();
        Stack<string> sg = new Stack<string>();
        if(raiz==null)return tabela;
        s.Push(raiz);
        sg.Push("");
        while(s.Count>0){
            No aux = s.Pop();
            string ss = sg.Pop();
            if(aux.dir==null && aux.esq==null){
                tabela[aux.valor] = ss=="" ? "0" : ss;
            }else{
                if(aux.dir!=null){
                    s.Push(aux.dir);
                    sg.Push(ss+"1");
                }
                if(aux.esq!=null){
                    s.Push(aux.esq);
                    sg.Push(ss+"0");
                }
            }
        }
        return tabela;
    }
    public string comprimir(out No raiz){
        int[] vet = zigzag();
        raiz = huffman(vet);
        Dictionary<int,string> tabela = gerarCodigo(raiz);
        string codigo = "";
        for(int i=0;i<vet.Length;i++){
            codigo += tabela[vet[i]];
        }
        return codigo;
    }
}