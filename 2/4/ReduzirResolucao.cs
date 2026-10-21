public class ReduzirResolucao{
    public static unsafe void reduzirResolucao(Bitmap imgS, Bitmap imgD){
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
            for(int i=0;i<height;i+=2){
                for(int j=0;j<width;j+=2){
                    byte* aux1 = src + (i * sS) + (j * 3);
                    byte* aux2 = src + (i * sD) + ((j+1) * 3);
                    byte* aux3 = src + ((i+1) * sD) + (j * 3);
                    byte* aux4 = src + ((i+1) * sD) + ((j+1) * 3);
                    byte b = (byte)((aux1[0]+aux2[0]+aux3[0]+aux4[0])/4);
                    byte g = (byte)((aux1[1]+aux2[1]+aux3[1]+aux4[1])/4);
                    byte r = (byte)((aux1[2]+aux2[2]+aux3[2]+aux4[2])/4);
                    int ii = i/2;
                    int jj = j/2;
                    byte* auxD = dst + (ii * sD) + (jj * 3);
                    auxD[0] = b;
                    auxD[1] = g;
                    auxD[2] = r;
                }
            }
        }
        finally{
            imgS.UnlockBits(bmS);
            imgD.UnlockBits(bmD);
        }
    }
}