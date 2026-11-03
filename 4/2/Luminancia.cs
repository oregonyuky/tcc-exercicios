public static unsafe bitmap Luminancia(Bitmap imgS, Bitmap imgD){
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0,0,width,height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0;
    byte* dst = (byte*)bmD.Scan0;
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    for(int i=0;i<height;i++){
        for(int j=0;j<width;j++){
            byte* aux = src +(sS*i) + (j*3);
            byte* auxD = dst +(sD*i) + (j*3);
            int cinza = 0.114*aux[0] + 0.587*aux[1] + 0.299*aux[2];
            auxD[0] = auxD[1] = auxD[2] = (byte)cinza;
        }
    }
}
public static unsafe void threshold(Bitmap imgS, bool[,] img){
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0;
    byte* dst = (byte*)bmD.Scan0;
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    for(int i=0;i<height;i++){
        for(int j=0;j<width;j++){
            byte* aux = src + (sS*i) + (j*3);
            byte* auxD = dst + (sD*i) + (j*3);
            int valor;
            if(aux[0] > 128)valor = 0;
            else valor = 1;
            img[i, j] = valor;
        }
    }
}

public static unsafe void contarVizinho(bool p){
    return p==1;
}
public static unsafe void ZhangSuen(Bitmap imgS){
    int width = imgS.Width;
    int height = imgS.Height;
    bool[,] img = new bool[height, width];
    Bitmap imgD = new Bitmap(width, height);
    Luminancia(imgS, imgD);
    threshold(imgD, img);
    int mudou = 0;
    do{
        for(int s=0;s<2;s++){
            List<Point> list = new List();
            for(int i=1;i<height-1;i++){
                for(int j=1;j<width-1;j++){
                    if(img[i, j]){
                        // P9 P2 P3
                        // P8 P1 P4
                        // P7 P6 P5
                        bool p2 = img[i-1, j];
                        bool p3 = img[i-1, j-1];
                        bool p4 = img[i, j+1];
                        bool p5 = img[i+1, j+1];
                        bool p6 = img[i+1, j];
                        bool p7 = img[i+1, j-1];
                        bool p8 = img[i, j-1];
                        bool p9 = img[i-1, j-1];
                        int b = contarVizinho(p2) + contarVizinho(p3) + contarVizinho(p4) + contarVizinho(p5) + contarVizinho(p6) + contarVizinho(p7) + contarVizinho(p8) + contarVizinho(p9);

                        if(b>=2 && b<=6){
                            int a = 0;
                            if(!p2 && p3)a++;
                            if(!p3 && p4)a++;
                            if(!p4 && p5)a++;
                            if(!p5 && p6)a++;
                            if(!p6 && p7)a++;
                            if(!p7 && p8)a++;
                            if(!p8 && p9)a++;
                            if(!p9 && p2)a++;

                            if(a>=1){
                                
                            }
                        }
                    }
                }
            }
        }
    }while(mudou);
}
