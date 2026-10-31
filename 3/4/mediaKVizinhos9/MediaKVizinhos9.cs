public static unsafe void mediaKVizinhos9(Bitmap imgS, Bitmap imgD){
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits( new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits( new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0;
    byte* dst = (byte*)bmD.Scan0;
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    int k = 9;
    int[] med = new int[25];
    int[] abs = new int[25];
    for(int i=2;i<height-2;i++){
        for(int j=2;j<width-2;j++){
            int m=0;
            byte* aux = src + (sS*i) + (j*3);
            for(int k=-2;k<=2;k++){
                for(int l=-2;l<=2;l++){
                    byte* aux2 = src + (sS*(i+k)) + ((j+l)*3);
                    med[m] = auxD[0];
                    abs[m++] = Math.Abs(aux2[0] - aux[0]);
                }
            }
            bubblesort(med, abs, 25);
            int sum=0;
            for(int i=0;i<9;i++){
                sum += med[i];
            }
            byte* auxD = dst + (sD*i) + (j*3);
            auxD[0] = auxD[1] = auxD[2] = (byte)(sum/9);
        }
    }
}