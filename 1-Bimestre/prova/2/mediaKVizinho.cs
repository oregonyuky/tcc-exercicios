public void bubblesort(int[] abs, int[] med, int n){
    for(int i=0;i<n-1;i++){
        for(int j=0;j<n-i-1;j++){
            if(abs[j] > abs[j+1]){
                int aux = med[j];
                med[j] = med[j+1];
                med[j+1] = aux;

                aux = abs[j];
                abs[j] = abs[j+1];
                abs[j+1] = aux;
            }
        }
    }
}
public static void mediaKVizinho(Bitmap imgS, Bitmap imgD){
    int height = imgS.Height;
    int width = imgS.Width;
    BitmapData bmS = imgS.LockBits(new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    int[] med = new int[25];
    int[] abs = new int[25];
    const int kk = 9;
    for(int i=0;i<height;i++){
        for(int j=0;j<width;j++){
            int m=0;
            byte* auxS = src + (sS*i) + j*3;
            for(int k=-2;k<=2;k++){
                for(int l=-2;l<=2;l++){
                    byte* aux = src + (sS*(i+k)) + (j+l)*3;
                    med[m] = aux[0];
                    abs[m++] = Math.Abs(auxS[0]-aux[0]);
                }
            }
            bubblesort(abs, med, 25);
            int sum =0 ;
            for(int k=0;k<kk;k++){
                sum += med[k];
            }
            byte* auxD = dst + (sD*i) + (j*3);
            auxD[0] = auxD[1] = auxD[2] = (byte)(sum/kk);
        }
    }
    imgS.UnlockBits(bmS);
    imgD.UnlockBits(bmD);
}