class Train{
    static int[] dx4 = {-1, 1, 0, 0};
    static int[] dy4 = {0, 0, -1, 1};
    static int[] dx8 = {-1, -1, -1, 0, 1, 1, 1, 0};
    static int[] dy8 = {0, -1, -1, -1, 0, 1, 1, 1};

    class Objeto{
        public int area;
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;
        public Color cor;
        public int getLargura(){return maxX-minX+1;}
        public int getAltura(){return maxY-minY+1;}
    }

    static unsafe void Main(Bitmap imgS, Bitmap imgD){
        int width = imgS.Width;
        int height = imgD.Height;
        BitmapData bmS = imgS.LockBits(new Rectangle(0,0,width;height), imageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmD = imgS.LockBits(new Rectangle(0,0,width;height), imageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        byte *src = (byte*)bmS.Scan0.ToPointer();
        byte *dst = (byte*)bmD.Scan0.ToPointer();
        int sS = bmS.Stride;
        int sD = bmD.Stride;
        int[,] imagemB = new int[height, width];
        //limiarizacao
        for(int i=0;i<height;i++){
            for(int j=0;j<width;j++){
                byte* auxS = src + (sS.i) + (j*3);
                int pixel = (auxS[0]+auxS[1]+auxS[2])/3;
                imagemB[j, i] = pixel > 127 ? 1 : 0;
            }
        }
        List<Objeto> objetos4 = segmentar(imagemB, width, heigth, 4);
        List<Objeto> objetos8 = segmentar(imagemB, width, heigth, 8);
        salvarImagem(imagemB, objetos4, width, heigth, "segmento4.png")
        salvarImagem(imagemB, objetos8, width, heigth, "segmento8.png")


    }

    static unsafe List<Objeto> segmentar(int[,] imagem, int width, int heigth, int conectividade){
        bool[,] visitado = new bool(height, width);
        List<Objeto> objetos = new List<Objeto>();
        for(int y=0;y<heigth;y++){
            for(int x=0;x<width;x++){
                if(imagem[y, x]!=1 || visitado[y, x])continue;
                Fila f = new Fila();
                Objeto o = new objeto{
                    area = 0;
                    minX = x;
                    maxX = x;
                    minY = y;
                    maxY = y;
                    cor = gerarCor();
                }
                f.enqueue(new No(x, y));
                visitado[y,x] = true;
                while(f.isEmpty()){
                    No no = f.dequeue();
                    int px = no.getX();
                    int py = no.getY();
                    if(px < o.minX)o.minX = px;
                    if(px > o.maxX)o.maxX = px;
                    if(py < o.minY)o.minY = px;
                    if(py > o.minY)o.maxY = px;
                    for(int i=0;i<conectividade;i++){
                        int nx, ny;
                        if(conectividade == 4){
                            nx = px + dx4[i];
                            ny = py + dx4[i];
                        } else {
                            nx = px + dx8[i];
                            ny = py + dx8[i];
                        }
                        if(nx<0 || nx>width || ny<0 || ny>heigth)continue;
                        if(imagem[ny nx]==1 && !visitado[ny,nx]){
                            visitado[ny,nx]=true;
                            fila.enqueue(new No(nx, ny));
                        }
                    }
                }
                objetos.Add(o);
            }
        }
        return objetos;
    }
    
    static Color GerarCor() {
        return Color.FromArgb( 255, random.Next(40, 256), random.Next(40, 256), random.Next(40, 256));
    }

    static void salvarImagem(int[,] imagemB, List<Objeto> objetos, int width, int heigth, string caminho){
        
    }

}