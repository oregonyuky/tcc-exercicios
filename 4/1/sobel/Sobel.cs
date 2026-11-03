public static unsafe void Sobel(Bitmap imgS, Bitmap imgD){
    int width = imgS.Width;
    int height = imgS.Height;
    BitmapData bmS = imgS.LockBits(new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    BitmapData bmD = imgD.LockBits(new Rectangle(0,0,width,height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
    byte* src = (byte*)bmS.Scan0;
    byte* dst = (byte*)bmD.Scan0;
    int sS = bmS.Stride;
    int sD = bmD.Stride;
    int[,] h1 = {{-1,-2,-1},{0,0,0},{1,2,1}};
    int[,] h2 = {{-1,0,1},{-2,0,2},{-1,0,1}};
    for(int i=0;i<height-2;i++){
        for(int j=0;j<width-2;j++){
            int fh1=0, fh2=0;
            for(int k=0;k<9;k++){
                byte* aux = src + (sS*(i+(k/3))) + ((j+(k%3))*3);
                fh1 += h1[k/3, k%3] * aux[0];
                fh2 += h2[k/3, k%3] * aux[0];
            }
            byte* auxD = dst + (sD*i) + (j*3);
            int res = (int)Math.Sqrt((fh1*fh1) + (fh2*fh2));
            if(res>255)res=255;
            auxD[0] = auxD[1] = auxD[2] = (byte)res;
        }
    }
    imgS.UnlockBits(bmS);
    imgD.UnlockBits(bmD);
}