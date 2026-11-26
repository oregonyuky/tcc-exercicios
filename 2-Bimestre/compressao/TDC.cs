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
    public unsafe double[] tdc(){
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
                        // Color pixelColor = imgS.GetPixel(x, y);
                        // double pixelVal = pixelColor.B;
                        sum += fxy[0] * Math.cos((2*x+1)*i*Math.PI/(2*N)) * Math.cos((2*y+1)*j*Math.PI/(2*N));
                        C[i, j] = sum * a(i) * a(j);
                    }
                }
            }
        }
        return C;
    }
    public unsafe double[] quantização(){
        int N = 8;
        int fator_de_qualidade = 2;
        double[,] C = tdc();
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
        return Valor_quantizado;
    }
    public unsafe int[] zigzag(){
        double[,] C = quantização();
        int[] vet = new int[N*N];
        int i=0;
        for(int s=0;s<2*N+1;s++){
            if(s%2==0){
                for(int y=N-1;y>=0;y--){
                    int x = s-y;
                    if(x>=0 && x<N)vet[i++] = C[y, x];
                }
            } else {
                for(int y=0;y<N;y++){
                    int x = s-y;
                    if(x>=0 && x<N)vet[i++] = C[y, x];
                }
            }
        }
        return vet;
    }
    public int getCount(int[] vet, int n){
        int c=0;
        for(int i=0;i<N*N;i++){
            if(vet[i]==n)c++;
        }
        return c;
    }
    public int[] contarFreq(){
        int maior=0;
        int[] vet = zigzag();
        int[] set = new int[N*N];
        for(int i=0;i<N*N;i++){
            if(getCount(set, vet[i])==0)set[j++] = vet[i];
        }
        int[] freq = new int[j];
        for(int i=0;i<j;i++){
            freq[i] = getCount(vet, set[i]);
        }
        return freq[i];
    }
    public void bubblesort(List<No> lista){
        int n = lista.Count;
        for(int i=0;i<n-1;i++){
            for(int j=0;j<n-i-1;j++){
                if(lista[j].freq > lista[j+1]){
                    No aux = lista[j];
                    lista[j] = lista[j+1];
                    lista[j+1] = aux;
                }
            }
        }
    }
    public No huffman(){
        Lista<No> lista = new List<No>();
        int[] freq = contarFreq();
        for(int i=0;i<freq.Length;i++){
            lista.Add(new No(i, freq[i]));
        }
        while(lista.Count > 1){
            bubblesort(lista);
            No esq = lista[0];
            No dir = lista[1];
            lista.removeAt(0);
            lista.removeAt(0);
            No pai = new No(0, dir.freq + esq.freq);
            pai.esq = esq;
            pai.dir = dir;
            lista.Add(pai);
        }
        return lista.Count>0 ? lista[0] : null;
    }
    public string gerarCodigo(){
        No raiz = huffman();
        String[] tabela = new String[255];
        Stack<No> s = new Stack<No>();
        Stack<String> sg = new Stack<String>();
        s.push(raiz);
        while(!s.empty()){
            No aux = s.pop();
            String ss = sg.pop();
            if(aux.dir==null && aux.esq==null){
                tabela.insert(ss);
            } else {
                if(aux.dir != null){
                    s.push(aux.dir);
                    sg.push(ss = "1") //depis trocar = por mais
                } 
                if(aux.esq != null){
                    s.push(aux.esq);
                    sg.push(ss = "0"); //depois trocar5 = por mais
                }
            }
                
}
