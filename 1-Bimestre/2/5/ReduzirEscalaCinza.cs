public class ReduzirEscaCinza{
    public static unsafe void reduzirEscaCinza(Bitmap imgS, Bitmap imgD, int valor ){
        int width = imgS.Width;
        int height = imgS.Height;
        Rectangle area = new Rectangle(0, 0, width, height);
        BitmapData bmS = imgS.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmD = imgD.LockBits(area, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        int sS = bmS.Stride;
        int sD = bmD.Stride;
        try{        
            byte* src = (byte*)bmS.Scan0.ToPointer();
            byte* dst = (byte*)bmD.Scan0.ToPointer();
            for(int i=0;i<height;i++){
                for(int j=0;j<width;j++){
                    byte* auxS = src + (i * sS) + (j * 3);
                    byte* auxD = dst + (i * sD) + (j * 3);
                    int pixel = (int)((auxS[0]+auxS[1]+auxS[2])/3);
                    auxD[0] = auxD[1] = auxD[2] = calcularNovoGr(pixel, valor);
                }
            }
        }
        finally{
            imgS.UnlockBits(bmS);
            imgD.UnlockBits(bmD);
        }
    }

    public static int calcularNovoGr(int gr, int valor){
        int i=0;
        while(gr >= i*256/valor)i++;
        return (i-1)*255/(valor-1);

    }
}